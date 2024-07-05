// Extensions/ServiceExtensions.cs
using Microsoft.Extensions.DependencyInjection;
using ApiMySQL.Repositories;
using ApiMySQL.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using ApiMySQL.Data;
using System;
using ApiMySQL.Services;

namespace ApiMySQL.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IMuscleGroupRepository, MuscleGroupRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IExerciseRepository, ExerciseRepository>();
            services.AddScoped<ITrainingRepository, TrainingRepository>();
            services.AddScoped<ITrainingLineRepository, TrainingLineRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IUserService, UserService>();

            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            services.AddSingleton<IJwtSettings>(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);
            services.AddHealthChecks();

            // Registrar IHttpContextAccessor
            services.AddHttpContextAccessor();

            // Registrar ApplicationDbContextFactory
            services.AddSingleton<ApplicationDbContextFactory>();

            // Configurar Entity Framework con MySQL para la conexión predeterminada
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 34));
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(configuration.GetConnectionString("DefaultConnection"), serverVersion));

            return services;
        }
    }
}
