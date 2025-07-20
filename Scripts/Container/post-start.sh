#!/bin/bash

# Post-start script for MCP Hub development environment
# This script runs every time the container starts

set -e

echo "🔄 Starting MCP Hub development environment..."

# Ensure services are healthy
echo "🏥 Checking service health..."

# Check PostgreSQL
echo "🐘 Checking PostgreSQL..."
timeout 30 bash -c 'until pg_isready -h postgres -U mcphub_user -d mcphub_dev; do sleep 1; done' || echo "⚠️  PostgreSQL not responding"

# Check Redis
echo "📦 Checking Redis..."
timeout 15 bash -c 'until redis-cli -h redis ping; do sleep 1; done' || echo "⚠️  Redis not responding"

# Check Elasticsearch
echo "🔍 Checking Elasticsearch..."
timeout 30 bash -c 'until curl -s http://elasticsearch:9200/_cluster/health | grep -q "green\|yellow"; do sleep 1; done' || echo "⚠️  Elasticsearch not responding"

# Check Qdrant
echo "🧠 Checking Qdrant..."
timeout 30 bash -c 'until curl -s http://qdrant:6333/health | grep -q "ok"; do sleep 1; done' || echo "⚠️  Qdrant not responding"

# Check RabbitMQ
echo "🐰 Checking RabbitMQ..."
timeout 30 bash -c 'until rabbitmq-diagnostics -q ping -h rabbitmq; do sleep 1; done' || echo "⚠️  RabbitMQ not responding"

# Update PATH for dotnet tools
export PATH="/home/vscode/.dotnet/tools:${PATH}"

# Restore dotnet tools if manifest exists
if [ -f "/workspace/Source/.config/dotnet-tools.json" ]; then
    echo "🔧 Restoring .NET tools..."
    cd /workspace/Source
    dotnet tool restore
fi

# Install playwright browsers if needed
if [ ! -d "/home/vscode/.cache/ms-playwright" ]; then
    echo "🎭 Installing Playwright browsers..."
    npx playwright install --with-deps
fi

echo "✅ Development environment ready!"
echo ""
echo "🌐 Service URLs:"
echo "  - Web App: http://localhost:5000 (HTTP) / https://localhost:5001 (HTTPS)"
echo "  - Aspire Dashboard: http://localhost:18080"
echo "  - PostgreSQL: localhost:5432"
echo "  - Redis: localhost:6379"
echo "  - Elasticsearch: http://localhost:9200"
echo "  - Qdrant: http://localhost:6333"
echo "  - RabbitMQ: localhost:5672"
echo "  - RabbitMQ Management: http://localhost:15672"
echo ""