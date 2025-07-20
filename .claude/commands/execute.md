---
allowed-tools: Bash(*), Read(*), Write(*), Edit(*), MultiEdit(*), LS(*), Glob(*), Grep(*), TodoWrite(*), WebSearch(*), WebFetch(*), mcp__memory__*, mcp__thinking__*, mcp__playwright__*, mcp__figma__*
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
2. **Parse Arguments**: Systematically analyze command arguments to determine execution mode:
   - No arguments: Execute current task mode
   - UUID format argument: Validate UUID and prepare for memory retrieval
   - Text argument: Prepare for direct task execution
4. **Determine Task**: Based on argument analysis:
   - **Current Task**: Use `TodoWrite` to identify in_progress or next pending task
   - **UUID Task**: Use `mcp__memory__open_nodes` to retrieve 'agent-{uuid}' entity and extract task definition
   - **Direct Task**: Use the provided argument as task definition
5. **Execute Task**: Use `mcp__thinking__sequentialthinking` to systematically execute the identified task:
   - Break down task into logical steps
   - Execute using appropriate tools (Bash, Read, Write, Edit, etc.)
   - Document progress and results
   - Handle errors gracefully with recovery options
6. **Update Memory**: For UUID-based tasks, use `mcp__memory__add_observations` to update the memory entity:
   - Add completion observation with timestamp
   - Mark task as 'completed' status
   - Include execution results and any relevant insights
7. **Document Completion**: Use `TodoWrite` to update task tracking and maintain execution history for systematic development intelligence.
8. **Provide Results**: Present execution summary including task details, completion status, memory updates applied, and any recommendations for next steps.

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
