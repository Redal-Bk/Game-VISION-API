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

            // اینجا می‌تونی بقیه سرویس‌ها رو اضافه کنی، مثلاً:
            //services.AddAutoMapper(typeof(MappingProfile));
            //services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SomeHandler).Assembly));
            //services.AddScoped<IGameRepository, GameRepository>();
            //services.AddAuthentication(...)

            return services;
        }
    }
}