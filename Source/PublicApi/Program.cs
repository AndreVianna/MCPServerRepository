using System.Text;

using Asp.Versioning;

using MCPHub.Common.Messaging;
using MCPHub.Common.Services;
using MCPHub.Data;
using MCPHub.Domain;
using MCPHub.Domain.Entities;
using MCPHub.Domain.Messaging;
using MCPHub.PublicApi.Configuration;
using MCPHub.PublicApi.Consumers;
using MCPHub.PublicApi.Middleware;
using MCPHub.PublicApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add API versioning
builder.Services.AddApiVersioning(options => {
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-API-Version"),
        new HeaderApiVersionReader("Accept-Version")
    );
}).AddApiExplorer(setup => {
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure options
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.Configure<RateLimitingOptions>(builder.Configuration.GetSection(RateLimitingOptions.SectionName));

// Register Data layer services (repositories, DbContext, etc.)
builder.Services.AddDataServices(builder.Configuration);

// Register Domain layer services (application services)
builder.Services.AddDomainServices();

// Add ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options => {
    // Password requirements
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;

    // Sign-in settings
    options.SignIn.RequireConfirmedEmail = false; // Will be enabled when email service is implemented
})
.AddEntityFrameworkStores<McpHubContext>()
.AddDefaultTokenProviders();

// Add JWT Authentication
var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
var secretKey = jwtSection.GetValue<string>("SecretKey") ??
    throw new InvalidOperationException("JWT SecretKey is not configured");
var issuer = jwtSection.GetValue<string>("Issuer") ??
    throw new InvalidOperationException("JWT Issuer is not configured");
var audience = jwtSection.GetValue<string>("Audience") ??
    throw new InvalidOperationException("JWT Audience is not configured");

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.FromMinutes(5),
    };

    options.Events = new JwtBearerEvents {
        OnAuthenticationFailed = context => {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("JWT authentication failed: {Error}", context.Exception.Message);
            return Task.CompletedTask;
        },
        OnChallenge = context => {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("JWT authentication challenge triggered for path: {Path}", context.Request.Path);
            return Task.CompletedTask;
        },
    };
});

// Add Authorization
builder.Services.AddAuthorization();

// Register JWT service
builder.Services.AddScoped<IJwtService, JwtService>();

// Add caching services for rate limiting  
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache(); // Simple in-memory distributed cache

// Register rate limiting service
builder.Services.AddSingleton<IRateLimitingService, RateLimitingService>();

// Note: Messaging implementations removed - only interfaces available
// Add minimal stub implementations for messaging
builder.Services.AddScoped<IMessagePublisher>(provider =>
    new StubMessagePublisher(provider.GetRequiredService<ILogger<StubMessagePublisher>>()));

// Register consumer services
builder.Services.AddScoped<ServerRegisteredEventConsumer>();

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Rate limiting middleware (before authentication to protect auth endpoints)
app.UseRateLimiting();

// Authentication & Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Temporary stub implementation for messaging
public class StubMessagePublisher(ILogger<StubMessagePublisher> logger) : IMessagePublisher {
    private readonly ILogger<StubMessagePublisher> _logger = logger;

    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : BaseMessage {
        _logger.LogInformation("Stub: Publishing message of type {MessageType}", typeof(T).Name);
        return Task.CompletedTask;
    }

    public Task PublishAsync<T>(T message, string routingKey, CancellationToken cancellationToken = default) where T : BaseMessage {
        _logger.LogInformation("Stub: Publishing message of type {MessageType} with routing key {RoutingKey}", typeof(T).Name, routingKey);
        return Task.CompletedTask;
    }
}

// Make Program class accessible for integration testing
public partial class Program { }