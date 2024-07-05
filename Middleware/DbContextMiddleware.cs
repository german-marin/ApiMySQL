// Middleware/DbContextMiddleware.cs
using Microsoft.AspNetCore.Http;
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
        var userService = context.RequestServices.GetService<IUserService>();
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();

            var schemaNameClaim = context.User.Claims.FirstOrDefault(c => c.Type == "SchemaName")?.Value;
            if (!string.IsNullOrEmpty(schemaNameClaim))
            {
                context.Items["DbContext"] = _dbContextFactory.CreateDbContext(schemaNameClaim);
            }
        }

        await _next(context);
    }
}
