using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Portfolio.Project.Data;
using Portfolio.Project.Data.Repositories;
using Portfolio.Project.Domain.Interfaces;
using Portfolio.Project.Endpoints;

var builder = WebApplication.CreateBuilder(args);

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

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ProjectServiceDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProjectServiceDbContext>();
    await db.Database.MigrateAsync();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Service API v1");
    c.SupportedSubmitMethods();
});

app.MapHealthChecks("/health");
app.MapProjectsEndpoints();

app.Run();
