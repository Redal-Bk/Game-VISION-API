namespace Game_Vision.AppExtention
{
    public static class IdentityExtention
    {
        public static IServiceCollection AddAuthExtention(this IServiceCollection services,IConfiguration configuration)
        {
        

  
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("ModeratorOnly", policy => policy.RequireRole("Moderator"));
                
            });

            return services;
        }
    }
}
