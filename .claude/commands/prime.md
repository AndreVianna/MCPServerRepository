---
allowed-tools: Bash(lt:*), Read(*), mcp__memory__*, mcp__thinking__*
error-handling: comprehensive-framework, project-structure-validation, claude-md-processing, memory-optimization-handling, context-preparation-validation, recovery-procedures
description: Prepare the context for generic working with this project.
---

# Prime

## Context

This command prepares the essential context for working with the Claude Code Tools project. It loads project structure information, applies project instructions, and ensures the project memory is properly initialized and optimized. This is a foundational command that should be run at the start of each development session.

## Your Task

Execute the following steps in order to prepare the project context:

1. **Load project structure**: Run the `tree -I '.claude|.git|bin|obj|lib|.github|.cursor|.vscode|.vs|Assets'` bash command to get the latest structure of the project. The '-I' option is important.
2. **Apply project instructions**: Read and apply all instructions defined in the CLAUDE.md file
3. **Remember**: Read the full content of the project memory using `mcp__memory__read_graph`

## Verification

- **Pre-Context**: Verify system tool availability and project accessibility
- **During Process**: Validate each operation and handle failures immediately
- **Project Structure**: Confirm project structure is loaded and current
- **CLAUDE.md Processing**: Verify CLAUDE.md instructions are applied successfully
- **Memory**: Check memory recovery success

## Output

The command should produce:

- **Recover memory graph**: Updated memory structure
- **Loaded project context**: Current project structure with proper accessibility
- **Applied project instructions**: CLAUDE.md instructions ready for the session
- **Memory recovered**: Data saved in memory recovered successfully
