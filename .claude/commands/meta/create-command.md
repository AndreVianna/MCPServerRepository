---
allowed-tools: mcp__thinking__sequentialthinking, WebSearch, WebFetch, Read, LS, Write, ExitPlanMode, Grep
description: Meta-command to intelligently create new slash commands with optional error handling (Windows version)
argument-hint: "Detailed description of what the command should accomplish"
---

# Create Command Meta-Generator (Windows)

Intelligently generate new slash commands based on natural language descriptions. This meta-command uses systematic analysis, research, and interactive consultation to create purpose-built commands with user-controlled complexity levels - from simple utilities to robust production-grade commands.

**Platform**: Windows PowerShell compatible

## Instructions

### Phase 1: Requirement Analysis with Sequential Thinking
- Use `mcp__thinking__sequentialthinking` to systematically analyze the provided description:
  ```
  - Parse the primary purpose and goals
  - Identify required tools and capabilities  
  - Determine input/output requirements
  - Assess integration points with existing system
  - Evaluate complexity level and scope appropriateness
  - Extract key action verbs and domain concepts
  - Identify potential operational risks and failure points
  ```

### Phase 2: Conflict Detection and Name Generation
- Use `LS` to examine existing commands in `.claude\commands\` directory
- Generate 3-5 potential command names following conventions:
  - Use kebab-case (lowercase with hyphens)
  - Be descriptive but concise (2-4 words maximum)
  - Reflect primary action/purpose
  - Avoid conflicts with existing commands
- Use `Grep` to search CLAUDE.md for related functionality to avoid duplication

### Phase 3: Research and Best Practices Discovery
- Use `WebSearch` to research relevant patterns and approaches:
  ```
  Search queries based on domain:
  - "[DOMAIN] automation best practices" 
  - "[TOOL_NAME] command line usage patterns"
  - "developer productivity [TASK] automation"
  - "[FRAMEWORK] CLI command examples"
  ```
- Use `WebFetch` to gather detailed information from promising sources
- Extract actionable patterns, tool recommendations, and proven approaches
- Adapt findings to Streamline Pro context and existing capabilities

### Phase 4: Tool Selection and Architecture Design
- Map requirements to appropriate allowed-tools:
  ```
  Analysis tasks → mcp__thinking__sequentialthinking
  File operations → Read, Write, Edit, LS, Glob, Grep
  Search operations → WebSearch, WebFetch  
  System operations → Bash (Note: Will use PowerShell commands within Bash tool)
  Planning tasks → ExitPlanMode, TodoWrite
  Memory tasks → mcp__memory__*
  Browser tasks → mcp__playwright__*
  ```
- **Tool Availability Verification**: Check if specialized tools (like browser automation) are actually needed and available
- Design command phases and instruction structure with Windows-specific considerations
- Plan integration with CLAUDE.md and existing commands

### Phase 5: Error Handling Consultation (Optional Enhancement)
- **Detect Explicit Error Handling**: Scan description for keywords:
  ```
  "validation", "error handling", "exception", "fail gracefully", 
  "robust", "production", "reliable", "handle failures"
  ```
- **Risk Assessment**: If not explicitly mentioned, analyze potential failure points:
  ```
  File operations → "File not found, permission errors, invalid paths"
  Network operations → "Connectivity issues, timeouts, API failures"
  User input → "Invalid formats, missing required data, malformed input"
  System commands → "Command failures, permission issues, dependency missing"
  External dependencies → "Service unavailable, authentication failures"
  ```
- **Interactive Consultation**: If risks identified and not explicitly addressed:
  ```
  Present to user:
  "Based on your command description, I identified these potential error scenarios:
  - [SCENARIO_1]: [DESCRIPTION AND IMPACT]
  - [SCENARIO_2]: [DESCRIPTION AND IMPACT]
  
  Would you like to include error handling for any of these scenarios?
  If yes, please specify what should happen when these errors occur:
  - Fail silently and continue?
  - Show error message and exit?
  - Provide alternative suggestions?
  - Attempt recovery actions?"
  ```
- **Gather Preferences**: Document user's choices for integration into command design

### Phase 6: Plan Presentation and Approval
- Use `ExitPlanMode` to present comprehensive implementation plan:
  ```markdown
  # Create Command: [SUGGESTED_NAME]

  ## Command Overview
  - **Name**: [suggested-name]
  - **Purpose**: [clear purpose statement]
  - **Scope**: [what it will and won't do]
  - **Platform**: Windows PowerShell
  - **Complexity**: [simple/moderate/complex]
  - **Error Handling**: [None/Basic/Comprehensive per user choice]

  ## Implementation Plan
  ### Required Tools
  - [tool1]: [specific justification]
  - [tool2]: [specific justification]

  ### Command Structure
  1. **Phase 1**: [phase name and detailed purpose]
  2. **Phase 2**: [phase name and detailed purpose]
  3. **Phase N**: [additional phases as needed]
  
  ### Error Handling Strategy (if requested)
  - **Validation Level**: [None/Input Only/Comprehensive]
  - **Error Response**: [User-specified behavior]
  - **Specific Scenarios**: [User-defined error handling]
    - [ERROR_TYPE]: [RESPONSE_STRATEGY]
    - [ERROR_TYPE]: [RESPONSE_STRATEGY]

  ### Integration Points
  - [how it complements existing commands]
  - [relationship to CLAUDE.md sections]
  - [workflow integration considerations]

  ## Research Findings
  - [relevant best practices discovered]
  - [applicable patterns or frameworks]
  - [recommended tools and approaches]

  ## User Preferences Applied
  - [error handling choices made]
  - [complexity level selected]
  - [specific customizations requested]

  ## File Creation
  - Location: `.claude\commands\[name].md`
  - Template: [command structure preview]
  ```

### Phase 7: Command Implementation (Post-Approval)
- Use `Write` to create the command file with dynamic structure based on user preferences:
  ```yaml
  ---
  allowed-tools: [RESEARCHED_TOOL_LIST]
  description: [CLEAR_CONCISE_DESCRIPTION] (Windows version)
  argument-hint: [CONTEXT_SPECIFIC_HINT]
  ---

  # [COMMAND_TITLE] (Windows)

  [PURPOSE_AND_CONTEXT_DESCRIPTION]

  **Platform**: Windows PowerShell compatible

  ## Instructions

  ### Phase 1: [DOMAIN_SPECIFIC_PHASE]
  - [STEP_BY_STEP_INSTRUCTIONS with PowerShell adaptations]
  - [TOOL_USAGE_WITH_JUSTIFICATION for Windows environment]

  ### Phase 2: [ADDITIONAL_PHASES]
  - [SEQUENTIAL_IMPLEMENTATION_STEPS with Windows-specific commands]

  ### Error Handling (if user requested)
  - [USER_SPECIFIED_VALIDATION_STEPS]
  - [ERROR_RECOVERY_PROCEDURES]
  - [FALLBACK_STRATEGIES]

  ## Notes
  - [CONTEXT_SPECIFIC_GUIDANCE for Windows environment]
  - [INTEGRATION_NOTES]
  - [TROUBLESHOOTING_HINTS - if error handling enabled]
  - **Windows Commands**: Uses PowerShell cmdlets and Windows file paths
  ```

## Windows-Specific Command Adaptations

### File Path Conventions
- Use Windows path separators: `.claude\commands\[name].md`
- Handle Windows drive letters and UNC paths
- Use PowerShell-friendly path formats

### PowerShell Command Patterns
- **Directory Creation**: `New-Item -ItemType Directory -Path "path" -Force`
- **File Operations**: PowerShell cmdlets for file manipulation
- **Directory Listing**: `Get-ChildItem` instead of `ls` or `tree`
- **Text Processing**: PowerShell string manipulation and filtering

### System Integration
- PowerShell execution policies and security considerations
- Windows-specific environment variables and paths
- Integration with Windows development tools

## Command Generation Principles

### Adaptive Complexity
- **Simple Commands**: Focus on core functionality, minimal error handling
- **Moderate Commands**: Include basic validation and clear error messages
- **Complex Commands**: Comprehensive error handling, recovery strategies, user guidance

### Windows-Optimized Design
- **PowerShell Native**: Use PowerShell cmdlets and patterns
- **Windows Integration**: Leverage Windows-specific capabilities
- **Cross-Tool Compatibility**: Ensure commands work with Windows development stack
- **Performance Optimization**: Use efficient PowerShell patterns

### Quality Standards (Applied Conditionally)
- **Purpose Clarity**: Single, well-defined responsibility (always)
- **Scope Appropriateness**: Neither too broad nor too narrow (always)
- **Tool Efficiency**: Minimal, appropriate tool selection for Windows (always)
- **Integration Harmony**: Complements existing ecosystem on Windows (always)
- **Error Resilience**: Based on user preference and risk assessment (optional)
- **User Experience**: Clear instructions and helpful guidance (scalable)

## Meta-Command Philosophy

This meta-command embodies intelligent automation with user agency - using systematic analysis and research-driven design while respecting user preferences for complexity and robustness. It creates commands that truly enhance developer productivity without imposing unnecessary overhead, optimized for Windows PowerShell environments.

**Note**: The meta-command demonstrates its own principles: systematic thinking, optional complexity, interactive design, and user-centered planning. This Windows version uses PowerShell commands and Windows file path conventions.