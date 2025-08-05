---
allowed-tools: mcp__thinking__sequentialthinking, WebSearch, WebFetch, Read, LS, Write, ExitPlanMode, Grep, Bash
description: Meta-command to create personal slash commands saved to local folder (not committed to repo) (Windows version)
argument-hint: "Detailed description of what the personal command should accomplish"
---

# Create Local Command Meta-Generator (Windows)

Intelligently generate personal slash commands that save to `.claude\commands\local\` folder (excluded from git). This meta-command uses the same systematic analysis, research, and interactive consultation as create-command, but creates personal commands for your local development environment that won't be committed to the repository.

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

### Phase 2: Local Conflict Detection and Name Generation
- Use `LS` to examine existing local commands in `.claude\commands\local\` directory
- **Note**: Skip checking `.claude\commands\` shared folder - slash command namespace handles conflicts
- Generate 3-5 potential command names following conventions:
  - Use kebab-case (lowercase with hyphens)
  - Be descriptive but concise (2-4 words maximum)
  - Reflect primary action/purpose
  - Avoid conflicts with existing local commands only
- Use `Grep` to search CLAUDE.md for related functionality awareness (not conflict prevention)

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
- Adapt findings to personal workflow and Streamline Pro context

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
- Consider personal workflow optimization opportunities

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
  # Create Local Command: [SUGGESTED_NAME]

  ## Command Overview
  - **Name**: [suggested-name]
  - **Purpose**: [clear purpose statement]
  - **Scope**: [what it will and won't do]
  - **Type**: Personal/Local command (not committed to repo)
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

  ### Personal Workflow Integration
  - [how it enhances personal development workflow]
  - [relationship to shared commands and CLAUDE.md]
  - [local environment specific considerations]

  ## Research Findings
  - [relevant best practices discovered]
  - [applicable patterns or frameworks]
  - [recommended tools and approaches]

  ## User Preferences Applied
  - [error handling choices made]
  - [complexity level selected]
  - [specific customizations requested]

  ## File Creation
  - Location: `.claude\commands\local\[name].md`
  - Directory: Will create local folder automatically if needed
  - Git Status: Excluded from repository (personal use only)
  ```

### Phase 7: Local Command Implementation (Post-Approval)
- **Ensure Local Directory Exists**: 
  - Use `LS` to check if `.claude\commands\local\` exists
  - If not found, use `Bash` to create directory with PowerShell: `powershell "New-Item -ItemType Directory -Path '.claude\commands\local' -Force"`
  - Handle silently as per user preference
- **Create Command File**: Use `Write` to create the command file with dynamic structure:
  ```yaml
  ---
  allowed-tools: [RESEARCHED_TOOL_LIST]
  description: [CLEAR_CONCISE_DESCRIPTION] (Windows version)
  argument-hint: [CONTEXT_SPECIFIC_HINT]
  ---

  # [COMMAND_TITLE] (Windows)

  [PURPOSE_AND_CONTEXT_DESCRIPTION]
  
  **Platform**: Windows PowerShell compatible
  **Note**: This is a personal command stored locally and not committed to the repository.

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
  - [PERSONAL_WORKFLOW_INTEGRATION_NOTES]
  - [TROUBLESHOOTING_HINTS - if error handling enabled]
  - **Windows Commands**: Uses PowerShell cmdlets and Windows file paths
  ```

## Personal Command Benefits

### Local Development Advantages
- **Experimentation**: Try command ideas without affecting team
- **Personal Workflow**: Create commands tailored to your specific Windows setup
- **Environment Specific**: Include Windows paths, registry settings, or PowerShell modules specific to your setup
- **Rapid Iteration**: Modify and test without git history concerns

### Namespace Separation
- **No Conflicts**: Slash command namespace handles shared vs local command conflicts automatically
- **Flexibility**: Can create local variants of shared commands with same names
- **Clean Repository**: Personal commands don't clutter shared command space
- **Team Independence**: Develop personal productivity tools without team coordination

### Windows-Specific Benefits
- **PowerShell Integration**: Leverage Windows PowerShell capabilities
- **Windows Tools**: Integrate with Windows-specific development tools
- **Registry Access**: Include Windows registry operations if needed
- **Windows Services**: Interact with Windows services and processes

### Command Generation Principles

#### Adaptive Complexity (Same as Original)
- **Simple Commands**: Focus on core functionality, minimal error handling
- **Moderate Commands**: Include basic validation and clear error messages
- **Complex Commands**: Comprehensive error handling, recovery strategies, user guidance

#### Personal-Focused Design
- **Individual Needs**: Optimize for personal workflow and Windows preferences
- **Environment Specific**: Can include Windows paths, personal credentials, custom PowerShell modules
- **Rapid Development**: Lower barrier to command creation for personal use
- **Experimentation Friendly**: Easy to create, test, and iterate on command ideas

## Quality Standards (Applied Conditionally)
- **Purpose Clarity**: Single, well-defined responsibility (always)
- **Personal Utility**: Enhances individual productivity and workflow (always)
- **Tool Efficiency**: Minimal, appropriate tool selection for Windows (always)
- **Local Integration**: Works well with personal Windows development environment (always)
- **Error Resilience**: Based on user preference and risk assessment (optional)
- **Documentation**: Clear instructions for personal reference (scalable)

## Meta-Command Philosophy

This local command generator embodies personal productivity enhancement - using the same systematic analysis and research-driven design as shared commands while optimizing for individual workflow needs and experimentation without repository impact, specifically tailored for Windows PowerShell environments.

**Note**: Local commands demonstrate the power of personalized automation - creating tools that perfectly fit your unique development patterns and preferences. This Windows version uses PowerShell commands and Windows file path conventions.