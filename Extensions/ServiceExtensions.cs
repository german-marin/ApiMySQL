using Microsoft.Extensions.DependencyInjection;
using ApiMySQL.Repositories;
using ApiMySQL.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using ApiMySQL.Services;
using ApiMySQL.Data;

namespace ApiMySQL.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register repositories
            services.AddScoped<IMuscleGroupRepository, MuscleGroupRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IExerciseRepository, ExerciseRepository>();
            services.AddScoped<ITrainingRepository, TrainingRepository>();
            services.AddScoped<ITrainingLineRepository, TrainingLineRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();

            // Register services
            services.AddScoped<IUserService, UserService>();

            // Configure JwtSettings
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            services.AddSingleton<IJwtSettings>(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);

            // Add health checks
            services.AddHealthChecks();

            // Register IHttpContextAccessor
            services.AddHttpContextAccessor();

            // Register ApplicationDbContextFactory
            services.AddSingleton<ApplicationDbContextFactory>();

            // Configure Entity Framework with MySQL for the default connection
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 34));
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseMySql(configuration.GetConnectionString("DefaultConnection"), serverVersion);

                // Logging database connection configuration
                Log.Logger.Information("MySQL ApplicationDbContext configured with connection string: {ConnectionString}", configuration.GetConnectionString("DefaultConnection"));
            });

            return services;
        }
    }
}
