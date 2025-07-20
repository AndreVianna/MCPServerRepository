# Playwright MCP Browser Automation and Testing Configuration

## Overview

This document outlines the configuration and usage of Playwright MCP for comprehensive browser automation and testing in the MCP Hub project.

## Testing Architecture

### Test Organization

``` text
tests/
├── e2e/
│   ├── web-portal/
│   │   ├── authentication.spec.ts
│   │   ├── package-discovery.spec.ts
│   │   ├── user-dashboard.spec.ts
│   │   └── administration.spec.ts
│   ├── public-api/
│   │   ├── package-management.spec.ts
│   │   ├── security-scanning.spec.ts
│   │   └── search-endpoints.spec.ts
│   └── cli-tool/
│       ├── package-installation.spec.ts
│       ├── security-verification.spec.ts
│       └── command-interface.spec.ts
├── visual-regression/
│   ├── components/
│   ├── pages/
│   └── screenshots/
└── accessibility/
    ├── wcag-compliance.spec.ts
    └── screen-reader.spec.ts
```

## Playwright MCP Configuration

### Core Functions Available

#### Navigation and Page Management

```typescript
// Navigate to specific pages
await mcp__playwright__browser_navigate({ url: "https://localhost:5001" });

// Browser history management
await mcp__playwright__browser_navigate_back();
await mcp__playwright__browser_navigate_forward();

// Page analysis
const snapshot = await mcp__playwright__browser_snapshot();
const screenshot = await mcp__playwright__browser_take_screenshot({
  filename: "test-screenshot.png",
  raw: false
});
```

#### Tab Management

```typescript
// Multi-tab testing
await mcp__playwright__browser_tab_new({ url: "https://localhost:5001/admin" });
const tabs = await mcp__playwright__browser_tab_list();
await mcp__playwright__browser_tab_select({ index: 1 });
await mcp__playwright__browser_tab_close({ index: 0 });
```

#### User Interactions

```typescript
// Form interactions
await mcp__playwright__browser_type({
  element: "Username input",
  ref: "username-input",
  text: "testuser@example.com"
});

await mcp__playwright__browser_click({
  element: "Login button",
  ref: "login-btn"
});

// Advanced interactions
await mcp__playwright__browser_hover({
  element: "Package card",
  ref: "package-card-1"
});

await mcp__playwright__browser_drag({
  startElement: "Package item",
  startRef: "package-1",
  endElement: "Favorites list",
  endRef: "favorites-list"
});
```

#### File Operations

```typescript
// File upload testing
await mcp__playwright__browser_file_upload({
  paths: ["/path/to/test-package.zip"]
});

// PDF generation
await mcp__playwright__browser_pdf_save({
  filename: "package-documentation.pdf"
});
```

#### Advanced Features

```typescript
// Dialog handling
await mcp__playwright__browser_handle_dialog({
  accept: true,
  promptText: "Confirm package deletion"
});

// Network monitoring
const requests = await mcp__playwright__browser_network_requests();

// Console monitoring
const messages = await mcp__playwright__browser_console_messages();
```

## Test Configuration Files

### Base Test Configuration

```typescript
// tests/config/base-config.ts
export const baseConfig = {
  baseURL: 'https://localhost:5001',
  timeout: 30000,
  retries: 2,
  screenshots: {
    mode: 'only-on-failure',
    fullPage: true
  },
  accessibility: {
    enabled: true,
    standards: ['WCAG2AA', 'Section508']
  }
};
```

### Environment-Specific Configuration

```typescript
// tests/config/environments.ts
export const environments = {
  development: {
    webApp: 'https://localhost:5001',
    publicApi: 'https://localhost:5002',
    searchService: 'https://localhost:5003',
    securityService: 'https://localhost:5004'
  },
  staging: {
    webApp: 'https://staging.mcphub.dev',
    publicApi: 'https://api-staging.mcphub.dev',
    searchService: 'https://search-staging.mcphub.dev',
    securityService: 'https://security-staging.mcphub.dev'
  },
  production: {
    webApp: 'https://mcphub.dev',
    publicApi: 'https://api.mcphub.dev',
    searchService: 'https://search.mcphub.dev',
    securityService: 'https://security.mcphub.dev'
  }
};
```

## Test Implementation Examples

### Web Portal End-to-End Tests

#### Authentication Flow

```typescript
// tests/e2e/web-portal/authentication.spec.ts
describe('Authentication Flow', () => {
  test('User can login successfully', async () => {
    // Navigate to login page
    await mcp__playwright__browser_navigate({
      url: 'https://localhost:5001/login'
    });

    // Take screenshot for visual regression
    await mcp__playwright__browser_take_screenshot({
      filename: 'login-page.png'
    });

    // Fill login form
    await mcp__playwright__browser_type({
      element: 'Email input',
      ref: 'email-input',
      text: 'testuser@example.com'
    });

    await mcp__playwright__browser_type({
      element: 'Password input',
      ref: 'password-input',
      text: 'SecurePassword123!'
    });

    // Submit form
    await mcp__playwright__browser_click({
      element: 'Login button',
      ref: 'login-btn'
    });

    // Wait for navigation
    await mcp__playwright__browser_wait_for({
      text: 'Dashboard'
    });

    // Verify successful login
    const snapshot = await mcp__playwright__browser_snapshot();
    expect(snapshot).toContain('Welcome back');
  });
});
```

#### Package Discovery

```typescript
// tests/e2e/web-portal/package-discovery.spec.ts
describe('Package Discovery', () => {
  test('User can search for packages', async () => {
    // Navigate to discovery page
    await mcp__playwright__browser_navigate({
      url: 'https://localhost:5001/discover'
    });

    // Search for packages
    await mcp__playwright__browser_type({
      element: 'Search input',
      ref: 'search-input',
      text: 'database'
    });

    await mcp__playwright__browser_press_key({
      key: 'Enter'
    });

    // Wait for search results
    await mcp__playwright__browser_wait_for({
      text: 'Search results'
    });

    // Verify search functionality
    const snapshot = await mcp__playwright__browser_snapshot();
    expect(snapshot).toContain('database');

    // Test package details
    await mcp__playwright__browser_click({
      element: 'First package card',
      ref: 'package-card-0'
    });

    await mcp__playwright__browser_wait_for({
      text: 'Package Details'
    });
  });
});
```

### API Testing

```typescript
// tests/e2e/public-api/package-management.spec.ts
describe('Package Management API', () => {
  test('Package registration flow', async () => {
    // Navigate to API documentation
    await mcp__playwright__browser_navigate({
      url: 'https://localhost:5002/swagger'
    });

    // Test API endpoint documentation
    await mcp__playwright__browser_click({
      element: 'POST /api/packages endpoint',
      ref: 'post-packages-endpoint'
    });

    // Verify API documentation
    const snapshot = await mcp__playwright__browser_snapshot();
    expect(snapshot).toContain('Register new package');

    // Test Try it out functionality
    await mcp__playwright__browser_click({
      element: 'Try it out button',
      ref: 'try-it-out-btn'
    });

    // Fill API request body
    await mcp__playwright__browser_type({
      element: 'Request body textarea',
      ref: 'request-body',
      text: JSON.stringify({
        name: 'test-package',
        description: 'Test package for API testing',
        version: '1.0.0'
      })
    });

    // Execute API request
    await mcp__playwright__browser_click({
      element: 'Execute button',
      ref: 'execute-btn'
    });

    // Verify response
    await mcp__playwright__browser_wait_for({
      text: 'Response'
    });
  });
});
```

### Visual Regression Testing

```typescript
// tests/visual-regression/components/button.spec.ts
describe('Button Component Visual Tests', () => {
  test('Button variants render correctly', async () => {
    await mcp__playwright__browser_navigate({
      url: 'https://localhost:5001/storybook'
    });

    // Test primary button
    await mcp__playwright__browser_click({
      element: 'Primary button story',
      ref: 'primary-btn-story'
    });

    await mcp__playwright__browser_take_screenshot({
      filename: 'button-primary.png',
      element: 'Button component',
      ref: 'button-component'
    });

    // Test secondary button
    await mcp__playwright__browser_click({
      element: 'Secondary button story',
      ref: 'secondary-btn-story'
    });

    await mcp__playwright__browser_take_screenshot({
      filename: 'button-secondary.png',
      element: 'Button component',
      ref: 'button-component'
    });

    // Test button states
    await mcp__playwright__browser_hover({
      element: 'Button component',
      ref: 'button-component'
    });

    await mcp__playwright__browser_take_screenshot({
      filename: 'button-hover.png',
      element: 'Button component',
      ref: 'button-component'
    });
  });
});
```

### Accessibility Testing

```typescript
// tests/accessibility/wcag-compliance.spec.ts
describe('WCAG Compliance', () => {
  test('Page meets accessibility standards', async () => {
    await mcp__playwright__browser_navigate({
      url: 'https://localhost:5001'
    });

    // Get accessibility snapshot
    const snapshot = await mcp__playwright__browser_snapshot();

    // Verify semantic HTML structure
    expect(snapshot).toContain('heading');
    expect(snapshot).toContain('navigation');
    expect(snapshot).toContain('main');
    expect(snapshot).toContain('article');

    // Test keyboard navigation
    await mcp__playwright__browser_press_key({
      key: 'Tab'
    });

    await mcp__playwright__browser_press_key({
      key: 'Enter'
    });

    // Verify focus management
    const focusedSnapshot = await mcp__playwright__browser_snapshot();
    expect(focusedSnapshot).toContain('focused');
  });
});
```

## Test Utilities

### Page Object Model

```typescript
// tests/utils/page-objects/LoginPage.ts
export class LoginPage {
  async navigateToLogin() {
    await mcp__playwright__browser_navigate({
      url: 'https://localhost:5001/login'
    });
  }

  async login(email: string, password: string) {
    await mcp__playwright__browser_type({
      element: 'Email input',
      ref: 'email-input',
      text: email
    });

    await mcp__playwright__browser_type({
      element: 'Password input',
      ref: 'password-input',
      text: password
    });

    await mcp__playwright__browser_click({
      element: 'Login button',
      ref: 'login-btn'
    });
  }

  async waitForLoginSuccess() {
    await mcp__playwright__browser_wait_for({
      text: 'Dashboard'
    });
  }
}
```

### Test Data Management

```typescript
// tests/utils/test-data.ts
export const testData = {
  users: {
    admin: {
      email: 'admin@mcphub.dev',
      password: 'AdminPassword123!'
    },
    developer: {
      email: 'developer@mcphub.dev',
      password: 'DevPassword123!'
    }
  },
  packages: {
    testPackage: {
      name: 'test-mcp-server',
      version: '1.0.0',
      description: 'Test MCP server for automation'
    }
  }
};
```

## Test Execution and Reporting

### Test Runner Configuration

```typescript
// tests/config/test-runner.ts
export const testRunner = {
  parallel: true,
  workers: 4,
  timeout: 30000,
  retries: 2,
  reporter: [
    ['html', { outputFolder: 'test-results/html' }],
    ['json', { outputFile: 'test-results/results.json' }],
    ['junit', { outputFile: 'test-results/junit.xml' }]
  ],
  screenshots: {
    mode: 'only-on-failure',
    fullPage: true
  }
};
```

### Performance Monitoring

```typescript
// tests/utils/performance.ts
export class PerformanceMonitor {
  async measurePageLoad() {
    const start = Date.now();
    await mcp__playwright__browser_navigate({
      url: 'https://localhost:5001'
    });
    const end = Date.now();
    return end - start;
  }

  async measureNetworkRequests() {
    await mcp__playwright__browser_navigate({
      url: 'https://localhost:5001'
    });
    const requests = await mcp__playwright__browser_network_requests();
    return requests.length;
  }
}
```

## Continuous Integration Integration

### GitHub Actions Workflow

```yaml
# .github/workflows/playwright-tests.yml
name: Playwright Tests
on:
  push:
    branches: [main, dev]
  pull_request:
    branches: [main]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0.x'
      - name: Install dependencies
        run: dotnet restore
      - name: Build application
        run: dotnet build
      - name: Start services
        run: dotnet run --project Source/AppHost &
      - name: Run Playwright tests
        run: |
          # Run tests using Claude Code with Playwright MCP
          # This would integrate with your CI/CD pipeline
      - name: Upload test results
        uses: actions/upload-artifact@v3
        if: always()
        with:
          name: playwright-results
          path: test-results/
```

### Docker Configuration

```dockerfile
# tests/Dockerfile.playwright
FROM mcr.microsoft.com/playwright:focal

WORKDIR /app
COPY . .

# Install dependencies
RUN apt-get update && apt-get install -y \
    nodejs \
    npm

# Configure test environment
ENV PLAYWRIGHT_BROWSERS_PATH=/ms-playwright
ENV NODE_ENV=test

# Run tests
CMD ["npm", "run", "test:e2e"]
```

## Best Practices

### Test Organization

1. **Separation of Concerns**: Separate unit, integration, and E2E tests
2. **Page Object Model**: Use page objects for maintainable tests
3. **Test Data**: Manage test data separately from test logic
4. **Environment Configuration**: Support multiple environments

### Performance Optimization

1. **Parallel Execution**: Run tests in parallel where possible
2. **Selective Testing**: Run only relevant tests for code changes
3. **Resource Management**: Properly clean up browser instances
4. **Caching**: Cache test data and resources

### Maintenance

1. **Regular Updates**: Keep test dependencies updated
2. **Flaky Test Management**: Identify and fix unstable tests
3. **Documentation**: Maintain comprehensive test documentation
4. **Code Reviews**: Review test code as rigorously as application code

### Error Handling

1. **Meaningful Assertions**: Use descriptive test assertions
2. **Error Screenshots**: Capture screenshots on test failures
3. **Logging**: Comprehensive logging for debugging
4. **Retry Logic**: Implement intelligent retry mechanisms

This configuration provides a comprehensive foundation for browser automation and testing using Playwright MCP in the MCP Hub project.
