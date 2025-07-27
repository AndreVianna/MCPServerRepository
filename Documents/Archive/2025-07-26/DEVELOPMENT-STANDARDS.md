# MCP Hub Development Standards

## Technology Stack

- **.NET 9 Aspire**: Microservices orchestration and cloud-native architecture
- **Azure**: Primary cloud provider
- **PostgreSQL**: Primary database with EF Core
- **Redis**: Caching and session management
- **RabbitMQ**: Message queue for inter-service communication
- **Blazor Web App**: SSR + WASM AOT web portal
- **Native AOT CLI**: High-performance command-line tool
- **Elasticsearch**: Full-text search capabilities
- **Qdrant**: Vector database for semantic search

## Development Approach

- **Use proper documentation reference** for all libraries, frameworks, APIs, and tools serach the web for correct and latest usage.
- **Agile delivery cycles** with clear MVPs and incremental value
- **Sequential thinking** for complex problem-solving
- **Parallel development** opportunities to maximize efficiency
- **Clean architecture** principles with comprehensive testing
- **Security-first** approach with three-stage validation
- **Standardized tooling**: dotnet CLI, dotnet ef, Figma MCP, Playwright MCP
- **Command-line driven** development for .NET and Entity Framework
- **MCP-based tools** for UI design and testing workflows
- **Maintain project progress** in memory for context continuity
- **Memory updates** should analyze the full memory graph to avoid duplications and remove stale items
- **TODO tracking** must be maintained in memory for task continuity across sessions

## Development Standards

**IMPORTANT**: All development work must follow these standardized tool requirements:

- **Use .NET 9 and C#13 latest techniques** consult microsoft documentation for best practices
- **All .NET instructions and scaffolding** must be done using `dotnet` commands within the project.sh script
- **All Entity Framework tasks** must be done with the `dotnet ef` tool within the container environment
- **All UI design** must be done using the **Figma MCP** for design work
- **All UI testing and visualization** must be done using the **Playwright MCP**
- **One-class-per-file organizational principle** strictly enforced
- **Uses xUnit testing framework** with AwesomeAssertions (not FluentAssertions)
- **Use Global using statements** for consistent imports across projects
- **Command-line driven development** approach
- **MCP-based tools** for UI design and testing workflows
- **Maintains project progress in memory** for context continuity

These requirements apply to all development phases and teams to ensure consistency and leverage MCP-based tooling where appropriate.
