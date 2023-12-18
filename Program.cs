using ApiMySQL.Data;
using ApiMySQL.Repositories;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RutinAPI", Version = "v1" });
    //Set the comments path for the swagger JSON
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Configurar el sistema de logging con Serilog
Log.Logger = new LoggerConfiguration()    
    .WriteTo.File("logs/mylog-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
//builder.Host.UseSerilog(); // Configurar Serilog como proveedor de logging

// Configurar el sistema de logging
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders(); // Limpiar los proveedores predeterminados
    loggingBuilder.AddSerilog(); // Añadir Serilog como proveedor de logging
});

var mySQLConfiguration = new MySQLConfiguration(builder.Configuration.GetConnectionString("MySQLConnection"));
builder.Services.AddSingleton(mySQLConfiguration);

builder.Services.AddScoped<IMuscleGroupRepository, MuscleGroupRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IExerciseRepository, ExerciseRepository>();
builder.Services.AddScoped<ITrainingRepository, TrainingRepository>();
builder.Services.AddScoped<ITrainingLineRepository, TrainingLineRepository>();

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

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
