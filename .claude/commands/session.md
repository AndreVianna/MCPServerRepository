---
allowed-tools: mcp__memory__*, mcp__thinking__*, TodoWrite(*)
error-handling: comprehensive-framework, session-tracking-validation, goal-management-handling, learning-capture-validation, memory-integration-handling, recovery-procedures
description: Initialize and manage development sessions with persistent intelligence
---

# Session

## Context

This command is the foundation of cognitive infrastructure, transforming development sessions into persistent, intelligent learning experiences. It manages systematic session tracking, builds knowledge across sessions, and maintains continuity in development work. The command embodies systematic, persistent, intelligent, and continuous development practices.

## Your Task

Execute the following steps to manage a development session:

1. **Initialize session**: Use `mcp__thinking__sequentialthinking` to establish clear session goals and context
   - **Error Handling**: Handle thinking process initialization failures and timeout issues
   - **Recovery**: Use simplified session initialization or direct goal setting

2. **Load project memory**: Use `mcp__memory__read_graph` to access relevant knowledge from previous sessions
   - **Error Handling**: Handle memory server connectivity issues and graph corruption
   - **Validation**: Verify memory graph integrity and session history availability
   - **Recovery**: Use `/meditate` for memory repair or initialize with minimal context

3. **Set up session tracking**: Use `TodoWrite` to create a structured task list for the session
   - **Error Handling**: Handle todo list creation failures and task structuring issues
   - **Recovery**: Use alternative task tracking methods or simplified task management

4. **Track session activities**: Document all significant actions, decisions, and discoveries throughout the session
   - **Error Handling**: Handle activity tracking failures and documentation issues
   - **Recovery**: Use simplified tracking or manual documentation methods

5. **Capture learning**: Identify new insights, patterns, and knowledge gained during the session
   - **Error Handling**: Handle learning capture failures and insight processing issues
   - **Recovery**: Use basic learning documentation or user-guided insight capture

6. **Build intelligence**: Recognize recurring patterns and solutions for future reference
   - **Error Handling**: Handle pattern recognition failures and intelligence building issues
   - **Recovery**: Use simplified pattern tracking or manual intelligence documentation

7. **Update memory**: Store important findings in project memory using `mcp__memory__*` tools
   - **Error Handling**: Handle memory storage failures and entity creation issues
   - **Recovery**: Use alternative memory storage or simplified memory updates

8. **Link sessions**: Connect current session to related previous sessions for continuity
   - **Error Handling**: Handle session linking failures and continuity issues
   - **Recovery**: Use basic session tracking or manual continuity management

9. **Synthesize outcomes**: Consolidate achievements and extract reusable insights
   - **Error Handling**: Handle synthesis failures and outcome processing issues
   - **Recovery**: Use simplified outcome documentation or manual synthesis

10. **Plan continuity**: Establish foundation for future related sessions
    - **Error Handling**: Handle continuity planning failures and future session setup issues
    - **Recovery**: Use basic continuity documentation or simplified planning

## Error Handling

### Tool Result Validation

- **Memory Operations**: Verify all `mcp__memory__*` tool results for errors, corruption, and timeout issues
- **Thinking Operations**: Handle sequential thinking timeouts or reasoning failures
- **TodoWrite Operations**: Validate task list creation and management operations
- **Session Tracking**: Check session activity documentation and tracking integrity
- **Learning Capture**: Verify insight processing and knowledge extraction accuracy

### Standard Error Response Format

``` markdown
ERROR: [Category] - [Specific Issue]
Context: [Operation being attempted]
Cause: [Root cause if identifiable]
Recovery: [Specific recovery steps]
```

### Recovery Procedures

1. **Session Initialization Failures**:
   - Use simplified session initialization approach
   - Set basic session goals manually
   - Continue with minimal session context
   - Request user clarification for session objectives

2. **Memory Operation Failures**:
   - Check memory server connectivity and permissions
   - Use `/meditate` command for memory corruption repair
   - Initialize with minimal context when memory is unavailable
   - Continue with session-local tracking

3. **Task Tracking Issues**:
   - Use alternative task tracking methods
   - Implement simplified task management
   - Continue with manual task documentation
   - Use basic todo list functionality

4. **Learning Capture Problems**:
   - Use basic learning documentation methods
   - Implement user-guided insight capture
   - Continue with simplified learning tracking
   - Request user assistance for insight identification

5. **Session Continuity Failures**:
   - Use basic session tracking methods
   - Implement manual continuity management
   - Continue with simplified session linking
   - Document continuity issues for future reference

### Context Preservation

- Save session progress before error recovery attempts
- Maintain session goals and tracking context through error conditions
- Document error context and recovery attempts for learning
- Preserve partial session results during error recovery

## Verification

- **Pre-Session**: Verify system tool availability and session initialization capability
- **During Process**: Validate each operation and handle failures immediately
- **Session Goals**: Verify session goals are clearly defined and measurable
- **Decision Documentation**: Confirm all significant decisions are documented with rationale
- **Learning Capture**: Ensure learning insights are captured and stored in memory
- **Memory Integration**: Check that session outcomes are linked to project memory
- **Session Continuity**: Validate that continuity is established for future sessions
- **Error Handling**: Confirm error handling provided clear feedback and recovery options

## Output

The command should produce:

- **Structured session**: Clear goals with systematic tracking and organized workflow
- **Documented decisions**: Rationale and alternatives considered with comprehensive context
- **Captured learning insights**: Recognized patterns and knowledge extraction with validation
- **Updated project memory**: Session findings integrated with existing knowledge
- **Established continuity**: Foundation for future sessions with proper linking
- **Comprehensive session summary**: Achievements and next steps with clear documentation
- **Error handling**: Clear error messages using standard format with specific recovery guidance
- **Recovery documentation**: Record of any errors encountered and recovery procedures applied
- **Context preservation**: Documentation of preserved session context during error conditions

### Error Response Examples

**Session Initialization Failure**:

``` markdown
ERROR: Session Management - Session initialization failed
Context: Establishing clear session goals and context
Cause: Thinking process timeout or initialization failure
Recovery:
1. Use simplified session initialization approach
2. Set basic session goals manually
3. Continue with minimal session context
```

**Memory Integration Error**:

``` markdown
ERROR: Memory Operation - Session memory storage failed
Context: Storing session findings in project memory
Cause: Memory server connectivity issues or storage limitations
Recovery:
1. Use alternative memory storage methods
2. Continue with session-local tracking
3. Use /meditate command for memory repair
```

**Learning Capture Failure**:

``` markdown
ERROR: Learning Capture - Insight processing failed
Context: Identifying new insights and patterns from session
Cause: Pattern recognition complexity or processing timeout
Recovery:
1. Use basic learning documentation methods
2. Implement user-guided insight capture
3. Continue with simplified learning tracking
```
