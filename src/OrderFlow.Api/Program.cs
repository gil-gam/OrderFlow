using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using OrderFlow.Api.Middleware;
using OrderFlow.Application;
using OrderFlow.Application.Commands.RegisterUser;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.Infrastructure;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Services;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ── Serilog ───────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "OrderFlow.Api")
    .CreateLogger();

builder.Host.UseSerilog();

// ── Services ──────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "OrderFlow API",
        Version = "v1",
        Description = "Order management system - Clean Architecture + CQRS"
    });

    options.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    options.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new()
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ── JWT Authentication ────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "Jwt:Key is not configured. Set it via appsettings or Jwt__Key environment variable.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "OrderFlow.Api";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "OrderFlow.Client";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ── Health Checks ─────────────────────────────────────────
builder.Services.AddHealthChecks()
    .AddDbContextCheck<OrderFlowDbContext>("Database");

// ── CORS ──────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",   // React
                "http://localhost:5173",   // Vite
                "https://seuapp.com"       // Produção
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ── Application Layers ────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommandHandler).Assembly));

var app = builder.Build();

// ── Middleware Pipeline ───────────────────────────────────
app.UseCors("AllowFrontend");
app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderFlow API v1");
    options.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ── Health Check Endpoint ─────────────────────────────────
app.MapHealthChecks("/health");

// ── Database Bootstrap ────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderFlowDbContext>();
    await db.Database.MigrateAsync();
}

// ── Application Start ─────────────────────────────────────
try
{
    Log.Information("Starting OrderFlow API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// ── Test Entry Point ──────────────────────────────────────
public partial class Program { }