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
2. **Parse & Validate Arguments**: Systematically parse the command arguments in format `[{number}] [{task}]`.
   - **number**: Validate the number is between 2-10 (default to 3 if invalid or missing).
   - **task**: Extract the task description. If no task is provided assume the current task.
   - Handle file references using @ symbol notation or direct file paths.
   - Ask the user for clarification on any unclear items or missing information.
   - **IMPORTANT** Do not make assumptions about incomplete or ambiguous requirements.
3. **Prepare Sub-Agent Tasks**: Design identical task prompts for each sub-agent that will analyze using the `/analyze id-only` command. Ensure each sub-agent receives the same analysis target and context. The `id-only` flag ensures sub-agents return only UUIDs for memory retrieval.
4. **Delegate Parallel Analysis**: Use `Task` tool to spawn the specified number of concurrent sub-agents for analysis planning. Each sub-agent executes `/analyze id-only` with the prepared prompt. Collect the returned UUIDs from each sub-agent and handle any failures gracefully.
   - **IMPORTAN!** Run multiple Task invocations in a SINGLE message.
5. **Retrieve Sub-Agent Results**: Systematically analyze and aggregate the analysis results from all sub-agents from the memory or context. Identify successful completions, execution patterns, common outcomes, and any errors or failures encountered across the agents.
6. **Analyze the Results**: Use `mcp__thinking__sequentialthinking` to perform the following analysis tasks:
    6.1. **Compare Analysis Results**: Use `mcp__thinking__sequentialthinking` to systematically compare the analysis results from all sub-agent memory entities. Identify strengths, weaknesses, unique insights, and areas of consensus or disagreement across the different analyses.
    6.2. **Synthesize Enhanced Analysis Plan**: Combine the best elements from each sub-agent analysis. Create a comprehensive analysis plan that leverages diverse perspectives, fills gaps identified in individual analyses, and provides superior insights than any single analysis without making system modifications.
7. **Store Plan Results**: Use `mcp__memory__create_entities` to create a memory entity named `agent-{uuid}` with entityType "plan_agent". Set the entity description to the original task prompt/arguments. Store the complete synthesis results as organized, clear, and detailed observations.
8. **Cleanup Sub-Agent Memory**: Use `mcp__memory__delete_entities` to remove the memory entities created by the sub-agents to maintain clean memory state.
9. **Deliver Results**: Use `mcp__thinking__sequentialthinking` to present the synthesized analysis plan in clear, structured format starting with `uuid: {uuid}` as the first line. Include a comparison summary showing how different perspectives contributed to the final result. Generate conditional todo list only when implementation tasks are identified, providing specific steps for planning.
**Available Resource**: Memory content can be accessed throughout all steps using `mcp__memory__read_graph`, `mcp__memory__search_nodes`, and `mcp__memory__open_nodes` to enhance analysis quality and context.

## Verification

- **Pre-Planning**: Verify UUID generation, argument parsing, and system tool availability
- **During Process**: Validate each operation and handle failures immediately
- **UUID Management**: Confirm UUID generation and format validation success
- **Argument Parsing**: Verify argument extraction and user clarification handling
- **Context Loading**: Check memory server connectivity and file access success
- **Sub-Agent Coordination**: Validate sub-agent spawn success and UUID collection
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

