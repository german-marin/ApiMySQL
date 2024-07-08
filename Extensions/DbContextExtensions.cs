using ApiMySQL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Serilog;
using System;

namespace ApiMySQL.Extensions
{
    public static class DbContextExtensions
    {
        public static IServiceCollection AddCustomDbContexts(this IServiceCollection services, IConfiguration configuration)
        {
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 34));

            services.AddDbContext<CommonDbContext>(options =>
            {
                options.UseMySql(configuration.GetConnectionString("DefaultConnection"), serverVersion);

                // Logging database connection configuration
                Log.Logger.Information("MySQL DbContext configured with connection string: {ConnectionString}", configuration.GetConnectionString("DefaultConnection"));
            });

            // Register ApplicationDbContextFactory as a singleton
            services.AddSingleton<ApplicationDbContextFactory>();

            return services;
        }
    }
}
