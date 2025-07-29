using MCPHub.AuthenticationService.Services;
using MCPHub.Data;
using MCPHub.Data.Extensions;
using MCPHub.Domain.Entities;

using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Data layer services
builder.Services.AddDataServices(builder.Configuration);

// Add Identity services (placeholder configuration)
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options => {
    // Password requirements will be configured when implementation is needed
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<McpHubContext>()
.AddDefaultTokenProviders();

// Register authentication services (skeletons)
builder.Services.AddScoped<IAuthenticationService, MCPHub.AuthenticationService.Services.AuthenticationService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();

// JWT Authentication (placeholder - will be configured when implementation is needed)
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options => { /* JWT configuration */ });

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Authentication middleware (placeholder)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();