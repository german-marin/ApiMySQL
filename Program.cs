using ApiMySQL.Extensions;
using Serilog;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Reflection;
using ApiMySQL.Data;
using ApiMySQL.Middleware;
using ApiMySQL.Mapping;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add custom DbContexts
builder.Services.AddCustomDbContexts(builder.Configuration);

// Add custom authentication
builder.Services.AddCustomAuthentication(builder.Configuration);

// Add custom services
builder.Services.AddCustomServices(builder.Configuration);

// Register the DbContextMiddleware
builder.Services.AddSingleton<ApplicationDbContextFactory>();
builder.Services.AddHttpContextAccessor();

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RutinAPI", Version = "v1" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] { }
        }
    });
});

// Leer configuraci�n de Serilog desde appsettings.json
var loggerConfiguration = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

Log.Logger = loggerConfiguration;

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddSerilog(loggerConfiguration);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RutinAPI V1");
    });
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// A�adir el middleware de DbContext 
app.UseMiddleware<DbContextMiddleware>();

app.UseMiddleware<RequestCorrelationMiddleware>();

app.MapControllers();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
