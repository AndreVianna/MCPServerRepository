#!/bin/bash

# MCP Hub Development Environment Setup Script
# This script sets up the development environment for MCP Hub

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[0;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to check if command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Function to check if Docker is running
check_docker() {
    if ! command_exists docker; then
        print_error "Docker is not installed. Please install Docker first."
        exit 1
    fi
    
    if ! docker info >/dev/null 2>&1; then
        print_error "Docker is not running. Please start Docker."
        exit 1
    fi
    
    print_success "Docker is installed and running"
}

# Function to check if Docker Compose is available
check_docker_compose() {
    if ! command_exists docker-compose && ! docker compose version >/dev/null 2>&1; then
        print_error "Docker Compose is not available. Please install Docker Compose."
        exit 1
    fi
    
    print_success "Docker Compose is available"
}

# Function to check if .NET is installed
check_dotnet() {
    if ! command_exists dotnet; then
        print_error ".NET is not installed. Please install .NET 9 SDK."
        exit 1
    fi
    
    local dotnet_version=$(dotnet --version)
    if [[ ! "$dotnet_version" =~ ^9\. ]]; then
        print_warning ".NET version is $dotnet_version. .NET 9 is recommended."
    else
        print_success ".NET 9 is installed"
    fi
}

# Function to check if Node.js is installed
check_nodejs() {
    if ! command_exists node; then
        print_error "Node.js is not installed. Please install Node.js 20 or later."
        exit 1
    fi
    
    local node_version=$(node --version | sed 's/v//')
    local major_version=$(echo "$node_version" | cut -d. -f1)
    
    if [ "$major_version" -lt 20 ]; then
        print_warning "Node.js version is $node_version. Node.js 20 or later is recommended."
    else
        print_success "Node.js $node_version is installed"
    fi
}

# Function to set up environment variables
setup_environment() {
    print_status "Setting up environment variables..."
    
    # Create .env file if it doesn't exist
    if [ ! -f ".env" ]; then
        cat > .env << EOF
# MCP Hub Development Environment Variables
ASPNETCORE_ENVIRONMENT=Development
DOTNET_ENVIRONMENT=Development
DOTNET_CLI_TELEMETRY_OPTOUT=1
DOTNET_NOLOGO=1
DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

# Database
POSTGRES_DB=mcphub_dev
POSTGRES_USER=mcphub_user
POSTGRES_PASSWORD=mcphub_dev_password

# JWT
JWT_SECRET_KEY=your-super-secret-jwt-key-here-change-in-production-environment
JWT_ISSUER=MCPHub
JWT_AUDIENCE=MCPHub
JWT_EXPIRATION_MINUTES=60

# GitHub OAuth (update these with your values)
GITHUB_CLIENT_ID=your-github-client-id
GITHUB_CLIENT_SECRET=your-github-client-secret

# Security
SECURITY_SCAN_TIMEOUT=300
SECURITY_MAX_CONCURRENT_SCANS=5
VIRUSTOTAL_API_KEY=your-virustotal-api-key

# Search
SEARCH_INDEX_NAME=mcphub-packages
SEARCH_MAX_RESULTS=100
SEARCH_SEMANTIC_SEARCH_ENABLED=true

# Monitoring
OPENTELEMETRY_ENDPOINT=http://localhost:4317
PROMETHEUS_PORT=9090
GRAFANA_PORT=3000

# Email (for development)
SMTP_HOST=mailhog
SMTP_PORT=1025
SMTP_USER=
SMTP_PASSWORD=

# Logging
SEQ_SERVER_URL=http://localhost:5341
SEQ_API_KEY=
LOG_LEVEL=Information
EOF
        print_success "Created .env file"
    else
        print_warning ".env file already exists"
    fi
}

# Function to set up hosts file entries
setup_hosts() {
    print_status "Setting up local hosts entries..."
    
    local hosts_entries=(
        "127.0.0.1 mcphub.local"
        "127.0.0.1 api.mcphub.local"
        "127.0.0.1 security.mcphub.local"
        "127.0.0.1 search.mcphub.local"
        "127.0.0.1 jaeger.mcphub.local"
        "127.0.0.1 prometheus.mcphub.local"
        "127.0.0.1 grafana.mcphub.local"
        "127.0.0.1 pgadmin.mcphub.local"
        "127.0.0.1 redis.mcphub.local"
        "127.0.0.1 mail.mcphub.local"
        "127.0.0.1 seq.mcphub.local"
        "127.0.0.1 portainer.mcphub.local"
        "127.0.0.1 traefik.mcphub.local"
    )
    
    for entry in "${hosts_entries[@]}"; do
        if ! grep -q "$entry" /etc/hosts; then
            echo "$entry" | sudo tee -a /etc/hosts >/dev/null
            print_success "Added $entry to hosts file"
        else
            print_warning "$entry already exists in hosts file"
        fi
    done
}

# Function to install .NET workloads
install_dotnet_workloads() {
    print_status "Installing .NET workloads..."
    
    dotnet workload install aspire --skip-sign-check
    dotnet workload install wasm-tools --skip-sign-check
    dotnet workload install wasm-tools-net6 --skip-sign-check
    
    print_success ".NET workloads installed"
}

# Function to install global .NET tools
install_dotnet_tools() {
    print_status "Installing global .NET tools..."
    
    local tools=(
        "dotnet-ef"
        "dotnet-aspnet-codegenerator"
        "dotnet-reportgenerator-globaltool"
        "dotnet-format"
        "dotnet-outdated-tool"
        "dotnet-trace"
        "dotnet-dump"
        "dotnet-counters"
        "Microsoft.dotnet-httprepl"
        "dotnet-sonarscanner"
        "Microsoft.Web.LibraryManager.Cli"
        "dotnet-dev-certs"
    )
    
    for tool in "${tools[@]}"; do
        if ! dotnet tool list -g | grep -q "$tool"; then
            dotnet tool install -g "$tool"
            print_success "Installed $tool"
        else
            print_warning "$tool already installed"
        fi
    done
}

# Function to install npm packages
install_npm_packages() {
    print_status "Installing npm packages..."
    
    npm install -g \
        playwright \
        @playwright/test \
        typescript \
        @types/node \
        eslint \
        prettier \
        @typescript-eslint/parser \
        @typescript-eslint/eslint-plugin \
        npm-check-updates
    
    print_success "npm packages installed"
}

# Function to install Playwright browsers
install_playwright_browsers() {
    print_status "Installing Playwright browsers..."
    
    npx playwright install --with-deps
    
    print_success "Playwright browsers installed"
}

# Function to set up development certificates
setup_dev_certificates() {
    print_status "Setting up development certificates..."
    
    dotnet dev-certs https --clean
    dotnet dev-certs https --trust
    
    print_success "Development certificates set up"
}

# Function to create necessary directories
create_directories() {
    print_status "Creating necessary directories..."
    
    local directories=(
        "Source"
        "Tests"
        "Scripts"
        "Build"
        "Deployment"
        "Monitoring/grafana/dashboards"
        "Monitoring/grafana/provisioning/dashboards"
        "Monitoring/grafana/provisioning/datasources"
        "Monitoring/grafana/provisioning/notifiers"
        "Nginx/conf.d"
        "Nginx/ssl"
        "Logs"
        "Temp"
    )
    
    for dir in "${directories[@]}"; do
        if [ ! -d "$dir" ]; then
            mkdir -p "$dir"
            print_success "Created directory: $dir"
        else
            print_warning "Directory already exists: $dir"
        fi
    done
}

# Function to build Docker images
build_docker_images() {
    print_status "Building Docker images..."
    
    # Only build if source code exists
    if [ -d "Source" ] && [ -f "Source/MCPHub.sln" ]; then
        docker-compose build --no-cache
        print_success "Docker images built"
    else
        print_warning "Source code not found. Skipping Docker image build."
    fi
}

# Function to start infrastructure services
start_infrastructure() {
    print_status "Starting infrastructure services..."
    
    # Start only infrastructure services
    docker-compose up -d postgres redis elasticsearch qdrant rabbitmq jaeger prometheus grafana pgadmin redis-commander mailhog seq portainer traefik
    
    print_success "Infrastructure services started"
    
    # Wait for services to be healthy
    print_status "Waiting for services to be healthy..."
    
    local max_attempts=30
    local attempt=1
    
    while [ $attempt -le $max_attempts ]; do
        if docker-compose ps | grep -q "Up (healthy)"; then
            print_success "Services are healthy"
            break
        fi
        
        if [ $attempt -eq $max_attempts ]; then
            print_error "Services failed to become healthy within timeout"
            exit 1
        fi
        
        print_status "Attempt $attempt/$max_attempts - waiting for services..."
        sleep 10
        ((attempt++))
    done
}

# Function to run database migrations
run_migrations() {
    print_status "Running database migrations..."
    
    # Only run migrations if source code exists
    if [ -d "Source/Data" ] && [ -f "Source/Data/Data.csproj" ]; then
        cd Source/Data
        dotnet ef database update --verbose
        cd ../..
        print_success "Database migrations completed"
    else
        print_warning "Data project not found. Skipping migrations."
    fi
}

# Function to display service URLs
display_service_urls() {
    print_success "Development environment is ready!"
    echo
    echo "Service URLs:"
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    echo "🌐 Web Application:     http://mcphub.local"
    echo "🔗 Public API:          http://api.mcphub.local"
    echo "🔒 Security Service:    http://security.mcphub.local"
    echo "🔍 Search Service:      http://search.mcphub.local"
    echo "📊 Traefik Dashboard:   http://traefik.mcphub.local"
    echo "🐘 PostgreSQL:          localhost:5432"
    echo "📦 Redis:               localhost:6379"
    echo "🔍 Elasticsearch:       http://localhost:9200"
    echo "🧠 Qdrant:              http://localhost:6333"
    echo "🐰 RabbitMQ:            localhost:5672"
    echo "🐰 RabbitMQ Management: http://localhost:15672"
    echo "🔍 Jaeger:              http://jaeger.mcphub.local"
    echo "📊 Prometheus:          http://prometheus.mcphub.local"
    echo "📊 Grafana:             http://grafana.mcphub.local"
    echo "🐘 pgAdmin:             http://pgadmin.mcphub.local"
    echo "📦 Redis Commander:     http://redis.mcphub.local"
    echo "📧 MailHog:             http://mail.mcphub.local"
    echo "📋 Seq:                 http://seq.mcphub.local"
    echo "🐳 Portainer:           http://portainer.mcphub.local"
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    echo
    echo "Default credentials:"
    echo "  pgAdmin:    admin@mcphub.local / admin"
    echo "  Grafana:    admin / admin"
    echo "  RabbitMQ:   guest / guest"
    echo "  Portainer:  admin / (set on first login)"
    echo
    echo "Next steps:"
    echo "  1. Run 'Scripts/scaffold.sh' to create the solution structure"
    echo "  2. Run 'Scripts/dev.sh' to start the application services"
    echo "  3. Run 'Scripts/test.sh' to run the test suite"
    echo
    echo "For more information, see the README.md file"
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
}

# Main execution
main() {
    print_status "Starting MCP Hub development environment setup..."
    
    # Check prerequisites
    check_docker
    check_docker_compose
    check_dotnet
    check_nodejs
    
    # Setup environment
    setup_environment
    setup_hosts
    create_directories
    
    # Install tools and dependencies
    install_dotnet_workloads
    install_dotnet_tools
    install_npm_packages
    install_playwright_browsers
    setup_dev_certificates
    
    # Start infrastructure
    start_infrastructure
    
    # Run migrations if possible
    run_migrations
    
    # Display completion message
    display_service_urls
}

# Run main function
main "$@"