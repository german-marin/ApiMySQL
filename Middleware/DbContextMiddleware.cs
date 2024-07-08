using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;
using ApiMySQL.Data;
using ApiMySQL.Services;

public class DbContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ApplicationDbContextFactory _dbContextFactory;

    public DbContextMiddleware(RequestDelegate next, ApplicationDbContextFactory dbContextFactory)
    {
        _next = next;
        _dbContextFactory = dbContextFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            var userService = context.RequestServices.GetService<IUserService>();
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();

                var schemaNameClaim = context.User.Claims.FirstOrDefault(c => c.Type == "SchemaName")?.Value;
                if (!string.IsNullOrEmpty(schemaNameClaim))
                {
                    var dbContext = _dbContextFactory.CreateDbContext(schemaNameClaim);
                    context.Items["DbContext"] = dbContext;

                    // Log successful creation of DbContext
                    Log.Logger.Information("DbContext created for SchemaName: {SchemaName}", schemaNameClaim);
                }
            }

            await _next(context);
        }
        catch (Exception ex)
        {
            // Log any exceptions that occur during middleware execution
            Log.Logger.Error(ex, "Error in DbContextMiddleware: {ErrorMessage}", ex.Message);

            // Re-throw the exception to be handled by the global exception handler
            throw;
        }
    }
}
