# Getting Started with MCP Hub

Welcome to MCP Hub! This guide will help you get up and running quickly.

## What is MCP Hub?

MCP Hub is a comprehensive MCP (Model Context Protocol) server registration repository that works like NPM and NuGet. It enables:

- **Package Discovery**: Search and discover MCP servers
- **Trust & Security**: Multi-tier trust system with security scanning
- **Easy Integration**: Simple installation and management
- **Community**: Collaborative development and sharing

## Prerequisites

Before starting, make sure you have:

- **Docker Desktop** installed and running
- **.NET 9 SDK** installed
- **Node.js 20+** installed
- **Git** configured with your credentials
- **16GB+ RAM** available
- **20GB+ free disk space**

## Quick Setup

### 1. Clone the Repository

```bash
git clone https://github.com/your-org/mcphub.git
cd mcphub
```

### 2. Run Setup Script

```bash
# Make scripts executable
chmod +x Scripts/*.sh

# Run the automated setup
./Scripts/dev-setup.sh
```

This will:
- ✅ Install required tools and dependencies
- ✅ Set up development certificates
- ✅ Configure local DNS entries
- ✅ Start infrastructure services
- ✅ Display service URLs

### 3. Create Project Structure

```bash
# Generate the solution structure
./Scripts/scaffold.sh
```

### 4. Start Development Server

```bash
# Start the application
./Scripts/dev.sh
```

### 5. Open in Browser

Navigate to [http://mcphub.local](http://mcphub.local) to see your running application!

## Development Environment

### Service Overview

Your development environment includes:

| Service | URL | Purpose |
|---------|-----|---------|
| 🌐 Web App | http://mcphub.local | Main application |
| 🔗 API | http://api.mcphub.local | REST API |
| 🔒 Security | http://security.mcphub.local | Security scanning |
| 🔍 Search | http://search.mcphub.local | Search service |
| 📊 Dashboard | http://localhost:18080 | .NET Aspire dashboard |
| 📊 Grafana | http://grafana.mcphub.local | Metrics & monitoring |
| 🗄️ pgAdmin | http://pgadmin.mcphub.local | Database management |
| 📧 Mail | http://mail.mcphub.local | Email testing |
| 📋 Logs | http://seq.mcphub.local | Structured logging |

### Default Credentials

| Service | Username | Password |
|---------|----------|----------|
| pgAdmin | admin@mcphub.local | admin |
| Grafana | admin | admin |
| RabbitMQ | guest | guest |

## Common Tasks

### Running Tests

```bash
# Run all tests
./Scripts/test.sh

# Run specific test types
./Scripts/test.sh --unit        # Unit tests only
./Scripts/test.sh --integration # Integration tests only
./Scripts/test.sh --e2e         # End-to-end tests only

# Run with coverage
./Scripts/test.sh --coverage

# Watch mode for development
./Scripts/test.sh --watch
```

### Building the Project

```bash
# Build everything
./Scripts/build.sh

# Format code
npm run format

# Lint code
npm run lint:fix

# Type checking
npm run typecheck
```

### Database Management

```bash
# Run migrations
./Scripts/migrate.sh

# Access database
# Go to http://pgadmin.mcphub.local
# Server: postgres, Database: mcphub_dev
# Username: mcphub_user, Password: mcphub_dev_password
```

### Monitoring & Debugging

```bash
# View logs
docker-compose logs -f

# View specific service logs
docker-compose logs -f webapp

# Check service health
docker-compose ps

# Monitor metrics
# Open http://grafana.mcphub.local
```

## Development Workflow

### 1. Creating Features

```bash
# Create feature branch
git checkout -b feature/your-feature-name

# Make changes
# ... develop your feature ...

# Test changes
./Scripts/test.sh

# Format and lint
npm run format
npm run lint:fix

# Commit changes
git add .
git commit -m "feat: add your feature description"

# Push changes
git push origin feature/your-feature-name
```

### 2. Code Quality

The project automatically enforces:
- **Code formatting** with Prettier and dotnet format
- **Linting** with ESLint and Roslyn analyzers
- **Type checking** with TypeScript
- **Testing** with comprehensive test suite
- **Security scanning** with vulnerability detection

### 3. Making Changes

#### Frontend Changes (Blazor)
- Edit files in `Source/WebApp/`
- Hot reload is enabled
- Changes appear immediately

#### Backend Changes (API)
- Edit files in `Source/PublicApi/`
- Hot reload is enabled
- API changes are reflected immediately

#### Database Changes
- Add migrations in `Source/Data/`
- Run `./Scripts/migrate.sh` to apply
- Use pgAdmin to view changes

## Architecture Overview

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Web App       │    │   Public API    │    │ Security Service│
│   (Blazor)      │    │   (REST)        │    │   (Scanning)    │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         └───────────────────────┼───────────────────────┘
                                 │
         ┌─────────────────┐    │    ┌─────────────────┐
         │ Search Service  │    │    │ CLI Tool (mcpm) │
         │ (Elasticsearch) │    │    │ (Native AOT)    │
         └─────────────────┘    │    └─────────────────┘
                                │
    ┌──────────────────────────────────────────────────────────┐
    │                   Infrastructure                          │
    │                                                          │
    │  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐        │
    │  │ PostgreSQL  │ │    Redis    │ │  RabbitMQ   │        │
    │  │ (Database)  │ │   (Cache)   │ │ (Messages)  │        │
    │  └─────────────┘ └─────────────┘ └─────────────┘        │
    │                                                          │
    │  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐        │
    │  │Elasticsearch│ │   Qdrant    │ │   Jaeger    │        │
    │  │  (Search)   │ │  (Vector)   │ │  (Tracing)  │        │
    │  └─────────────┘ └─────────────┘ └─────────────┘        │
    └──────────────────────────────────────────────────────────┘
```

## Next Steps

### For Developers

1. **Explore the codebase** - Start with `Source/` directory
2. **Run tests** - Understand the testing strategy
3. **Check documentation** - Review `Documents/` folder
4. **Make a small change** - Try adding a simple feature
5. **Join the community** - Participate in discussions

### For Contributors

1. **Read contributing guidelines** - See `CONTRIBUTING.md`
2. **Check open issues** - Find something to work on
3. **Set up development environment** - Follow this guide
4. **Make your first PR** - Start with a small improvement
5. **Join development discussions** - Connect with maintainers

### For DevOps

1. **Review CI/CD pipeline** - Check `.github/workflows/`
2. **Understand deployment** - Review `Deployment/` directory
3. **Check monitoring setup** - Explore `Monitoring/` directory
4. **Security configuration** - Review security settings
5. **Performance optimization** - Check performance metrics

## Troubleshooting

### Common Issues

#### "Docker not running"
```bash
# Start Docker Desktop
# Or on Linux:
sudo systemctl start docker
```

#### "Port already in use"
```bash
# Find process using port
lsof -i :5000
# Kill process
sudo kill -9 <PID>
```

#### "Database connection failed"
```bash
# Restart database
docker-compose restart postgres
# Wait for it to be healthy
docker-compose ps
```

#### "Services not accessible"
```bash
# Check hosts file
cat /etc/hosts | grep mcphub.local
# If missing, run setup again
./Scripts/dev-setup.sh
```

### Getting Help

1. **Check logs**: `docker-compose logs -f`
2. **Review documentation**: See `DEVELOPMENT.md`
3. **Search issues**: Check GitHub issues
4. **Ask community**: Join our Discord
5. **Contact maintainers**: Create an issue

## Resources

- **[Full Development Guide](DEVELOPMENT.md)** - Comprehensive development documentation
- **[API Documentation](docs/api/)** - API reference and examples
- **[Architecture Guide](docs/architecture/)** - System architecture details
- **[Contributing Guide](CONTRIBUTING.md)** - How to contribute to the project
- **[Security Guide](docs/security/)** - Security best practices
- **[Performance Guide](docs/performance/)** - Performance optimization

## Support

- **Documentation**: [docs/](docs/)
- **Issues**: [GitHub Issues](https://github.com/your-org/mcphub/issues)
- **Discord**: [Join our Discord](https://discord.gg/mcphub)
- **Stack Overflow**: [mcphub tag](https://stackoverflow.com/questions/tagged/mcphub)
- **Email**: [support@mcphub.io](mailto:support@mcphub.io)

---

**Happy coding!** 🚀

If you encounter any issues or have questions, don't hesitate to reach out to the community or maintainers.