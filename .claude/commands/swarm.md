---
allowed-tools: Task(*), Read(*), Grep(*), Bash(*), TodoWrite(*), mcp__memory__read_graph, mcp__memory__search_nodes, mcp__memory__open_nodes, mcp__memory__create_entities, mcp__thinking__sequentialthinking
error-handling: comprehensive-framework, parallel-execution-handling, task-delegation-validation, result-aggregation-validation, memory-storage-handling, recovery-procedures
description: Parallel task execution command that coordinates multiple sub-agents to execute tasks concurrently using the /execute command, aggregating results for enhanced productivity.
---

# Swarm

## Context

This command provides parallel task execution capabilities by coordinating multiple sub-agents to execute tasks concurrently. It accepts arguments in the format `[{number}] [{task}]` where the number (1-10) defines how many parallel sub-agents to spawn (default 3), and the task describes the work to be executed. Each sub-agent executes the `/execute` command with the provided task, and the main agent aggregates all execution results to provide a comprehensive summary. This approach leverages Claude Code's Task tool architecture to implement enhanced productivity through parallel task execution and systematic result coordination.

**Execution Model:**

- **Current Task Mode**: No arguments - delegates current task to multiple agents
- **Task Distribution Mode**: Task argument - distributes specified task across multiple agents
- **Agent Coordination**: Each sub-agent uses `/execute` command for task execution
- **Result Aggregation**: Collects and synthesizes execution results from all agents

**Memory Integration**: All swarm execution results are stored in memory entities for persistent intelligence building and execution history tracking.

## Your Task

Execute the following steps to perform enhanced parallel task execution:

1. **Generate UUID**: Use `Bash` to run `uuidgen --time-v7` to generate a unique time-based UUID for this swarm session. Store this UUID for use throughout the process.
2. **Parse & Validate Arguments**: Systematically parse the command arguments in format `[{number}] [{task}]`.
   - **number**: Validate the number is between 2-10 (default to 3 if invalid or missing).
   - **task**: Extract the task description. If no task is provided assume the current task.
   - Handle file references using @ symbol notation or direct file paths.
   - Ask the user for clarification on any unclear items or missing information.
   - **IMPORTANT** Do not make assumptions about incomplete or ambiguous requirements.
3. **Prepare Sub-Agent Tasks**: Determine the task each sub-agent will execute:
   - If task provided: each agent executes `/execute {task}`
   - If no task provided: each agent executes `/execute` (current task mode)
4. **Execute Parallel Delegation**: Use `Task` tool to spawn the specified number of concurrent sub-agents. Each sub-agent executes the `/execute` command with the prepared task. Collect the direct execution results from each sub-agent and handle any failures gracefully.
   - **IMPORTAN!** Run multiple Task invocations in a SINGLE message.
5. **Aggregate Results**: Systematically analyze and aggregate the execution results from all sub-agents from the memory or context. Identify successful completions, execution patterns, common outcomes, and any errors or failures encountered across the swarm.
6. **Analyze Execution Outcomes**: Use `mcp__thinking__sequentialthinking` to compare the results from different sub-agents to identify:
   - Successful execution patterns and outcomes
   - Common errors or challenges encountered
   - Complementary results that enhance overall task completion
   - Efficiency patterns in parallel execution
7. **Store Swarm Results**: Create a memory entity named `agent-{uuid}` with entityType "swarm_agent". Set the entity description to the original task prompt/arguments. Store the complete swarm execution results as organized, clear, and detailed observations.
8. **Document Execution**: Use `TodoWrite` to update task tracking with swarm execution completion and maintain execution history for systematic development intelligence.
9. **Deliver Results**: Present the swarm execution results in clear, structured format starting with `uuid: {uuid}` as the first line. Include the number of agents used, execution summary, success/failure breakdown, key outcomes, and any recommendations for follow-up actions.
**Available Resource**: Memory content can be accessed throughout all steps using `mcp__memory__read_graph`, `mcp__memory__search_nodes`, and `mcp__memory__open_nodes` to enhance execution quality and context.

## Verification

- **Pre-Execution**: Verify UUID generation, argument parsing, and system tool availability
- **During Process**: Validate each operation and handle failures immediately
- **UUID Management**: Confirm UUID generation and format validation success
- **Argument Parsing**: Verify argument extraction and validation success
- **Context Loading**: Check memory server connectivity and file access success
- **Sub-Agent Coordination**: Validate sub-agent spawn success and execution monitoring
- **Result Collection**: Confirm execution results were collected from available sub-agents
- **Failure Handling**: Verify failed agents were handled gracefully with appropriate documentation
- **Result Aggregation**: Ensure aggregation identified patterns and key outcomes effectively
- **Memory Operations**: Validate memory entity creation and storage operations
- **Result Delivery**: Confirm results are presented with proper UUID format and content
- **Error Handling**: Verify error handling provided clear feedback and recovery options

## Output

The command should produce:

- **UUID identifier**: First line of output must be `uuid: {uuid}` using the generated time-based UUID
- **Agent count**: Clear statement of how many agents were used for execution (as specifically requested)
- **Execution summary**: Comprehensive overview of all sub-agent execution results
- **Success/failure breakdown**: Clear accounting of which agents completed successfully and which encountered issues
- **Key outcomes**: Consolidated results showing what was accomplished across all agents
- **Collaboration analysis**: Insights into how parallel execution enhanced overall task completion
- **Memory entity**: Created with name `agent-{uuid}`, entityType "swarm_agent", containing complete execution results as organized observations
