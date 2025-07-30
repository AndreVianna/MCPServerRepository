# MCP Hub BDD Integration Tests

This project contains comprehensive Behavior-Driven Development (BDD) integration tests for the entire MCP Hub platform using SpecFlow with xUnit.

## Overview

The BDD tests provide comprehensive coverage of:

- **API Testing**: REST API endpoints, authentication, and business logic
- **Web Application E2E**: Full user interface testing with Selenium
- **CLI Tool Testing**: Command-line interface workflows
- **System Integration**: End-to-end scenarios across all components
- **Performance Testing**: Load testing and performance validation
- **Security Testing**: Security scenarios and penetration testing

## Architecture

### Test Framework Stack
- **SpecFlow 3.x**: BDD framework for .NET
- **xUnit**: Test execution framework
- **TestContainers**: Isolated test environment with PostgreSQL and Redis
- **Selenium WebDriver**: Web UI automation
- **FluentAssertions**: Expressive test assertions
- **Bogus**: Test data generation
- **NSubstitute**: Mocking framework

### Project Structure
```
IntegrationTests/
├── Features/                     # Gherkin feature files
│   ├── PackageManagement.feature # API package operations
│   ├── Authentication.feature    # Auth and authorization
│   ├── SecurityScanning.feature  # Security workflows
│   ├── WebApplication.feature    # Web UI scenarios
│   ├── CLIWorkflows.feature      # CLI command testing
│   └── SystemIntegration.feature # E2E integration
├── StepDefinitions/              # Step implementation
│   ├── ApiStepDefinitions.cs     # API test steps
│   ├── WebApplicationStepDefinitions.cs # Web UI steps
│   ├── CLIStepDefinitions.cs     # CLI test steps
│   └── AuthenticationStepDefinitions.cs # Auth steps
├── Infrastructure/               # Test infrastructure
│   ├── TestWebApplicationFactory.cs # Web app factory
│   └── TestContainerFixture.cs  # Container management
├── Support/                      # Test support classes
│   ├── ScenarioContext.cs        # Scenario state management
│   ├── WebDriverExtensions.cs    # Selenium helpers
│   └── TestDataBuilder.cs       # Test data generation
└── Builders/                     # Test data builders
    └── TestDataBuilder.cs        # Fluent test data builders
```

## Features Covered

### 1. Package Management API (`PackageManagement.feature`)
- Package search with filters, pagination, and sorting
- Package retrieval by ID and name
- Package CRUD operations with authentication
- Publisher-specific package management
- Advanced search with multiple criteria
- Performance testing with concurrent operations

### 2. Authentication & Authorization (`Authentication.feature`)
- User login/logout workflows
- JWT token generation and refresh
- Multi-factor authentication
- Role-based access control
- API key authentication
- Password reset and change
- Security audit logging

### 3. Security Scanning (`SecurityScanning.feature`)
- Automatic security scanning on package publish
- Static and dynamic analysis
- Dependency vulnerability scanning
- Trust tier progression
- Security policy enforcement
- Manual security reviews
- False positive management

### 4. Web Application E2E (`WebApplication.feature`)
- Homepage functionality and navigation
- Package search and discovery
- User registration and authentication
- Publisher dashboard operations
- Package detail pages
- Responsive design testing
- Accessibility compliance

### 5. CLI Tool Workflows (`CLIWorkflows.feature`)
- All CLI commands (search, install, publish, etc.)
- Three-stage security model (fetch → verify → install)
- Interactive prompts and user consent
- Offline mode operations
- Configuration management
- Cross-platform compatibility
- Performance requirements

### 6. System Integration (`SystemIntegration.feature`)
- End-to-end package lifecycle
- Multi-user concurrent operations
- Cross-platform consistency
- System monitoring and alerting
- Backup and disaster recovery
- Performance and scalability
- Security compliance

## Running the Tests

### Prerequisites
- .NET 9 SDK
- Docker (for TestContainers)
- Chrome browser (for Selenium tests)

### Command Line Execution

```bash
# Run all BDD tests
dotnet test Source/IntegrationTests/

# Run specific feature
dotnet test --filter "Category=api"
dotnet test --filter "Category=web"
dotnet test --filter "Category=cli"

# Run tests with specific tags
dotnet test --filter "TestCategory=authentication"
dotnet test --filter "TestCategory=security"
dotnet test --filter "TestCategory=performance"

# Generate test report
dotnet test --logger "trx;LogFileName=IntegrationTestResults.trx"
```

### Visual Studio
1. Open the solution in Visual Studio
2. Build the solution
3. Open Test Explorer
4. Run tests by category or individually

### Integration with project.sh
```bash
# Run BDD tests through project script
./Scripts/project.sh test --project IntegrationTests

# Run specific test categories
./Scripts/project.sh test --project IntegrationTests --filter "api"
```

## Test Data Management

### Test Data Builders
The project uses the Builder pattern for creating test data:

```csharp
var scenario = new TestScenarioBuilder()
    .WithPackages(10)
    .WithUsers(3)
    .WithPackage(specificPackage)
    .Build();
```

### TestContainers
Tests run in isolated environments using TestContainers:
- PostgreSQL database container for data persistence
- Redis container for caching
- Automatic cleanup after test runs

### Data Seeding
Each test scenario can seed specific data:
- Publishers with packages
- Users with different roles
- Security scan results
- Trust tier configurations

## Configuration

### Test Configuration (`specflow.json`)
- Language: English (en)
- Binding Culture: en-US
- Runtime: xUnit
- Tracing: Enabled with timing information

### Environment Variables
- `MCPHUB_TEST_HEADLESS`: Run browser tests in headless mode (default: true)
- `MCPHUB_TEST_TIMEOUT`: Test timeout in seconds (default: 300)
- `MCPHUB_TEST_LOGLEVEL`: Logging level (default: Information)

## Test Organization

### Tags
Tests are organized using SpecFlow tags:
- `@api`: API endpoint tests
- `@web`: Web application tests
- `@cli`: Command-line interface tests
- `@security`: Security-related tests
- `@performance`: Performance and load tests
- `@integration`: Cross-component integration tests

### Scenarios Categories
- `@smoke`: Critical path tests
- `@regression`: Full regression suite
- `@e2e`: End-to-end workflows
- `@concurrent`: Multi-user scenarios

## Reporting

### Test Reports
- SpecFlow generates HTML reports with scenario results
- xUnit produces standard test result files (TRX format)
- Screenshots captured for failed web tests
- Performance metrics logged for load tests

### Continuous Integration
The tests are designed to run in CI/CD pipelines:
- Docker containers for consistent test environment
- Parallel execution support
- Detailed failure reporting with screenshots
- Integration with Azure DevOps and GitHub Actions

## Best Practices

### Writing BDD Scenarios
1. Use clear, business-focused language
2. Follow Given-When-Then structure
3. Keep scenarios focused and atomic
4. Use scenario outlines for data-driven tests
5. Include both positive and negative test cases

### Test Data
1. Use builders for consistent test data creation
2. Clean up test data after each scenario
3. Use realistic data volumes for performance tests
4. Avoid hard-coded test data in scenarios

### Page Objects (Web Tests)
1. Encapsulate page interactions in page objects
2. Use meaningful element selectors
3. Implement wait strategies for dynamic content
4. Keep page objects focused on single pages

### Step Definitions
1. Make steps reusable across scenarios
2. Use strongly-typed scenario context
3. Include proper error handling and logging
4. Implement proper cleanup in hooks

## Troubleshooting

### Common Issues
1. **Container startup failures**: Ensure Docker is running
2. **Browser not found**: Install Chrome or update ChromeDriver
3. **Database connection issues**: Check PostgreSQL container logs
4. **Timeout errors**: Increase test timeout values
5. **Port conflicts**: Ensure required ports are available

### Debugging
1. Set `MCPHUB_TEST_HEADLESS=false` to see browser tests
2. Use logging to trace test execution
3. Take screenshots on test failures
4. Check container logs for infrastructure issues

### Performance
1. Run tests in parallel where possible
2. Use TestContainers cleanup for faster execution
3. Optimize test data creation
4. Consider test sharding for large test suites

## Contributing

When adding new BDD tests:
1. Write the feature file first in Gherkin syntax
2. Generate step definition skeletons
3. Implement step definitions with proper error handling
4. Add appropriate test data builders
5. Include both positive and negative scenarios
6. Tag tests appropriately for organization
7. Update this README if adding new categories or features
