---
allowed-tools: Bash(*), Read(*), Write(*), Edit(*), mcp__memory__*, mcp__thinking__*
error-handling: comprehensive-framework, test-execution-validation, tdd-cycle-handling, memory-integration-handling, test-framework-detection, recovery-procedures
description: Systematic testing intelligence with TDD red-green-refactor cycles and pattern accumulation
---

# Test

## Context

This command implements systematic testing intelligence that builds comprehensive testing knowledge over time. It enforces TDD methodology while accumulating testing patterns, strategies, and domain-specific insights from each testing session. The command guides through disciplined TDD red-green-refactor cycles and transforms testing from ad-hoc activities into systematic, intelligence-building processes.

## Your Task

Execute the following steps to conduct systematic TDD testing:

1. **Initialize sequential thinking**: Use `mcp__thinking__sequentialthinking` to plan the testing approach
   - **Error Handling**: Handle thinking process timeouts, test planning failures, and approach validation issues
   - **Validation**: Verify testing scope and methodology are appropriate for the requirements
   - **Recovery**: Use simplified testing approach or incremental test planning

2. **Read memory context**: Use `mcp__memory__read_graph` to understand existing testing patterns and strategies
   - **Error Handling**: Handle memory server connectivity issues, graph corruption, and access failures
   - **Validation**: Verify memory graph integrity and testing pattern accessibility
   - **Recovery**: Use `/meditate` for memory repair or continue with available testing knowledge

3. **Analyze requirements**: Parse and understand testing requirements, identify testable behaviors and edge cases
   - **Error Handling**: Handle requirement parsing failures, ambiguous specifications, and edge case identification issues
   - **Validation**: Verify requirement completeness and testability
   - **Recovery**: Request clarification or use simplified requirement analysis

4. **Design test strategy**: Determine appropriate testing levels and select testing patterns from accumulated knowledge
   - **Error Handling**: Handle test strategy design failures, pattern selection issues, and framework detection problems
   - **Recovery**: Use basic testing patterns or manual test design

5. **Red phase**: Write failing tests that capture requirements using `Write` and `Edit`
   - **Error Handling**: Handle test creation failures, file writing errors, and test framework issues
   - **Validation**: Verify tests properly capture requirements and fail as expected
   - **Recovery**: Use alternative test creation methods or simplified test structures

6. **Green phase**: Write minimal code to make tests pass using `Write` and `Edit`
   - **Error Handling**: Handle code implementation failures, file editing errors, and compilation issues
   - **Validation**: Verify code makes tests pass without breaking existing functionality
   - **Recovery**: Use alternative implementation approaches or incremental code changes

7. **Refactor phase**: Improve code quality without changing behavior using `Edit`
   - **Error Handling**: Handle refactoring failures, behavior changes, and code quality issues
   - **Validation**: Verify refactoring maintains test passing status and improves code quality
   - **Recovery**: Revert changes or use safer refactoring techniques

8. **Run tests**: Execute tests using `Bash` to validate the TDD cycle
   - **Error Handling**: Handle test execution failures, environment issues, and result validation problems
   - **Validation**: Verify all tests pass and provide meaningful feedback
   - **Recovery**: Use alternative test execution methods or debug test failures

9. **Capture intelligence**: Identify and document reusable testing patterns
   - **Error Handling**: Handle pattern identification failures and documentation issues
   - **Recovery**: Use simplified pattern documentation or focus on critical patterns

10. **Update memory**: Store testing patterns and strategies using `mcp__memory__*` tools
    - **Error Handling**: Handle memory update failures, entity creation issues, and storage problems
    - **Recovery**: Use alternative memory storage methods or retry with simplified entities

## Error Handling

### Tool Result Validation
- **Test Execution**: Validate all `Bash` test execution commands for success and meaningful output
- **File Operations**: Check all `Read`, `Write`, and `Edit` operations for access and permission issues
- **Memory Operations**: Verify all `mcp__memory__*` tool results for errors, corruption, and timeout issues
- **Thinking Operations**: Handle sequential thinking timeouts or reasoning failures
- **Test Framework Detection**: Validate test framework availability and configuration
- **TDD Cycle Validation**: Ensure red-green-refactor cycle integrity

### Standard Error Response Format
```
ERROR: [Category] - [Specific Issue]
Context: [Operation being attempted]
Cause: [Root cause if identifiable]
Recovery: [Specific recovery steps]
```

### Recovery Procedures
1. **Test Execution Failures**:
   - Validate test framework installation and configuration
   - Use alternative test execution methods
   - Debug test failures and fix issues

2. **Memory Operation Failures**:
   - Check memory server connectivity and permissions
   - Use `/meditate` command for memory corruption repair
   - Continue with available testing intelligence

3. **TDD Cycle Issues**:
   - Verify red-green-refactor cycle integrity
   - Use simplified TDD approach
   - Focus on critical test coverage

4. **File Operation Problems**:
   - Validate file paths and write permissions
   - Use alternative file manipulation methods
   - Implement incremental file changes

### Context Preservation
- Save testing progress to memory before error recovery attempts
- Maintain TDD cycle context through error conditions
- Document error context and recovery attempts for learning
- Preserve partial test results during error recovery

## Verification

- **Pre-Testing**: Verify tool availability, memory connectivity, and test framework installation
- **During Process**: Validate each operation and handle failures immediately
- **Requirements Analysis**: Confirm requirements were properly analyzed and test strategy was designed
- **TDD Cycle**: Verify TDD red-green-refactor cycle was followed systematically
- **Test Quality**: Check that tests are well-structured, isolated, and provide clear failure messages
- **Code Quality**: Ensure code quality improvements were made without changing behavior
- **Memory Integration**: Validate that testing intelligence was updated in memory
- **Error Handling**: Verify error handling provided clear feedback and recovery options

## Output

The command should produce:

- Comprehensive test suite following TDD methodology with red-green-refactor cycles
- Well-structured tests with proper naming conventions and clear assertions
- Improved code quality through systematic refactoring while maintaining functionality
- Updated testing intelligence in project memory for future reference
- Documented testing patterns and strategies for knowledge accumulation
- **Error handling**: Clear error messages using standard format with specific recovery guidance
- **Recovery documentation**: Record of any errors encountered and recovery procedures applied
- **Context preservation**: Documentation of preserved testing context during error conditions

### Error Response Examples

**Test Execution Error**:
```
ERROR: Test Execution - Test framework failure
Context: Running tests to validate TDD cycle
Cause: Test framework not installed or misconfigured
Recovery:
1. Verify test framework installation and configuration
2. Use alternative test execution methods
3. Debug test failures and fix issues
```

**TDD Cycle Validation Error**:
```
ERROR: TDD Cycle - Red-green-refactor integrity compromised
Context: Validating TDD cycle progression
Cause: Tests not failing properly in red phase or behavior changed during refactor
Recovery:
1. Verify red-green-refactor cycle integrity
2. Use simplified TDD approach
3. Focus on critical test coverage
```

**Memory Integration Failure**:
```
ERROR: Memory Operation - Testing intelligence update failed
Context: Storing testing patterns and strategies in memory
Cause: Memory server connectivity issues or storage corruption
Recovery:
1. Use `/meditate` to repair memory corruption
2. Continue with available testing intelligence
3. Store patterns using alternative methods
```
