using Common.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

// Add MCP Hub infrastructure with comprehensive setup
builder.AddMcpHubInfrastructure();

builder.Build().Run();