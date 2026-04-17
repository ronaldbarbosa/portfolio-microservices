using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Portfolio.Project.Data;
using Portfolio.Project.Data.Repositories;
using Portfolio.Project.Domain.Interfaces;
using Portfolio.Project.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Project Service API",
        Version = "v1"
    });
    
    c.CustomSchemaIds(type =>
    {
        if (type.Name != "Request") return type.Name;
        var declaringType = type.DeclaringType?.Name;
        return declaringType + "Request";
    });
});

// Add DbContext
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ProjectServiceDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add Repositories
builder.Services.AddTransient<IProjectRepository, ProjectRepository>();

// Add OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Service API v1"));
}

app.MapProjectsEndpoints();

app.Run();
