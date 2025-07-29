var builder = DistributedApplication.CreateBuilder(args);

// Note: External services removed during cleanup - only interface abstractions available
// Future implementations will add specific providers as needed

// Configure basic service projects
builder.AddProject<Projects.MCPHub_PublicApi>("publicapi");

// TODO: WebApp temporarily removed due to compilation errors
//builder.AddProject<Projects.MCPHub_WebApp>("webapp");

builder.AddProject<Projects.MCPHub_SecurityService>("securityservice");

builder.AddProject<Projects.MCPHub_SearchService>("searchservice");

builder.Build().Run();