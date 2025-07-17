# MCP Hub Development Guide

This guide provides comprehensive instructions for setting up and working with the MCP Hub development environment.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Quick Start](#quick-start)
- [Development Environment](#development-environment)
- [Project Structure](#project-structure)
- [Development Workflow](#development-workflow)
- [Testing](#testing)
- [Deployment](#deployment)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)

## Prerequisites

Before you begin, ensure you have the following installed:

### Required Software

- **Docker Desktop** (latest version)
  - Download from [docker.com](https://www.docker.com/products/docker-desktop)
  - Ensure it's running and configured properly

- **.NET 9 SDK** (latest version)
  - Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download)
  - Verify installation: `dotnet --version`

- **Node.js 20+** (LTS version)
  - Download from [nodejs.org](https://nodejs.org)
  - Verify installation: `node --version`

- **Git** (latest version)
  - Download from [git-scm.com](https://git-scm.com)
  - Configure with your credentials

### Development Tools

- **Visual Studio Code** (recommended)
  - Install the recommended extensions (see `.vscode/extensions.json`)
  - The dev container will automatically configure VS Code

- **Alternative IDEs**
  - JetBrains Rider
  - Visual Studio 2022
  - Vim/Neovim with LSP support

### System Requirements

- **Operating System**: Windows 10/11, macOS 10.15+, or Linux (Ubuntu 20.04+)
- **RAM**: 16GB minimum, 32GB recommended
- **Storage**: 20GB free space for development environment
- **CPU**: 4 cores minimum, 8 cores recommended

## Quick Start

Get up and running in minutes:

### 1. Clone the Repository

```bash
git clone https://github.com/your-org/mcphub.git
cd mcphub
```

### 2. Set Up Development Environment

```bash
# Make scripts executable
chmod +x Scripts/*.sh

# Run the setup script
./Scripts/dev-setup.sh
```

This script will:

- Install required .NET workloads and tools
- Install npm packages and Playwright browsers
- Set up development certificates
- Configure local hosts entries
- Start infrastructure services
- Display service URLs and next steps

### 3. Create Project Structure

```bash
# Scaffold the solution structure
./Scripts/scaffold.sh
```

### 4. Start Development Server

```bash
# Start the development server
./Scripts/dev.sh
```

### 5. Open in Browser

Navigate to [http://mcphub.local](http://mcphub.local) to see the application.

## Development Environment

### Development Containers

The project uses Docker containers for consistent development environments:

#### Dev Container (Recommended)

1. Open the project in VS Code
2. Install the "Dev Containers" extension
3. Press `F1` and select "Dev Containers: Reopen in Container"
4. The container will build and configure automatically

#### Manual Docker Setup

```bash
# Start all services
docker-compose up -d

# View service status
docker-compose ps

# View logs
docker-compose logs -f

# Stop all services
docker-compose down
```

### Service URLs

When running, the following services are available:

| Service | URL | Description |
|---------|-----|-------------|
| Web App | <http://mcphub.local> | Main web application |
| Public API | <http://api.mcphub.local> | REST API endpoints |
| Security Service | <http://security.mcphub.local> | Security scanning service |
| Search Service | <http://search.mcphub.local> | Search and discovery service |
| Traefik Dashboard | <http://traefik.mcphub.local> | Reverse proxy dashboard |
| Aspire Dashboard | <http://localhost:18080> | .NET Aspire dashboard |
| Jaeger | <http://jaeger.mcphub.local> | Distributed tracing |
| Prometheus | <http://prometheus.mcphub.local> | Metrics collection |
| Grafana | <http://grafana.mcphub.local> | Metrics visualization |
| pgAdmin | <http://pgadmin.mcphub.local> | Database administration |
| Redis Commander | <http://redis.mcphub.local> | Redis management |
| MailHog | <http://mail.mcphub.local> | Email testing |
| Seq | <http://seq.mcphub.local> | Structured logging |
| Portainer | <http://portainer.mcphub.local> | Docker management |

### Default Credentials

| Service | Username | Password |
|---------|----------|----------|
| pgAdmin | <admin@mcphub.local> | admin |
| Grafana | admin | admin |
| RabbitMQ | guest | guest |
| Portainer | admin | (set on first login) |

## Project Structure

``` text
MCPHub/
├── .devcontainer/          # Dev container configuration
├── .github/               # GitHub Actions workflows
├── .vscode/               # VS Code settings
├── Documents/             # Project documentation
├── Scripts/              # Development scripts
├── Source/               # Source code
│   ├── Domain/           # Domain layer (entities, value objects)
│   ├── Common/           # Shared infrastructure
│   ├── Core/             # Shared utilities
│   ├── Data/             # Data access layer
│   ├── Data.MigrationService/  # Database migrations
│   ├── CommandLineApp/   # CLI tool (mcpm)
│   ├── PublicApi/        # Public API service
│   ├── WebApp/           # Blazor web application
│   ├── SecurityService/  # Security scanning service
│   ├── SearchService/    # Search service
│   ├── AppHost/          # .NET Aspire orchestration
│   └── *.UnitTests/      # Unit tests for each project
├── Tests/                # Integration and E2E tests
├── Monitoring/           # Monitoring configuration
├── Deployment/           # Deployment scripts
└── Build/               # Build artifacts
```

### Architecture Overview

The MCP Hub follows Clean Architecture principles:

- **Domain Layer**: Pure business logic with no external dependencies
- **Application Layer**: Use cases and application services
- **Infrastructure Layer**: Data access and external service integrations
- **Presentation Layer**: APIs and user interfaces

## Development Workflow

### 1. Feature Development

```bash
# Create a feature branch
git checkout -b feature/your-feature-name

# Make your changes
# ...

# Run tests
./Scripts/test.sh

# Format code
npm run format

# Lint code
npm run lint:fix

# Commit changes
git add .
git commit -m "feat: add your feature description"

# Push changes
git push origin feature/your-feature-name
```

### 2. Code Quality

The project enforces code quality through:

- **EditorConfig**: Consistent formatting across editors
- **ESLint**: JavaScript/TypeScript linting
- **Prettier**: Code formatting
- **dotnet format**: C# code formatting
- **Pre-commit hooks**: Automated quality checks

### 3. Database Changes

```bash
# Add a new migration
cd Source/Data
dotnet ef migrations add YourMigrationName

# Update database
dotnet ef database update

# Or use the script
./Scripts/migrate.sh
```

### 4. Testing

```bash
# Run all tests
./Scripts/test.sh

# Run specific test types
./Scripts/test.sh --unit
./Scripts/test.sh --integration
./Scripts/test.sh --e2e

# Run tests with coverage
./Scripts/test.sh --coverage

# Run tests in watch mode
./Scripts/test.sh --watch
```

### 5. Building

```bash
# Build solution
./Scripts/build.sh

# Or manually
cd Source
dotnet build
```

## Testing

### Test Categories

- **Unit Tests**: Fast, isolated tests for individual components
- **Integration Tests**: Test interactions between components
- **E2E Tests**: End-to-end browser testing with Playwright
- **Performance Tests**: Load and stress testing
- **Security Tests**: Vulnerability scanning

### Test Structure

``` text
Tests/
├── Unit/                 # Unit tests
├── Integration/          # Integration tests
├── E2E/                  # End-to-end tests
├── Performance/          # Performance tests
├── Security/             # Security tests
└── Fixtures/             # Test data and fixtures
```

### Running Tests

```bash
# All tests
npm run test

# Unit tests only
npm run test:unit

# Integration tests only
npm run test:integration

# E2E tests only
npm run test:e2e

# Tests with coverage
npm run test:coverage

# Tests in watch mode
npm run test:watch
```

### Test Environment

Test infrastructure is automatically set up with:

- Test database (PostgreSQL)
- Test cache (Redis)
- Test search (Elasticsearch)
- Test vector DB (Qdrant)
- Test message queue (RabbitMQ)

## Deployment

### Development Deployment

```bash
# Start development environment
./Scripts/dev.sh

# Build and deploy to staging
npm run deploy:staging

# Build and deploy to production
npm run deploy:production
```

### CI/CD Pipeline

The project uses GitHub Actions for CI/CD:

- **Build**: Compile and test code
- **Test**: Run full test suite
- **Security**: Scan for vulnerabilities
- **Deploy**: Deploy to environments
- **Release**: Create releases with artifacts

### Docker Deployment

```bash
# Build Docker images
docker-compose build

# Deploy with Docker Compose
docker-compose up -d

# Scale services
docker-compose up -d --scale webapp=3
```

## Troubleshooting

### Common Issues

#### Docker Issues

```bash
# Docker not running
sudo systemctl start docker

# Permission denied
sudo usermod -aG docker $USER
# Then logout and login again

# Clean up Docker
docker system prune -a
```

#### .NET Issues

```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore

# Clean and rebuild
dotnet clean
dotnet build
```

#### Database Issues

```bash
# Reset database
docker-compose down -v
docker-compose up -d postgres
./Scripts/migrate.sh
```

#### Port Conflicts

```bash
# Find process using port
lsof -i :5000

# Kill process
sudo kill -9 <PID>

# Or use different ports in docker-compose.override.yml
```

### Debugging

#### Application Debugging

- Use VS Code debugger with launch configurations
- Enable detailed logging in `appsettings.Development.json`
- Use Seq for structured logging: <http://seq.mcphub.local>

#### Database Debugging

- Use pgAdmin: <http://pgadmin.mcphub.local>
- Check database logs: `docker-compose logs postgres`
- Run SQL queries directly in pgAdmin

#### Network Debugging

- Check Traefik dashboard: <http://traefik.mcphub.local>
- Verify service health: `docker-compose ps`
- Check service logs: `docker-compose logs <service-name>`

### Getting Help

1. Check the [FAQ](FAQ.md)
2. Search existing [Issues](https://github.com/your-org/mcphub/issues)
3. Join our [Discord](https://discord.gg/mcphub)
4. Ask on [Stack Overflow](https://stackoverflow.com/questions/tagged/mcphub)

## Contributing

### Development Process

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Update documentation
7. Submit a pull request

### Code Standards

- Follow the existing code style
- Write meaningful commit messages
- Add tests for new features
- Update documentation
- Use semantic versioning

### Pull Request Process

1. Update README.md with details of changes
2. Update CHANGELOG.md following Keep a Changelog format
3. Ensure CI/CD pipeline passes
4. Request review from maintainers
5. Address feedback and merge

### Release Process

1. Create release branch: `git checkout -b release/v1.0.0`
2. Update version numbers
3. Update CHANGELOG.md
4. Test thoroughly
5. Merge to main
6. Create GitHub release
7. Deploy to production

## Additional Resources

- [Architecture Decision Records](docs/architecture/)
- [API Documentation](docs/api/)
- [User Guide](docs/user-guide/)
- [Administrator Guide](docs/admin-guide/)
- [Security Guide](docs/security/)
- [Performance Guide](docs/performance/)

---

For more information, see the [main README](README.md) or contact the development team.
