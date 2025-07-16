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
   - **Error Handling**: Handle UUID generation failures with alternative methods
   - **Validation**: Verify UUID format (36 characters with dashes) before proceeding
   - **Recovery**: Use manual UUID generation or accept user input if system generation fails

2. **Parse & Validate Arguments**: Systematically parse the command arguments in format `[{number}] {task}`. Validate the number is between 1-10 (default to 3 if invalid or missing). Extract the task description and handle file references using @ symbol notation.
   - **Error Handling**: Handle argument parsing failures and format validation issues
   - **Recovery**: Request user clarification for ambiguous arguments or provide format examples

3. **Load Context**: Use `mcp__memory__read_graph` and `mcp__memory__search_nodes` to retrieve relevant memory content related to the execution task. Use `Read` to load contents of any referenced files into the execution context.
   - **Error Handling**: Handle memory server connectivity issues, file access failures, and permission errors
   - **Validation**: Verify memory graph integrity and file accessibility
   - **Recovery**: Use `/meditate` for memory repair or alternative file access methods

4. **Prepare Sub-Agent Tasks**: Determine the task each sub-agent will execute:
   - If task provided: each agent executes `/execute {task}`
   - If no task provided: each agent executes `/execute` (current task mode)
   - **Error Handling**: Handle task preparation failures or context issues
   - **Recovery**: Use simplified task definitions or request user clarification

5. **Execute Parallel Delegation**: Use `Task` tool to spawn the specified number of concurrent sub-agents. Each sub-agent executes the `/execute` command with the prepared task. Run multiple Task invocations in a SINGLE message. Collect the direct execution results from each sub-agent and handle any failures gracefully.
   - **Error Handling**: Handle sub-agent spawn failures, timeout issues, and execution failures
   - **Validation**: Verify sub-agent execution success and result collection
   - **Recovery**: Continue with successful agents and document failed executions

6. **Aggregate Results**: Use `mcp__thinking__sequentialthinking` to systematically analyze and aggregate the execution results from all sub-agents. Identify successful completions, execution patterns, common outcomes, and any errors or failures encountered across the swarm.
   - **Error Handling**: Handle thinking process timeouts, analysis failures, and aggregation issues
   - **Recovery**: Use simplified analysis approach or partial result aggregation

7. **Analyze Execution Patterns**: Compare the results from different sub-agents to identify:
   - Successful execution patterns and outcomes
   - Common errors or challenges encountered
   - Complementary results that enhance overall task completion
   - Efficiency patterns in parallel execution
   - **Error Handling**: Handle pattern analysis failures and comparison issues
   - **Recovery**: Use simplified pattern analysis or focus on key outcomes

8. **Store Swarm Results**: Use `mcp__memory__create_entities` to create a memory entity named `agent-{uuid}` with entityType "swarm_agent". Set the entity description to the original task prompt/arguments. Store the complete swarm execution results as organized, clear, and detailed observations.
   - **Error Handling**: Handle memory entity creation failures and storage issues
   - **Recovery**: Use alternative memory storage methods or retry with simplified entities

9. **Document Execution**: Use `TodoWrite` to update task tracking with swarm execution completion and maintain execution history for systematic development intelligence.
   - **Error Handling**: Handle TodoWrite failures gracefully without affecting main results
   - **Recovery**: Continue operation even if todo updates fail

10. **Deliver Results**: Present the swarm execution results in clear, structured format starting with `uuid: {uuid}` as the first line. Include the number of agents used, execution summary, success/failure breakdown, key outcomes, and any recommendations for follow-up actions.
    - **Error Handling**: Handle result presentation failures and formatting issues
    - **Recovery**: Use fallback presentation formats or simplified result delivery

**Available Resource**: Memory content can be accessed throughout all steps using `mcp__memory__read_graph`, `mcp__memory__search_nodes`, and `mcp__memory__open_nodes` to enhance execution quality and context.

## Error Handling

### Tool Result Validation

- **UUID Generation**: Validate `uuidgen --time-v7` output format and handle generation failures
- **Task Coordination**: Verify sub-agent spawn success and execution result collection
- **Memory Operations**: Check all `mcp__memory__*` tool results for errors, corruption, and timeout issues
- **File Operations**: Validate all `Read` operations for file access and permission issues
- **Thinking Operations**: Handle sequential thinking timeouts or reasoning failures

### Standard Error Response Format

``` markdown
ERROR: [Category] - [Specific Issue]
Context: [Operation being attempted]
Cause: [Root cause if identifiable]
Recovery: [Specific recovery steps]
```

### Recovery Procedures

1. **Parallel Execution Failures**:
   - Continue with successful agents and document failures
   - Reduce number of agents if coordination fails
   - Use sequential execution as fallback approach

2. **Task Delegation Issues**:
   - Simplify task definitions for sub-agents
   - Use alternative task coordination methods
   - Request user clarification for complex tasks

3. **Result Aggregation Problems**:
   - Work with partial results from successful agents
   - Use simplified aggregation when complex analysis fails
   - Focus on key outcomes rather than comprehensive analysis

4. **Memory Operation Failures**:
   - Check memory server connectivity and permissions
   - Use alternative memory storage methods
   - Continue operation with reduced memory integration

### Context Preservation

- Save swarm execution progress before error recovery attempts
- Maintain sub-agent coordination state through error conditions
- Document error context and recovery attempts for learning
- Preserve partial execution results during error recovery

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
- **Error documentation**: Clear summary of any failures or issues encountered during execution
- **Recommendations**: Next steps or follow-up actions based on collective execution results
- **Efficiency metrics**: Analysis of parallel execution effectiveness and patterns
- **Memory entity**: Created with name `agent-{uuid}`, entityType "swarm_agent", containing complete execution results as organized observations
- **Systematic documentation**: Comprehensive swarm execution report maintaining cognitive infrastructure standards
- **Error handling**: Clear error messages using standard format with specific recovery guidance
- **Recovery documentation**: Record of any errors encountered and recovery procedures applied
- **Context preservation**: Documentation of preserved swarm context during error conditions

### Error Response Examples

**Sub-Agent Execution Failure**:

``` markdown
ERROR: Task Coordination - Sub-agent execution failed
Context: Attempting to execute task across 5 parallel sub-agents
Cause: 2 sub-agents failed due to resource limitations or timeout
Recovery:
1. Continue with 3 successful sub-agents
2. Document failed executions for analysis
3. Reduce agent count for future swarm operations
```

**Result Aggregation Error**:

``` markdown
ERROR: Result Aggregation - Analysis synthesis failed
Context: Aggregating execution results from multiple sub-agents
Cause: Complex result patterns exceeded analysis capabilities
Recovery:
1. Use simplified aggregation approach
2. Focus on key outcomes from successful agents
3. Provide individual agent summaries
```

**Memory Storage Failure**:

``` markdown
ERROR: Memory Operation - Swarm results storage failed
Context: Storing swarm execution results in memory entity
Cause: Memory server connectivity issues or entity size limitations
Recovery:
1. Use alternative memory storage methods
2. Store simplified swarm results
3. Continue with reduced memory integration
```