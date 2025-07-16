---
allowed-tools: mcp__memory__*, mcp__thinking__*
error-handling: comprehensive-framework, memory-server-validation, corruption-detection, optimization-validation, backup-management, rollback-procedures
description: Analyze and optimize memory using MCP server with double-pass validation for comprehensive memory intelligence enhancement.
---

# Meditate

## Context

This command provides comprehensive memory optimization and analysis by systematically examining the entire memory graph, validating entities and relationships, removing duplicates and stale items, and applying double-pass validation to ensure memory consistency and intelligence enhancement. It maintains optimal memory structure for enhanced retrieval and intelligence building.

## Your Task

Execute the following steps to optimize memory systematically with comprehensive error handling:

1. **Initialize sequential thinking**: Use `mcp__thinking__sequentialthinking` to plan the optimization approach
   - **Error Handling**: Handle thinking process timeouts or reasoning failures
   - **Recovery**: Restart with simpler scope or use basic optimization approach

2. **Retrieve memory graph**: Use `mcp__memory__read_graph` to get complete memory structure
   - **Error Handling**: Handle memory server connectivity issues and permission errors
   - **Validation**: Verify memory server accessibility before proceeding
   - **Recovery**: Check MCP server configuration and restart if needed

3. **Analyze memory structure**: Examine entities, relationships, and observations systematically
   - **Error Handling**: Handle memory corruption, inconsistent entities, and broken relationships
   - **Validation**: Verify memory graph integrity during analysis
   - **Recovery**: Document corruption patterns and prepare repair strategies

4. **Identify optimization targets**: Determine duplicates, stale items, and improvement opportunities
   - **Error Handling**: Handle analysis failures or ambiguous optimization opportunities
   - **Recovery**: Use conservative optimization approach or request user guidance

5. **Execute first-pass optimization**: Perform initial cleanup and optimization using `mcp__memory__*` tools
   - **Error Handling**: Handle memory operation failures, timeout issues, and permission errors
   - **Validation**: Verify each optimization operation success before proceeding
   - **Recovery**: Rollback failed operations and use alternative optimization methods

6. **Validate changes**: Ensure optimization maintains memory consistency and integrity
   - **Error Handling**: Handle validation failures and consistency issues
   - **Recovery**: Rollback problematic changes and use more conservative optimization

7. **Execute second-pass validation**: Perform comprehensive validation of optimized memory
   - **Error Handling**: Handle comprehensive validation failures and integrity issues
   - **Recovery**: Provide detailed validation reports and repair recommendations

8. **Document optimization results**: Record improvements and optimization patterns for future reference
   - **Error Handling**: Handle documentation generation failures
   - **Recovery**: Use fallback documentation templates or basic reporting

## Error Handling

### Tool Result Validation
- **Memory Operations**: Validate all `mcp__memory__*` tool results for errors, corruption, and timeout issues
- **Thinking Operations**: Handle sequential thinking timeouts or reasoning failures
- **Memory Server**: Check memory server connectivity and permissions before operations
- **Data Integrity**: Verify memory graph consistency throughout optimization process

### Standard Error Response Format
```
ERROR: [Category] - [Specific Issue]
Context: [Operation being attempted]
Cause: [Root cause if identifiable]
Recovery: [Specific recovery steps]
```

### Recovery Procedures
1. **Memory Server Issues**:
   - Check MCP server configuration and connectivity
   - Restart memory server if connectivity fails
   - Validate memory server permissions and access

2. **Memory Corruption Issues**:
   - Document corruption patterns and scope
   - Use conservative repair approaches
   - Backup uncorrupted memory sections before repair

3. **Optimization Failures**:
   - Rollback failed optimization operations
   - Use more conservative optimization strategies
   - Validate memory consistency after each operation

4. **Validation Failures**:
   - Provide detailed validation failure reports
   - Implement incremental validation approach
   - Document validation issues for future reference

### Context Preservation
- Backup memory state before optimization operations
- Save optimization progress to temporary storage during operations
- Maintain optimization context through error conditions
- Document error context and recovery attempts for learning

## Verification

- **Pre-Optimization**: Verify memory server connectivity and access permissions
- **During Optimization**: Validate each memory operation and handle failures immediately
- **Memory Server**: Check memory server stability and connectivity throughout process
- **Data Integrity**: Verify memory graph consistency after each optimization operation
- **Optimization Results**: Confirm duplicates and stale items were properly identified and removed
- **Consistency Validation**: Ensure memory structure optimization maintained consistency
- **Double-Pass Validation**: Confirm comprehensive validation confirmed memory integrity
- **Documentation**: Validate optimization results were properly documented
- **Error Handling**: Confirm error handling provided clear feedback and recovery options

## Output

The command should produce:

- Optimized memory graph with improved organization and accessibility
- Removed duplicates and stale information while preserving critical context
- Enhanced memory structure with validated entity relationships
- Comprehensive optimization report with improvements and patterns
- Maintained memory health with efficient organization for future use
- **Error handling**: Clear error messages using standard format with specific recovery guidance
- **Recovery documentation**: Record of any errors encountered and recovery procedures applied
- **Context preservation**: Documentation of preserved memory context during error conditions
- **Backup information**: Details of memory backups created during optimization process

### Error Response Examples

**Memory Server Connectivity Error**:
```
ERROR: Memory Server - Connection failed
Context: Attempting to retrieve memory graph for optimization
Cause: Memory server not accessible or MCP server configuration issues
Recovery:
1. Check MCP server configuration in .claude/settings.local.json
2. Restart memory server if configuration is correct
3. Validate memory server permissions and access
```

**Memory Corruption Detection**:
```
ERROR: Memory Corruption - Inconsistent entity relationships
Context: Analyzing memory structure for optimization opportunities
Cause: Memory graph contains broken relationships or corrupted entities
Recovery:
1. Document corruption patterns and scope
2. Use conservative repair approach for corrupted sections
3. Backup uncorrupted memory sections before repair
```

**Optimization Validation Failure**:
```
ERROR: Optimization Validation - Memory consistency check failed
Context: Validating memory optimization results
Cause: Optimization operations introduced inconsistencies
Recovery:
1. Rollback problematic optimization operations
2. Use more conservative optimization strategy
3. Validate memory consistency after each operation
```