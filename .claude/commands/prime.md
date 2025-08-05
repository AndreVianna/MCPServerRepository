---
allowed-tools: Bash, TodoRead, LS, Grep, mcp__memory__read_graph, mcp__memory__search_nodes, mcp__memory__open_nodes, mcp__thinking__sequentialthinking
description: Prepare comprehensive session context for Streamline Pro development work (Windows version)
argument-hint: Optional depth level (quick|full) - defaults to full
---

# Prime Context for Streamline Pro (Windows)

Systematically prepare the development environment by loading project state, memory, and active work context. This command establishes comprehensive session context without duplicating information already available in CLAUDE.md.

**Platform**: Windows PowerShell compatible

## Instructions

### Phase 0: Environment Validation
- Fetch the current local date and time from the system.
- Check if current directory is a git repository with `git rev-parse --is-inside-work-tree`  
- Verify git is available and working properly
- If $ARGUMENTS contains "quick", set quick mode flag for streamlined execution

### Phase 1: Project State Discovery  
- Execute targeted project structure discovery commands:
  - Project structure (directories only): Use PowerShell command via Bash tool:
    `powershell "Get-ChildItem -Recurse -Directory | Where-Object { $_.Name -notmatch 'bin|target|node_modules|\.git.*|\.m2|\.p2|\.cache|\.husky|\.vscode|\.cursor' } | Sort-Object FullName"`
- Run `git status --porcelain --untracked-files=all` to check for uncommitted changes
- Check current branch with `git branch --show-current`
- Review recent activity with `git log --oneline -5`
- Identify any new or modified files that indicate current work areas

### Phase 2: Memory & History Integration
- **If NOT in quick mode**: Load full project memory using `mcp__memory__read_graph`
- **If in quick mode**: Skip memory integration for faster execution
- Search for current branch-specific context if not on master and not in quick mode
- Open relevant memory nodes that contain recent development context (full mode only)
- Identify continuation points from previous development sessions

### Phase 3: Active Work Assessment
- Check current todo list status using `TodoRead`
- Use `Grep` to search for TODO/FIXME comments: pattern "TODO|FIXME|XXX"
- Scan recent commits for work patterns and development focus areas

### Phase 4: Environment & Context Summary
- Summarize current context state and tasks status
- Highlight recommended next steps based on git status, memory, and todo list
- Flag any issues that need immediate attention before productive work
- Prepare focused development session based on discovered context

## Windows-Specific Implementation Notes

### Directory Listing Alternative
Since `tree` command may not be available on all Windows systems, this command uses PowerShell's `Get-ChildItem` with filtering:
```powershell
Get-ChildItem -Recurse -Directory | 
Where-Object { $_.Name -notmatch 'bin|target|node_modules|\.git.*|\.m2|\.p2|\.cache|\.husky|\.vscode|\.cursor' } | 
Sort-Object FullName
```

### File Path Handling
- Uses Windows path separators (`\`) where appropriate
- Handles Windows drive letters and UNC paths
- PowerShell-compatible path formats

### Command Execution
- All system commands executed through the Bash tool using PowerShell when needed
- Git commands work cross-platform (no changes needed)
- PowerShell cmdlets wrapped in appropriate syntax

### Windows Environment Considerations
- PowerShell execution policies (commands designed to work with default policies)
- Windows-specific environment variables and paths
- Integration with Windows development tools and file systems

**Note**: This command complements CLAUDE.md (which provides static project information) by loading dynamic session-specific context including git state, memory, todos, and recent activity. This Windows version uses PowerShell commands for directory operations while maintaining identical functionality to the Unix version.