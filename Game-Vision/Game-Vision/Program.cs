using Game_Vision.AppExtention;
using Game_Vision.Domain;
using Game_Vision.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddAuthExtention(builder.Configuration);

// === تنظیم Authentication با Discord ===
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "Discord";
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.SameSite = SameSiteMode.None;          // مهم! None برای third-party redirect
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // چون لوکال HTTPS هست
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
})
.AddDiscord("Discord", options =>
{
    // تنظیمات اصلی از appsettings.json
    options.ClientId = builder.Configuration["Authentication:Discord:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Discord:ClientSecret"]!;

    // فقط مسیر کال‌بک (نه آدرس کامل!)
    options.CallbackPath = "/Account/CallBack";

    // Scopes مورد نیاز
    options.Scope.Add("identify");
    options.Scope.Add("email");
    // اگر بخوای guildها رو هم بگیری: options.Scope.Add("guilds");

    // ذخیره توکن‌ها (مفید برای APIهای بعدی دیسکورد)
    options.SaveTokens = true;

    // === حل مشکل OverflowException برای Discord User ID ===
    // پیش‌فرض ASP.NET Core سعی می‌کنه id رو به Int32 تبدیل کنه → خطا می‌ده
    // ما دستی به string مپ می‌کنیم
    options.ClaimActions.Clear(); // پاک کردن مپینگ‌های پیش‌فرض

    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");           // User ID به string
    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "username");              // نام کاربری
    options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");                 // ایمیل
    options.ClaimActions.MapJsonKey("urn:discord:discriminator", "discriminator");
    options.ClaimActions.MapJsonKey("urn:discord:avatar", "avatar");
    options.ClaimActions.MapJsonKey("urn:discord:global_name", "global_name"); // نام نمایشی (اگر داشته باشه)

    // === گرفتن اطلاعات کاربر از API دیسکورد ===
    options.Events = new OAuthEvents
    {
        OnCreatingTicket = async context =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://discord.com/api/users/@me");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
            var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
            response.EnsureSuccessStatusCode();
            var userJson = await response.Content.ReadFromJsonAsync<JsonElement>();

            context.RunClaimActions(userJson);

            // گرفتن اطلاعات دیسکورد
            var discordId = userJson.GetProperty("id").GetString()!;
            var discordUsername = userJson.GetProperty("username").GetString()!;
            var discordGlobalName = userJson.TryGetProperty("global_name", out var gn) ? gn.GetString() : null;
            var discordEmail = userJson.TryGetProperty("email", out var e) ? e.GetString() : null;
            var discordAvatar = userJson.TryGetProperty("avatar", out var a) ? a.GetString() : null;

            // ساخت URL آواتار اگر hash داشته باشه
            string? avatarUrl = null;
            if (!string.IsNullOrEmpty(discordAvatar))
            {
                avatarUrl = $"https://cdn.discordapp.com/avatars/{discordId}/{discordAvatar}.png?size=512";
            }
            else
            {
                // آواتار پیش‌فرض دیسکورد
                var discriminator = userJson.GetProperty("discriminator").GetString();
                var defaultIndex = int.Parse(discriminator) % 5;
                avatarUrl = $"https://cdn.discordapp.com/embed/avatars/{defaultIndex}.png";
            }

            // دسترسی به DbContext
            var dbContext = context.HttpContext.RequestServices.GetRequiredService<GameVisionDbContext>();

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.DiscordId == discordId);

            if (user == null)
            {
                // کاربر جدید از دیسکورد
                user = new User
                {
                    Username = discordUsername, // یا discordGlobalName اگر ترجیح می‌دی
                    Email = discordEmail ?? $"{discordId}@discord.user", // اگر ایمیل نداد، یه فیک بساز
                    PasswordHash = "DISCORD_NO_PASSWORD", // یا Guid.NewGuid().ToString()
                    DiscordId = discordId,
                    DiscordAvatarHash = discordAvatar,
                    ProfileImageUrl = avatarUrl,
                    RoleId = 1, // مثلاً نقش کاربر معمولی (باید نقش پیش‌فرض داشته باشی)
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                dbContext.Users.Add(user);
            }
            else
            {
                // کاربر قدیمی → آپدیت اطلاعات
                user.Username = discordUsername;
                if (!string.IsNullOrEmpty(discordEmail))
                    user.Email = discordEmail;

                user.ProfileImageUrl = avatarUrl;
                user.DiscordAvatarHash = discordAvatar;
                user.LastLogin = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;
                user.DiscordLastSync = DateTime.UtcNow;
            }

            await dbContext.SaveChangesAsync(context.HttpContext.RequestAborted);

            // اختیاری: اضافه کردن Claim برای ID داخلی اپ
            context.Identity?.AddClaim(new Claim("AppUserId", user.Id.ToString()));
        }
    };
});

builder.Host.UseGameVisionLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
// app.UseGameVisionRequestLogging(); // اگر لازم داشتی آنکامنت کن
app.UseStaticFiles(); // یا app.MapStaticAssets() اگر اکستنشن خودت داری

app.UseRouting();

app.UseAuthentication();  // حتماً قبل از UseAuthorization
app.UseAuthorization();

app.MapStaticAssets(); // اگر اکستنشن جداگانه داری

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();