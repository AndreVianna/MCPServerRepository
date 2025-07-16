---
allowed-tools: Bash(*), Read(*), Grep(*), mcp__memory__*, mcp__thinking__*
error-handling: comprehensive-framework, problem-analysis-validation, investigation-handling, memory-integration-handling, solution-validation, recovery-procedures
description: Systematic problem-solving framework for debugging with persistent intelligence building
---

# Debug Command

## Context

This command provides systematic problem-solving for debugging issues, building persistent debugging intelligence, and preventing hasty decisions through methodical analysis. It accumulates debugging patterns, solutions, and insights from previous sessions while maintaining debugging history across sessions.

## Your Task

Execute the following steps to debug systematically:

1. **Initialize sequential thinking**: Use `mcp__thinking__sequentialthinking` to plan the debugging approach systematically
   - **Error Handling**: Handle thinking process timeouts, debugging planning failures, and approach validation issues
   - **Validation**: Verify debugging scope and methodology are appropriate for the problem
   - **Recovery**: Use simplified debugging approach or incremental problem analysis

2. **Read debugging memory**: Use `mcp__memory__read_graph` to access accumulated debugging patterns and solutions
   - **Error Handling**: Handle memory server connectivity issues, graph corruption, and access failures
   - **Validation**: Verify memory graph integrity and debugging pattern accessibility
   - **Recovery**: Use `/meditate` for memory repair or continue with available debugging knowledge

3. **Analyze current problem**: Use `Read` and `Grep` to examine error messages, logs, and context systematically
   - **Error Handling**: Handle file access failures, log parsing errors, and context analysis issues
   - **Validation**: Verify problem analysis completeness and accuracy
   - **Recovery**: Use alternative analysis methods or focus on accessible information

4. **Search pattern database**: Use `mcp__memory__search_nodes` to find similar issues and previous solutions
   - **Error Handling**: Handle memory search failures, pattern matching issues, and result validation problems
   - **Recovery**: Use alternative pattern search methods or manual pattern identification

5. **Create investigation plan**: Develop methodical debugging approach with tools and techniques
   - **Error Handling**: Handle investigation planning failures, tool availability issues, and methodology problems
   - **Recovery**: Use simplified investigation approach or basic debugging techniques

6. **Execute systematic investigation**: Use `Bash` to run debugging commands and gather evidence
   - **Error Handling**: Handle command execution failures, environment issues, and evidence collection problems
   - **Validation**: Verify evidence collection completeness and reliability
   - **Recovery**: Use alternative investigation methods or manual evidence gathering

7. **Document findings**: Record observations, hypotheses, and test results
   - **Error Handling**: Handle documentation failures, observation recording issues, and hypothesis validation problems
   - **Recovery**: Use simplified documentation methods or focus on critical findings

8. **Validate solution**: Ensure fixes address root causes through systematic verification
   - **Error Handling**: Handle solution validation failures, verification issues, and root cause analysis problems
   - **Validation**: Verify solution effectiveness and root cause resolution
   - **Recovery**: Use alternative validation methods or incremental solution testing

9. **Update debugging memory**: Add new patterns and solutions using `mcp__memory__*` tools
   - **Error Handling**: Handle memory update failures, entity creation issues, and storage problems
   - **Recovery**: Use alternative memory storage methods or retry with simplified entities

10. **Build future intelligence**: Strengthen debugging capabilities for future sessions
    - **Error Handling**: Handle intelligence building failures and capability enhancement issues
    - **Recovery**: Use simplified intelligence building or focus on critical patterns

## Error Handling

### Tool Result Validation

- **Problem Analysis**: Validate all `Read` and `Grep` operations for log access and analysis completeness
- **Investigation Commands**: Check all `Bash` debugging commands for execution success and meaningful output
- **Memory Operations**: Verify all `mcp__memory__*` tool results for errors, corruption, and timeout issues
- **Thinking Operations**: Handle sequential thinking timeouts or reasoning failures
- **Solution Validation**: Ensure solution testing and verification completeness
- **Evidence Collection**: Validate evidence gathering and documentation accuracy

### Standard Error Response Format

``` markdown
ERROR: [Category] - [Specific Issue]
Context: [Operation being attempted]
Cause: [Root cause if identifiable]
Recovery: [Specific recovery steps]
```

### Recovery Procedures

1. **Problem Analysis Failures**:
   - Validate file paths and log accessibility
   - Use alternative analysis methods
   - Focus on accessible information sources

2. **Memory Operation Failures**:
   - Check memory server connectivity and permissions
   - Use `/meditate` command for memory corruption repair
   - Continue with available debugging intelligence

3. **Investigation Command Issues**:
   - Verify debugging tool availability and permissions
   - Use alternative investigation methods
   - Implement manual evidence gathering

4. **Solution Validation Problems**:
   - Use alternative validation methods
   - Implement incremental solution testing
   - Focus on critical solution components

### Context Preservation

- Save debugging progress to memory before error recovery attempts
- Maintain investigation context through error conditions
- Document error context and recovery attempts for learning
- Preserve partial debugging results during error recovery

## Verification

- **Pre-Debugging**: Verify tool availability, memory connectivity, and system access
- **During Process**: Validate each operation and handle failures immediately
- **Problem Analysis**: Confirm problem analysis was thorough and systematic
- **Solution Validation**: Verify solution addresses root cause, not just symptoms
- **Pattern Documentation**: Check that debugging patterns were properly documented
- **Evidence Chain**: Ensure evidence chain is complete and logical
- **Memory Integration**: Validate debugging intelligence was updated in memory
- **Error Handling**: Verify error handling provided clear feedback and recovery options

## Output

The command should produce:

- A systematic problem analysis with evidence and reasoning
- A validated solution that addresses the root cause
- Updated debugging intelligence in project memory
- Documented debugging patterns for future reference
- A complete evidence chain of investigation steps and findings
- **Error handling**: Clear error messages using standard format with specific recovery guidance
- **Recovery documentation**: Record of any errors encountered and recovery procedures applied
- **Context preservation**: Documentation of preserved debugging context during error conditions

### Error Response Examples

**Problem Analysis Error**:

``` markdown
ERROR: Problem Analysis - Log access failed
Context: Analyzing error messages, logs, and context systematically
Cause: Log files not accessible or corrupted
Recovery:
1. Validate file paths and log accessibility
2. Use alternative analysis methods
3. Focus on accessible information sources
```

**Investigation Command Failure**:

``` markdown
ERROR: Investigation - Debugging command execution failed
Context: Running debugging commands to gather evidence
Cause: Debugging tool not available or permission issues
Recovery:
1. Verify debugging tool availability and permissions
2. Use alternative investigation methods
3. Implement manual evidence gathering
```

**Memory Integration Failure**:

``` markdown
ERROR: Memory Operation - Debugging intelligence update failed
Context: Storing debugging patterns and solutions in memory
Cause: Memory server connectivity issues or storage corruption
Recovery:
1. Use `/meditate` to repair memory corruption
2. Continue with available debugging intelligence
3. Store patterns using alternative methods
```
