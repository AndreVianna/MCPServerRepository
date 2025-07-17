#!/bin/bash

# MCP Tools Validation Script
# Validates that MCP tools are properly configured and functional

set -e

echo "🔍 Validating MCP tools configuration..."

# Check directory structure
echo "📁 Checking directory structure..."
required_dirs=(
    "tests/mcp-tools"
    "docs/mcp-tools"
    "assets/figma"
    "assets/playwright-screenshots"
    "config/mcp-tools"
)

for dir in "${required_dirs[@]}"; do
    if [ -d "$dir" ]; then
        echo "  ✅ $dir exists"
    else
        echo "  ❌ $dir missing"
        exit 1
    fi
done

# Check configuration files
echo "⚙️ Checking configuration files..."
config_files=(
    "config/mcp-tools/figma-config.json"
    "config/mcp-tools/playwright-config.json"
    "config/mcp-tools/dev-environment.json"
    "config/mcp-tools/team-collaboration.json"
    "config/mcp-tools/ci-cd.json"
)

for file in "${config_files[@]}"; do
    if [ -f "$file" ]; then
        echo "  ✅ $file exists"
        # Validate JSON syntax
        if command -v jq &> /dev/null; then
            if jq empty "$file" 2>/dev/null; then
                echo "    ✅ Valid JSON"
            else
                echo "    ❌ Invalid JSON"
                exit 1
            fi
        fi
    else
        echo "  ❌ $file missing"
        exit 1
    fi
done

# Check scripts
echo "📜 Checking scripts..."
scripts=(
    "scripts/mcp-tools-integration.sh"
    "scripts/validate-mcp-tools.sh"
)

for script in "${scripts[@]}"; do
    if [ -f "$script" ]; then
        echo "  ✅ $script exists"
        if [ -x "$script" ]; then
            echo "    ✅ Executable"
        else
            echo "    ❌ Not executable"
            exit 1
        fi
    else
        echo "  ❌ $script missing"
        exit 1
    fi
done

# Check Claude Code settings
echo "🤖 Checking Claude Code settings..."
claude_settings=".claude/settings.local.json"
if [ -f "$claude_settings" ]; then
    echo "  ✅ Claude Code settings exist"
    
    # Check for required MCP tools
    required_tools=(
        "mcp__figma__get_figma_data"
        "mcp__figma__download_figma_images"
        "mcp__playwright__browser_navigate"
        "mcp__playwright__browser_take_screenshot"
        "mcp__playwright__browser_snapshot"
        "mcp__playwright__browser_click"
        "mcp__playwright__browser_type"
    )
    
    for tool in "${required_tools[@]}"; do
        if grep -q "$tool" "$claude_settings"; then
            echo "    ✅ $tool configured"
        else
            echo "    ❌ $tool not configured"
            exit 1
        fi
    done
else
    echo "  ❌ Claude Code settings missing"
    exit 1
fi

echo ""
echo "🎉 MCP tools validation completed successfully!"
echo "📋 Summary:"
echo "  - Directory structure: ✅"
echo "  - Configuration files: ✅"
echo "  - Scripts: ✅"
echo "  - Claude Code settings: ✅"
echo ""
echo "🚀 MCP tools are ready for use!"
