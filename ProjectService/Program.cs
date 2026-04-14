using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using ProjectService.Data;
using ProjectService.Data.Repositories;
using ProjectService.Domain.Interfaces;
using ProjectService.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Add Swagger
builder.Services.AddSwaggerGen();

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

var port = builder.Configuration["PORT"] ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();
