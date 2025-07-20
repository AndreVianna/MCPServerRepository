using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MCPHub.Data;

namespace MCPHub.Data.MigrationService;

public class Program {
    public static async Task Main(string[] args) {
        var host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) => {
                services.AddDataServices(context.Configuration);
            })
            .Build();

        // Add migration logic here when needed
        Console.WriteLine("Migration service started. No migrations to run yet.");

        await host.RunAsync();
    }
}