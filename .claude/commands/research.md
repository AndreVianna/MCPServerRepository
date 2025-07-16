---
allowed-tools: WebSearch(*), WebFetch(*), Bash(*), Read(*), mcp__memory__*, mcp__thinking__*
error-handling: comprehensive-framework, information-gathering-validation, source-validation-handling, web-operation-handling, synthesis-processing, memory-integration-handling, recovery-procedures
description: Comprehensive research command with systematic methodology and accumulated knowledge
---

# Research

## Context

This command provides deep research intelligence through systematic methodology, accumulated knowledge patterns, and actionable insights. It conducts comprehensive research with rigorous investigation processes, builds knowledge bases, and integrates findings into project knowledge for future reference.

## Your Task

Execute the following steps to conduct systematic research:

1. **Initialize sequential thinking**: Use `mcp__thinking__sequentialthinking` to plan the research approach
   - **Error Handling**: Handle thinking process initialization failures and timeout issues
   - **Recovery**: Use simplified research planning or direct research execution

2. **Read research memory**: Use `mcp__memory__read_graph` to understand existing research patterns and accumulated knowledge
   - **Error Handling**: Handle memory server connectivity issues and graph corruption
   - **Validation**: Verify memory graph integrity and research history availability
   - **Recovery**: Use `/meditate` for memory repair or initialize with minimal research context

3. **Define research objectives**: Establish clear research scope, success criteria, and evaluation frameworks
   - **Error Handling**: Handle objective definition failures and scope clarification issues
   - **Recovery**: Use basic research objectives or request user clarification

4. **Gather information systematically**: Use `WebSearch` and `WebFetch` to collect information from multiple sources
   - **Error Handling**: Handle web search failures, connectivity issues, and content access problems
   - **Validation**: Verify web operation results and content quality
   - **Recovery**: Use alternative information sources or cached research content

5. **Analyze and validate**: Assess source credibility, validate findings, and identify patterns across sources
   - **Error Handling**: Handle validation failures and credibility assessment issues
   - **Recovery**: Use conservative validation approach or request user guidance

6. **Synthesize insights**: Generate comprehensive understanding and actionable recommendations
   - **Error Handling**: Handle synthesis failures and insight generation issues
   - **Recovery**: Use simplified synthesis or incremental insight development

7. **Update memory**: Store research findings and patterns using `mcp__memory__*` tools
   - **Error Handling**: Handle memory storage failures and research integration issues
   - **Recovery**: Use alternative memory storage or simplified research tracking

8. **Document results**: Provide comprehensive research report with insights and recommendations
   - **Error Handling**: Handle result documentation failures and report generation issues
   - **Recovery**: Use simplified reporting or basic research summary

## Error Handling

### Tool Result Validation
- **Web Operations**: Verify all `WebSearch` and `WebFetch` operations for connectivity and content access
- **Memory Operations**: Verify all `mcp__memory__*` tool results for errors, corruption, and timeout issues
- **Thinking Operations**: Handle sequential thinking timeouts or reasoning failures
- **File Operations**: Check all `Read` operations for file access and permission issues
- **Information Gathering**: Validate research data quality and source accessibility
- **Synthesis Processing**: Check insight generation and recommendation development

### Standard Error Response Format
```
ERROR: [Category] - [Specific Issue]
Context: [Operation being attempted]
Cause: [Root cause if identifiable]
Recovery: [Specific recovery steps]
```

### Recovery Procedures
1. **Web Operation Failures**:
   - Check network connectivity and web service availability
   - Use alternative information sources or cached content
   - Continue with available research data
   - Request user guidance for critical missing information

2. **Memory Operation Failures**:
   - Check memory server connectivity and permissions
   - Use `/meditate` command for memory corruption repair
   - Continue with session-local research tracking
   - Use simplified memory integration

3. **Information Gathering Issues**:
   - Use alternative search strategies and sources
   - Continue with available information
   - Apply conservative validation approach
   - Document information limitations

4. **Source Validation Problems**:
   - Use basic credibility assessment methods
   - Continue with available validated sources
   - Apply conservative validation standards
   - Request user guidance for source evaluation

5. **Synthesis Process Failures**:
   - Use simplified synthesis approach
   - Focus on key insights from available research
   - Provide incremental insight development
   - Continue with basic research summary

### Context Preservation
- Save research progress before error recovery attempts
- Maintain research objectives and methodology through error conditions
- Document error context and recovery attempts for learning
- Preserve partial research results during error recovery

## Verification

- **Pre-Research**: Verify web connectivity and system tool availability
- **During Process**: Validate each operation and handle failures immediately
- **Research Objectives**: Confirm research objectives were clearly defined and systematically pursued
- **Information Sources**: Verify information sources were credible and findings were validated
- **Analysis Quality**: Check that analysis identified patterns and generated actionable insights
- **Research Methodology**: Ensure research methodology was rigorous and systematic
- **Memory Integration**: Validate that research intelligence was updated in memory
- **Error Handling**: Confirm error handling provided clear feedback and recovery options

## Output

The command should produce:

- **Comprehensive research report**: Systematic investigation findings with validated insights
- **Actionable recommendations**: Implementation guidance with credible source backing
- **Updated research intelligence**: Project memory integration for future reference
- **Documented research patterns**: Methodologies for knowledge accumulation and reuse
- **Strategic insights**: Enhanced project understanding and decision-making capabilities
- **Error handling**: Clear error messages using standard format with specific recovery guidance
- **Recovery documentation**: Record of any errors encountered and recovery procedures applied
- **Context preservation**: Documentation of preserved research context during error conditions

### Error Response Examples

**Web Operation Failure**:
```
ERROR: Web Operation - Search connectivity failed
Context: Gathering information using WebSearch for research topic
Cause: Network connectivity issues or search service unavailable
Recovery:
1. Check network connectivity and retry search
2. Use alternative information sources or cached content
3. Continue with available research data
```

**Source Validation Error**:
```
ERROR: Source Validation - Credibility assessment failed
Context: Validating research findings and assessing source credibility
Cause: Source evaluation complexity or validation timeout
Recovery:
1. Use basic credibility assessment methods
2. Apply conservative validation standards
3. Request user guidance for source evaluation
```

**Research Synthesis Failure**:
```
ERROR: Research Synthesis - Insight generation timeout
Context: Generating comprehensive understanding and recommendations
Cause: Research complexity exceeded synthesis processing limits
Recovery:
1. Use simplified synthesis approach
2. Focus on key insights from available research
3. Provide incremental insight development
```