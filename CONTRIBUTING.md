# Contributing to MCP Hub

Thank you for your interest in contributing to MCP Hub! This document provides guidelines and information for contributors.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [How to Contribute](#how-to-contribute)
- [Development Process](#development-process)
- [Code Standards](#code-standards)
- [Testing Guidelines](#testing-guidelines)
- [Documentation](#documentation)
- [Pull Request Process](#pull-request-process)
- [Release Process](#release-process)
- [Community](#community)

## Code of Conduct

This project and everyone participating in it is governed by our [Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code. Please report unacceptable behavior to [conduct@mcphub.io](mailto:conduct@mcphub.io).

## Getting Started

### Prerequisites

Before contributing, ensure you have:

1. **Development Environment**: Follow the [Getting Started Guide](GETTING-STARTED.md)
2. **GitHub Account**: For submitting pull requests and issues
3. **Git Configuration**: Properly configured with your name and email
4. **IDE Setup**: VS Code with recommended extensions (or your preferred IDE)

### Setting Up Your Development Environment

1. **Fork the repository** on GitHub
2. **Clone your fork** locally:

   ```bash
   git clone https://github.com/your-username/mcphub.git
   cd mcphub
   ```

3. **Add upstream remote**:

   ```bash
   git remote add upstream https://github.com/original-org/mcphub.git
   ```

4. **Set up development environment**:

   ```bash
   ./Scripts/dev-setup.sh
   ```

## How to Contribute

### Types of Contributions

We welcome various types of contributions:

#### 🐛 Bug Reports

- Use the bug report template
- Include reproduction steps
- Provide environment details
- Include relevant logs and screenshots

#### 💡 Feature Requests

- Use the feature request template
- Explain the use case and benefits
- Provide mockups or examples if applicable
- Discuss with maintainers before implementation

#### 📚 Documentation

- Improve existing documentation
- Add missing documentation
- Fix typos and clarity issues
- Translate documentation

#### 🔧 Code Contributions

- Bug fixes
- New features
- Performance improvements
- Refactoring
- Test improvements

#### 🎨 Design Contributions

- UI/UX improvements
- Icons and graphics
- Design system enhancements
- Accessibility improvements

### Finding Issues to Work On

1. **Good First Issues**: Look for issues labeled `good first issue`
2. **Help Wanted**: Check issues labeled `help wanted`
3. **Bug Reports**: Fix reported bugs
4. **Feature Requests**: Implement requested features
5. **Documentation**: Improve documentation

## Development Process

### Branching Strategy

We use a feature branch workflow:

``` text
main
├── develop
├── feature/your-feature-name
├── bugfix/issue-number-description
├── hotfix/critical-fix
└── release/v1.0.0
```

### Workflow Steps

1. **Create an issue** (if one doesn't exist)
2. **Fork and clone** the repository
3. **Create a feature branch**:

   ```bash
   git checkout -b feature/your-feature-name
   ```

4. **Make your changes**
5. **Test thoroughly**
6. **Commit with clear messages**
7. **Push to your fork**
8. **Create a pull request**

### Commit Message Convention

We follow [Conventional Commits](https://www.conventionalcommits.org/):

``` text
type(scope): description

[optional body]

[optional footer(s)]
```

**Types:**

- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Test-related changes
- `chore`: Maintenance tasks
- `perf`: Performance improvements
- `ci`: CI/CD changes
- `build`: Build system changes

**Examples:**

``` text
feat(auth): add OAuth2 login support
fix(api): resolve null reference in user service
docs(readme): update installation instructions
test(integration): add tests for package search
```

## Code Standards

### General Principles

1. **Clean Code**: Write self-documenting, readable code
2. **SOLID Principles**: Follow SOLID design principles
3. **DRY**: Don't repeat yourself
4. **YAGNI**: You aren't gonna need it
5. **KISS**: Keep it simple, stupid

### C# Guidelines

```csharp
// ✅ Good
public class PackageService : IPackageService
{
    private readonly IPackageRepository _packageRepository;

    public PackageService(IPackageRepository packageRepository)
    {
        _packageRepository = packageRepository ?? throw new ArgumentNullException(nameof(packageRepository));
    }

    public async Task<Package> GetPackageAsync(string packageId)
    {
        if (string.IsNullOrWhiteSpace(packageId))
            throw new ArgumentException("Package ID cannot be null or empty", nameof(packageId));

        return await _packageRepository.GetByIdAsync(packageId);
    }
}

// ❌ Bad
public class PackageService
{
    public IPackageRepository repo;

    public Package GetPackage(string id)
    {
        return repo.GetById(id);
    }
}
```

### TypeScript Guidelines

```typescript
// ✅ Good
interface IPackageSearchRequest {
  readonly query: string;
  readonly pageSize: number;
  readonly pageNumber: number;
}

const searchPackages = async (request: IPackageSearchRequest): Promise<IPackageSearchResult> => {
  if (!request.query.trim()) {
    throw new Error('Search query cannot be empty');
  }

  const response = await fetch('/api/packages/search', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  });

  if (!response.ok) {
    throw new Error(`Search failed: ${response.statusText}`);
  }

  return response.json();
};

// ❌ Bad
const searchPackages = (query, pageSize, pageNumber) => {
  return fetch('/api/packages/search', {
    method: 'POST',
    body: JSON.stringify({ query, pageSize, pageNumber }),
  }).then(r => r.json());
};
```

### Formatting and Linting

Code formatting is enforced through:

- **EditorConfig**: Consistent formatting across editors
- **Prettier**: JavaScript/TypeScript formatting
- **ESLint**: JavaScript/TypeScript linting
- **dotnet format**: C# formatting
- **Roslyn Analyzers**: C# code analysis

Run formatting and linting:

```bash
# Format all code
npm run format

# Fix linting issues
npm run lint:fix

# Format C# code
dotnet format
```

### Code Review Guidelines

#### For Contributors

1. **Self-review**: Review your own code before submitting
2. **Small PRs**: Keep pull requests focused and small
3. **Clear descriptions**: Explain what and why, not just how
4. **Test coverage**: Include tests for new functionality
5. **Documentation**: Update documentation for user-facing changes

#### For Reviewers

1. **Be constructive**: Provide helpful feedback
2. **Be timely**: Review within 24-48 hours
3. **Be thorough**: Check functionality, tests, and documentation
4. **Be respectful**: Maintain a positive tone
5. **Focus on code**: Review the code, not the person

## Testing Guidelines

### Testing Philosophy

- **Test-Driven Development**: Write tests before implementation
- **Comprehensive Coverage**: Aim for high test coverage
- **Fast Feedback**: Tests should run quickly
- **Reliable**: Tests should be deterministic and stable
- **Maintainable**: Tests should be easy to understand and maintain

### Test Types

#### Unit Tests

```csharp
[Test]
public async Task GetPackageAsync_WithValidId_ReturnsPackage()
{
    // Arrange
    var packageId = "test-package";
    var expectedPackage = new Package { Id = packageId, Name = "Test Package" };
    _mockRepository.Setup(x => x.GetByIdAsync(packageId)).ReturnsAsync(expectedPackage);

    // Act
    var result = await _packageService.GetPackageAsync(packageId);

    // Assert
    result.Should().NotBeNull();
    result.Id.Should().Be(packageId);
    result.Name.Should().Be("Test Package");
}
```

#### Integration Tests

```csharp
[Test]
public async Task PackageApi_CreatePackage_StoresInDatabase()
{
    // Arrange
    var package = new CreatePackageRequest
    {
        Name = "Test Package",
        Description = "Test Description"
    };

    // Act
    var response = await _client.PostAsJsonAsync("/api/packages", package);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Created);

    var createdPackage = await response.Content.ReadFromJsonAsync<Package>();
    createdPackage.Should().NotBeNull();
    createdPackage.Name.Should().Be("Test Package");

    // Verify in database
    var dbPackage = await _context.Packages.FirstOrDefaultAsync(p => p.Id == createdPackage.Id);
    dbPackage.Should().NotBeNull();
}
```

#### E2E Tests

```typescript
import { test, expect } from '@playwright/test';

test('user can search for packages', async ({ page }) => {
  // Navigate to search page
  await page.goto('/search');

  // Enter search query
  await page.fill('[data-testid="search-input"]', 'test-package');
  await page.click('[data-testid="search-button"]');

  // Verify results
  await expect(page.locator('[data-testid="search-results"]')).toBeVisible();
  await expect(page.locator('[data-testid="package-item"]')).toHaveCount.toBeGreaterThan(0);
});
```

### Running Tests

```bash
# All tests
./Scripts/test.sh

# Unit tests only
./Scripts/test.sh --unit

# Integration tests only
./Scripts/test.sh --integration

# E2E tests only
./Scripts/test.sh --e2e

# With coverage
./Scripts/test.sh --coverage

# In watch mode
./Scripts/test.sh --watch
```

### Test Data and Fixtures

Use the `Tests/Fixtures/` directory for test data:

```csharp
public static class PackageFixtures
{
    public static Package CreateValidPackage(string id = "test-package") =>
        new Package
        {
            Id = id,
            Name = "Test Package",
            Description = "Test Description",
            Version = "1.0.0",
            CreatedAt = DateTime.UtcNow
        };
}
```

## Documentation

### Documentation Types

1. **Code Documentation**: Inline comments and XML documentation
2. **API Documentation**: OpenAPI/Swagger specifications
3. **User Documentation**: Guides and tutorials
4. **Developer Documentation**: Architecture and development guides

### Writing Guidelines

- **Clear and concise**: Use simple, direct language
- **User-focused**: Write from the user's perspective
- **Examples**: Include code examples and screenshots
- **Up-to-date**: Keep documentation current with code changes
- **Accessible**: Use proper headings and structure

### Documentation Tools

- **Markdown**: For most documentation
- **Mermaid**: For diagrams and flowcharts
- **OpenAPI**: For API documentation
- **DocFX**: For .NET API documentation

## Pull Request Process

### Before Submitting

1. **Sync with upstream**:

   ```bash
   git fetch upstream
   git rebase upstream/main
   ```

2. **Run tests**:

   ```bash
   ./Scripts/test.sh
   ```

3. **Check code quality**:

   ```bash
   npm run lint
   npm run format:check
   ```

4. **Update documentation** if needed

### PR Template

Fill out the PR template completely:

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Unit tests pass
- [ ] Integration tests pass
- [ ] E2E tests pass
- [ ] Manual testing completed

## Checklist
- [ ] Code follows style guidelines
- [ ] Self-review completed
- [ ] Documentation updated
- [ ] Tests added/updated
```

### Review Process

1. **Automated checks** must pass
2. **Code review** by maintainers
3. **Testing** in staging environment
4. **Approval** from at least one maintainer
5. **Merge** by maintainers

### Merge Requirements

- ✅ All CI checks pass
- ✅ Code review approval
- ✅ No merge conflicts
- ✅ Up-to-date with target branch
- ✅ All conversations resolved

## Release Process

### Version Numbering

We use [Semantic Versioning](https://semver.org/):

- **MAJOR**: Breaking changes
- **MINOR**: New features (backward compatible)
- **PATCH**: Bug fixes (backward compatible)

### Release Workflow

1. **Create release branch**: `git checkout -b release/v1.0.0`
2. **Update version numbers** in all relevant files
3. **Update CHANGELOG.md** with new changes
4. **Test thoroughly** in staging environment
5. **Create pull request** to main branch
6. **Review and merge** release PR
7. **Create GitHub release** with changelog
8. **Deploy to production**
9. **Announce release** to community

### Changelog Format

We follow [Keep a Changelog](https://keepachangelog.com/):

```markdown
# Changelog

## [1.0.0] - 2024-01-15

### Added
- New package search functionality
- OAuth2 authentication support
- Package trust tier system

### Changed
- Improved search performance
- Updated UI design

### Fixed
- Fixed package upload validation
- Resolved security vulnerabilities

### Removed
- Deprecated API endpoints
```

## Community

### Communication Channels

- **GitHub Issues**: Bug reports and feature requests
- **GitHub Discussions**: General discussions and questions
- **Discord**: Real-time chat and collaboration
- **Stack Overflow**: Technical questions (use `mcphub` tag)
- **Email**: [contact@mcphub.io](mailto:contact@mcphub.io)

### Community Guidelines

1. **Be respectful**: Treat everyone with respect
2. **Be helpful**: Help others when you can
3. **Be constructive**: Provide actionable feedback
4. **Stay on topic**: Keep discussions relevant
5. **Follow guidelines**: Adhere to community standards

### Recognition

We recognize contributors through:

- **Contributors file**: Listed in CONTRIBUTORS.md
- **Release notes**: Mentioned in changelog
- **Social media**: Featured on our channels
- **Badges**: GitHub profile badges
- **Swag**: Contributor merchandise

## Getting Help

If you need help with contributing:

1. **Read the documentation**: Start with existing guides
2. **Search existing issues**: Someone might have asked already
3. **Ask on Discord**: Get real-time help from the community
4. **Create an issue**: For specific problems or questions
5. **Contact maintainers**: For sensitive or urgent matters

## Thank You

Thank you for contributing to MCP Hub! Your contributions help make the project better for everyone. We appreciate your time, effort, and dedication to improving the MCP ecosystem.

---

**Happy contributing!** 🎉

For questions about this contributing guide, please [open an issue](https://github.com/your-org/mcphub/issues) or reach out to the maintainers.
