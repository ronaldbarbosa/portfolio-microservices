using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("yarp"));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var keycloakConfig = builder.Configuration.GetSection("Authentication:KeyCloak");

        options.Authority = keycloakConfig["Authority"];
        options.RequireHttpsMetadata = keycloakConfig.GetValue<bool>("RequireHttpsMetadata");

        var validIssuers = keycloakConfig.GetSection("ValidIssuers").Get<string[]>() ?? [];
        var validateAudience = keycloakConfig.GetValue<bool>("ValidateAudience");
        var audience = keycloakConfig["Audience"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = validateAudience,
            ValidAudiences = validateAudience && !string.IsNullOrEmpty(audience) ? [audience] : null,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuers = validIssuers.Length > 0 ? validIssuers : null
        };
    });

builder.Services
    .AddAuthorization(options =>
    {
        options.AddPolicy(
            "authenticated", policy => policy.RequireAuthenticatedUser()
        );
    });

var app = builder.Build();

app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.MapGet("/health", () => "OK");

app.Run();
