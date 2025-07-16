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
   - **Error Handling**: Handle UUID generation failures with alternative methods
   - **Validation**: Verify UUID format (36 characters with dashes) before proceeding
   - **Recovery**: Use manual UUID generation or accept user input if system generation fails

2. **Parse & Identify**: Systematically parse command arguments and identify the analysis target and output mode:
   - **Empty arguments**: Analyze current task, full output mode
   - **`id-only` only**: Analyze current task, UUID-only output mode
   - **Arguments without `id-only`**: Analyze provided arguments, full output mode
   - **Arguments starting with `id-only`**: Analyze content after `id-only`, UUID-only output mode
   Handle file references using @ symbol notation or direct file paths.
   - **Error Handling**: Handle argument parsing failures and invalid format detection
   - **Recovery**: Request user clarification for ambiguous arguments or provide format examples

3. **Load Context**: Use `mcp__memory__read_graph` and `mcp__memory__search_nodes` to retrieve relevant memory content related to the analysis target. Use `Read` to load entire contents of any referenced files into the analysis context.
   - **Error Handling**: Handle memory server connectivity issues, file access failures, and permission errors
   - **Validation**: Verify memory graph integrity and file accessibility
   - **Recovery**: Use `/meditate` for memory repair or alternative file access methods

4. **Execute Analysis**: Use `mcp__thinking__sequentialthinking` to execute the following tasks:
    4.1. **Validate Scope**: Review the analysis target and requirements. Ask the user for clarification on any unclear items or missing information. Do not make assumptions about incomplete or ambiguous requirements.
    4.2. **Execute Analysis**: Perform systematic analysis of the target using `Grep`, `Read`, `LS`, and `Glob` as needed. Leverage memory context and file content to provide comprehensive insights without making assumptions.
    4.3. **Synthesize Findings**: Identify patterns, connections, and key insights from the analysis. Connect findings to existing project knowledge from memory to provide deeper understanding.
    4.4. **Generate Recommendations**: Determine if implementation or specific actions are needed based on the analysis. Prepare actionable recommendations with clear explanations and rationale.
   - **Error Handling**: Handle thinking process timeouts, analysis failures, and synthesis issues
   - **Recovery**: Use simplified analysis approach or incremental analysis steps

5. **Store Analysis Results**: Use `mcp__memory__create_entities` to create a memory entity named `agent-{uuid}` with entityType "analysis_agent". Set the entity description to the original task prompt/arguments. Store the complete analysis results as organized, clear, and detailed observations in the entity.
   - **Error Handling**: Handle memory entity creation failures and storage issues
   - **Recovery**: Use alternative memory storage methods or retry with simplified entities

6. **Deliver Results**: Use `mcp__thinking__sequentialthinking` to provide analysis results based on the output mode determined in step 2:
   - **Full output mode**: Provide detailed analysis in clear, accessible format starting with `uuid: {uuid}` as the first line. Generate conditional todo list if implementation tasks are identified. Ensure all explanations are thorough and easy to understand.
   - **UUID-only output mode**: Output only the single line `uuid: {uuid}` using the generated UUID.
   - **Error Handling**: Handle result presentation failures and formatting issues
   - **Recovery**: Use fallback presentation formats or simplified result delivery
**Available Resource**: Memory content can be accessed throughout all steps using the memory mcp server to enhance analysis quality and context.

## Error Handling

### Tool Result Validation
- **UUID Generation**: Validate `uuidgen --time-v7` output format and handle generation failures
- **File Operations**: Check all `Read`, `LS`, `Glob`, and `Grep` operations for access and permission issues
- **Memory Operations**: Verify all `mcp__memory__*` tool results for errors, corruption, and timeout issues
- **Thinking Operations**: Handle sequential thinking timeouts or reasoning failures
- **Analysis Scope**: Validate analysis target clarity and completeness

### Standard Error Response Format
```
ERROR: [Category] - [Specific Issue]
Context: [Operation being attempted]
Cause: [Root cause if identifiable]
Recovery: [Specific recovery steps]
```

### Recovery Procedures
1. **File Reference Errors**:
   - Validate file paths and accessibility
   - Use alternative file access methods
   - Request user clarification for missing files

2. **Memory Operation Failures**:
   - Check memory server connectivity and permissions
   - Use `/meditate` command for memory corruption repair
   - Continue with available memory content

3. **Analysis Scope Issues**:
   - Request user clarification for ambiguous requirements
   - Use simplified analysis approach
   - Focus on essential analysis elements

4. **Output Mode Handling**:
   - Validate output mode selection
   - Use fallback output formats when needed
   - Ensure UUID is available for all output modes

### Context Preservation
- Save analysis progress to memory before error recovery attempts
- Maintain analysis context through error conditions
- Document error context and recovery attempts for learning
- Preserve partial analysis results during error recovery

## Verification

- **Pre-Analysis**: Verify UUID generation, argument parsing, and system tool availability
- **During Process**: Validate each operation and handle failures immediately
- **UUID Management**: Confirm UUID generation and format validation success
- **Argument Parsing**: Verify correct identification of analysis target and output mode
- **Context Loading**: Check memory server connectivity and file access success
- **File Operations**: Validate all file references were successfully read and integrated
- **Analysis Execution**: Ensure analysis was performed without assumptions or system modifications
- **Memory Integration**: Confirm memory content was effectively leveraged for analysis context
- **Analysis Quality**: Verify analysis findings are comprehensive and well-synthesized
- **Recommendations**: Check recommendations are actionable and clearly explained (full output mode)
- **Memory Storage**: Confirm memory entity `agent-{uuid}` was created with proper structure
- **Output Validation**: Validate output format matches selected mode
- **Error Handling**: Verify error handling provided clear feedback and recovery options

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

### Error Response Examples

**File Reference Error**:
```
ERROR: File Operation - Referenced file not accessible
Context: Loading file content for analysis using @ symbol notation
Cause: File path invalid or insufficient permissions
Recovery:
1. Verify file path and permissions
2. Use alternative file access methods
3. Continue analysis with available files
```

**Analysis Scope Validation Error**:
```
ERROR: Analysis Scope - Requirements ambiguous
Context: Validating analysis target and scope
Cause: Analysis requirements unclear or incomplete
Recovery:
1. Request user clarification with specific questions
2. Use simplified analysis approach
3. Focus on essential analysis elements
```

**Memory Integration Failure**:
```
ERROR: Memory Operation - Context loading failed
Context: Loading memory content to enhance analysis context
Cause: Memory server connectivity issues or corruption
Recovery:
1. Use `/meditate` to repair memory corruption
2. Continue with available memory content
3. Perform analysis with reduced context
```