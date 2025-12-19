using FluentValidation;
using Game_Vision.Application.Command.Auth;
using Game_Vision.Application.Interface;
using Game_Vision.Application.Validator;
using Game_Vision.Models; 
using Microsoft.EntityFrameworkCore;

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
            return services;
        }
    }
}