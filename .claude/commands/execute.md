---
allowed-tools: Bash(*), Read(*), Write(*), Edit(*), MultiEdit(*), LS(*), Glob(*), Grep(*), TodoWrite(*), WebSearch(*), WebFetch(*), mcp__memory__search_nodes, mcp__memory__open_nodes, mcp__memory__add_observations, mcp__thinking__sequentialthinking
error-handling: comprehensive-framework, tool-result-validation, standard-error-format, recovery-procedures, context-preservation
description: Task execution command with memory integration and systematic approach for executing current tasks, UUID-based memory tasks, or arbitrary task definitions.
---

# Execute

## Context

This command provides flexible task execution capabilities with three distinct modes: current task execution, UUID-based memory retrieval, and arbitrary task execution. It integrates with the cognitive infrastructure framework by maintaining persistent task intelligence through memory integration and systematic execution tracking. The command follows the established 4-section format and supports the 'agent-{uuid}' memory pattern for consistent ecosystem integration.

**Execution Modes:**

- **Current Task Mode**: No arguments - executes current task from TodoWrite or session context
- **UUID Mode**: UUID argument - searches memory for 'agent-{uuid}' entry and executes stored task
- **Direct Task Mode**: Text argument - executes task defined in the argument

**Memory Integration**: When executing UUID-based tasks, the command updates memory with completion observations to maintain persistent task intelligence and execution history.

## Your Task

Execute the following steps to perform systematic task execution with comprehensive error handling:

1. **Generate UUID**: Use `Bash` to run `uuidgen --time-v7` to generate a unique time-based UUID for tracking this execution session.
   - **Error Handling**: If UUID generation fails, use alternative UUID generation methods or generate manually
   - **Validation**: Verify UUID format (36 characters with dashes) before proceeding
   - **Recovery**: If UUID is invalid, regenerate or accept manual UUID input

2. **Parse Arguments**: Systematically analyze command arguments to determine execution mode:
   - No arguments: Execute current task mode
   - UUID format argument: Validate UUID and prepare for memory retrieval
   - Text argument: Prepare for direct task execution
   - **Error Handling**: Request clarification for ambiguous arguments or invalid formats
   - **Validation**: Ensure argument format matches expected patterns
   - **Recovery**: Provide specific format requirements and examples

3. **Load Memory Context**: Use `mcp__memory__read_graph` and `mcp__memory__search_nodes` to retrieve relevant memory content related to the execution context.
   - **Error Handling**: Handle memory server connectivity issues and permission errors
   - **Validation**: Verify memory graph integrity before proceeding
   - **Recovery**: Use `/meditate` for memory repair if corruption detected

4. **Determine Task**: Based on argument analysis:
   - **Current Task**: Use `TodoWrite` to identify in_progress or next pending task
   - **UUID Task**: Use `mcp__memory__open_nodes` to retrieve 'agent-{uuid}' entity and extract task definition
   - **Direct Task**: Use the provided argument as task definition
   - **Error Handling**: Handle missing entities, corrupted memory, or undefined tasks
   - **Validation**: Ensure task definition is clear and actionable
   - **Recovery**: Search for alternative entities or request task clarification

5. **Execute Task**: Use `mcp__thinking__sequentialthinking` to systematically execute the identified task:
   - Break down task into logical steps
   - Execute using appropriate tools (Bash, Read, Write, Edit, etc.)
   - Document progress and results
   - Handle errors gracefully with recovery options
   - **Error Handling**: Validate each tool invocation and handle failures
   - **Recovery**: Use alternative tools or approaches when primary tools fail
   - **Context Preservation**: Save progress to memory before attempting error recovery

6. **Update Memory**: For UUID-based tasks, use `mcp__memory__add_observations` to update the memory entity:
   - Add completion observation with timestamp
   - Mark task as 'completed' status
   - Include execution results and any relevant insights
   - **Error Handling**: Handle memory update failures with retry mechanisms
   - **Validation**: Verify memory updates were applied successfully
   - **Recovery**: Use alternative memory storage methods if updates fail

7. **Document Completion**: Use `TodoWrite` to update task tracking and maintain execution history for systematic development intelligence.
   - **Error Handling**: Handle TodoWrite failures gracefully
   - **Recovery**: Continue operation even if todo updates fail

8. **Provide Results**: Present execution summary including task details, completion status, memory updates applied, and any recommendations for next steps.
   - **Error Handling**: Provide clear error summaries and recovery guidance
   - **Recovery**: Suggest specific next steps for any unresolved issues

## Error Handling

### Tool Result Validation

- **UUID Generation**: Validate `uuidgen --time-v7` output format and handle generation failures
- **Memory Operations**: Check all `mcp__memory__*` tool results for errors, timeouts, or corruption
- **File Operations**: Validate all file operations and handle permission or access issues
- **System Operations**: Check bash command exit codes and handle system-level failures
- **Thinking Operations**: Handle sequential thinking timeouts or reasoning failures

### Standard Error Response Format

``` markdown
ERROR: [Category] - [Specific Issue]
Context: [Operation being attempted]
Cause: [Root cause if identifiable]
Recovery: [Specific recovery steps]
```

### Recovery Procedures

1. **UUID Generation Failure**:
   - Retry with alternative UUID generation methods
   - Accept manual UUID input if system generation fails
   - Continue without UUID for non-critical operations

2. **Memory Operation Failures**:
   - Check memory server connectivity and permissions
   - Use `/meditate` command for memory corruption repair
   - Retry with smaller scope or alternative memory operations

3. **Task Execution Failures**:
   - Break complex tasks into smaller components
   - Use alternative tools when primary tools fail
   - Preserve progress in memory before attempting recovery

4. **System Operation Failures**:
   - Validate system prerequisites before operations
   - Use alternative commands or approaches
   - Handle permission and resource limitation issues

### Context Preservation

- Save execution progress to memory before error recovery attempts
- Maintain UUID tracking through error conditions
- Document error context and recovery attempts for learning
- Preserve task state for continuation after error resolution

## Verification

- **Pre-Execution**: Verify UUID generation, argument parsing, and system tool availability
- **During Execution**: Validate each tool invocation and handle failures immediately
- **Memory Operations**: Check memory server connectivity and validate all memory operations
- **Task Completion**: Verify task execution completed successfully or document failure reasons
- **Memory Updates**: Ensure memory updates were applied for UUID-based tasks
- **Error Handling**: Confirm error handling provided clear feedback and recovery options
- **Documentation**: Verify completion documentation includes error context and recovery actions

## Output

The command should produce:

- **UUID identifier**: Execution session UUID for tracking and reference
- **Task execution results**: Detailed results of the executed task with completion status
- **Memory updates**: Documentation of any memory updates applied during execution
- **Completion status**: Clear indication of task completion or failure with explanations
- **Systematic documentation**: Comprehensive execution report maintaining cognitive infrastructure standards
- **Error handling**: Clear error messages using standard format with specific recovery guidance
- **Recovery documentation**: Record of any errors encountered and recovery procedures applied
- **Context preservation**: Documentation of preserved context during error conditions
- **Next steps**: Recommendations for follow-up actions or related tasks when applicable

### Error Response Examples

**Memory Entity Not Found**:

``` markdown
ERROR: Memory Operation - Entity not found
Context: Attempting to retrieve agent-01234567-89ab-cdef-0123-456789abcdef
Cause: UUID may be incorrect or entity may have been deleted
Recovery: 
1. Verify UUID format and retry
2. Use `/remember` to search for similar entities
3. Recreate content if entity contained analysis results
```

**Tool Execution Failure**:

``` markdown
ERROR: Tool Failure - Sequential thinking timeout
Context: Executing complex task analysis
Cause: Analysis exceeded maximum thinking iterations
Recovery:
1. Break task into smaller components
2. Restart with simplified scope
3. Use alternative analysis approach
```

**System Operation Error**:

``` markdown
ERROR: System Operation - Command not found
Context: Attempting to execute system command
Cause: Required tool not installed or not in PATH
Recovery:
1. Check system requirements
2. Use alternative commands
3. Install missing tools if possible
```
