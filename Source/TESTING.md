# MCP Hub Testing Framework

This document describes the comprehensive testing framework configured for the MCP Hub project.

## Overview

The testing framework is designed to support scalable, maintainable, and comprehensive testing across all projects in the MCP Hub solution. It follows Clean Architecture principles and includes:

- **1:1 mapping** between main projects and test projects
- **Consistent tooling** across all test projects
- **Common test utilities** for shared functionality
- **Test categorization** for organizing different types of tests
- **Coverage tracking** with >90% target
- **Mirror folder structure** for organizational consistency

## Test Projects

Each main project has a corresponding `.UnitTests` project:

- `Domain.UnitTests` → Tests for `Domain`
- `Common.UnitTests` → Tests for `Common` (also contains shared test utilities)
- `Core.UnitTests` → Tests for `Core`
- `Data.UnitTests` → Tests for `Data`
- `Data.MigrationService.UnitTests` → Tests for `Data.MigrationService`
- `CommandLineApp.UnitTests` → Tests for `CommandLineApp`
- `PublicApi.UnitTests` → Tests for `PublicApi`
- `WebApp.UnitTests` → Tests for `WebApp`
- `SecurityService.UnitTests` → Tests for `SecurityService`
- `SearchService.UnitTests` → Tests for `SearchService`

## Testing Stack

### Core Testing Frameworks
- **xUnit** - Primary testing framework
- **NSubstitute** - Mocking and stubbing framework
- **AwesomeAssertions** - Assertion library for readable tests
- **Coverlet** - Code coverage analysis

### Common Dependencies
- **Microsoft.Extensions.DependencyInjection** - Dependency injection support
- **Microsoft.Extensions.Logging** - Logging support for tests
- **Microsoft.Extensions.Configuration** - Configuration support for tests

## Test Structure

### Global Usings
All test projects include consistent global usings in `GlobalUsings.cs`:

```csharp
global using Xunit;
global using NSubstitute;
global using AwesomeAssertions;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Configuration;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using System.Threading;
global using System.IO;
global using System.Text;
global using System.Text.Json;
global using System.Reflection;
global using System.ComponentModel.DataAnnotations;
global using Common.UnitTests.TestUtilities;
```

### Test Categories

Tests are organized using category attributes:

- `[UnitTest]` - Unit tests that test individual components in isolation
- `[IntegrationTest]` - Integration tests that test multiple components working together
- `[EndToEndTest]` - End-to-end tests that test complete application flow
- `[DatabaseTest]` - Tests that require database access
- `[SecurityTest]` - Tests that verify security constraints
- `[PerformanceTest]` - Tests that measure system performance
- `[ExternalApiTest]` - Tests that require external service calls
- `[LongRunningTest]` - Tests that may take significant time to complete

### Base Classes

#### TestBase
Base class for all unit tests providing:
- Service provider for dependency injection
- Mock creation using NSubstitute
- Logger creation
- Configuration setup
- Proper disposal

#### DatabaseTestBase
Base class for database-related tests providing:
- In-memory database setup
- Database seeding utilities
- Database cleanup
- DbContext management

## Test Utilities

### TestData
Utility class for generating test data:
- `RandomString(length)` - Generate random strings
- `RandomEmail()` - Generate random email addresses
- `RandomGuid()` - Generate random GUIDs
- `RandomInt(min, max)` - Generate random integers
- `RandomDateTime()` - Generate random dates
- `RandomBool()` - Generate random booleans
- `RandomStringList(count, length)` - Generate lists of random strings
- `CreateTestConfiguration(settings)` - Create test configuration

### TestCategories
Constants for test categories to ensure consistency.

### TestCategoryAttributes
Attribute classes for marking tests with categories.

## Running Tests

### Command Line Scripts

#### Bash Script (Linux/Mac)
```bash
./run-tests.sh [option]
```

#### PowerShell Script (Windows)
```powershell
.\run-tests.ps1 [option]
```

### Available Options

- `all` - Run all tests
- `unit` - Run only unit tests
- `integration` - Run only integration tests
- `database` - Run only database tests
- `e2e` - Run only end-to-end tests
- `coverage` - Run tests with coverage report
- `coverage-html` - Run tests with HTML coverage report
- `watch` - Run tests in watch mode
- `clean` - Clean test artifacts
- `help` - Show usage information

### Direct dotnet Commands

```bash
# Run all tests
dotnet test

# Run specific category
dotnet test --filter Category=Unit

# Run with coverage
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings

# Run with coverage threshold
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:Threshold=90
```

## Coverage Configuration

### Target Coverage
- **Overall target**: >90% line coverage
- **Enforcement**: Configured in `Directory.Build.props` and `coverlet.runsettings`
- **Exclusions**: Test projects, migrations, generated files

### Coverage Files
- `coverlet.runsettings` - Coverage configuration
- `Directory.Build.props` - Build-time coverage settings

## Folder Structure

Test projects mirror the folder structure of their corresponding main projects:

```
Domain.UnitTests/
├── Common/
├── DomainServices/
├── Entities/
│   ├── PackageTests.cs
│   └── ...
├── Repositories/
│   ├── IServerRepositoryTests.cs
│   └── ...
├── Services/
├── ValueObjects/
│   ├── SecurityScanResultTests.cs
│   └── ...
├── GlobalUsings.cs
└── UnitTest1.cs
```

## Example Test Files

### Unit Test Example
```csharp
[UnitTest]
public class PackageTests : TestBase
{
    [Fact]
    public void Package_Should_Be_Created_With_Valid_Properties()
    {
        // Arrange
        var packageName = TestData.RandomString(10);
        var description = TestData.RandomString(50);
        
        // Act
        var result = $"Package: {packageName}, Description: {description}";
        
        // Assert
        result.Should().Contain(packageName);
        result.Should().Contain(description);
    }
}
```

### Integration Test Example
```csharp
[IntegrationTest]
[DatabaseTest]
public class PackageIntegrationTests : DatabaseTestBase
{
    [Fact]
    public async Task Package_Should_Be_Persisted_To_Database()
    {
        // Arrange
        var packageName = TestData.RandomString(10);
        
        // Act
        await SeedDatabaseAsync();
        
        // Assert
        packageName.Should().NotBeNullOrEmpty();
    }
}
```

## Configuration Files

### Directory.Build.props
- Configures test project properties
- Sets up coverage thresholds
- Defines package references for test projects

### Directory.Packages.props
- Centralizes package version management
- Defines all testing framework versions
- Ensures consistent package versions across projects

### coverlet.runsettings
- Configures code coverage collection
- Sets coverage thresholds
- Defines inclusion/exclusion patterns

## Best Practices

1. **Test Naming**: Use descriptive names that clearly indicate what is being tested
2. **Arrange-Act-Assert**: Structure tests using the AAA pattern
3. **Test Isolation**: Each test should be independent and not rely on other tests
4. **Mock Dependencies**: Use NSubstitute to mock external dependencies
5. **Test Data**: Use TestData utilities for consistent test data generation
6. **Categories**: Apply appropriate category attributes to organize tests
7. **Coverage**: Maintain >90% test coverage for all projects
8. **Documentation**: Document complex test scenarios and setup

## Development Workflow

1. Create test project if it doesn't exist
2. Mirror the folder structure of the main project
3. Write tests using appropriate base classes
4. Apply category attributes
5. Use TestData utilities for test data generation
6. Run tests locally before committing
7. Ensure coverage thresholds are met

## Troubleshooting

### Common Issues

1. **Package Version Conflicts**: Update `Directory.Packages.props` with correct versions
2. **Missing References**: Ensure test projects reference their corresponding main projects
3. **Coverage Failures**: Check exclusion patterns in `coverlet.runsettings`
4. **Test Discovery**: Ensure tests are properly attributed with `[Fact]` or `[Theory]`

### Debugging Tests

Use the following commands for debugging:

```bash
# Run specific test method
dotnet test --filter "FullyQualifiedName=Namespace.Class.TestMethod"

# Run with verbose output
dotnet test --verbosity detailed

# Run with blame mode for crash detection
dotnet test --blame
```

## Future Enhancements

- Integration with CI/CD pipelines
- Performance testing framework
- Test result reporting and dashboards
- Mutation testing integration
- Test data management strategies
- Advanced mocking scenarios