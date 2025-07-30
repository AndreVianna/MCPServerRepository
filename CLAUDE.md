# CLAUDE.md

This file provides essential guidance to Claude Code (claude.ai/code) when working with this repository.

## Project Overview

**MCP Hub** is a comprehensive MCP (Model Context Protocol) server registration repository that works like NPM and NuGet. Authors register their servers and users search and use them in projects.

- **Native AOT CLI**: High-performance mcpm tool for package management
- **Blazor Web App**: Rich web portal for package discovery and management  
- **Semantic search**: AI-powered search with vector embeddings

## Reference Documents

- See `Documents/Core/ARCHITECTURE.md` for detailed project structure guidelines
- See `Documents/Core/ROADMAP.md` for implementation timeline and success metrics
- See `Documents/Core/DEVELOPMENT-GUIDE.md` for technology stack and development approach
- See `Documents/README.md` for complete documentation organization guide

## Documents Folder Structure

The `Documents/` folder is organized as follows:

```
Documents/
├── Core/                           # Master project documents
│   ├── ARCHITECTURE.md             # Technical architecture and design patterns
│   ├── BUSINESS-CASE.md            # Business case and market analysis
│   ├── DEVELOPMENT-GUIDE.md        # Development workflow and standards
│   ├── ROADMAP.md                  # Project roadmap and implementation timeline
│   └── PHASE1-COMPLETION-REPORT.md # Phase 1 completion documentation
├── UX/                            # User Experience design specifications
│   ├── README.md                   # UX documentation navigation index
│   ├── CLI-Command-Reference.md    # Complete CLI command structure and flows
│   ├── Web-Application-User-Journey.md # Web app site architecture and specifications
│   ├── UX-Design-Summary.md        # Executive summary of UX deliverables
│   ├── UI-DESIGN-SYSTEM.md        # Visual design foundation and component library
│   ├── UI-DESIGN-IMPLEMENTATION-SUMMARY.md # Technical implementation guidelines
│   ├── HOMEPAGE-MOCKUP-SPEC.md    # Homepage design specifications
│   ├── SEARCH-RESULTS-MOCKUP-SPEC.md # Search interface specifications
│   ├── PACKAGE-DETAIL-MOCKUP-SPEC.md # Package detail page specifications
│   └── PUBLISHER-DASHBOARD-MOCKUP-SPEC.md # Publisher dashboard specifications
├── Investor-Package/              # Investor and funding documentation
│   ├── MARKET-ANALYSIS.md          # Market research and competitive analysis
│   ├── FINANCIAL-PROJECTIONS.md   # Financial models and projections
│   ├── TECHNICAL-OVERVIEW.md       # Technical overview for investors
│   └── [8 additional investor documents]
├── Technical-Specifications/      # Detailed technical specifications
│   ├── HEXAGONAL-ARCHITECTURE-SPECIFICATION.md # Architecture patterns
│   └── HEXAGONAL-ARCHITECTURE-IMPLEMENTATION-STATUS.md # Implementation status
└── README.md                      # Documentation overview and navigation
```

**Key Documentation Areas:**
- **Core/**: Essential project documents for development teams
- **UX/**: Complete user experience specifications for Phase 2 implementation
- **Investor-Package/**: Business-focused documentation for funding and partnerships
- **Technical-Specifications/**: Detailed architectural and implementation specifications

## Development Tool Requirements

**CRITICAL REQUIREMENT**: All build, lint, and test operations must use the project.sh script.

**MANDATORY Commands:**
- `./Scripts/project.sh build` - Build the entire solution or specific projects
- `./Scripts/project.sh lint` - Format code using dotnet format
- `./Scripts/project.sh test` - Execute all unit tests with proper reporting
- `./Scripts/project.sh init` - Create and configure development container
- `./Scripts/project.sh doctor` - Validate development environment

**Development Rules:**
1. Always use `./Scripts/project.sh` for development operations
2. Never use direct `dotnet` commands - all operations must go through the script
3. Fix underlying project issues to enable successful script execution
4. Script correctness takes precedence over all other considerations

**Required Tools:**
- All .NET instructions must use `dotnet` commands within project.sh script
- All Entity Framework tasks must use `dotnet ef` tool within container environment
- All UI design must use **Figma MCP**
- All UI testing must use **Playwright MCP**

## Main commands for project.sh

- `./scripts/project.sh init` - Initializes the development container
- `./scripts/project.sh doctor` - Validates the development environment
- `./scripts/project.sh lint` - Formats code using dotnet format
- `./scripts/project.sh build` - Builds the entire solution or specified projects
- `./scripts/project.sh build --project <project>` - Builds a specific project
- `./scripts/project.sh build --project <project>.UnitTests` - Builds a specific project's unit tests
- `./scripts/project.sh test` - Runs all unit tests with proper reporting
- `./scripts/project.sh test --project <project>` - Runs unit tests for a specific project

## Core Architecture Principles

**EXTREMELY IMPORTANT**: Always maintain full abstraction layers while implementing only one provider:

- **Storage**: Keep `IStorageService` interface + factory pattern, implement only Azure Blob Storage
- **Event Handling**: Keep messaging abstractions + factory pattern, implement only one provider  
- **APIs**: Keep service abstractions + factory pattern, implement only one provider per service
- **Configuration**: Keep provider-agnostic interfaces with single provider implementations
- **All Services**: This principle applies to Storage, Event Handling, APIs, and ALL other services

This ensures future extensibility while maintaining clean architecture principles and allows easy addition of new providers without architectural changes.

## Essential Technology Constraints

- **.NET 9** with C# 13 preview features as unified platform
- **PostgreSQL** as primary database with Entity Framework Core  
- **Clean Architecture** principles with comprehensive testing

## Audit System Architecture

**CRITICAL REQUIREMENT**: All domain entities use granular audit trail system:

- **Entity IDs**: All entities use `Guid` type initialized with `Guid.CreateVersion7()` for better database indexing
- **Audit Trail**: Replace old audit properties (`CreatedAt`, `UpdatedAt`, `UpdatedBy`) with `ICollection<IAuditEntry> AuditTrail`
- **Granular Tracking**: Each entity operation adds specific audit entries with `Action`, `UserId`, and `DateTime`
- **ApplicationUser Integration**: ApplicationUser includes audit trail to track user profile changes
- **Entity Framework**: AuditTrail collections stored as JSON in database with proper EF configuration
- **Repository Sorting**: Use AuditTrail queries instead of old audit properties for creation/update date sorting

**Example Audit Entry:**
```csharp
AuditTrail.Add(new AuditEntry {
    Action = "Created", // or "Updated", "Verified", etc.
    UserId = currentUserId, // or Guid.Empty for system actions
    DateTime = DateTimeOffset.UtcNow
});
```

This system provides comprehensive change tracking while maintaining clean domain entity design.

## Contracts First Development Principle

**CRITICAL ARCHITECTURAL GUIDELINE**: Prioritize contracts and interfaces over implementations.

**Core Principle**: "Do not code what is not needed. Contracts and interfaces are more important at this moment."

### **Development Approach:**

- **Interfaces First**: Define clear interfaces and contracts before any implementation
- **YAGNI Enforcement**: You Aren't Gonna Need It - implement functionality only when actually required by consumers
- **Buildable Skeletons**: Create service classes with NotImplementedException to maintain compilation
- **Explicit Dependencies**: Make all dependencies clear through interface contracts
- **Parallel Development**: Teams can work against interfaces while implementations are developed separately

### **Implementation Guidelines:**

1. **Define Interface**: Create clear method signatures with proper return types and parameters
2. **Create Skeleton**: Implement interface with NotImplementedException placeholders  
3. **Register Services**: Add to DI container to prevent build errors
4. **Implement When Needed**: Add actual logic only when functionality is consumed
5. **Test Contracts**: Focus on testing interface contracts, not implementation details

**Benefits**: Prevents over-engineering, maintains clean architecture boundaries, enables parallel development, reduces unnecessary complexity, and follows Lean principles.

**Example Pattern**:
```csharp
public interface IAuthenticationService {
    Task<AuthenticationResult> LoginAsync(LoginRequest request);
    Task<TokenResult> RefreshTokenAsync(string refreshToken);
}

public class AuthenticationService : IAuthenticationService {
    public Task<AuthenticationResult> LoginAsync(LoginRequest request) 
        => throw new NotImplementedException("Authentication logic will be implemented when first consumer requires it");
}
```

## Critical Development Rule: No Assumptions About Library APIs

**EXTREMELY IMPORTANT**: Never make assumptions about library APIs, syntax, or behavior without verification:

- **Use documentation reference** for all libraries, frameworks, APIs, and tools serach the web for correct and latest usage.
- **Always verify library syntax** by searching official documentation when in doubt
- **Ask the user for clarification** if documentation is unclear or contradictory
- **Never assume API patterns** based on library names or similar libraries
- **Check working examples** in the codebase before making changes
- **Web search official docs** for authoritative syntax and usage patterns

This rule applies to ALL external libraries, frameworks, and APIs to prevent incorrect implementations.

## Project Structure Exploration

- Always use the bash command `tree -I '.claude|.git|bin|obj|lib|.github|.cursor|.vscode|.vs|Assets'` to get the file structure of the project.

## Git Repository

- **Main branch**: `main` | **License**: MIT

## TODO Tracking Process

**IMPORTANT**: All TODO management must follow this process to ensure task continuity across conversation sessions:

### Memory Integration
- **TODO Memory Entity**: A special memory entity named 'TODO' tracks all active tasks
- **Automatic Updates**: Every time the TodoWrite tool is used, the TODO memory entity must be updated
- **Format**: 'Number. Task description - Status: [pending|in_progress|completed], Priority: [high|medium|low]'
- **Hierarchical Numbering**: Tasks must be numbered with hierarchy: '1. Main task', '1.1. Sub-task', '1.2. Sub-task', '2. Next main task'
- **Cleanup**: Completed tasks should be removed from the TODO memory entity during updates

### Process Steps
1. **Task Creation**: When new tasks are added via TodoWrite, add them to the TODO memory entity
2. **Status Updates**: When task status changes, update the corresponding entry in the TODO memory entity
3. **Task Completion**: When tasks are completed, remove them from the TODO memory entity
4. **Session Continuity**: The TODO memory entity provides persistent context across conversation sessions

### Memory Operations
- Use `mcp__memory__add_observations` to add new tasks to the TODO memory entity
- Use `mcp__memory__delete_observations` to remove completed tasks from the TODO memory entity
- Maintain hierarchical numbering with proper task dependencies and logical grouping
- Main tasks (1., 2., 3.) represent major work areas or phases
- Sub-tasks (1.1., 1.2., 1.3.) represent specific actions within a main task
- Sub-sub-tasks (1.1.1., 1.1.2.) can be used for detailed breakdowns when needed

This system ensures no work is lost between sessions and provides clear project progress tracking.

### **Contracts First Task Creation Guidelines**

**IMPORTANT**: All new service and feature tasks must follow the Contracts First approach:

- **Phase 1 - Contracts**: Always separate "Define Interfaces" from "Implement Logic"
- **Task Pattern**: Use format like "1.1 Define IServiceName interface" then "1.2 Implement ServiceName skeleton"
- **Implementation Tasks**: Mark implementation tasks as "when needed" or "when first consumer requires"
- **Skeleton Priority**: Interface and skeleton creation should be high priority, implementation should be lower priority
- **Dependency Clarity**: Tasks should make interface dependencies explicit before implementation dependencies

**Example Task Structure**:
```
1. Create AuthenticationService Infrastructure
1.1. Define IAuthenticationService interface - Status: pending, Priority: high
1.2. Create AuthenticationService skeleton with NotImplementedException - Status: pending, Priority: high
1.3. Register services in DI container - Status: pending, Priority: high
1.4. Implement login logic - Status: pending, Priority: low (when first consumer needs it)
```

This ensures all future development follows the contracts-first architectural principle.

## Specialized Agent Usage Requirement

**CRITICAL REQUIREMENT**: All implementation tasks must use the appropriate specialized sub-agents:

### **Mandatory Agent Usage:**
- **Backend Development**: MUST use `backend-developer` sub-agent for all .NET/C# backend implementation
- **Frontend Development**: MUST use `frontend-developer` sub-agent for all Blazor/UI implementation
- **Never Implement Directly**: Never implement backend or frontend code directly - always delegate to specialized agents

### **Parallel Execution Strategy:**
- **Multiple Task Invocations**: Run multiple Task tool calls in a SINGLE message for parallel execution
- **Task Decomposition**: Break complex tasks requiring both agents into smaller, agent-specific subtasks
- **Expertise Alignment**: Ensures proper technical expertise is applied to each implementation area

### **Implementation Process:**
1. **Identify Task Type**: Determine if task requires backend, frontend, or both agents
2. **Decompose Complex Tasks**: Split tasks that need both agents into separate backend and frontend subtasks
3. **Parallel Execution**: Use multiple Task invocations in one message when possible
4. **Agent Specialization**: Let each agent focus on their technical domain expertise

**Benefits**: Ensures expert implementation quality, enables true parallel development, maintains separation of concerns, and maximizes development efficiency.

This requirement applies to ALL Phase 2 implementation work and ensures optimal use of specialized technical expertise.

## Critical Build Verification Protocol

**EXTREMELY CRITICAL**: Prevent false success claims through rigorous verification.

### **Root Cause of False Claims:**
- Regular build success (0 errors, 0 warnings) does NOT equal strict build success  
- Warnings become errors in strict mode - must verify actual requirement
- Using grep to filter build output hides true error counts
- Trusting agent success claims without independent verification

### **Mandatory Verification Protocol:**

**NEVER** claim success without following this exact sequence:

1. **Run Exact User Command**: Use the EXACT command specified by user (e.g., `./Scripts/project.sh build --strict`)
2. **No Grep During Verification**: NEVER use grep during final verification - always check full unfiltered build output
3. **Check Complete Output**: Look for actual "X Warning(s) Y Error(s)" counts in full output
4. **Only Report Verified Results**: Only claim success when command shows "0 Warning(s) 0 Error(s)"

### **Verification Rules:**
- **grep Usage**: ONLY use grep during correction tasks to analyze specific error patterns
- **Final Verification**: Must use complete unfiltered build output to get accurate counts
- **Progress Reporting**: Report honest progress (e.g., "reduced from 88 to 45 errors") instead of false success
- **Todo Status**: Keep todos as "in_progress" until truly verified complete
- **Agent Oversight**: Always verify agent success claims with independent testing

### **Critical Commands:**
- **Verification**: `./Scripts/project.sh build --strict` (no grep)
- **Analysis**: `./Scripts/project.sh build --strict 2>&1 | grep "error"` (grep OK for analysis)

**Violation of this protocol has led to repeated false success claims and wasted user time.**