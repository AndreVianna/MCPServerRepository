#!/bin/bash

# Post-create script for MCP Hub development environment
# This script runs after the container is created

set -e

echo "🚀 Starting MCP Hub development environment setup..."

# Create necessary directories
mkdir -p /workspace/Source
mkdir -p /workspace/Tests
mkdir -p /workspace/Scripts
mkdir -p /workspace/Build
mkdir -p /workspace/Deployment

# Set up git configuration (if not already configured)
if [ ! -f ~/.gitconfig ]; then
    echo "📝 Setting up git configuration..."
    git config --global init.defaultBranch main
    git config --global pull.rebase false
    git config --global core.autocrlf input
    git config --global core.editor "code --wait"
    git config --global diff.tool vscode
    git config --global difftool.vscode.cmd 'code --wait --diff $LOCAL $REMOTE'
    git config --global merge.tool vscode
    git config --global mergetool.vscode.cmd 'code --wait $MERGED'
fi

# Trust the workspace for Git (security)
git config --global --add safe.directory /workspace

# Install/update .NET workloads
echo "📦 Installing .NET workloads..."
dotnet workload install aspire
dotnet workload install wasm-tools
dotnet workload install wasm-tools-net6
dotnet workload install maui

# Restore .NET tools
echo "🔧 Restoring .NET tools..."
dotnet tool restore 2>/dev/null || echo "No .NET tools manifest found, skipping restore"

# Set up development certificates
echo "🔐 Setting up development certificates..."
dotnet dev-certs https --trust --quiet || echo "Certificate setup completed"

# Update npm packages
echo "📦 Updating npm packages..."
npm update -g

# Create common development scripts
echo "📜 Creating development scripts..."

# Create build script
cat > /workspace/Scripts/build.sh << 'EOL'
#!/bin/bash
set -e

echo "🏗️  Building MCP Hub..."

# Navigate to source directory
cd /workspace/Source

# Clean previous builds
echo "🧹 Cleaning previous builds..."
dotnet clean --configuration Release --verbosity quiet

# Restore packages
echo "📦 Restoring packages..."
dotnet restore --verbosity quiet

# Build solution
echo "🔨 Building solution..."
dotnet build --configuration Release --no-restore --verbosity quiet

echo "✅ Build completed successfully!"
EOL

# Create test script
cat > /workspace/Scripts/test.sh << 'EOL'
#!/bin/bash
set -e

echo "🧪 Running tests for MCP Hub..."

# Navigate to source directory
cd /workspace/Source

# Run unit tests
echo "🏃 Running unit tests..."
dotnet test --configuration Release --no-build --verbosity quiet --logger "console;verbosity=minimal"

echo "✅ All tests passed!"
EOL

# Create dev script
cat > /workspace/Scripts/dev.sh << 'EOL'
#!/bin/bash
set -e

echo "🚀 Starting MCP Hub development server..."

# Navigate to source directory
cd /workspace/Source

# Check if AppHost project exists
if [ -d "AppHost" ]; then
    echo "🏃 Starting Aspire AppHost..."
    cd AppHost
    dotnet run --configuration Development
else
    echo "❌ AppHost project not found. Please run project scaffolding first."
    exit 1
fi
EOL

# Create migration script
cat > /workspace/Scripts/migrate.sh << 'EOL'
#!/bin/bash
set -e

echo "🗄️  Running database migrations..."

# Navigate to source directory
cd /workspace/Source

# Check if Data project exists
if [ -d "Data" ]; then
    echo "🏃 Applying migrations..."
    cd Data
    dotnet ef database update --verbose
    echo "✅ Database migrations completed!"
else
    echo "❌ Data project not found. Please run project scaffolding first."
    exit 1
fi
EOL

# Create format script
cat > /workspace/Scripts/format.sh << 'EOL'
#!/bin/bash
set -e

echo "🎨 Formatting code..."

# Navigate to source directory
cd /workspace/Source

# Format code
echo "🖌️  Running dotnet format..."
dotnet format --verbosity quiet

echo "✅ Code formatting completed!"
EOL

# Make scripts executable
chmod +x /workspace/Scripts/*.sh

# Create VS Code tasks
echo "📋 Setting up VS Code tasks..."
mkdir -p /workspace/.vscode

cat > /workspace/.vscode/tasks.json << 'EOL'
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "Build Solution",
            "type": "shell",
            "command": "/workspace/Scripts/build.sh",
            "group": {
                "kind": "build",
                "isDefault": true
            },
            "presentation": {
                "echo": true,
                "reveal": "always",
                "focus": false,
                "panel": "shared",
                "showReuseMessage": true,
                "clear": false
            },
            "problemMatcher": "$msCompile"
        },
        {
            "label": "Run Tests",
            "type": "shell",
            "command": "/workspace/Scripts/test.sh",
            "group": {
                "kind": "test",
                "isDefault": true
            },
            "presentation": {
                "echo": true,
                "reveal": "always",
                "focus": false,
                "panel": "shared",
                "showReuseMessage": true,
                "clear": false
            },
            "problemMatcher": "$msCompile"
        },
        {
            "label": "Start Development Server",
            "type": "shell",
            "command": "/workspace/Scripts/dev.sh",
            "group": "build",
            "presentation": {
                "echo": true,
                "reveal": "always",
                "focus": false,
                "panel": "dedicated",
                "showReuseMessage": true,
                "clear": false
            },
            "problemMatcher": []
        },
        {
            "label": "Run Migrations",
            "type": "shell",
            "command": "/workspace/Scripts/migrate.sh",
            "group": "build",
            "presentation": {
                "echo": true,
                "reveal": "always",
                "focus": false,
                "panel": "shared",
                "showReuseMessage": true,
                "clear": false
            },
            "problemMatcher": []
        },
        {
            "label": "Format Code",
            "type": "shell",
            "command": "/workspace/Scripts/format.sh",
            "group": "build",
            "presentation": {
                "echo": true,
                "reveal": "always",
                "focus": false,
                "panel": "shared",
                "showReuseMessage": true,
                "clear": false
            },
            "problemMatcher": []
        }
    ]
}
EOL

# Wait for database to be ready
echo "🔍 Waiting for database to be ready..."
until pg_isready -h postgres -U mcphub_user -d mcphub_dev; do
    echo "Waiting for PostgreSQL..."
    sleep 2
done

echo "✅ Post-create setup completed successfully!"
echo ""
echo "🎉 MCP Hub development environment is ready!"
echo ""
echo "Available commands:"
echo "  - /workspace/Scripts/build.sh    - Build the solution"
echo "  - /workspace/Scripts/test.sh     - Run tests"
echo "  - /workspace/Scripts/dev.sh      - Start development server"
echo "  - /workspace/Scripts/migrate.sh  - Run database migrations"
echo "  - /workspace/Scripts/format.sh   - Format code"
echo ""
echo "VS Code tasks are also available via Ctrl+Shift+P > Tasks: Run Task"
echo ""
echo "📚 Next steps:"
echo "  1. Run project scaffolding to create the solution structure"
echo "  2. Set up database migrations"
echo "  3. Start the development server"
echo ""