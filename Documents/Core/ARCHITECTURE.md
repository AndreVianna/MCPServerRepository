# MCP Hub Architecture Guidelines

## Project Structure Guidelines

**IMPORTANT**: All development work must follow these project structure guidelines based on Clean Architecture and Domain-Driven Design principles.

### Core Principles

- **Clean Architecture**: Clear separation between Domain, Application, Infrastructure, and Presentation layers
- **Domain-Driven Design**: Proper bounded contexts with domain entities, value objects, and services
- **Template-Based Organization**: Consistent patterns that scale horizontally with new features
- **Comprehensive Testing**: 1:1 mapping between main projects and test projects
- **Separation of Concerns**: Each layer has specific responsibilities with proper dependency flow

### Application Types Structure

All applications in the MCP Hub solution follow consistent organizational patterns:

```
Source/
├── AppHost/                       # .NET Aspire orchestration
├── Core/                          # Shared utilities and helpers
├── Domain/                        # Pure business logic (entities, value objects, domain services)
├── Common/                        # Shared infrastructure components
├── Data/                          # Data access and Entity Framework context
├── Data.MigrationService/         # Database migration service
├── SecurityService/               # Security scanning and analysis
├── SecurityService.UnitTests/     # Security service testing
├── SearchService/                 # Search and discovery service
├── SearchService.UnitTests/       # Search service testing
├── CommandLineApp/                # Native AOT CLI tool (mcpm)
├── CommandLineApp.UnitTests/      # CLI comprehensive testing
├── PublicApi/                     # Public registry API endpoints
├── PublicApi.UnitTests/           # API comprehensive testing
├── WebApp/                        # Blazor web portal (SSR + WASM)
└── WebApp.UnitTests/              # Web app comprehensive testing
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

Each Servie API project follows consistent internal organization:

```
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