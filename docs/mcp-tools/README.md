# MCP Tools Documentation

This directory contains comprehensive documentation for MCP (Model Context Protocol) tools integration in the MCP Hub project.

## Documentation Structure

### Core Documentation

- **[Figma Workflow](figma-workflow.md)** - Complete UI design workflow using Figma MCP
- **[Playwright Testing](playwright-testing.md)** - Browser automation and testing configuration

### Test Documentation

- **[Figma Test Results](../../tests/mcp-tools/figma-test.md)** - Figma MCP integration test results
- **[Playwright Test Results](../../tests/mcp-tools/playwright-test.md)** - Playwright MCP integration test results

### Configuration Files

- **[Figma Config](../../config/mcp-tools/figma-config.json)** - Figma MCP configuration
- **[Playwright Config](../../config/mcp-tools/playwright-config.json)** - Playwright MCP configuration
- **[Development Environment](../../config/mcp-tools/dev-environment.json)** - Development integration settings
- **[Team Collaboration](../../config/mcp-tools/team-collaboration.json)** - Team workflow configuration
- **[CI/CD Pipeline](../../config/mcp-tools/ci-cd.json)** - Continuous integration settings

### Scripts

- **[MCP Tools Integration](../../scripts/mcp-tools-integration.sh)** - Utility functions for MCP tools
- **[Validation Script](../../scripts/validate-mcp-tools.sh)** - Environment validation

## Quick Start

1. **Validate Setup**

   ```bash
   ./scripts/validate-mcp-tools.sh
   ```

2. **Extract Figma Assets**

   ```bash
   ./scripts/mcp-tools-integration.sh extract-figma FIGMA_FILE_KEY [output_path]
   ```

3. **Run Playwright Tests**

   ```bash
   ./scripts/mcp-tools-integration.sh run-tests [category] [environment]
   ```

4. **Update Design Tokens**

   ```bash
   ./scripts/mcp-tools-integration.sh design-tokens FIGMA_FILE_KEY
   ```

## Development Workflow

### Design-to-Code Workflow

1. Designer updates Figma file
2. Extract assets using Figma MCP
3. Update design tokens and components
4. Run visual regression tests with Playwright MCP
5. Validate accessibility and performance
6. Create pull request with design changes

### Testing Workflow

1. Write test specifications
2. Implement tests using Playwright MCP
3. Run tests in multiple environments
4. Generate test reports and screenshots
5. Integrate with CI/CD pipeline

## Integration Status

### Figma MCP ✅

- [x] Configuration complete
- [x] Asset extraction workflow
- [x] Design token generation
- [x] Team collaboration setup

### Playwright MCP ✅

- [x] Browser automation configured
- [x] Test framework setup
- [x] Visual regression testing
- [x] Accessibility testing
- [x] Performance monitoring

### Development Environment ✅

- [x] Script automation
- [x] Configuration management
- [x] Validation tools
- [x] Documentation

## Support

For questions and issues:

1. Check the documentation in this directory
2. Review configuration files
3. Run validation scripts
4. Consult team collaboration guidelines

## Contributing

When contributing to MCP tools integration:

1. Follow existing patterns and conventions
2. Update documentation for changes
3. Run validation scripts before committing
4. Include tests for new functionality
