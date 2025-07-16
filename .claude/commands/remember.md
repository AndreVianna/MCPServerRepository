---
allowed-tools: mcp__memory__*, mcp__thinking__*
error-handling: comprehensive-framework, memory-retrieval-validation, search-filtering-handling, entity-access-validation, content-organization-handling, recovery-procedures
description: Intelligent memory retrieval with filtering capabilities for accessing stored project knowledge and named memory entries.
---

# Remember

## Context

This command provides intelligent memory retrieval with comprehensive filtering and search functionality. It enables access to the complete memory graph or targeted retrieval of specific information, including named memory entries created with the memorize command. The command has three modes: complete memory (no arguments), filtered search (with search terms), and named entry retrieval (specific names).

## Your Task

Execute the following steps to retrieve memory systematically:

1. **Initialize sequential thinking**: Use `mcp__thinking__sequentialthinking` to plan the retrieval approach
   - **Error Handling**: Handle thinking process initialization failures and timeout issues
   - **Recovery**: Use simplified planning approach or direct memory access

2. **Analyze query**: Parse search terms and determine retrieval scope and strategy
   - **Error Handling**: Handle query parsing failures and ambiguous search terms
   - **Validation**: Verify query format and search parameters
   - **Recovery**: Request user clarification or use default retrieval strategy

3. **Access memory**: Use appropriate `mcp__memory__*` tools to retrieve relevant memory content
   - **Error Handling**: Handle memory server connectivity issues and access failures
   - **Validation**: Verify memory operations return valid results
   - **Recovery**: Use alternative memory access methods or partial retrieval

4. **Filter content**: Apply intelligent filtering based on search criteria and relevance
   - **Error Handling**: Handle filtering failures and relevance scoring issues
   - **Recovery**: Use basic filtering or return unfiltered results with user guidance

5. **Organize information**: Structure retrieved content logically and coherently
   - **Error Handling**: Handle content organization failures and structural issues
   - **Recovery**: Use simplified organization or present raw results with context

6. **Present results**: Display organized information in accessible, actionable format
   - **Error Handling**: Handle presentation formatting failures and display issues
   - **Recovery**: Use fallback formatting or plain text presentation

7. **Validate results**: Ensure information completeness and accuracy
   - **Error Handling**: Handle validation failures and accuracy checking issues
   - **Recovery**: Present results with accuracy disclaimers or request user verification

## Error Handling

### Tool Result Validation
- **Memory Operations**: Verify all `mcp__memory__*` tool results for errors, corruption, and timeout issues
- **Thinking Operations**: Handle sequential thinking timeouts or reasoning failures
- **Search Operations**: Validate search query processing and result accuracy
- **Content Filtering**: Check filtering operations for completeness and relevance
- **Information Organization**: Verify content structuring and presentation quality

### Standard Error Response Format
```
ERROR: [Category] - [Specific Issue]
Context: [Operation being attempted]
Cause: [Root cause if identifiable]
Recovery: [Specific recovery steps]
```

### Recovery Procedures
1. **Memory Retrieval Failures**:
   - Check memory server connectivity and permissions
   - Use alternative memory access methods
   - Continue with partial memory content
   - Use `/meditate` command for memory corruption repair

2. **Search Query Processing Issues**:
   - Validate search query format and parameters
   - Use simplified search approach
   - Request user clarification for ambiguous queries
   - Use default search strategy when parsing fails

3. **Content Filtering Problems**:
   - Use basic filtering methods
   - Return unfiltered results with user guidance
   - Apply conservative relevance scoring
   - Skip problematic filtering operations

4. **Information Organization Failures**:
   - Use simplified organization structure
   - Present raw results with context
   - Apply basic categorization methods
   - Request user preferences for organization

5. **Content Presentation Issues**:
   - Use fallback formatting options
   - Apply plain text presentation
   - Provide results in simplified format
   - Document presentation limitations

### Context Preservation
- Save retrieval progress before error recovery attempts
- Maintain search context and filtering criteria through error conditions
- Document error context and recovery attempts for learning
- Preserve partial retrieval results during error recovery

## Verification

- **Pre-Retrieval**: Verify memory server connectivity and query validation
- **During Process**: Validate each operation and handle failures immediately
- **Query Analysis**: Confirm query analysis correctly identified retrieval scope and strategy
- **Memory Access**: Check memory operations for successful content retrieval
- **Content Filtering**: Verify filtering maintained important contextual relationships
- **Information Organization**: Check that information is organized logically for easy navigation
- **Result Presentation**: Ensure results are presented in accessible, actionable format
- **Information Accuracy**: Validate that information completeness and accuracy are maintained
- **Error Handling**: Confirm error handling provided clear feedback and recovery options

## Output

The command should produce:

- **Complete memory graph**: Organized format with full context (no arguments)
- **Filtered memory content**: Relevant to search terms with maintained relationships (with arguments)
- **Specific named memory entries**: Full context with exact name matching
- **Structured presentation**: Clear information hierarchy with logical organization
- **Actionable insights**: Preserved contextual relationships and practical guidance
- **Error handling**: Clear error messages using standard format with specific recovery guidance
- **Recovery documentation**: Record of any errors encountered and recovery procedures applied
- **Context preservation**: Documentation of preserved retrieval context during error conditions

### Error Response Examples

**Memory Retrieval Failure**:
```
ERROR: Memory Operation - Memory server connectivity failed
Context: Accessing memory graph for content retrieval
Cause: Memory server unavailable or connection timeout
Recovery:
1. Check memory server connectivity and permissions
2. Use alternative memory access methods
3. Continue with available cached memory content
```

**Search Query Processing Error**:
```
ERROR: Query Processing - Search terms validation failed
Context: Parsing search parameters and determining retrieval scope
Cause: Query format invalid or ambiguous search terms
Recovery:
1. Request user clarification for ambiguous queries
2. Use simplified search approach
3. Apply default search strategy
```

**Content Organization Failure**:
```
ERROR: Content Organization - Information structuring failed
Context: Organizing retrieved content logically and coherently
Cause: Content complexity exceeded organization capabilities
Recovery:
1. Use simplified organization structure
2. Present raw results with context
3. Apply basic categorization methods
```