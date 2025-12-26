using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SurveyBackend.Api.Middleware;
using SurveyBackend.Api.Services;
using SurveyBackend.Api.Authorization;
using SurveyBackend.Api.Extensions;
using SurveyBackend.Api.Health;
using SurveyBackend.Application;
using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Infrastructure;
using SurveyBackend.Infrastructure.BackgroundServices;
using SurveyBackend.Infrastructure.Configurations;
using SurveyBackend.Infrastructure.Persistence;
using SurveyBackend.Infrastructure.Seeding;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Bearer {token}",
        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.SetIsOriginAllowed(_ => true) // TODO: tighten to specific origins in prod
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database");

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
                   ?? throw new InvalidOperationException("Jwt ayarları bulunamadı.");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue(jwtSettings.AccessTokenCookieName, out var token) &&
                    !string.IsNullOrWhiteSpace(token))
                {
                    context.Token = token;
                    return Task.CompletedTask;
                }

                var authorizationHeader = context.Request.Headers.Authorization.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(authorizationHeader) &&
                    authorizationHeader.StartsWith("Bearer ", System.StringComparison.OrdinalIgnoreCase))
                {
                    context.Token = authorizationHeader["Bearer ".Length..].Trim();
                }

                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                // Allow [AllowAnonymous] endpoints to proceed even if token validation fails
                var endpoint = context.HttpContext.GetEndpoint();
                var allowAnonymous = endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.IAllowAnonymous>() != null;

                if (allowAnonymous)
                {
                    context.NoResult();
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

// Register background services
builder.Services.AddHostedService<TokenCleanupService>();

var app = builder.Build();

// Apply migrations automatically in development
if (app.Environment.IsDevelopment())
{
    await app.ApplyDatabaseMigrationsAsync();
}

// Seed essential data in ALL environments (dev, test, prod)
// This creates the system department, permissions, roles, and admin user
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Add rate limiting middleware before authentication
app.UseMiddleware<LoginRateLimitingMiddleware>();

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
