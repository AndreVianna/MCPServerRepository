---
allowed-tools: Bash(*), Read(*), LS(*), Glob(*), Grep(*), mcp__memory__*, mcp__thinking__*
error-handling: comprehensive-framework, file-reference-validation, analysis-scope-validation, memory-integration-handling, output-mode-validation, recovery-procedures
description: Flexible analysis command that analyzes specific tasks, files, or components based on arguments, operating in plan mode with interactive clarification.
---

# Analyze

## Context

This command provides flexible, argument-driven analysis capabilities that can analyze specific tasks, files, or components based on user input. It operates in plan mode (read-only, no system modifications) and uses interactive clarification to ensure accurate analysis without assumptions. The command can handle file references using @ symbol notation and leverages existing memory content to enhance analysis quality.

**Argument Handling:**

- **Empty arguments**: Analyzes current task, provides full output report
- **`id-only` only**: Analyzes current task, outputs only UUID line
- **Regular arguments**: Analyzes provided content, provides full output report  
- **`id-only {task}`**: Analyzes content after flag, outputs only UUID line

In all cases, complete analysis results are stored in memory with `agent-{uuid}` entity naming.

## Your Task

Execute the following steps to perform flexible, argument-driven analysis:

1. **Generate UUID**: Use `Bash` to run `uuidgen --time-v7` to generate a unique time-based UUID for this analysis session. Store this UUID for use throughout the analysis process.
2. **Parse & Identify**: Systematically parse command arguments and identify the analysis target and output mode:
   - **Empty arguments**: Analyze current task, full output mode
   - **`id-only` only**: Analyze current task, UUID-only output mode
   - **Arguments without `id-only`**: Analyze provided arguments, full output mode
   - **Arguments starting with `id-only`**: Analyze content after `id-only`, UUID-only output mode
   Handle file references using @ symbol notation or direct file paths.
3. **Execute Analysis**: Use `mcp__thinking__sequentialthinking` to execute the following tasks:
    3.1. **Validate Scope**: Review the analysis target and requirements. Ask the user for clarification on any unclear items or missing information. Do not make assumptions about incomplete or ambiguous requirements.
    3.2. **Execute Analysis**: Perform systematic analysis of the target using `Grep`, `Read`, `LS`, and `Glob` as needed. Leverage memory context and file content to provide comprehensive insights without making assumptions.
    3.3. **Synthesize Findings**: Identify patterns, connections, and key insights from the analysis. Connect findings to existing project knowledge from memory to provide deeper understanding.
    3.4. **Generate Recommendations**: Determine if implementation or specific actions are needed based on the analysis. Prepare actionable recommendations with clear explanations and rationale.
4. **Store Analysis Results**: Use `mcp__memory__create_entities` to create a memory entity named `agent-{uuid}` with entityType "analysis_agent". Set the entity description to the original task prompt/arguments. Store the complete analysis results as organized, clear, and detailed observations in the entity.
5. **Deliver Results**: Use `mcp__thinking__sequentialthinking` to provide analysis results based on the output mode determined in step 2:
   - **Full output mode**: Provide detailed analysis in clear, accessible format starting with `uuid: {uuid}` as the first line. Generate conditional todo list if implementation tasks are identified. Ensure all explanations are thorough and easy to understand.
   - **UUID-only output mode**: Output only the single line `uuid: {uuid}` using the generated UUID.
**Available Resource**: Memory content can be accessed throughout all steps using the memory mcp server to enhance analysis quality and context.


## Verification

- **Pre-Analysis**: Verify UUID generation, argument parsing, and system tool availability
- **During Process**: Validate each operation and handle failures immediately
- **UUID Management**: Confirm UUID generation and format validation success
- **Argument Parsing**: Verify correct identification of analysis target and output mode
- **Context Loading**: Check memory server connectivity and file access success
- **File Operations**: Validate all file references were successfully read and integrated
- **Analysis Execution**: Ensure analysis was performed without assumptions or system modifications
- **Analysis Quality**: Verify analysis findings are comprehensive and well-synthesized
- **Recommendations**: Check recommendations are actionable and clearly explained (full output mode)
- **Memory Storage**: Confirm memory entity `agent-{uuid}` was created with proper structure
- **Output Validation**: Validate output format matches selected mode

## Output

The command should produce output based on the argument mode:

**Full Output Mode** (empty args or regular args):

- **UUID identifier**: First line of output must be `uuid: {uuid}` using the generated time-based UUID
- Detailed analysis report with findings and insights relevant to the specified target
- Clear explanations of patterns, connections, and key insights identified
- Comprehensive synthesis of findings connected to existing project knowledge
- Actionable recommendations with clear explanations and rationale
- **Conditional todo list**: Generated only when implementation tasks are identified, providing specific steps for execution
- Interactive clarification requests when requirements are unclear or incomplete
- Memory-enhanced context that leverages existing project knowledge for deeper understanding
- **Error handling**: Clear error messages using standard format with specific recovery guidance
- **Recovery documentation**: Record of any errors encountered and recovery procedures applied

**UUID-Only Output Mode** (`id-only` flag):

- **Single line output**: Only `uuid: {uuid}` using the generated time-based UUID

**Universal Output** (all modes):

- **Memory entity**: Created with name `agent-{uuid}`, entityType "analysis_agent", containing complete analysis results as organized observations
- Plan mode compliant output (no system modifications, read-only analysis)
- **Context preservation**: Documentation of preserved analysis context during error conditions
