---
allowed-tools: Bash(*), Write(*), Edit(*), mcp__memory__*, mcp__thinking__*
error-handling: comprehensive-framework, file-operation-validation, directory-creation-handling, code-generation-validation, configuration-validation, recovery-procedures
description: Generate intelligent project structures and boilerplate code based on accumulated architectural patterns and best practices.
---

# Scaffold

## Context

This command provides intelligent project scaffolding by generating project structures, boilerplate code, and configuration files based on accumulated architectural patterns, best practices, and systematic scaffolding intelligence. It creates complete, working projects that can be immediately used for development.

## Your Task

Execute the following steps to scaffold a project systematically with comprehensive error handling:

1. **Initialize sequential thinking**: Use `mcp__thinking__sequentialthinking` to plan the scaffolding approach
   - **Error Handling**: Handle thinking process timeouts or reasoning failures
   - **Recovery**: Restart with simpler scope or use basic scaffolding approach

2. **Read memory context**: Use `mcp__memory__read_graph` to understand existing scaffolding patterns
   - **Error Handling**: Handle memory server connectivity issues and permission errors
   - **Validation**: Verify memory graph integrity before proceeding
   - **Recovery**: Use `/meditate` for memory repair if corruption detected

3. **Analyze requirements**: Determine project requirements and constraints
   - **Error Handling**: Handle ambiguous requirements or missing information
   - **Recovery**: Request user clarification with specific options

4. **Select patterns**: Choose appropriate scaffolding patterns and templates
   - **Error Handling**: Handle pattern selection failures or template unavailability
   - **Recovery**: Use fallback patterns or create custom templates

5. **Generate structure**: Create directory structure and file hierarchy using `Bash`
   - **Error Handling**: Handle file system permission errors and directory creation failures
   - **Validation**: Verify directory creation success before proceeding
   - **Recovery**: Use alternative directory structures or permissions

6. **Create configurations**: Generate build, development, and deployment configurations using `Write`
   - **Error Handling**: Handle file creation failures and permission issues
   - **Validation**: Verify configuration file validity after creation
   - **Recovery**: Use alternative configuration formats or locations

7. **Generate code**: Create boilerplate code and component templates using `Write` and `Edit`
   - **Error Handling**: Handle code generation failures and syntax errors
   - **Validation**: Verify code syntax and structure after generation
   - **Recovery**: Use alternative code templates or manual code creation

8. **Set up development environment**: Configure tools and workflows
   - **Error Handling**: Handle tool configuration failures and dependency issues
   - **Recovery**: Use alternative tools or simplified configurations

9. **Generate documentation**: Create comprehensive project documentation
   - **Error Handling**: Handle documentation generation failures
   - **Recovery**: Use fallback documentation templates or skip optional docs

10. **Update memory**: Store scaffolding patterns and decisions using `mcp__memory__*` tools
    - **Error Handling**: Handle memory update failures with retry mechanisms
    - **Recovery**: Use alternative memory storage methods if updates fail

11. **Validate and test**: Ensure generated project works correctly
    - **Error Handling**: Handle validation failures and test errors
    - **Recovery**: Provide specific correction guidance and retry

12. **Provide guidance**: Offer next steps and development recommendations
    - **Error Handling**: Handle guidance generation failures
    - **Recovery**: Use fallback guidance templates or basic recommendations

## Error Handling

### Tool Result Validation

- **File Operations**: Validate all `Write` and `Edit` operations for success and handle permission issues
- **System Operations**: Check all `Bash` commands for directory creation and tool setup failures
- **Memory Operations**: Verify all `mcp__memory__*` tool results for errors and corruption
- **Thinking Operations**: Handle sequential thinking timeouts or reasoning failures

### Standard Error Response Format

``` markdown
ERROR: [Category] - [Specific Issue]
Context: [Operation being attempted]
Cause: [Root cause if identifiable]
Recovery: [Specific recovery steps]
```

### Recovery Procedures

1. **File System Issues**:
   - Check file and directory permissions
   - Validate disk space availability
   - Use alternative file locations if needed

2. **Memory Operation Failures**:
   - Check memory server connectivity and permissions
   - Use `/meditate` command for memory corruption repair
   - Retry with alternative memory operations

3. **Code Generation Failures**:
   - Validate code syntax and structure
   - Use alternative code templates
   - Request user input for custom code sections

4. **Configuration Issues**:
   - Validate configuration file formats
   - Use fallback configuration templates
   - Handle tool-specific configuration problems

### Context Preservation

- Save scaffolding progress to memory before error recovery attempts
- Maintain project structure state through error conditions
- Document error context and recovery attempts for learning
- Preserve generated files during error resolution

## Verification

- **Pre-Scaffolding**: Verify project requirements analysis and pattern selection
- **During Generation**: Validate each file creation and directory structure operation
- **File System**: Check file permissions and disk space before operations
- **Memory Operations**: Verify memory server connectivity and validate all memory operations
- **Code Quality**: Validate generated code syntax and structure
- **Configuration Validity**: Ensure all configuration files are valid and functional
- **Development Environment**: Verify tool setup and workflow configuration
- **Documentation**: Check documentation completeness and accuracy
- **Memory Updates**: Validate scaffolding intelligence was updated in memory
- **Error Handling**: Confirm error handling provided clear feedback and recovery options

## Output

The command should produce:

- Complete, working project structure with optimal configurations
- Generated boilerplate code following established conventions and best practices
- Comprehensive documentation with development workflows and guidelines
- Updated scaffolding intelligence in project memory for future use
- Clear guidance on next steps and development recommendations
- **Error handling**: Clear error messages using standard format with specific recovery guidance
- **Recovery documentation**: Record of any errors encountered and recovery procedures applied
- **Context preservation**: Documentation of preserved scaffolding context during error conditions

### Error Response Examples

**File System Permission Error**:

``` markdown
ERROR: File Operation - Permission denied
Context: Attempting to create project directory structure
Cause: Insufficient permissions to create directories in target location
Recovery:
1. Check and modify directory permissions
2. Use alternative project location with write access
3. Run with elevated permissions if appropriate
```

**Code Generation Failure**:

``` markdown
ERROR: Code Generation - Template processing failed
Context: Generating boilerplate code from templates
Cause: Template syntax error or missing template variables
Recovery:
1. Use alternative code template
2. Generate code manually with user input
3. Skip optional code sections and continue
```

**Configuration Validation Error**:

``` markdown
ERROR: Configuration - Invalid configuration format
Context: Validating generated configuration files
Cause: Configuration file format does not match expected schema
Recovery:
1. Use fallback configuration template
2. Validate configuration syntax manually
3. Request user input for configuration values
```
