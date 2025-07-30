using System.Text;

using MCPHub.WebApp.Components;
using MCPHub.WebApp.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;

using MudBlazor.Services;

namespace MCPHub.WebApp;

public class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        // Configuration
        var configuration = builder.Configuration;
        var publicApiUrl = configuration.GetValue<string>("PublicApiUrl") ?? "https://localhost:7001";
        var jwtKey = configuration.GetValue<string>("JwtSettings:Key") ?? "your-secret-key-here-must-be-at-least-256-bits";
        var jwtIssuer = configuration.GetValue<string>("JwtSettings:Issuer") ?? "MCPHub";
        var jwtAudience = configuration.GetValue<string>("JwtSettings:Audience") ?? "MCPHub.WebApp";

        // Add services to the container
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        // Add MudBlazor services
        builder.Services.AddMudServices(config => {
            config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomRight;
            config.SnackbarConfiguration.PreventDuplicates = false;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 10000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
        });

        // Theme service is static, no registration needed

        // Add authentication services
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            });

        builder.Services.AddAuthorizationCore();

        // Add HTTP client services
        builder.Services.AddHttpClient<IApiClientService, ApiClientService>(client => {
            client.BaseAddress = new Uri(publicApiUrl);
            client.DefaultRequestHeaders.Add("User-Agent", "MCPHub.WebApp/1.0");
        });

        builder.Services.AddHttpClient<IAuthenticationService, AuthenticationService>(client => {
            client.BaseAddress = new Uri(publicApiUrl);
            client.DefaultRequestHeaders.Add("User-Agent", "MCPHub.WebApp/1.0");
        });

        // Add custom services
        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
        builder.Services.AddScoped<CustomAuthenticationStateProvider>(provider =>
            (CustomAuthenticationStateProvider)provider.GetRequiredService<AuthenticationStateProvider>());
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<IApiClientService, ApiClientService>();

        // Add publisher dashboard services
        builder.Services.AddScoped<IPublisherDashboardService, PublisherDashboardService>();
        builder.Services.AddScoped<IPackageManagementService, PackageManagementService>();
        builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
        builder.Services.AddScoped<IPublisherProfileService, PublisherProfileService>();

        // Add user profile and security services
        builder.Services.AddScoped<IUserProfileService, UserProfileService>();
        builder.Services.AddScoped<ISecurityService, SecurityService>();

        // Add export service
        builder.Services.AddScoped<IExportService, ExportService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (!app.Environment.IsDevelopment()) {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}