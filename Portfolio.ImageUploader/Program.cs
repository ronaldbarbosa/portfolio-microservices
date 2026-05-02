using MassTransit;
using Microsoft.OpenApi;
using Portfolio.ImageUploader.Consumers;
using Portfolio.ImageUploader.Endpoints;
using Portfolio.ImageUploader.Middleware;
using Portfolio.ImageUploader.Services;

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Image Uploader API",
        Version = "v1"
    });
});

var rabbitMqConnectionString = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION_STRING")
    ?? builder.Configuration["RabbitMq:ConnectionString"];

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProjectDeletedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(rabbitMqConnectionString!));
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.Configure<HostOptions>(options =>
{
    options.ServicesStartConcurrently = true;
    options.ServicesStopConcurrently = true;
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddScoped<IImageStorageService, CloudinaryImageStorageService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Image Uploader API v1");
    c.SupportedSubmitMethods();
});

app.MapHealthChecks("/health");
app.MapImageEndpoints();

app.Run();
