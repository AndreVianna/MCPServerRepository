---
allowed-tools: Bash(*), Read(*), Grep(*), Glob(*), mcp__memory__*, mcp__thinking__*
error-handling: comprehensive-framework, file-access-validation, memory-entity-creation, conflict-resolution, context-capture-validation, recovery-procedures
description: Enhanced memory storage with automatic incremental updates and named memory entries for comprehensive task state preservation.
---

# Memorize

## Context

This command provides enhanced memory storage with automatic incremental updates and named memory entries. It captures work progress, creates task checkpoints, and manages memory entries with intelligent conflict resolution. The command has two modes: automatic (no arguments) captures all work since last call, and named mode (with arguments) creates special memory entries for task tracking.

## Your Task

Execute the following steps to memorize work systematically with comprehensive error handling:

1. **Initialize sequential thinking**: Use `mcp__thinking__sequentialthinking` to plan the memorization approach
   - **Error Handling**: Handle thinking process timeouts or reasoning failures
   - **Recovery**: Restart with simpler scope or use basic memorization approach

2. **Analyze recent work**: Use `Read` and `Grep` to understand work done since last memorize call
   - **Error Handling**: Handle file access failures, permission issues, and search failures
   - **Validation**: Verify file accessibility and search pattern validity
   - **Recovery**: Use alternative file access methods or simplified work analysis

3. **Read memory graph**: Use `mcp__memory__read_graph` to understand current memory state
   - **Error Handling**: Handle memory server connectivity issues and permission errors
   - **Validation**: Verify memory graph integrity and accessibility
   - **Recovery**: Use `/meditate` for memory repair if corruption detected

4. **Detect conflicts**: Check for existing entries with same name (named mode only)
   - **Error Handling**: Handle conflict detection failures and entity access issues
   - **Recovery**: Use alternative conflict detection methods or skip conflict resolution

5. **Resolve conflicts**: Rename existing entries based on content if conflicts exist
   - **Error Handling**: Handle conflict resolution failures and entity modification issues
   - **Validation**: Verify conflict resolution preserves entity integrity
   - **Recovery**: Use alternative conflict resolution strategies or request user guidance

6. **Capture current state**: Document current task state, decisions, and progress comprehensively
   - **Error Handling**: Handle state capture failures and context analysis issues
   - **Recovery**: Use simplified state capture or request user input for missing context

7. **Update memory**: Store information using appropriate `mcp__memory__*` tools
   - **Error Handling**: Handle memory entity creation failures and storage issues
   - **Validation**: Verify memory updates were applied successfully
   - **Recovery**: Use alternative memory storage methods or retry with simplified entities

8. **Validate updates**: Ensure memory consistency and accuracy after storage
   - **Error Handling**: Handle validation failures and consistency issues
   - **Recovery**: Provide detailed validation reports and repair recommendations

## Error Handling

### Tool Result Validation
- **File Operations**: Validate all `Read` and `Grep` operations for success and handle access issues
- **Memory Operations**: Verify all `mcp__memory__*` tool results for errors, corruption, and timeout issues
- **Thinking Operations**: Handle sequential thinking timeouts or reasoning failures
- **Context Analysis**: Verify context capture completeness and accuracy

### Standard Error Response Format
```
ERROR: [Category] - [Specific Issue]
Context: [Operation being attempted]
Cause: [Root cause if identifiable]
Recovery: [Specific recovery steps]
```

### Recovery Procedures
1. **File Access Issues**:
   - Check file permissions and accessibility
   - Use alternative file access methods
   - Request user clarification for missing files

2. **Memory Operation Failures**:
   - Check memory server connectivity and permissions
   - Use `/meditate` command for memory corruption repair
   - Retry with simplified memory entities

3. **Conflict Resolution Issues**:
   - Use alternative conflict detection methods
   - Request user guidance for complex conflicts
   - Implement conservative conflict resolution strategies

4. **Context Capture Failures**:
   - Use simplified context capture methods
   - Request user input for missing context
   - Focus on essential context elements only

### Context Preservation
- Save work analysis progress before memory operations
- Maintain memorization context through error conditions
- Document error context and recovery attempts for learning
- Preserve partial memorization results during error recovery

## Verification

- **Pre-Memorization**: Verify file accessibility and memory server connectivity
- **During Process**: Validate each operation and handle failures immediately
- **Work Analysis**: Confirm work analysis captured all significant changes comprehensively
- **Conflict Detection**: Verify naming conflicts were detected and resolved intelligently (named mode)
- **Context Capture**: Check that current task state is fully documented
- **Memory Operations**: Verify memory server connectivity and validate all memory operations
- **Memory Updates**: Ensure memory updates maintain consistency and relationships
- **Memory Structure**: Validate that memory structure remains organized and accessible
- **Error Handling**: Confirm error handling provided clear feedback and recovery options

## Output

The command should produce:

- Comprehensive capture of work progress since last memorize call
- Named memory entry with complete task state (named mode)
- Intelligently resolved naming conflicts with content-based renaming
- Updated memory with new observations and relationships
- Maintained memory health with efficient organization and structure
- **Error handling**: Clear error messages using standard format with specific recovery guidance
- **Recovery documentation**: Record of any errors encountered and recovery procedures applied
- **Context preservation**: Documentation of preserved memorization context during error conditions
- **Conflict resolution**: Details of any naming conflicts resolved during the process

### Error Response Examples

**File Access Error**:
```
ERROR: File Operation - File not accessible
Context: Attempting to analyze recent work for memorization
Cause: File permissions or path issues preventing access
Recovery:
1. Verify file paths and permissions
2. Use alternative file access methods
3. Continue with available files only
```

**Memory Entity Creation Failure**:
```
ERROR: Memory Operation - Entity creation failed
Context: Attempting to create memory entity for captured work
Cause: Memory server issues or entity format problems
Recovery:
1. Check memory server connectivity
2. Retry with simplified entity structure
3. Use alternative memory storage approach
```

**Conflict Resolution Error**:
```
ERROR: Conflict Resolution - Unable to resolve naming conflict
Context: Resolving naming conflicts for memory entities
Cause: Complex conflicts requiring user guidance
Recovery:
1. Request user input for conflict resolution
2. Use conservative conflict resolution strategy
3. Document conflict for future resolution
```