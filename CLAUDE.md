# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is an MCP (Model Context Protocol) server registration repository that will work like NPM and Nuget. Authors will be able to register their servers and users will be able to search and use the servers in their projects.

**Current Status**: Early development phase - project structure and tooling are still being established.

## Development Environment Setup

### Custom Commands
- `prime` - Prepares context for working with the project by loading memory and applying sequential thinking approach

## Architecture

### Current Structure
- **/.claude/**: Claude Code configuration and memory persistence
- **No source code directories yet** - implementation pending

### Development Approach
- Use sequential thinking for complex problem-solving
- Maintain project progress in memory for context continuity
- Memory updates should analyze the full memory graph to avoid duplications and remove stale items

## Git Repository
- **Main branch**: `main`
- **License**: MIT
- **Current uncommitted changes**: Configuration files and setup scripts

## Future Development
This project will eventually include:
- Server registration system
- Search functionality for MCP servers
- User management for server authors
- Integration capabilities for end users