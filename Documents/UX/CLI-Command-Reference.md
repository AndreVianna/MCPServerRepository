# MCP Hub CLI (mcpm) Command Reference

**Version**: Phase 2 MVP Design  
**Date**: July 27, 2025  
**Status**: UX Design Specification  

---

## Executive Summary

The MCP Package Manager (mcpm) is a high-performance Native AOT CLI tool designed for seamless MCP server discovery, installation, and management. This document defines the complete command structure, interactive flows, and user experience for Phase 2 implementation.

## Current State Analysis

### Existing CLI Infrastructure
- **Location**: Source/CommandLineApp/
- **Current State**: Minimal skeleton with console output
- **Technology**: .NET 9 Native AOT (<10ms startup target)
- **Dependencies**: Domain and Core projects
- **Status**: Ready for Phase 2 enhancement

### Required Enhancements
1. **System.CommandLine Integration**: Full command parsing and help system
2. **Interactive Flows**: Rich user prompts for publish and install workflows
3. **API Integration**: Connection to PublicApi for all operations
4. **Configuration Management**: User credentials and preferences
5. **Progress Indicators**: Real-time feedback for long-running operations

---

## Command Architecture

### Design Principles
- **npm-like Familiarity**: Follow established package manager conventions
- **Security-First**: Three-stage model (fetch → verify → install) prominence
- **Interactive UX**: Guided workflows for complex operations
- **Performance**: <10ms startup, efficient operations
- **Consistency**: Uniform parameter naming and output formats

### Command Hierarchy
```
mcpm [global-options] <command> [command-options] [arguments]
```

### Global Options
```
--help, -h          Show help information
--version, -v       Show version information
--verbose           Enable verbose logging
--quiet, -q         Suppress non-essential output
--config <file>     Use specific configuration file
--registry <url>    Use alternative registry URL
```

---

## Core Commands

### 1. Discovery Commands

#### mcpm search
**Purpose**: Search for MCP packages across the registry  
**Syntax**: `mcpm search <query> [options]`

```bash
# Basic search
mcpm search "file manager"

# Search with filters
mcpm search "ai" --category tools --trust-tier verified --author anthropic

# Output specific packages
mcpm search "data" --json --limit 10
```

**Options**:
- `--category <category>`: Filter by category (ai, data, tools, etc.)
- `--trust-tier <tier>`: Filter by trust tier (unverified, community, professional, enterprise)
- `--author <author>`: Filter by publisher/author
- `--tag <tag>`: Filter by specific tags
- `--limit <num>`: Limit results (default: 20)
- `--json`: Output as JSON
- `--sort <field>`: Sort by downloads, rating, updated (default: relevance)

**Interactive Elements**:
- Fuzzy search suggestions for typos
- "Did you mean?" suggestions
- Category browsing prompt if no results

**Output Format**:
```
Found 15 packages matching "file manager":

📦 @anthropic/file-organizer                    🔒 A+  ⭐ 4.8  📥 15.2k
   Smart file organization with AI assistance
   By: Anthropic • Updated: 2 days ago • v2.1.0

📦 @github/file-browser                         🔒 B+  ⭐ 4.2  📥 8.1k  
   Browse and manage files in repositories
   By: GitHub • Updated: 1 week ago • v1.5.3

Use 'mcpm info <package>' for detailed information
```

#### mcpm info
**Purpose**: Show detailed information about a specific package  
**Syntax**: `mcpm info <package-name> [options]`

```bash
# Package information
mcpm info @anthropic/file-organizer

# Specific version
mcpm info @anthropic/file-organizer@2.0.0

# JSON output
mcpm info @anthropic/file-organizer --json
```

**Options**:
- `--version <version>`: Show specific version info
- `--json`: Output as JSON
- `--security`: Show detailed security report
- `--dependencies`: Show dependency tree

**Output Format**:
```
📦 @anthropic/file-organizer v2.1.0

📋 PACKAGE INFORMATION
   Description: Smart file organization with AI assistance
   Author:      Anthropic <packages@anthropic.com>
   License:     MIT
   Repository:  https://github.com/anthropic/mcp-file-organizer
   Updated:     2 days ago (July 25, 2025)

🔒 SECURITY SCORE: A+ (9.2/10)
   Trust Tier:       Professional
   Vulnerabilities:  0 Critical, 0 High, 1 Medium, 2 Low
   Last Scanned:     1 day ago
   Security Policy:  ✅ Vulnerability disclosure
   
⚡ CAPABILITIES
   Tools:       5 (file-scan, organize, rename, backup, restore)
   Resources:   2 (file-templates, organization-rules)
   Prompts:     3 (organize-prompt, cleanup-prompt, categorize-prompt)
   
📊 STATS
   Downloads:      15,247 (↑ 12% this week)
   GitHub Stars:   1,205
   Contributors:   8
   Dependencies:   3

🏷️  TAGS
   files, organization, ai, productivity, automation

Use 'mcpm install @anthropic/file-organizer' to install
Use 'mcpm verify @anthropic/file-organizer' to run security scan
```

#### mcpm browse
**Purpose**: Interactive category-based package exploration  
**Syntax**: `mcpm browse [category] [options]`

```bash
# Browse all categories
mcpm browse

# Browse specific category
mcpm browse ai-tools

# List categories
mcpm browse --list-categories
```

**Interactive Flow**:
```
📚 MCP Package Categories

   1. 🤖 AI & Machine Learning (342 packages)
   2. 📁 File Management (89 packages)  
   3. 🌐 Web & APIs (156 packages)
   4. 📊 Data & Analytics (203 packages)
   5. 🛠️  Development Tools (178 packages)
   6. 🔐 Security & Privacy (67 packages)
   7. 📱 System Integration (124 packages)

Select category (1-7) or 'q' to quit: 1

🤖 AI & Machine Learning Packages

   📦 @anthropic/claude-assistant     🔒 A+  ⭐ 4.9  📥 28.5k
   📦 @openai/gpt-integration        🔒 A   ⭐ 4.7  📥 22.1k  
   📦 @google/gemini-tools           🔒 A-  ⭐ 4.5  📥 18.3k

[Next] [Previous] [Info: package-name] [Search] [Quit]
```

#### mcpm trending
**Purpose**: Show trending and popular packages  
**Syntax**: `mcpm trending [options]`

```bash
# Show trending packages
mcpm trending

# Trending in specific category
mcpm trending --category ai

# Trending this week
mcpm trending --period week
```

**Options**:
- `--category <category>`: Filter by category
- `--period <period>`: Time period (day, week, month)
- `--limit <num>`: Number of packages to show

---

### 2. Package Management Commands

#### mcpm fetch
**Purpose**: Download package without installation (Stage 1 of security model)  
**Syntax**: `mcpm fetch <package-name> [options]`

```bash
# Fetch latest version
mcpm fetch @anthropic/file-organizer

# Fetch specific version
mcpm fetch @anthropic/file-organizer@2.0.0

# Fetch to specific directory
mcpm fetch @anthropic/file-organizer --output ./downloads
```

**Options**:
- `--version <version>`: Specific version to fetch
- `--output <dir>`: Download directory
- `--verify-signature`: Verify cryptographic signature
- `--no-deps`: Don't fetch dependencies

**Output Format**:
```
🔄 Fetching @anthropic/file-organizer@2.1.0...

   ✅ Package signature verified
   ✅ Downloaded package (2.4 MB)
   ✅ Downloaded dependencies (3 packages)
   
📍 Package fetched to: ~/.mcpm/cache/@anthropic/file-organizer@2.1.0
   
⚠️  Package is NOT installed. Use 'mcpm verify' then 'mcpm install' to complete.
```

#### mcpm verify
**Purpose**: Security verification (Stage 2 of security model)  
**Syntax**: `mcpm verify <package-name> [options]`

```bash
# Verify cached package
mcpm verify @anthropic/file-organizer

# Verify and show detailed report
mcpm verify @anthropic/file-organizer --detailed

# Verify with custom policy
mcpm verify @anthropic/file-organizer --policy ./security-policy.json
```

**Options**:
- `--detailed`: Show comprehensive security report
- `--policy <file>`: Custom security policy
- `--no-sandbox`: Skip sandbox testing (not recommended)
- `--timeout <seconds>`: Sandbox timeout (default: 300)

**Interactive Flow**:
```
🔒 Security Verification: @anthropic/file-organizer@2.1.0

🔍 STATIC ANALYSIS                              ✅ PASSED (2.3s)
   ✅ No malicious patterns detected
   ✅ Code quality score: 8.7/10
   ✅ Dependency audit clean

🏃 DYNAMIC ANALYSIS                             ✅ PASSED (45.2s)
   ✅ Sandbox execution completed
   ✅ No suspicious network activity
   ✅ File system access within bounds
   ✅ Memory usage normal

📋 PERMISSIONS ANALYSIS                         ⚠️  REVIEW REQUIRED
   🟡 File System: Read/Write access to user documents
   🟡 Network: HTTPS requests to api.anthropic.com
   ✅ Environment: No environment variable access
   
🔐 OVERALL SECURITY SCORE: A+ (9.2/10)

⚠️  This package requests the following permissions:
   • Read and write files in your Documents folder
   • Make network requests to api.anthropic.com
   
Do you accept these permissions? [y/N]: y

✅ Security verification completed successfully
   Package is ready for installation with consent

Use 'mcpm install @anthropic/file-organizer' to proceed with installation
```

#### mcpm install
**Purpose**: Install verified package (Stage 3 of security model)  
**Syntax**: `mcpm install <package-name> [options]`

```bash
# Install verified package
mcpm install @anthropic/file-organizer

# Install globally
mcpm install @anthropic/file-organizer --global

# Install with configuration
mcpm install @anthropic/file-organizer --config ./mcp-config.json
```

**Options**:
- `--global`: Install globally for all projects
- `--config <file>`: Configuration file
- `--dry-run`: Show what would be installed
- `--force`: Override security warnings (dangerous)

**Interactive Consent Flow**:
```
🚀 Installing: @anthropic/file-organizer@2.1.0

📋 INSTALLATION CONSENT

   This package will be installed with the following configuration:
   
   🔧 CAPABILITIES
   • 5 Tools: file-scan, organize, rename, backup, restore
   • 2 Resources: file-templates, organization-rules  
   • 3 Prompts: organize-prompt, cleanup-prompt, categorize-prompt
   
   🔐 PERMISSIONS
   • File System: Read/Write access to Documents folder
   • Network: HTTPS requests to api.anthropic.com
   • Environment: No access
   
   📍 INSTALLATION LOCATION
   • Global: ~/.mcpm/packages/@anthropic/file-organizer@2.1.0
   • Config: ~/.mcpm/config/file-organizer.json
   
   ⚠️  By proceeding, you consent to these capabilities and permissions.
   
Continue with installation? [y/N]: y

🔄 Installing package...
   ✅ Package files extracted
   ✅ Dependencies resolved
   ✅ Configuration applied
   ✅ MCP server registered

✅ Installation completed successfully!

   Package: @anthropic/file-organizer@2.1.0
   Installed: ~/.mcpm/packages/@anthropic/file-organizer@2.1.0
   Config: ~/.mcpm/config/file-organizer.json
   
🚀 NEXT STEPS
   • Use 'mcpm list' to see installed packages
   • Use 'mcpm config file-organizer' to modify settings
   • Package is now available in your MCP environment
```

#### mcpm update
**Purpose**: Update installed packages  
**Syntax**: `mcpm update [package-name] [options]`

```bash
# Update all packages
mcpm update

# Update specific package
mcpm update @anthropic/file-organizer

# Check for updates without installing
mcpm update --dry-run
```

**Options**:
- `--dry-run`: Show available updates
- `--major`: Include major version updates
- `--pre-release`: Include pre-release versions

#### mcpm uninstall
**Purpose**: Remove installed packages  
**Syntax**: `mcpm uninstall <package-name> [options]`

```bash
# Uninstall package
mcpm uninstall @anthropic/file-organizer

# Uninstall with cleanup
mcpm uninstall @anthropic/file-organizer --cleanup

# Force uninstall
mcpm uninstall @anthropic/file-organizer --force
```

**Options**:
- `--cleanup`: Remove configuration and cache files
- `--force`: Skip confirmation prompts
- `--keep-config`: Preserve configuration files

---

### 3. Publishing Commands

#### mcpm init
**Purpose**: Initialize new MCP package project  
**Syntax**: `mcpm init [project-name] [options]`

```bash
# Interactive initialization
mcpm init

# Initialize with name
mcpm init my-mcp-server

# Use template
mcpm init my-server --template ai-assistant
```

**Options**:
- `--template <template>`: Use project template
- `--typescript`: Use TypeScript template
- `--python`: Use Python template
- `--minimal`: Minimal package structure

**Interactive Flow**:
```
🚀 MCP Package Initialization

📝 PACKAGE INFORMATION
   Package name: my-awesome-tool
   Description: AI-powered productivity assistant
   Author: John Doe <john@example.com>
   License: MIT
   
🏷️  PACKAGE DETAILS
   Category: [1] AI Tools [2] Data [3] Files [4] Web [5] Other: 1
   Tags (comma-separated): ai, productivity, assistant
   Repository URL: https://github.com/johndoe/my-awesome-tool
   
⚡ CAPABILITIES
   Tools needed? [y/N]: y
   Resources needed? [y/N]: y  
   Prompts needed? [y/N]: n
   
🛠️  DEVELOPMENT SETUP
   Language: [1] TypeScript [2] Python [3] Go [4] Rust: 1
   Package manager: [1] npm [2] yarn [3] pnpm: 1
   
🔧 Creating package structure...
   ✅ Created mcp-manifest.json
   ✅ Created package.json
   ✅ Created src/index.ts
   ✅ Created README.md
   ✅ Created .gitignore
   
✅ Package initialized successfully!

📚 NEXT STEPS
   • cd my-awesome-tool
   • npm install
   • Edit src/index.ts to implement your tools
   • Use 'mcpm test' to test locally
   • Use 'mcpm publish' when ready
```

#### mcpm test
**Purpose**: Test MCP package locally  
**Syntax**: `mcpm test [options]`

```bash
# Test current package
mcpm test

# Test with specific MCP client
mcpm test --client claude-desktop

# Test specific capability
mcpm test --tool my-tool-name
```

**Options**:
- `--client <client>`: Test with specific MCP client
- `--tool <tool>`: Test specific tool
- `--resource <resource>`: Test specific resource
- `--interactive`: Interactive testing mode

#### mcpm validate
**Purpose**: Validate package before publishing  
**Syntax**: `mcpm validate [options]`

```bash
# Validate current package
mcpm validate

# Validate with strict mode
mcpm validate --strict

# Show validation details
mcpm validate --verbose
```

**Output Format**:
```
🔍 Validating MCP Package...

✅ MANIFEST VALIDATION
   ✅ Valid mcp-manifest.json structure
   ✅ Required fields present
   ✅ Version follows SemVer
   ✅ Capabilities properly defined

✅ CODE QUALITY
   ✅ TypeScript compilation successful
   ✅ No lint errors (ESLint)
   ✅ Tests passing (15/15)
   ✅ Code coverage: 92%

✅ SECURITY
   ✅ No known vulnerabilities in dependencies
   ✅ No hardcoded secrets detected
   ✅ Permission manifest valid

✅ COMPATIBILITY
   ✅ MCP protocol version supported
   ✅ Node.js version compatibility
   ✅ Cross-platform compatibility

🎉 Package validation completed successfully!
   Ready for publishing with 'mcpm publish'
```

#### mcpm publish
**Purpose**: Publish package to registry  
**Syntax**: `mcpm publish [options]`

```bash
# Publish package
mcpm publish

# Publish with tag
mcpm publish --tag beta

# Dry run
mcpm publish --dry-run
```

**Interactive Publishing Flow**:
```
🚀 Publishing: my-awesome-tool@1.0.0

🔍 PRE-PUBLISH VALIDATION
   ✅ Package validation passed
   ✅ Authentication verified
   ✅ Version 1.0.0 is new
   ✅ Package name available

📦 PACKAGE SUMMARY
   Name: @johndoe/my-awesome-tool
   Version: 1.0.0
   Size: 2.4 MB (packed)
   Files: 15 included, 432 excluded
   
⚡ CAPABILITIES
   • 3 Tools: search, analyze, optimize
   • 2 Resources: templates, configs
   • 1 Prompt: assistant-prompt
   
🔐 SECURITY
   • Package will be automatically scanned
   • Initial trust tier: Unverified
   • Security scan ETA: ~5 minutes
   
💰 PUBLISHING COST
   • Package publishing: Free
   • Security scanning: Free
   • Storage: Free (under 10MB)
   
⚠️  Once published, version 1.0.0 cannot be unpublished.
   
Proceed with publishing? [y/N]: y

📤 Publishing package...
   ✅ Package uploaded (2.4 MB)
   ✅ Manifest processed
   ✅ Security scan queued
   ✅ Package registered

🎉 Publication successful!

   Package: @johndoe/my-awesome-tool@1.0.0
   Registry: https://registry.mcphub.dev/package/@johndoe/my-awesome-tool
   
🔒 Security scan in progress... (ETA: 5 minutes)
   • Track progress: mcpm status @johndoe/my-awesome-tool
   • View when complete: https://mcphub.dev/@johndoe/my-awesome-tool
   
📢 Share your package:
   • Install command: mcpm install @johndoe/my-awesome-tool
   • Package page: https://mcphub.dev/@johndoe/my-awesome-tool
```

---

### 4. Utility Commands

#### mcpm list
**Purpose**: List installed packages  
**Syntax**: `mcpm list [options]`

```bash
# List all installed packages
mcpm list

# List with details
mcpm list --detailed

# List outdated packages
mcpm list --outdated
```

#### mcpm config
**Purpose**: Manage configuration  
**Syntax**: `mcpm config <command> [options]`

```bash
# Show current config
mcpm config show

# Set configuration value
mcpm config set registry.url https://custom-registry.com

# Reset to defaults
mcpm config reset
```

#### mcpm login
**Purpose**: Authenticate with registry  
**Syntax**: `mcpm login [options]`

```bash
# Interactive login
mcpm login

# Login with token
mcpm login --token <auth-token>
```

#### mcpm doctor
**Purpose**: Diagnose system health  
**Syntax**: `mcpm doctor [options]`

```bash
# Full system check
mcpm doctor

# Check specific component
mcpm doctor --component network
```

**Output Format**:
```
🏥 MCP Hub System Diagnostics

✅ SYSTEM HEALTH
   ✅ mcpm version: 2.1.0 (latest)
   ✅ .NET runtime: 9.0.0
   ✅ Operating system: Windows 11 x64
   ✅ Available memory: 8.2 GB
   ✅ Available disk: 156 GB

✅ CONNECTIVITY
   ✅ Registry reachable: registry.mcphub.dev (42ms)
   ✅ CDN reachable: cdn.mcphub.dev (28ms)
   ✅ Authentication valid
   ✅ DNS resolution working

✅ CONFIGURATION
   ✅ Config file valid: ~/.mcpm/config.json
   ✅ Cache directory: ~/.mcpm/cache (2.1 GB used)
   ✅ Package directory: ~/.mcpm/packages (15 packages)
   ✅ Permissions valid

⚠️  RECOMMENDATIONS
   🟡 Cache cleanup recommended (last cleaned 30 days ago)
   💡 Run 'mcpm cache clean' to free up space

🎉 System is healthy! No issues detected.
```

#### mcpm cache
**Purpose**: Manage package cache  
**Syntax**: `mcpm cache <command> [options]`

```bash
# Show cache status
mcpm cache status

# Clean cache
mcpm cache clean

# Verify cache integrity
mcpm cache verify
```

---

## Error Handling and User Feedback

### Error Categories

#### 1. Network Errors
```
❌ Network Error: Unable to connect to registry
   
   Registry: registry.mcphub.dev
   Error: Connection timeout after 30 seconds
   
🔧 TROUBLESHOOTING
   • Check your internet connection
   • Verify registry URL: mcpm config show registry.url
   • Try using a different DNS server
   • Check firewall settings
   
💡 Need help? Visit: https://docs.mcphub.dev/troubleshooting
```

#### 2. Authentication Errors
```
❌ Authentication Failed
   
   Your authentication token has expired or is invalid.
   
🔧 SOLUTION
   Please log in again: mcpm login
   
💡 For CI/CD environments, use: mcpm login --token $MCPM_TOKEN
```

#### 3. Package Not Found
```
❌ Package Not Found: @example/missing-package
   
   The package '@example/missing-package' does not exist in the registry.
   
🔍 DID YOU MEAN?
   • @example/missing-packages (similar name)
   • @other/missing-package (similar author)
   
💡 Search for packages: mcpm search "missing package"
```

#### 4. Security Violations
```
❌ Security Violation Detected
   
   Package: @suspicious/package@1.0.0
   Issue: Contains obfuscated code attempting network access
   Severity: Critical
   
🔒 SECURITY DETAILS
   • Pattern: Base64 encoded HTTP requests
   • Risk: Data exfiltration potential
   • Scanner: Static analysis v2.1.0
   
⚠️  This package has been quarantined for review.
   
💡 Report security issues: security@mcphub.dev
```

### Progress Indicators

#### Download Progress
```
🔄 Downloading @anthropic/file-organizer@2.1.0...

   ▓▓▓▓▓▓▓▓░░ 75% (1.8MB / 2.4MB) - 2.1 MB/s - ETA: 3s
   
   ✅ Package signature verified
   🔄 Extracting files...
```

#### Security Scan Progress
```
🔒 Security Scanning: @anthropic/file-organizer@2.1.0

   ✅ Static Analysis    (12.3s)
   🔄 Dynamic Analysis   (▓▓▓▓▓░░░░░ 45s / 90s)
   ⏳ Dependency Audit  (pending)
   ⏳ Behavior Analysis  (pending)
```

---

## Configuration Management

### Configuration File Location
- **Windows**: `%USERPROFILE%\.mcpm\config.json`
- **macOS/Linux**: `~/.mcpm/config.json`

### Configuration Structure
```json
{
  "registry": {
    "url": "https://registry.mcphub.dev",
    "timeout": 30000,
    "retries": 3
  },
  "auth": {
    "token": "<encrypted-token>",
    "username": "johndoe"
  },
  "security": {
    "autoVerify": true,
    "trustTierMinimum": "community",
    "sandboxTimeout": 300
  },
  "ui": {
    "colorOutput": true,
    "progressBars": true,
    "verboseErrors": false
  },
  "paths": {
    "cache": "~/.mcpm/cache",
    "packages": "~/.mcpm/packages",
    "temp": "~/.mcpm/temp"
  }
}
```

---

## Performance Targets

### Startup Performance
- **Cold start**: <10ms (Native AOT)
- **Warm start**: <5ms
- **Help display**: <50ms
- **Command parsing**: <1ms

### Operation Performance
- **Search results**: <500ms
- **Package info**: <200ms
- **Install completion**: <30s (average package)
- **Security scan**: <5 minutes (average package)

### Memory Usage
- **Base memory**: <50MB
- **During install**: <200MB
- **Cache size**: Configurable (default: 5GB)

---

## Integration with Web Portal

The CLI is designed to complement the web portal experience:

### Shared Features
- **Consistent Search**: Same search API and results
- **Unified Authentication**: Single sign-on between CLI and web
- **Package Information**: Identical package details
- **Security Reports**: Same security scanning results

### CLI-Specific Advantages
- **Performance**: Faster for repetitive operations
- **Automation**: Scriptable and CI/CD friendly
- **Offline Access**: Cached package information
- **Integration**: Native OS integration

### Web Portal Links
Many CLI commands provide web portal links for enhanced experience:
```bash
mcpm info @anthropic/file-organizer
   # Output includes: "View online: https://mcphub.dev/@anthropic/file-organizer"

mcpm publish
   # Output includes: "Package page: https://mcphub.dev/@johndoe/my-package"
```

---

## Migration from Current State

### Phase 2 Implementation Plan

#### Week 1-2: Core Infrastructure
1. **System.CommandLine Integration**
   - Replace simple console output with full command parsing
   - Implement global options and help system
   - Add configuration management

2. **API Client Development**
   - Create HttpClient wrapper for PublicApi
   - Implement authentication flow
   - Add error handling and retries

#### Week 3-4: Discovery Commands
1. **Search Implementation**
   - Implement `mcpm search` with API integration
   - Add filtering and sorting options
   - Create rich output formatting

2. **Package Information**
   - Implement `mcpm info` command
   - Add security score display
   - Create detailed package views

#### Week 5-6: Package Management
1. **Installation Flow**
   - Implement three-stage security model
   - Create interactive consent flows
   - Add progress indicators

2. **Package Operations**
   - Implement fetch, verify, install commands
   - Add update and uninstall functionality
   - Create configuration management

#### Week 7-8: Publishing Commands
1. **Project Initialization**
   - Implement `mcpm init` with templates
   - Create project validation
   - Add testing capabilities

2. **Publishing Flow**
   - Implement `mcpm publish` with validation
   - Create interactive publishing experience
   - Add progress tracking

#### Week 9-10: Polish and Testing
1. **Error Handling**
   - Comprehensive error messages
   - User-friendly troubleshooting
   - Recovery suggestions

2. **Performance Optimization**
   - Native AOT compilation
   - Startup time optimization
   - Memory usage optimization

### Backward Compatibility
- Current skeleton will be completely replaced
- No breaking changes for users (new installation)
- Configuration migration not needed (new system)

---

## Success Metrics

### User Experience Metrics
- **Command Completion Rate**: >95%
- **Error Resolution Rate**: >90%
- **User Satisfaction**: >4.5/5
- **Task Completion Time**: <30s for common operations

### Performance Metrics
- **Startup Time**: <10ms consistently
- **Search Response**: <500ms
- **Install Success Rate**: >98%
- **Crash Rate**: <0.1%

### Adoption Metrics
- **CLI vs Web Usage**: Target 60/40 split
- **Command Usage**: Most used should be search, install, info
- **Power User Features**: >30% using publish commands
- **Documentation Usage**: <20% requiring help docs

---

*This CLI Command Reference provides the foundation for implementing a world-class package manager CLI that rivals npm, pip, and other established tools while bringing MCP-specific security and capability features to the forefront.*