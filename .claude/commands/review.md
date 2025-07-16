---
allowed-tools: Read(*), Grep(*), mcp__memory__*, mcp__thinking__*
error-handling: comprehensive-framework, code-analysis-validation, quality-assessment-handling, memory-integration-handling, recommendation-generation-validation, recovery-procedures
description: Conduct systematic code review using accumulated patterns and standards to generate comprehensive improvement recommendations and build review intelligence over time.
---

# Review

## Context

This command provides systematic code review intelligence by applying accumulated patterns and standards, generating detailed improvement recommendations, and building persistent knowledge about review patterns and quality standards over time. It evaluates code quality, security, performance, and maintainability using established patterns.

## Your Task

Execute the following steps to conduct systematic code review:

1. **Initialize sequential thinking**: Use `mcp__thinking__sequentialthinking` to plan the review approach
   - **Error Handling**: Handle thinking process timeouts, review planning failures, and approach validation issues
   - **Validation**: Verify review scope and methodology are appropriate for the codebase
   - **Recovery**: Use simplified review approach or incremental review steps

2. **Read memory context**: Use `mcp__memory__read_graph` to understand existing review patterns and standards
   - **Error Handling**: Handle memory server connectivity issues, graph corruption, and access failures
   - **Validation**: Verify memory graph integrity and review pattern accessibility
   - **Recovery**: Use `/meditate` for memory repair or continue with available review knowledge

3. **Review scope analysis**: Use `Bash` and `LS` to identify files and changes for review
   - **Error Handling**: Handle file system access failures, permission errors, and scope identification issues
   - **Validation**: Verify review scope is appropriate and file access is available
   - **Recovery**: Use alternative file discovery methods or adjust review scope

4. **Code analysis**: Use `Grep` and `Read` to analyze code quality, security, and performance
   - **Error Handling**: Handle code analysis failures, file access errors, and pattern matching issues
   - **Validation**: Verify code analysis completeness and accuracy
   - **Recovery**: Use alternative analysis methods or focus on accessible code sections

5. **Pattern recognition**: Apply accumulated review intelligence to identify issues and improvements
   - **Error Handling**: Handle pattern recognition failures, intelligence application issues, and analysis synthesis problems
   - **Recovery**: Use simplified pattern matching or manual review techniques

6. **Memory integration**: Update review memory with findings using `mcp__memory__*` tools
   - **Error Handling**: Handle memory update failures, entity creation issues, and storage problems
   - **Recovery**: Use alternative memory storage methods or retry with simplified entities

7. **Generate recommendations**: Provide actionable insights with detailed reasoning and implementation guidance
   - **Error Handling**: Handle recommendation generation failures, prioritization issues, and implementation guidance problems
   - **Recovery**: Use simplified recommendation formats or focus on critical issues

## Error Handling

### Tool Result Validation

- **Code Analysis**: Validate all `Read` and `Grep` operations for file access and analysis completeness
- **Memory Operations**: Verify all `mcp__memory__*` tool results for errors, corruption, and timeout issues
- **Thinking Operations**: Handle sequential thinking timeouts or reasoning failures
- **Review Scope**: Validate review scope identification and file accessibility
- **Quality Assessment**: Check code quality analysis completeness and accuracy

### Standard Error Response Format

``` markdown
ERROR: [Category] - [Specific Issue]
Context: [Operation being attempted]
Cause: [Root cause if identifiable]
Recovery: [Specific recovery steps]
```

### Recovery Procedures

1. **Code Analysis Failures**:
   - Validate file paths and accessibility
   - Use alternative analysis methods
   - Focus on accessible code sections

2. **Memory Operation Failures**:
   - Check memory server connectivity and permissions
   - Use `/meditate` command for memory corruption repair
   - Continue with available review intelligence

3. **Quality Assessment Issues**:
   - Use simplified quality metrics
   - Focus on critical quality aspects
   - Apply manual review techniques

4. **Recommendation Generation Problems**:
   - Use simplified recommendation formats
   - Focus on high-priority issues
   - Provide basic implementation guidance

### Context Preservation

- Save review progress to memory before error recovery attempts
- Maintain review context through error conditions
- Document error context and recovery attempts for learning
- Preserve partial review results during error recovery

## Verification

- **Pre-Review**: Verify tool availability, memory connectivity, and file system access
- **During Process**: Validate each operation and handle failures immediately
- **Review Scope**: Confirm review scope was properly identified and prioritized
- **Code Analysis**: Verify code analysis was comprehensive and covered all quality aspects
- **Pattern Recognition**: Check that accumulated review intelligence was applied effectively
- **Memory Integration**: Confirm review intelligence was successfully updated in memory
- **Recommendations**: Ensure recommendations are actionable with clear implementation guidance
- **Error Handling**: Verify error handling provided clear feedback and recovery options

## Output

The command should produce:

- Comprehensive code review report with detailed findings and quality assessment
- Actionable improvement recommendations with implementation guidance and priorities
- Updated review intelligence in project memory for future reference
- Documented review patterns and quality standards for knowledge accumulation
- Quality metrics and improvement opportunities with clear next steps
- **Error handling**: Clear error messages using standard format with specific recovery guidance
- **Recovery documentation**: Record of any errors encountered and recovery procedures applied
- **Context preservation**: Documentation of preserved review context during error conditions

### Error Response Examples

**Code Analysis Error**:

``` markdown
ERROR: Code Analysis - File analysis failed
Context: Analyzing code quality, security, and performance patterns
Cause: File access permissions or corrupted file content
Recovery:
1. Verify file permissions and accessibility
2. Use alternative analysis methods
3. Focus on accessible code sections
```

**Quality Assessment Failure**:

``` markdown
ERROR: Quality Assessment - Pattern recognition failed
Context: Applying accumulated review intelligence to identify issues
Cause: Review pattern database corruption or analysis complexity
Recovery:
1. Use simplified quality metrics
2. Apply manual review techniques
3. Focus on critical quality aspects
```

**Memory Integration Failure**:

``` markdown
ERROR: Memory Operation - Review intelligence update failed
Context: Updating review memory with findings and patterns
Cause: Memory server connectivity issues or storage corruption
Recovery:
1. Use `/meditate` to repair memory corruption
2. Continue with available review intelligence
3. Store findings using alternative methods
```
