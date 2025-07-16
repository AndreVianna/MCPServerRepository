---
allowed-tools: Bash(lt:*), Read(*), mcp__memory__*, mcp__thinking__*
error-handling: comprehensive-framework, project-structure-validation, claude-md-processing, memory-optimization-handling, context-preparation-validation, recovery-procedures
description: Prepare the context for generic working with this project.
---

# Prime

## Context

This command prepares the essential context for working with the Claude Code Tools project. It loads project structure information, applies project instructions, and ensures the project memory is properly initialized and optimized. This is a foundational command that should be run at the start of each development session.

## Your Task

Execute the following steps in order to prepare the project context:

1. **Load project structure**: Run the `lt` bash command to get the latest structure of the project
   - **Error Handling**: Handle command execution failures and output parsing issues
   - **Validation**: Verify project structure output is complete and accessible
   - **Recovery**: Use alternative directory listing methods or request user confirmation

2. **Apply project instructions**: Read and apply all instructions defined in the CLAUDE.md file
   - **Error Handling**: Handle file access failures, parsing errors, and format issues
   - **Validation**: Verify CLAUDE.md exists and content is properly formatted
   - **Recovery**: Use alternative instruction sources or request user clarification

3. **Initialize memory context**: Read the full content of the project memory using `mcp__memory__read_graph`
   - **Error Handling**: Handle memory server connectivity issues and graph corruption
   - **Validation**: Verify memory graph integrity and completeness
   - **Recovery**: Use `/meditate` for memory repair or initialize with minimal context

4. **Analyze memory structure**: Use `mcp__thinking__sequentialthinking` to systematically analyze the memory graph
   - **Error Handling**: Handle thinking process timeouts and analysis failures
   - **Recovery**: Use simplified analysis approach or incremental processing

5. **Optimize memory**: Identify and remove duplications, redundancies, and stale items from memory
   - **Error Handling**: Handle memory deletion failures and optimization issues
   - **Recovery**: Use conservative optimization or skip problematic entities

6. **Update memory relations**: Carefully update relations to represent the interconnection of entities correctly
   - **Error Handling**: Handle relation creation/update failures and validation issues
   - **Recovery**: Use existing relations or request user guidance for complex relationships

7. **Save session state**: Store the current context preparation state in memory for continuity
   - **Error Handling**: Handle session state storage failures and entity creation issues
   - **Recovery**: Use alternative storage methods or simplified state tracking

## Error Handling

### Tool Result Validation
- **Bash Operations**: Validate `lt` command execution and output format
- **File Operations**: Check all `Read` operations for file access and permission issues
- **Memory Operations**: Verify all `mcp__memory__*` tool results for errors, corruption, and timeout issues
- **Thinking Operations**: Handle sequential thinking timeouts or reasoning failures
- **Project Structure**: Validate project structure completeness and accessibility

### Standard Error Response Format
```
ERROR: [Category] - [Specific Issue]
Context: [Operation being attempted]
Cause: [Root cause if identifiable]
Recovery: [Specific recovery steps]
```

### Recovery Procedures
1. **Project Structure Loading Failures**:
   - Use alternative directory listing methods (`ls`, `find`)
   - Request user confirmation of project structure
   - Continue with partial structure information

2. **CLAUDE.md Processing Failures**:
   - Check file existence and permissions
   - Use alternative instruction sources
   - Request user clarification for missing instructions

3. **Memory Operation Failures**:
   - Check memory server connectivity and permissions
   - Use `/meditate` command for memory corruption repair
   - Initialize with minimal context when memory is unavailable

4. **Memory Optimization Issues**:
   - Use conservative optimization approach
   - Skip problematic entities and continue
   - Document optimization failures for future reference

5. **Session State Storage Failures**:
   - Use alternative storage methods
   - Simplify state tracking approach
   - Continue without persistent state if necessary

### Context Preservation
- Save context preparation progress before error recovery attempts
- Maintain project structure and instruction context through error conditions
- Document error context and recovery attempts for learning
- Preserve partial optimization results during error recovery

## Verification

- **Pre-Context**: Verify system tool availability and project accessibility
- **During Process**: Validate each operation and handle failures immediately
- **Project Structure**: Confirm project structure is loaded and current
- **CLAUDE.md Processing**: Verify CLAUDE.md instructions are applied successfully
- **Memory Operations**: Check memory graph integrity and optimization success
- **Memory Relations**: Ensure all entities have proper relationships
- **Session State**: Validate session state is saved for future continuity
- **Error Handling**: Confirm error handling provided clear feedback and recovery options

## Output

The command should produce:

- **Clean project memory graph**: Optimized memory structure with removed duplications and redundancies
- **Loaded project context**: Current project structure with proper accessibility
- **Applied project instructions**: CLAUDE.md instructions ready for the session
- **Session state persistence**: Saved state for continuity across context limits and restarts
- **Error handling**: Clear error messages using standard format with specific recovery guidance
- **Recovery documentation**: Record of any errors encountered and recovery procedures applied
- **Context preservation**: Documentation of preserved context during error conditions

### Error Response Examples

**Project Structure Loading Error**:
```
ERROR: Project Structure - Command execution failed
Context: Loading project structure using lt command
Cause: Command not found or execution permission issues
Recovery:
1. Use alternative directory listing methods (ls, find)
2. Verify project directory accessibility
3. Request user confirmation of project structure
```

**CLAUDE.md Processing Error**:
```
ERROR: File Operation - Project instructions not accessible
Context: Reading and applying CLAUDE.md instructions
Cause: File missing or insufficient permissions
Recovery:
1. Check file existence and permissions
2. Use alternative instruction sources
3. Request user clarification for missing instructions
```

**Memory Optimization Failure**:
```
ERROR: Memory Operation - Optimization process failed
Context: Identifying and removing duplications from memory
Cause: Memory corruption or operation timeout
Recovery:
1. Use conservative optimization approach
2. Skip problematic entities and continue
3. Use /meditate command for memory repair
```