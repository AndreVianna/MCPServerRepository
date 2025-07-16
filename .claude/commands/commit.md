---
allowed-tools: Bash(git *), mcp__memory__*, mcp__thinking__*
error-handling: comprehensive-framework, git-operation-validation, memory-error-handling, commit-format-validation, recovery-procedures
description: Intelligent commit management that builds comprehensive commit intelligence through systematic change analysis and pattern recognition.
---

# Commit

## Context

This command provides intelligent commit management by analyzing changes, applying accumulated commit patterns, and generating conventional commits. It builds persistent knowledge about commit strategies, follows systematic change analysis, and maintains commit intelligence across sessions.

## Your Task

Execute the following steps to create an intelligent commit with comprehensive error handling:

1. **Initialize thinking**: Use `mcp__thinking__sequentialthinking` to plan the commit approach systematically
   - **Error Handling**: Handle thinking process timeouts or reasoning failures
   - **Recovery**: Restart with simpler scope or use basic commit approach

2. **Read commit memory**: Use `mcp__memory__read_graph` to understand existing commit knowledge and patterns
   - **Error Handling**: Handle memory server connectivity issues and permission errors
   - **Validation**: Verify memory graph integrity before proceeding
   - **Recovery**: Use `/meditate` for memory repair if corruption detected

3. **Analyze current changes**: Use `Bash` with `git status` and `git diff` to examine staged and unstaged changes
   - **Error Handling**: Handle git repository issues and command failures
   - **Validation**: Verify git repository state and working directory
   - **Recovery**: Check git repository integrity and provide repair guidance

4. **Review commit history**: Use `Bash` with `git log` to analyze recent commit patterns and conventions
   - **Error Handling**: Handle git log failures and repository access issues
   - **Recovery**: Use alternative git commands or skip history analysis

5. **Classify changes**: Categorize changes by type (feat, fix, docs, style, refactor, test, chore) and determine scope
   - **Error Handling**: Handle ambiguous changes or classification failures
   - **Recovery**: Request user clarification or use default classification

6. **Generate commit message**: Create conventional commit message following project patterns and standards
   - **Error Handling**: Handle message generation failures or format issues
   - **Recovery**: Use fallback message templates or request user input

7. **Validate quality**: Ensure commit message follows conventions and all relevant changes are staged
   - **Error Handling**: Handle validation failures and format issues
   - **Recovery**: Provide specific correction guidance and retry

8. **Execute commit**: Create commit with generated message using `Bash` git commands
   - **Error Handling**: Handle commit failures, permission issues, and pre-commit hook failures
   - **Recovery**: Retry with corrected message or alternative commit approach

9. **Update memory**: Store commit patterns and conventions using `mcp__memory__*` tools
   - **Error Handling**: Handle memory update failures with retry mechanisms
   - **Recovery**: Use alternative memory storage methods if updates fail

10. **Verify success**: Confirm commit was created successfully and update intelligence
    - **Error Handling**: Handle verification failures and commit status issues
    - **Recovery**: Validate commit existence and provide status information

## Error Handling

### Tool Result Validation
- **Git Operations**: Validate all git commands and handle repository state issues
- **Memory Operations**: Check all `mcp__memory__*` tool results for errors and corruption
- **Thinking Operations**: Handle sequential thinking timeouts or reasoning failures
- **System Operations**: Validate bash command execution and handle system-level failures

### Standard Error Response Format
```
ERROR: [Category] - [Specific Issue]
Context: [Operation being attempted]
Cause: [Root cause if identifiable]
Recovery: [Specific recovery steps]
```

### Recovery Procedures
1. **Git Repository Issues**:
   - Check git repository integrity and status
   - Validate working directory and staging area
   - Provide repository repair guidance if needed

2. **Memory Operation Failures**:
   - Check memory server connectivity and permissions
   - Use `/meditate` command for memory corruption repair
   - Retry with alternative memory operations

3. **Commit Message Failures**:
   - Validate conventional commit format
   - Provide specific format correction guidance
   - Offer fallback message templates

4. **System Operation Failures**:
   - Validate git command availability and permissions
   - Handle pre-commit hook failures gracefully
   - Provide alternative commit approaches

### Context Preservation
- Save commit analysis to memory before error recovery attempts
- Maintain commit patterns and conventions through error conditions
- Document error context and recovery attempts for learning
- Preserve staging area state during error resolution

## Verification

- **Pre-Commit**: Verify git repository state and tool availability
- **During Process**: Validate each git operation and handle failures immediately
- **Memory Operations**: Check memory server connectivity and validate all memory operations
- **Commit Message**: Verify conventional commit format and content quality
- **Staging Validation**: Ensure all relevant changes are staged and no sensitive information included
- **Commit Success**: Confirm commit was created successfully with proper message
- **Memory Updates**: Validate commit intelligence was updated in memory
- **Error Handling**: Confirm error handling provided clear feedback and recovery options

## Output

The command should produce:

- A conventional commit with proper type, scope, and description
- Updated commit intelligence in project memory
- Commit message that explains the why, not just the what
- Documented commit patterns for future reference
- Successful commit creation with verification
- **Error handling**: Clear error messages using standard format with specific recovery guidance
- **Recovery documentation**: Record of any errors encountered and recovery procedures applied
- **Context preservation**: Documentation of preserved commit context during error conditions

### Error Response Examples

**Git Repository Issues**:
```
ERROR: Git Operation - Repository state invalid
Context: Attempting to analyze current changes with git status
Cause: Git repository may be corrupted or in invalid state
Recovery:
1. Check git repository integrity with `git fsck`
2. Verify working directory is in valid git repository
3. Reset repository state if needed
```

**Memory Operation Failure**:
```
ERROR: Memory Operation - Commit patterns not accessible
Context: Attempting to load commit intelligence from memory
Cause: Memory server connectivity issues or entity corruption
Recovery:
1. Check memory server connectivity
2. Use `/meditate` to repair memory corruption
3. Continue with basic commit approach
```

**Commit Message Validation**:
```
ERROR: Commit Validation - Message format invalid
Context: Validating generated commit message against conventions
Cause: Message does not follow conventional commit format
Recovery:
1. Correct message format: type(scope): description
2. Ensure type is valid (feat, fix, docs, style, refactor, test, chore)
3. Validate scope and description follow project conventions
```