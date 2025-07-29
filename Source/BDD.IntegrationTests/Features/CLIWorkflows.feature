Feature: CLI Tool Workflows and Commands
  In order to manage MCP packages efficiently from the command line
  As a developer or package manager
  I want to use the mcpm CLI for all package operations with security-first approach

  Background:
    Given the MCP Hub API is running
    And the CLI tool is installed and configured
    And the database contains test packages

  @cli @search @basic
  Scenario: Basic package search
    When I run "mcpm search file"
    Then the command should succeed
    And I should see packages matching "file" in the output
    And each package should display:
      | Information   | Format                          |
      | Name          | @publisher/package-name         |
      | Trust Tier    | Security badge (A+, B+, etc.)  |
      | Rating        | Star rating (⭐ 4.8)           |
      | Downloads     | Download count (📥 15.2k)       |
      | Description   | Brief package description       |
    And I should see usage hint about "mcpm info <package>" for details

  @cli @search @filters
  Scenario: Search with category filter
    When I run "mcpm search ai --category tools --trust-tier community"
    Then the command should succeed
    And I should see only packages that match all criteria:
      | Criteria      | Expected                        |
      | Query         | Contains "ai"                   |
      | Category      | In "tools" category             |
      | Trust Tier    | Community Trusted or higher     |
    And the search results should indicate active filters
    And I should see the total count of matching packages

  @cli @search @pagination
  Scenario: Search with pagination
    Given there are more than 20 packages in the registry
    When I run "mcpm search * --limit 10 --page 2"
    Then the command should succeed
    And I should see exactly 10 package results
    And I should see pagination information showing "Page 2 of X"
    And I should see hints for navigating to other pages

  @cli @info @detailed
  Scenario: Get detailed package information
    Given there is a package "@anthropic/file-organizer" with full details
    When I run "mcpm info @anthropic/file-organizer"
    Then the command should succeed
    And I should see comprehensive package information:
      | Section           | Content                             |
      | Package Info      | Name, version, description, author  |
      | Security Score    | A+ (9.2/10) with last scan date    |
      | Capabilities      | Tools (5), Resources (2), Prompts (3)|
      | Statistics        | Downloads, GitHub stars, contributors|
      | Tags              | Comma-separated tag list            |
      | Installation      | mcpm install command                |
      | Verification      | mcpm verify command option         |
    And the output should be well-formatted and readable

  @cli @info @security
  Scenario: Get package security details
    Given there is a package with security scan results
    When I run "mcpm info @publisher/package --security"
    Then the command should succeed
    And I should see detailed security information:
      | Security Detail   | Information                         |
      | Trust Tier        | Current tier with explanation       |
      | Security Score    | Numerical score breakdown           |
      | Vulnerabilities   | Count by severity (Critical/High/Medium/Low)|
      | Last Scan Date    | When security scan was performed    |
      | Scan Duration     | Time taken for complete scan        |
      | Policy Compliance | Which security policies it meets    |
    And if vulnerabilities exist, I should see remediation guidance

  @cli @fetch @basic
  Scenario: Fetch package without installation (Stage 1)
    Given there is a package "@publisher/test-package"
    When I run "mcpm fetch @publisher/test-package"
    Then the command should succeed
    And I should see progress indicators during download
    And the output should show:
      | Information       | Details                             |
      | Package Signature | ✅ Package signature verified       |
      | Download Status   | ✅ Downloaded package (X.X MB)      |
      | Dependencies      | ✅ Downloaded dependencies (N packages)|
      | Cache Location    | Path where package was cached       |
      | Next Steps        | Guidance to verify then install     |
    And the package should be cached locally
    And the package should NOT be installed yet

  @cli @verify @interactive
  Scenario: Security verification with user consent (Stage 2)
    Given I have fetched a package "@publisher/test-package"
    When I run "mcpm verify @publisher/test-package"
    Then the command should start security verification
    And I should see detailed verification progress:
      | Verification Stage| Progress Indicator                  |
      | Static Analysis   | ✅ PASSED (X.Xs) with details      |
      | Dynamic Analysis  | ✅ PASSED (XX.Xs) with sandbox info|
      | Dependency Audit  | ✅ PASSED with vulnerability check |
      | Permissions Review| ⚠️ REVIEW REQUIRED with details    |
    And I should see permission requests clearly:
      | Permission Type   | Description                         |
      | File System       | Read/Write access to specific folders|
      | Network           | HTTPS requests to specific domains  |
      | Environment       | Access to environment variables     |
    And I should be prompted "Do you accept these permissions? [y/N]"
    And the overall security score should be displayed

  @cli @verify @automated
  Scenario: Automated verification for trusted packages
    Given I have fetched a highly trusted package with no permission requests
    When I run "mcpm verify @trusted/clean-package"
    Then the command should succeed automatically
    And I should see verification results:
      | Result            | Status                              |
      | Static Analysis   | ✅ PASSED - No issues found        |
      | Dynamic Analysis  | ✅ PASSED - Clean execution         |
      | Dependencies      | ✅ PASSED - All dependencies clean  |
      | Permissions       | ✅ No special permissions required  |
      | Overall Score     | A+ (9.5/10)                        |
    And I should see message "Package is ready for installation"
    And I should see next step: "mcpm install @trusted/clean-package"

  @cli @install @interactive
  Scenario: Install verified package with consent (Stage 3)
    Given I have verified a package "@publisher/test-package"
    When I run "mcpm install @publisher/test-package"
    Then I should see installation consent dialog:
      | Consent Section   | Information                         |
      | Package Summary   | Name, version, publisher details    |
      | Capabilities      | List of tools, resources, prompts   |
      | Permissions       | Required permissions summary        |
      | Installation Path | Where package will be installed     |
      | Configuration     | Config files to be created         |
    And I should be prompted "Continue with installation? [y/N]"
    And when I respond "y"
    Then the installation should proceed with progress indicators
    And I should see successful installation confirmation
    And the package should be registered in my MCP environment

  @cli @install @global
  Scenario: Global package installation
    Given I have verified a package "@publisher/global-tool"
    When I run "mcpm install @publisher/global-tool --global"
    Then the installation should proceed globally
    And I should see installation location: "~/.mcpm/packages/"
    And the package should be available system-wide
    And configuration should be stored in global config directory
    And I should see post-installation setup instructions

  @cli @list @installed
  Scenario: List installed packages
    Given I have installed several packages
    When I run "mcpm list"
    Then the command should succeed
    And I should see a table of installed packages:
      | Column        | Information                         |
      | Name          | Package name with namespace         |
      | Version       | Currently installed version         |
      | Trust Tier    | Current trust tier badge            |
      | Status        | Active, needs update, deprecated    |
      | Location      | Global or local installation        |
    And I should see summary statistics at the bottom
    And I should see hints for package management commands

  @cli @list @outdated
  Scenario: List outdated packages
    Given I have installed packages with available updates
    When I run "mcpm list --outdated"
    Then the command should succeed
    And I should see only packages with available updates
    And each outdated package should show:
      | Information       | Details                             |
      | Current Version   | Currently installed version         |
      | Latest Version    | Available latest version            |
      | Update Type       | Patch, minor, or major update       |
      | Security Updates  | Whether update includes security fixes|
    And I should see update command for each package
    And I should see bulk update option

  @cli @update @single
  Scenario: Update single package
    Given I have an outdated package "@publisher/old-package"
    When I run "mcpm update @publisher/old-package"
    Then the command should fetch the latest version
    And I should see security verification for the new version
    And if the update changes permissions, I should be prompted for consent
    And the installation should proceed with progress indicators
    And I should see update success confirmation
    And the old version should be preserved for rollback if needed

  @cli @update @all
  Scenario: Update all packages
    Given I have multiple outdated packages
    When I run "mcpm update"
    Then I should see a list of packages to be updated
    And I should be prompted to confirm bulk update
    And when I confirm, each package should be updated sequentially
    And I should see overall progress indicator
    And any packages with permission changes should require individual consent
    And I should see summary of successful and failed updates

  @cli @publish @init
  Scenario: Initialize new package project
    When I run "mcpm init my-awesome-tool" in an empty directory
    Then the command should start the interactive initialization wizard
    And I should be prompted for package information:
      | Prompt            | Example                             |
      | Package name      | my-awesome-tool                     |
      | Description       | AI-powered productivity assistant   |
      | Author            | John Doe <john@example.com>        |
      | License           | MIT, Apache-2.0, etc.              |
      | Category          | AI Tools, Data, Files, Web, Other  |
      | Tags              | ai, productivity, assistant         |
      | Repository URL    | https://github.com/user/repo       |
    And I should be asked about capabilities (tools, resources, prompts)
    And I should choose development language (TypeScript, Python, Go, Rust)
    And the project structure should be created with all necessary files

  @cli @publish @validate
  Scenario: Validate package before publishing
    Given I am in a package project directory
    When I run "mcpm validate"
    Then the command should perform comprehensive validation:
      | Validation Type   | Checks                              |
      | Manifest          | Valid mcp-manifest.json structure   |
      | Code Quality      | Compilation, linting, tests         |
      | Security          | No hardcoded secrets, dependencies |
      | Compatibility     | MCP protocol, platform support     |
    And I should see detailed validation results
    And if validation fails, I should see specific error messages with guidance
    And if validation succeeds, I should see "Ready for publishing" message

  @cli @publish @interactive
  Scenario: Publish package to registry
    Given I have a validated package project
    And I am authenticated as a publisher
    When I run "mcpm publish"
    Then I should see pre-publish validation summary
    And I should see package publishing preview:
      | Preview Section   | Information                         |
      | Package Summary   | Name, version, size, files included |
      | Capabilities      | Tools, resources, prompts summary   |
      | Security Status   | Pre-publish security assessment     |
      | Publishing Cost   | Storage and scanning costs (if any) |
      | Trust Tier        | Initial tier (typically Unverified)|
    And I should be warned about immutability of published versions
    And I should be prompted "Proceed with publishing? [y/N]"
    And when I confirm, the upload should proceed with progress indicators
    And I should receive publication confirmation with package URL

  @cli @config @management
  Scenario: Configuration management
    When I run "mcpm config show"
    Then I should see current configuration settings:
      | Configuration     | Current Value                       |
      | Registry URL      | https://registry.mcphub.dev        |
      | Cache Directory   | ~/.mcpm/cache                       |
      | Package Directory | ~/.mcpm/packages                    |
      | Auth Token        | [Hidden] Valid until X             |
      | Security Settings | Trust tier minimum, auto-verify    |
      | UI Preferences    | Colors, progress bars, verbosity   |
    And I should see commands to modify each setting
    And sensitive information should be appropriately masked

  @cli @config @modification
  Scenario: Modify configuration settings
    When I run "mcpm config set security.trustTierMinimum community"
    Then the configuration should be updated
    And I should see confirmation message
    And when I run "mcpm config show"
    Then I should see the updated value
    And when I run "mcpm config reset security.trustTierMinimum"
    Then the setting should return to default value

  @cli @auth @login
  Scenario: Interactive authentication
    Given I am not authenticated
    When I run "mcpm login"
    Then I should be prompted for my credentials:
      | Prompt            | Type                                |
      | Email             | Email address input                 |
      | Password          | Hidden password input               |
      | 2FA Code          | If 2FA is enabled for account      |
    And after successful authentication, I should see welcome message
    And my auth token should be stored securely in configuration
    And I should see my account information summary

  @cli @auth @token
  Scenario: Token-based authentication for CI/CD
    Given I have a valid API token
    When I run "mcpm login --token $MCPM_TOKEN"
    Then the authentication should succeed
    And I should see confirmation of token-based login
    And the token should be stored for subsequent commands
    And I should see account information for the token owner

  @cli @doctor @diagnosis
  Scenario: System health check
    When I run "mcpm doctor"
    Then the command should perform comprehensive system diagnostics:
      | Check Category    | Diagnostic Tests                    |
      | System Health     | mcpm version, .NET runtime, OS info|
      | Connectivity      | Registry reachable, CDN access, DNS |
      | Configuration     | Config file valid, permissions OK   |
      | Cache Status      | Cache directory size and integrity  |
      | Package Status    | Installed packages validation       |
    And I should see clear status indicators (✅ ❌ ⚠️) for each check
    And any issues should include troubleshooting recommendations
    And I should see overall system health summary

  @cli @doctor @recommendations
  Scenario: System optimization recommendations
    Given my system has optimization opportunities
    When I run "mcpm doctor"
    Then I should see recommendations section:
      | Recommendation Type| Suggestion                          |
      | Cache Cleanup      | Cache cleanup to free space        |
      | Package Updates    | Outdated packages to update         |
      | Security Updates   | Packages with security updates      |
      | Configuration      | Settings that could be optimized    |
    And each recommendation should include the command to address it
    And I should see priority levels for recommendations

  @cli @cache @management
  Scenario: Cache management operations
    When I run "mcpm cache status"
    Then I should see cache information:
      | Cache Information | Details                             |
      | Location          | ~/.mcpm/cache                       |
      | Total Size        | X.X GB used                         |
      | Package Count     | Number of cached packages           |
      | Last Cleanup      | Date of last cleanup                |
      | Oldest Entry      | Date of oldest cached item          |
    And when I run "mcpm cache clean"
    Then I should see cleanup progress
    And obsolete cache entries should be removed
    And I should see space reclaimed summary

  @cli @offline @mode
  Scenario: Offline mode operations
    Given I have cached packages and metadata
    When I lose internet connectivity
    And I run "mcpm search file --offline"
    Then the command should search cached metadata
    And I should see packages matching "file" from cache
    And I should see indicators that results are from offline cache
    And I should see message about limited offline functionality

  @cli @offline @installation
  Scenario: Install cached package offline
    Given I have cached and verified a package "@publisher/cached-package"
    And I am offline
    When I run "mcpm install @publisher/cached-package --offline"
    Then the installation should proceed using cached files
    And I should see confirmation that installation used offline cache
    And the package should be installed successfully
    And I should see warning about inability to check for updates

  @cli @interactive @prompts
  Scenario: Interactive prompts and user experience
    Given I am running an interactive command
    When the command requires user input
    Then prompts should be clear and well-formatted
    And I should see default values where appropriate
    And I should be able to navigate options with arrow keys
    And I should be able to cancel operations with Ctrl+C
    And progress indicators should be smooth and informative
    And error messages should be helpful and actionable

  @cli @error @handling
  Scenario: Comprehensive error handling
    When I run "mcpm install non-existent-package"
    Then I should see a clear error message:
      | Error Element     | Content                             |
      | Error Type        | Package Not Found                   |
      | Error Description | Clear explanation of what went wrong|
      | Suggestions       | Similar package names if available  |
      | Next Steps        | Commands to search for packages     |
      | Help Reference    | Link to documentation or support    |
    And the exit code should be non-zero
    And the error should be logged appropriately

  @cli @performance @startup
  Scenario: CLI performance requirements
    When I run any mcpm command
    Then the command should start within 10ms (cold start)
    And subsequent commands should start within 5ms (warm start)
    And help display should appear within 50ms
    And command parsing should complete within 1ms
    And memory usage should remain under 50MB for basic operations

  @cli @cross-platform @compatibility
  Scenario: Cross-platform compatibility
    When I run mcpm commands on different platforms
    Then commands should work identically on:
      | Platform          | Requirements                        |
      | Windows           | PowerShell and Command Prompt       |
      | macOS             | Terminal with standard Unix tools   |
      | Linux             | Bash and standard distributions     |
    And file paths should use appropriate separators for each platform
    And configuration should be stored in platform-appropriate locations
    And native OS credential storage should be used where available