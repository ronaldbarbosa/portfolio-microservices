using Microsoft.EntityFrameworkCore;
using ProjectService.Data;
using ProjectService.Data.Repositories;
using ProjectService.Domain.Interfaces;
using ProjectService.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Add DbContext
builder.Services.AddDbContext<ProjectServiceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Repositories
builder.Services.AddTransient<IProjectRepository, ProjectRepository>();

// Add OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapProjectsEndpoints();

var port = builder.Configuration["PORT"] ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();
