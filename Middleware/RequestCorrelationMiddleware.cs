using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Threading.Tasks;

namespace ApiMySQL.Middleware
{
    public class RequestCorrelationMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestCorrelationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = Guid.NewGuid().ToString();
            context.Items["CorrelationId"] = correlationId;
            context.Response.Headers.Add("X-Correlation-ID", correlationId);

            // Log correlation ID for incoming request
            Log.Logger.Information("Incoming request with CorrelationId: {CorrelationId}", correlationId);

            using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }

            // Log completion of request
            Log.Logger.Information("Request with CorrelationId: {CorrelationId} completed", correlationId);
        }
    }
}
