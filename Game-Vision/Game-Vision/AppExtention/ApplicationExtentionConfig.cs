using FluentValidation;
using Game_Vision.Application.Command.Auth;
using Game_Vision.Application.Interface;
using Game_Vision.Application.Validator;
using Game_Vision.Domain;
using Game_Vision.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Game_Vision.AppExtention 
{
    public static class ServiceCollectionExtensions 
    {

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
   
            services.AddDbContext<GameVisionDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("GameVision"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                    }));

            // inject services 
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterCommand).Assembly));
            services.AddValidatorsFromAssembly(typeof(RegisterCommandValidator).Assembly);
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; 
                options.DefaultChallengeScheme = "Discord"; 
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS
                options.Cookie.SameSite = SameSiteMode.Lax; // یا None اگر cross-site داری
                options.ExpireTimeSpan = TimeSpan.FromHours(24);
                options.SlidingExpiration = true;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                    ClockSkew = TimeSpan.Zero
                };

               
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                       
                        if (context.Request.Headers.ContainsKey("Authorization"))
                        {
                            return Task.CompletedTask;
                        }

                
                        if (context.Request.Cookies.TryGetValue("authToken", out var token))
                        {
                            context.Token = token;
                        }

                        return Task.CompletedTask;
                    }
                };
            })
            .AddDiscord(options =>
            {
                options.ClientId = configuration["Authentication:Discord:ClientId"]!;
                options.ClientSecret = configuration["Authentication:Discord:ClientSecret"]!;
                options.CallbackPath = "/Account/CallBack";
                options.Scope.Add("identify");
                options.Scope.Add("email");
                options.SaveTokens = true;

                options.ClaimActions.Clear();
                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "username");
                options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                options.ClaimActions.MapJsonKey("urn:discord:avatar", "avatar");

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        // درخواست به API دیسکورد برای اطلاعات کاربر
                        var request = new HttpRequestMessage(HttpMethod.Get, "https://discord.com/api/users/@me");
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                        var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();
                        var userJson = await response.Content.ReadFromJsonAsync<JsonElement>();

                        context.RunClaimActions(userJson);

                        // استخراج اطلاعات از JSON
                        var discordId = userJson.GetProperty("id").GetString()!;
                        var discordUsername = userJson.GetProperty("username").GetString()!;
                        var discordEmail = userJson.TryGetProperty("email", out var emailProp) ? emailProp.GetString() : null;
                        var discordAvatar = userJson.TryGetProperty("avatar", out var avatarProp) ? avatarProp.GetString() : null;

                        // ساخت URL آواتار
                        string avatarUrl = $"https://cdn.discordapp.com/embed/avatars/0.png"; // پیش‌فرض
                        if (!string.IsNullOrEmpty(discordAvatar))
                        {
                            avatarUrl = $"https://cdn.discordapp.com/avatars/{discordId}/{discordAvatar}.png?size=512";
                        }
                        else if (userJson.TryGetProperty("discriminator", out var discProp) && discProp.GetString() is string discriminator && discriminator != "0")
                        {
                            var defaultIndex = int.Parse(discriminator) % 5;
                            avatarUrl = $"https://cdn.discordapp.com/embed/avatars/{defaultIndex}.png";
                        }

                        // دسترسی به سرویس‌ها
                        var dbContext = context.HttpContext.RequestServices.GetRequiredService<GameVisionDbContext>();
                        var jwtGenerator = context.HttpContext.RequestServices.GetRequiredService<IJwtTokenGenerator>();

                        // پیدا کردن یا ساخت کاربر
                        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.DiscordId == discordId, context.HttpContext.RequestAborted);

                        if (user == null)
                        {
                            user = new User
                            {
                                Username = discordUsername,
                                Email = discordEmail ?? $"{discordId}@discord.user",
                                PasswordHash = "DISCORD_NO_PASSWORD",
                                DiscordId = discordId,
                                DiscordAvatarHash = discordAvatar,
                                ProfileImageUrl = avatarUrl,
                                RoleId = 3, // User عادی
                                CreatedAt = DateTime.UtcNow,
                                IsActive = true
                            };
                            dbContext.Users.Add(user);
                        }
                        else
                        {
                            user.Username = discordUsername;
                            if (!string.IsNullOrEmpty(discordEmail))
                                user.Email = discordEmail;
                            user.ProfileImageUrl = avatarUrl;
                            user.DiscordAvatarHash = discordAvatar;
                            user.LastLogin = DateTime.UtcNow;
                            user.UpdatedAt = DateTime.UtcNow;
                        }

                        await dbContext.SaveChangesAsync(context.HttpContext.RequestAborted);

                        // تولید JWT و ست کردن در کوکی
                        var token = jwtGenerator.GenerateToken(user);

                        context.HttpContext.Response.Cookies.Append("authToken", token, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Lax,
                            Expires = DateTime.UtcNow.AddHours(24)
                        });
                    }
                };
            });

            return services;
        }
    }
}