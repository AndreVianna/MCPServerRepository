using System.CommandLine;

using MCPHub.CommandLineApp.Commands;
using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Extensions;
using MCPHub.CommandLineApp.Services;
using MCPHub.CommandLineApp.Utilities;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Create application builder
var builder = Host.CreateApplicationBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Configure configuration sources
builder.Configuration.AddJsonFile("appsettings.json", optional: true);
builder.Configuration.AddEnvironmentVariables("MCPM_");
builder.Configuration.AddCommandLine(args);

// Load MCPM configuration
var configManager = new McpmConfigurationManager(
    builder.Services.BuildServiceProvider().GetRequiredService<ILogger<McpmConfigurationManager>>());
var mcpmConfig = await configManager.LoadConfigurationAsync();

// Register core services
builder.Services.AddSingleton(mcpmConfig);
builder.Services.AddSingleton<IMcpmConfigurationManager>(configManager);
builder.Services.AddHttpClient<McpHubApiClient>();

// Register application services using extension methods
builder.Services.AddCachingServices();
builder.Services.AddApiClientServices();
builder.Services.AddAuthenticationServices();
builder.Services.AddPackageManagementServices();
builder.Services.AddOutputServices();
builder.Services.AddInteractiveServices();
builder.Services.AddConfigurationServices();
builder.Services.AddSecurityServices();

// Register commands
builder.Services.AddTransient<SearchCommand>();
builder.Services.AddTransient<InfoCommand>();
builder.Services.AddTransient<ListCommand>();
builder.Services.AddTransient<PublishCommand>();
builder.Services.AddTransient<InstallCommand>();
builder.Services.AddTransient<UpdateCommand>();
builder.Services.AddTransient<UninstallCommand>();
builder.Services.AddTransient<DoctorCommand>();
builder.Services.AddTransient<AuthCommand>();
builder.Services.AddTransient<ConfigCommand>();
builder.Services.AddTransient<VerifyCommand>();
builder.Services.AddTransient<SecurityCommand>();
builder.Services.AddTransient<CacheCommand>();

// Build the application
var app = builder.Build();

// Create root command
var rootCommand = new RootCommand("MCP Package Manager (mcpm) - Discover, install, and manage MCP servers");

// Add global options
var verboseOption = new Option<bool>(
    aliases: ["--verbose", "-v"],
    description: "Enable verbose output");
rootCommand.AddGlobalOption(verboseOption);

var quietOption = new Option<bool>(
    aliases: ["--quiet", "-q"],
    description: "Suppress non-essential output");
rootCommand.AddGlobalOption(quietOption);

var configOption = new Option<FileInfo?>(
    aliases: ["--config"],
    description: "Use specific configuration file");
rootCommand.AddGlobalOption(configOption);

var registryOption = new Option<string?>(
    aliases: ["--registry"],
    description: "Use alternative registry URL");
rootCommand.AddGlobalOption(registryOption);

// Add commands
var searchCommand = app.Services.GetRequiredService<SearchCommand>();
var infoCommand = app.Services.GetRequiredService<InfoCommand>();
var listCommand = app.Services.GetRequiredService<ListCommand>();
var publishCommand = app.Services.GetRequiredService<PublishCommand>();
var installCommand = app.Services.GetRequiredService<InstallCommand>();
var updateCommand = app.Services.GetRequiredService<UpdateCommand>();
var uninstallCommand = app.Services.GetRequiredService<UninstallCommand>();
var doctorCommand = app.Services.GetRequiredService<DoctorCommand>();
var authCommand = app.Services.GetRequiredService<AuthCommand>();
var configCommand = app.Services.GetRequiredService<ConfigCommand>();
var verifyCommand = app.Services.GetRequiredService<VerifyCommand>();
var securityCommand = app.Services.GetRequiredService<SecurityCommand>();
var cacheCommand = app.Services.GetRequiredService<CacheCommand>();

rootCommand.AddCommand(searchCommand.CreateCommand());
rootCommand.AddCommand(infoCommand.CreateCommand());
rootCommand.AddCommand(listCommand.CreateCommand());
rootCommand.AddCommand(publishCommand.CreateCommand());
rootCommand.AddCommand(installCommand.CreateCommand());
rootCommand.AddCommand(updateCommand.CreateCommand());
rootCommand.AddCommand(uninstallCommand.CreateCommand());
rootCommand.AddCommand(doctorCommand.CreateCommand());
rootCommand.AddCommand(authCommand.CreateCommand());
rootCommand.AddCommand(configCommand.CreateCommand());
rootCommand.AddCommand(verifyCommand.CreateCommand());
rootCommand.AddCommand(securityCommand.CreateCommand());
rootCommand.AddCommand(cacheCommand.CreateCommand());

// Add version handling
rootCommand.SetHandler(() => {
    var outputFormatter = app.Services.GetRequiredService<IOutputFormatter>();
    outputFormatter.WriteInfo("MCP Package Manager (mcpm) v1.0.0");
    outputFormatter.WriteInfo("Use 'mcpm --help' to see available commands");
    return Task.FromResult(0);
});

// Handle global options (basic implementation)
rootCommand.SetHandler((bool verbose, bool quiet, FileInfo? config, string? registry) => {
    // Update configuration based on global options
    if (verbose) {
        mcpmConfig.Ui.VerboseErrors = true;
    }

    if (quiet) {
        mcpmConfig.Ui.ProgressBars = false;
    }

    if (!string.IsNullOrEmpty(registry)) {
        mcpmConfig.Registry.Url = registry;
    }

    return Task.FromResult(0);
}, verboseOption, quietOption, configOption, registryOption);

// Execute the command
try {
    return await rootCommand.InvokeAsync(args);
}
catch (Exception ex) {
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    var outputFormatter = app.Services.GetRequiredService<IOutputFormatter>();

    logger.LogError(ex, "Unhandled exception in main program");
    outputFormatter.WriteError("An unexpected error occurred");

    if (mcpmConfig.Ui.VerboseErrors) {
        outputFormatter.WriteError(ex.ToString());
    }

    return 1;
}