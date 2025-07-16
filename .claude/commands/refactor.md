---
allowed-tools: Bash(*), Read(*), Write(*), Edit(*), mcp__memory__*, mcp__thinking__*
error-handling: comprehensive-framework, file-operation-validation, code-analysis-validation, refactoring-validation, backup-management, rollback-procedures
description: Intelligently refactor code by applying accumulated patterns, systematic improvement practices, and risk-minimized transformation strategies.
---

# Refactor

## Context

This command provides intelligent code improvement through systematic refactoring that applies accumulated patterns, builds refactoring intelligence over time, and enforces safe transformation practices with comprehensive validation. It identifies code improvements, applies proven patterns, and maintains functionality throughout the refactoring process.

## Your Task

Execute the following steps to refactor code systematically with comprehensive error handling:

1. **Initialize sequential thinking**: Use `mcp__thinking__sequentialthinking` to plan the refactoring approach
   - **Error Handling**: Handle thinking process timeouts or reasoning failures
   - **Recovery**: Restart with simpler scope or use basic refactoring approach

2. **Load memory context**: Use `mcp__memory__read_graph` to understand refactoring history and patterns
   - **Error Handling**: Handle memory server connectivity issues and permission errors
   - **Validation**: Verify memory graph integrity before proceeding
   - **Recovery**: Use `/meditate` for memory repair if corruption detected

3. **Analyze target code**: Use `Read`, `Grep`, and `Glob` to analyze code structure and identify issues
   - **Error Handling**: Handle file access failures, permission issues, and format problems
   - **Validation**: Verify file accessibility and code readability
   - **Recovery**: Use alternative file access methods or request file location clarification

4. **Select refactoring strategy**: Choose appropriate patterns and techniques from accumulated knowledge
   - **Error Handling**: Handle strategy selection failures or pattern unavailability
   - **Recovery**: Use fallback refactoring patterns or request user guidance

5. **Plan execution**: Design systematic refactoring sequence with validation checkpoints
   - **Error Handling**: Handle execution planning failures or complexity issues
   - **Recovery**: Break down complex refactoring into smaller, manageable steps

6. **Execute refactoring**: Apply transformations using `Edit` and `MultiEdit` with continuous validation
   - **Error Handling**: Handle file modification failures, syntax errors, and transformation issues
   - **Validation**: Verify code syntax and functionality after each transformation
   - **Recovery**: Rollback failed transformations and use alternative approaches

7. **Validate outcomes**: Verify functionality, quality, and performance improvements
   - **Error Handling**: Handle validation failures and regression issues
   - **Recovery**: Provide specific correction guidance and rollback procedures

8. **Update memory**: Store refactoring results and lessons learned using `mcp__memory__*` tools
   - **Error Handling**: Handle memory update failures with retry mechanisms
   - **Recovery**: Use alternative memory storage methods if updates fail

9. **Document results**: Provide comprehensive report on refactoring outcomes and improvements
   - **Error Handling**: Handle documentation generation failures
   - **Recovery**: Use fallback documentation templates or basic reporting

## Error Handling

### Tool Result Validation

- **File Operations**: Validate all `Read`, `Edit`, and `MultiEdit` operations for success and handle permission issues
- **Search Operations**: Check all `Grep` and `Glob` operations for proper pattern matching and file access
- **Memory Operations**: Verify all `mcp__memory__*` tool results for errors and corruption
- **Thinking Operations**: Handle sequential thinking timeouts or reasoning failures

### Standard Error Response Format

``` markdown
ERROR: [Category] - [Specific Issue]
Context: [Operation being attempted]
Cause: [Root cause if identifiable]
Recovery: [Specific recovery steps]
```

### Recovery Procedures

1. **File Operation Issues**:
   - Check file permissions and accessibility
   - Create backup copies before refactoring
   - Use alternative file modification approaches

2. **Code Analysis Failures**:
   - Validate code syntax and structure
   - Use alternative analysis tools or methods
   - Request user clarification for ambiguous code

3. **Refactoring Validation Issues**:
   - Implement rollback procedures for failed transformations
   - Use incremental refactoring with validation checkpoints
   - Provide specific syntax error corrections

4. **Memory Operation Failures**:
   - Check memory server connectivity and permissions
   - Use `/meditate` command for memory corruption repair
   - Retry with alternative memory operations

### Context Preservation

- Create file backups before refactoring operations
- Save refactoring progress to memory before error recovery attempts
- Maintain code state and transformation history through error conditions
- Document error context and recovery attempts for learning

## Verification

- **Pre-Refactoring**: Verify code analysis and strategy selection before execution
- **During Refactoring**: Validate each transformation and handle failures immediately
- **File Operations**: Check file accessibility and permissions before modifications
- **Code Quality**: Validate code syntax and functionality after each transformation
- **Memory Operations**: Verify memory server connectivity and validate all memory operations
- **Functionality Preservation**: Ensure code functionality is maintained throughout refactoring
- **Quality Improvements**: Verify that refactoring achieved intended improvements
- **Memory Updates**: Validate refactoring intelligence was updated in memory
- **Error Handling**: Confirm error handling provided clear feedback and recovery options

## Output

The command should produce:

- Systematically refactored code with improved quality and maintainability
- Applied refactoring patterns with comprehensive validation
- Maintained code functionality throughout the transformation process
- Documented refactoring outcomes and lessons learned
- Updated refactoring intelligence in project memory for future use
- **Error handling**: Clear error messages using standard format with specific recovery guidance
- **Recovery documentation**: Record of any errors encountered and recovery procedures applied
- **Context preservation**: Documentation of preserved refactoring context during error conditions
- **Backup information**: Details of file backups created during refactoring process

### Error Response Examples

**File Modification Error**:

``` markdown
ERROR: File Operation - Permission denied
Context: Attempting to apply refactoring transformation to source file
Cause: Insufficient permissions to modify target file
Recovery:
1. Check and modify file permissions
2. Create backup copy in accessible location
3. Use alternative file modification approach
```

**Code Analysis Failure**:

``` markdown
ERROR: Code Analysis - Syntax parsing failed
Context: Analyzing code structure for refactoring opportunities
Cause: Invalid syntax or unsupported language features
Recovery:
1. Validate code syntax manually
2. Use alternative analysis approach
3. Request user clarification for ambiguous code sections
```

**Refactoring Validation Error**:

``` markdown
ERROR: Refactoring Validation - Functionality regression detected
Context: Validating refactored code functionality
Cause: Refactoring transformation introduced functional changes
Recovery:
1. Rollback to previous code state
2. Apply more conservative refactoring approach
3. Validate each transformation step individually
```
