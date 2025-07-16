# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is **MCP Hub**, a comprehensive MCP (Model Context Protocol) server registration repository that will work like NPM and Nuget. Authors will be able to register their servers and users will be able to search and use the servers in their projects.

**Current Status**: Detailed roadmap completed - ready for implementation phase execution

### Key Features

- **Three-stage security model**: fetch → verify → install with comprehensive scanning
- **Trust tier system**: Progressive trust levels from unverified to certified
- **Native AOT CLI**: High-performance mcpm tool for package management
- **Blazor Web App**: Rich web portal for package discovery and management
- **Semantic search**: AI-powered search with vector embeddings
- **Enterprise features**: Organization management, private registries, compliance

## Development Environment Setup

### Custom Commands

- `prime` - Prepares context for working with the project by loading memory and applying sequential thinking approach
- `plan` - Creates detailed implementation roadmaps using parallel sub-agent analysis

### Development Tool Requirements

**IMPORTANT**: All development work must follow these standardized tool requirements:

- **All .NET instructions and scaffolding** must be done using `dotnet` commands
- **All Entity Framework tasks** must be done with the `dotnet ef` tool
- **All UI design** must be done using the **Figma MCP** for design work
- **All UI testing and visualization** must be done using the **Playwright MCP**

These requirements apply to all development phases and teams to ensure consistency and leverage MCP-based tooling where appropriate.

## Project Structure Guidelines

**IMPORTANT**: All development work must follow these project structure guidelines based on Clean Architecture and Domain-Driven Design principles:

### Core Principles

- **Clean Architecture**: Clear separation between Domain, Application, Infrastructure, and Presentation layers
- **Domain-Driven Design**: Proper bounded contexts with domain entities, value objects, and services
- **Template-Based Organization**: Consistent patterns that scale horizontally with new features
- **Comprehensive Testing**: 1:1 mapping between main projects and test projects
- **Separation of Concerns**: Each layer has specific responsibilities with proper dependency flow

### Application Types Structure

All applications in the MCP Hub solution follow consistent organizational patterns:

``` text
Source/
├── Domain/                        # Pure business logic (entities, value objects, domain services)
├── Common/                        # Shared infrastructure components
├── Core/                          # Shared utilities and helpers
├── Data/                          # Data access and Entity Framework context
├── Data.MigrationService/         # Database migration service
├── CommandLineApp/                # Native AOT CLI tool (mcpm)
├── CommandLineApp.UnitTests/      # CLI comprehensive testing
├── PublicApi/                     # Public registry API endpoints
├── PublicApi.UnitTests/           # API comprehensive testing
├── WebApp/                        # Blazor web portal (SSR + WASM)
├── WebApp.UnitTests/              # Web app comprehensive testing
├── SecurityService/               # Security scanning and analysis
├── SecurityService.UnitTests/     # Security service testing
├── SearchService/                 # Search and discovery service
├── SearchService.UnitTests/       # Search service testing
└── AppHost/                       # .NET Aspire orchestration
```

### Naming Conventions

- **Projects**: Use descriptive names (CommandLineApp, PublicApi, WebApp, SecurityService)
- **Test Projects**: Add `.UnitTests` suffix to main project name
- **Folders**: Use PascalCase with clear, descriptive names
- **Domain Entities**: Use `{DomainEntity}` template pattern for scalability
- **Services**: Use `{DomainEntity}Service` pattern for domain-specific services

### Layer Organization

#### Domain Layer (`Domain/`)

- **Pure business logic** with no external dependencies
- **Entities**: Business objects with identity and behavior
- **Value Objects**: Immutable objects representing concepts
- **Domain Services**: Business operations that don't belong to a single entity
- **Repositories**: Interfaces for data access (implementation in Infrastructure)

#### Application Layer (`{ApplicationType}/`)

- **Use cases and application services**
- **Commands and queries** (CQRS pattern)
- **Application-specific business rules**
- **Interfaces for external services**

#### Infrastructure Layer (`Data/`, `Common/`)

- **Data access implementation** (Entity Framework)
- **External service integrations**
- **Cross-cutting concerns** (logging, caching, messaging)

#### Presentation Layer (`CommandLineApp/`, `PublicApi/`, `WebApp/`)

- **User interface and API endpoints**
- **Input validation and formatting**
- **Authentication and authorization**
- **Request/response handling**

### Testing Strategy

- **Comprehensive Coverage**: Every project has corresponding `.UnitTests` project
- **Mirror Structure**: Test projects mirror the folder structure of main projects
- **Testing Framework**: Use xUnit with proper isolation and test utilities
- **Mocking**: Use nSubstitute for creating mocks and stubs
- **Assertions**: Use AwesomeAssertions (not FluentAssertions) for assert clauses
- **Global Usings**: Consistent using statements across test projects
- **Test Categories**: Unit, Integration, End-to-End testing as appropriate

### Folder Structure Standards

Each project follows consistent internal organization:

``` text
ProjectName/
├── EndpointMappers/              # API endpoint mapping (for services)
├── Handlers/                     # Command/query handlers
├── Services/                     # Application services
├── Models/                       # DTOs and request/response models
├── Extensions/                   # Extension methods
├── Utilities/                    # Helper classes
├── GlobalUsings.cs               # Global using statements
└── Program.cs                    # Application entry point
```

### Integration with Development Tools

- **dotnet CLI**: All scaffolding and project creation
- **dotnet ef**: All Entity Framework migrations and database operations
- **Figma MCP**: All UI design and mockup creation
- **Playwright MCP**: All UI testing and browser automation
- **xUnit**: All unit and integration testing
- **.NET Aspire**: All service orchestration and deployment

## Architecture

### Technology Stack

- **.NET 9 Aspire**: Microservices orchestration and cloud-native architecture
- **PostgreSQL**: Primary database with EF Core
- **Redis**: Caching and session management
- **Elasticsearch**: Full-text search capabilities
- **Qdrant**: Vector database for semantic search
- **RabbitMQ**: Message queue for inter-service communication
- **Blazor Web App**: SSR + WASM AOT web portal
- **Native AOT CLI**: High-performance command-line tool

### Current Structure

- **/.claude/**: Claude Code configuration and memory persistence
- **/Documents/**: Project documentation and roadmaps
  - **Detailed Roadmap.md**: Comprehensive 12-month implementation plan
  - **Project Details.md**: Complete system specifications
  - **Technical Plan.md**: Technical implementation details
  - **Technical Blueprint.md**: Consolidated technical blueprint
  - **High-Level To-Do List.md**: Sprint-based task breakdown
- **No source code directories yet** - implementation pending

### Development Approach

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

## Git Repository

- **Main branch**: `main`
- **License**: MIT
- **Current uncommitted changes**: Configuration files and setup scripts

## Implementation Roadmap

The project follows a **4-phase, 12-month roadmap** with clear dependencies and parallel execution opportunities:

### Phase 1: Foundation (Months 1-3)

- .NET Aspire infrastructure setup
- PostgreSQL database and EF Core models
- Basic authentication with JWT and OAuth2
- Core API services foundation
- CLI infrastructure and web portal base

### Phase 2: Core Functionality (Months 4-6)

- Three-stage security model implementation
- Package management APIs and workflows
- CLI core commands (fetch, verify, install)
- Web portal package discovery and management
- Basic search with PostgreSQL full-text

### Phase 3: Advanced Features (Months 7-9)

- Elasticsearch and semantic search with Qdrant
- Trust tier system and Security Council governance
- Real-time features with SignalR
- Enterprise organization management
- Advanced security scanning and monitoring

### Phase 4: Production Ready (Months 10-12)

- Performance optimization and global scaling
- Comprehensive testing and quality assurance
- Documentation and community platform
- Production deployment and monitoring
- Launch and post-launch support

## Success Metrics

- **Technical**: API <100ms (p95), CLI <5ms startup, 99.9% uptime
- **Security**: 100% scan coverage, <48h vulnerability response
- **Business**: 2000+ developers, 1500+ packages, 30% community-trusted
- **User Experience**: >4.5/5 satisfaction, >95% task completion rate
