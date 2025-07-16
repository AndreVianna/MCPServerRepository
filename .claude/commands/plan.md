---
allowed-tools: Task(*), Read(*), Grep(*), Bash(*), mcp__memory__read_graph, mcp__memory__search_nodes, mcp__memory__open_nodes, mcp__memory__create_entities, mcp__memory__delete_entities, mcp__thinking__*
error-handling: comprehensive-framework, sub-agent-coordination, uuid-management, memory-synthesis, argument-validation, recovery-procedures
description: Meta-analysis planning command that provides enhanced analysis planning through parallel sub-agent processing, operating in plan mode with read-only analysis and systematic synthesis.
---

# Plan

## Context

This command provides meta-analysis planning capabilities that enhance analysis quality through parallel sub-agent processing and systematic synthesis. It operates in plan mode (read-only, no system modifications) and uses interactive clarification to ensure accurate analysis planning without assumptions. The command accepts arguments in the format `[{number}] [{task}]` where the number (2-10) defines how many parallel sub-agents to spawn, and the task describes the analysis to be planned. Each sub-agent executes the `/analyze` command with identical prompts, and the main agent compares and synthesizes the results to produce an enhanced analysis plan that combines the best elements from multiple perspectives. This approach leverages Claude Code's Task tool architecture to implement cognitive amplification through parallel analytical planning.

## Your Task

Analyze the following steps to perform enhanced meta-analysis planning through parallel processing:

1. **Generate UUID**: Use `Bash` to run `uuidgen --time-v7` to generate a unique time-based UUID for this plan session. Store this UUID for use throughout the process.
   - **Error Handling**: Handle UUID generation failures with alternative methods
   - **Validation**: Verify UUID format (36 characters with dashes) before proceeding
   - **Recovery**: Use manual UUID generation or accept user input if system generation fails

2. **Parse & Validate Arguments**: Systematically parse the command arguments in format `[{number}] [{task}]`. Validate the number is between 2-10 (default to 3 if invalid or missing). Extract the task description and handle file references using @ symbol notation. If no task is provided assume the current task. Ask the user for clarification on any unclear items or missing information. Do not make assumptions about incomplete or ambiguous requirements.
   - **Error Handling**: Handle argument parsing failures and format validation issues
   - **Recovery**: Request user clarification for ambiguous arguments or provide format examples

3. **Load Context**: Use `mcp__memory__read_graph` and `mcp__memory__search_nodes` to retrieve relevant memory content related to the analysis task. Use `Read` to load contents of any referenced files into the analysis context.
   - **Error Handling**: Handle memory server connectivity issues, file access failures, and permission errors
   - **Validation**: Verify memory graph integrity and file accessibility
   - **Recovery**: Use `/meditate` for memory repair or alternative file access methods

4. **Prepare Sub-Agent Tasks**: Design identical task prompts for each sub-agent that will analyze using the `/analyze id-only` command. Ensure each sub-agent receives the same analysis target and context. The `id-only` flag ensures sub-agents return only UUIDs for memory retrieval.
   - **Error Handling**: Handle task preparation failures or context issues
   - **Recovery**: Use simplified task prompts or request user clarification

5. **Delegate Parallel Analysis**: Use `Task` tool to spawn the specified number of concurrent sub-agents for analysis planning. Each sub-agent executes `/analyze id-only` with the prepared prompt. Collect the returned UUIDs from each sub-agent and handle any failures gracefully. Run multiple Task invocations in a SINGLE message.
   - **Error Handling**: Handle sub-agent spawn failures, timeout issues, and communication failures
   - **Validation**: Verify sub-agent execution success and UUID collection
   - **Recovery**: Reduce number of agents or use alternative coordination approaches

6. **Retrieve Sub-Agent Results**: Use `mcp__memory__open_nodes` to retrieve the memory entities named `agent-{uuid}` for each UUID returned by the sub-agents. Extract the complete analysis results from the memory entities.
   - **Error Handling**: Handle memory entity retrieval failures and missing entities
   - **Recovery**: Use alternative memory search methods or partial result synthesis

7. **Analyze the Results**: Use `mcp__thinking__sequentialthinking` to perform the following analysis tasks:
    7.1. **Compare Analysis Results**: Use `mcp__thinking__sequentialthinking` to systematically compare the analysis results from all sub-agent memory entities. Identify strengths, weaknesses, unique insights, and areas of consensus or disagreement across the different analyses.
    7.2. **Synthesize Enhanced Analysis Plan**: Combine the best elements from each sub-agent analysis. Create a comprehensive analysis plan that leverages diverse perspectives, fills gaps identified in individual analyses, and provides superior insights than any single analysis without making system modifications.
   - **Error Handling**: Handle thinking process timeouts, analysis failures, and synthesis issues
   - **Recovery**: Use simplified analysis approach or incremental synthesis

8. **Store Plan Results**: Use `mcp__memory__create_entities` to create a memory entity named `agent-{uuid}` with entityType "plan_agent". Set the entity description to the original task prompt/arguments. Store the complete synthesis results as organized, clear, and detailed observations.
   - **Error Handling**: Handle memory entity creation failures and storage issues
   - **Recovery**: Use alternative memory storage methods or retry with simplified entities

9. **Cleanup Sub-Agent Memory**: Use `mcp__memory__delete_entities` to remove the memory entities created by the sub-agents to maintain clean memory state.
   - **Error Handling**: Handle cleanup failures gracefully without affecting main results
   - **Recovery**: Document cleanup issues and continue with main operation

10. **Deliver Results**: Use `mcp__thinking__sequentialthinking` to present the synthesized analysis plan in clear, structured format starting with `uuid: {uuid}` as the first line. Include a comparison summary showing how different perspectives contributed to the final result. Generate conditional todo list only when implementation tasks are identified, providing specific steps for planning.
    - **Error Handling**: Handle result presentation failures and formatting issues
    - **Recovery**: Use fallback presentation formats or simplified result delivery

**Available Resource**: Memory content can be accessed throughout all steps using `mcp__memory__read_graph`, `mcp__memory__search_nodes`, and `mcp__memory__open_nodes` to enhance analysis quality and context.

## Error Handling

### Tool Result Validation

- **UUID Generation**: Validate `uuidgen --time-v7` output format and handle generation failures
- **Memory Operations**: Verify all `mcp__memory__*` tool results for errors, corruption, and timeout issues
- **File Operations**: Check all `Read` operations for file access and permission issues
- **Task Coordination**: Validate sub-agent spawn success and communication
- **Thinking Operations**: Handle sequential thinking timeouts or reasoning failures

### Standard Error Response Format

``` bash
ERROR: [Category] - [Specific Issue]
Context: [Operation being attempted]
Cause: [Root cause if identifiable]
Recovery: [Specific recovery steps]
```

### Recovery Procedures

1. **Sub-Agent Coordination Failures**:
   - Reduce number of sub-agents or use single-agent approach
   - Retry with different coordination strategies
   - Use alternative task delegation methods

2. **Memory Operation Failures**:
   - Check memory server connectivity and permissions
   - Use `/meditate` command for memory corruption repair
   - Retry with alternative memory operations

3. **UUID Management Issues**:
   - Use alternative UUID generation methods
   - Handle UUID collisions gracefully
   - Validate UUID format before use

4. **Synthesis Process Failures**:
   - Use incremental synthesis approach
   - Work with partial results if some sub-agents fail
   - Provide simplified synthesis when complex analysis fails

### Context Preservation

- Save planning progress to memory before error recovery attempts
- Maintain sub-agent coordination state through error conditions
- Document error context and recovery attempts for learning
- Preserve partial synthesis results during error recovery

## Verification

- **Pre-Planning**: Verify UUID generation, argument parsing, and system tool availability
- **During Process**: Validate each operation and handle failures immediately
- **UUID Management**: Confirm UUID generation and format validation success
- **Argument Parsing**: Verify argument extraction and user clarification handling
- **Context Loading**: Check memory server connectivity and file access success
- **Sub-Agent Coordination**: Validate sub-agent spawn success and UUID collection
- **Memory Operations**: Verify memory entity retrieval and storage operations
- **Synthesis Process**: Confirm analysis comparison and synthesis quality
- **Memory Management**: Validate memory entity creation and cleanup success
- **Result Delivery**: Ensure results are presented with proper UUID format
- **Error Handling**: Confirm error handling provided clear feedback and recovery options

## Output

The command should produce:

- **UUID identifier**: First line of output must be `uuid: {uuid}` using the generated time-based UUID
- **Enhanced synthesized analysis plan** combining best elements from multiple perspectives
- **Comparison summary** showing how different sub-agent analyses contributed to final result
- **Diverse insights integration** demonstrating cognitive amplification through parallel analytical planning
- **Consensus and disagreement analysis** highlighting areas of alignment and divergence
- **Quality improvement demonstration** showing how synthesis exceeds individual analyses
- **Conditional todo list** generated only when implementation tasks are identified, providing specific steps for planning
- **Interactive clarification requests** when requirements are unclear or incomplete
- **Methodology transparency** explaining how synthesis was performed and why certain elements were selected
- **Meta-analysis metrics** such as number of sub-agents used, analysis time, and quality indicators
- **Memory entity**: Created with name `agent-{uuid}`, entityType "plan_agent", containing complete synthesis results as organized observations
- **Clean memory state**: Sub-agent memory entities removed after synthesis completion
- **Plan mode compliant output**: (no system modifications, read-only analysis planning)
- **Error handling**: Clear error messages using standard format with specific recovery guidance
- **Recovery documentation**: Record of any errors encountered and recovery procedures applied
- **Context preservation**: Documentation of preserved planning context during error conditions

### Error Response Examples

**Sub-Agent Coordination Failure**:

``` markdown
ERROR: Task Coordination - Sub-agent spawn failed
Context: Attempting to spawn 5 sub-agents for parallel analysis
Cause: Task coordination system overload or resource limitations
Recovery:
1. Reduce number of sub-agents to 2-3
2. Retry with sequential execution approach
3. Use single-agent analysis if parallel execution fails
```

**Memory Entity Retrieval Error**:

``` markdown
ERROR: Memory Operation - Sub-agent results not found
Context: Retrieving analysis results from sub-agent memory entities
Cause: Memory entities may not have been created or were deleted
Recovery:
1. Search for alternative memory entities with similar content
2. Use partial results from available sub-agents
3. Retry analysis with remaining functional sub-agents
```

**Synthesis Process Failure**:

``` markdown
ERROR: Analysis Synthesis - Synthesis process timeout
Context: Combining sub-agent analysis results into comprehensive plan
Cause: Analysis complexity exceeded processing limits
Recovery:
1. Use incremental synthesis approach
2. Focus on key insights from available analyses
3. Provide simplified synthesis with essential elements
```
