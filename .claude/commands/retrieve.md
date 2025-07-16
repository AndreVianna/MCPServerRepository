---
allowed-tools: mcp__memory__open_nodes, mcp__memory__delete_entities, mcp__thinking__sequentialthinking
error-handling: comprehensive-framework, uuid-validation, memory-entity-access, content-display-handling, deletion-management, recovery-procedures
description: Simple UUID-based memory retrieval command that retrieves and displays agent memory entities with optional deletion capability.
---

# Retrieve

## Context

This command provides simple UUID-based memory retrieval functionality for accessing stored agent analysis results. It retrieves memory entities named `agent-{uuid}` and displays their content in an organized, readable format. The command supports an optional `and-erase` flag that deletes the memory entity after retrieval when the flag matches exactly. This command is designed to work with memory entities created by `/analyze` and `/plan` commands, providing easy access to stored analysis results and synthesis outputs.

## Your Task

Execute the following steps to retrieve and display memory entity content:

1. **Parse Arguments**: Check if UUID argument is provided and validate format. Check if second argument equals exactly `and-erase` (case-sensitive, exact match only). Any other second argument is ignored.
   - **Error Handling**: Handle missing UUID arguments and invalid format detection
   - **Validation**: Verify UUID format (36 characters with dashes) and flag parsing
   - **Recovery**: Request user to provide valid UUID or clarify command usage

2. **Retrieve Memory Entity**: Use `mcp__memory__open_nodes` to retrieve the memory entity named `agent-{uuid}` using the provided UUID. Handle cases where the entity does not exist.
   - **Error Handling**: Handle memory server connectivity issues and missing entity errors
   - **Validation**: Verify entity retrieval success and content availability
   - **Recovery**: Use alternative memory search methods or provide helpful error messages

3. **Process Content**: Use `mcp__thinking__sequentialthinking` to analyze the retrieved memory entity structure and determine the optimal presentation format:
   - Analyze entity type (analysis_agent, plan_agent, etc.)
   - Understand the relationship between entity description and observations
   - Organize observations into logical sections or themes
   - Determine how to present content for maximum clarity and usefulness
   - **Error Handling**: Handle thinking process timeouts and content analysis failures
   - **Recovery**: Use simplified content processing or basic presentation format

4. **Display Content**: Present the memory entity content in a clear, organized format that includes:
   - Entity type and description context
   - Organized observations with logical grouping
   - Clear formatting for readability
   - Preserve important insights and relationships
   - **Error Handling**: Handle content formatting failures and display issues
   - **Recovery**: Use fallback formatting or plain text presentation

5. **Conditional Deletion**: If the `and-erase` flag was provided as the second argument (exact match), use `mcp__memory__delete_entities` to remove the memory entity after successful display. Confirm deletion to user.
   - **Error Handling**: Handle memory deletion failures and confirmation issues
   - **Recovery**: Document deletion failures and continue with retrieval operation

6. **Handle Errors**: Provide clear error messages for missing entities, invalid UUIDs, or other failures. Ensure graceful handling of all error conditions.
   - **Error Handling**: Implement comprehensive error handling for all failure scenarios
   - **Recovery**: Provide specific recovery guidance for each error type

## Error Handling

### Tool Result Validation

- **UUID Validation**: Validate UUID format (36 characters with dashes) and argument parsing
- **Memory Operations**: Verify all `mcp__memory__*` tool results for errors, corruption, and timeout issues
- **Thinking Operations**: Handle sequential thinking timeouts or reasoning failures
- **Content Processing**: Check content analysis and formatting operations for completeness
- **Deletion Operations**: Validate deletion operations and confirmation handling

### Standard Error Response Format

``` markdown
ERROR: [Category] - [Specific Issue]
Context: [Operation being attempted]
Cause: [Root cause if identifiable]
Recovery: [Specific recovery steps]
```

### Recovery Procedures

1. **UUID Validation Failures**:
   - Provide UUID format examples and requirements
   - Request user to provide valid UUID
   - Offer command usage clarification
   - Continue with error guidance

2. **Memory Entity Access Failures**:
   - Check memory server connectivity and permissions
   - Use alternative memory search methods
   - Provide helpful error messages for missing entities
   - Suggest checking UUID accuracy

3. **Content Processing Issues**:
   - Use simplified content processing approach
   - Apply basic presentation format
   - Continue with raw entity content display
   - Request user preferences for presentation

4. **Content Display Problems**:
   - Use fallback formatting options
   - Apply plain text presentation
   - Provide content in simplified format
   - Document display limitations

5. **Deletion Operation Failures**:
   - Document deletion failures clearly
   - Continue with retrieval operation
   - Provide alternative deletion methods
   - Confirm deletion status to user

### Context Preservation

- Save retrieval progress before error recovery attempts
- Maintain entity content and context through error conditions
- Document error context and recovery attempts for learning
- Preserve partial retrieval results during error recovery

## Verification

- **Pre-Retrieval**: Verify UUID format validation and argument parsing
- **During Process**: Validate each operation and handle failures immediately
- **UUID Validation**: Confirm UUID argument was provided and validated
- **Flag Detection**: Verify `and-erase` flag detection works with exact match only
- **Memory Access**: Check memory entity was successfully retrieved using the UUID
- **Content Processing**: Ensure content analysis and formatting provides clear, organized output
- **Deletion Management**: Validate conditional deletion only occurs with exact `and-erase` flag
- **Error Handling**: Confirm error handling works for missing entities and invalid UUIDs
- **Output Quality**: Verify output is readable and preserves important information relationships

## Output

The command should produce:

- **Retrieved Content**: Organized display of memory entity content including entity type, description, and structured observations
- **Clear Formatting**: Content presented in logical sections with appropriate headers and organization
- **Context Preservation**: Entity description and observation relationships maintained for understanding
- **Deletion Confirmation**: Clear confirmation message when entity is deleted via `and-erase` flag
- **Error Messages**: Helpful error messages for missing entities, invalid UUIDs, or other failures
- **Content Analysis**: Intelligently formatted output that makes stored information easily accessible and actionable
- **Error handling**: Clear error messages using standard format with specific recovery guidance
- **Recovery documentation**: Record of any errors encountered and recovery procedures applied
- **Context preservation**: Documentation of preserved retrieval context during error conditions

### Error Response Examples

**UUID Validation Error**:

``` markdown
ERROR: UUID Validation - Invalid UUID format
Context: Validating UUID argument for memory entity retrieval
Cause: UUID format does not match required 36-character pattern with dashes
Recovery:
1. Provide valid UUID in format: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
2. Check UUID accuracy from original command output
3. Use /help retrieve for command usage examples
```

**Memory Entity Access Error**:

``` markdown
ERROR: Memory Operation - Entity not found
Context: Retrieving memory entity agent-{uuid} from memory server
Cause: Memory entity does not exist or was previously deleted
Recovery:
1. Verify UUID accuracy from original command output
2. Check available memory entities with /remember command
3. Confirm entity was created successfully
```

**Content Processing Failure**:

``` markdown
ERROR: Content Processing - Analysis timeout
Context: Analyzing memory entity structure for optimal presentation
Cause: Content complexity exceeded processing limits
Recovery:
1. Use simplified content processing approach
2. Present raw entity content with basic formatting
3. Continue with essential information display
```
