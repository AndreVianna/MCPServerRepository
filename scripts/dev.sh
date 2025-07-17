#!/bin/bash

# MCP Hub Development Server Script
# This script starts the development server with hot reload

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

# Function to check if Docker is running
check_docker() {
    if ! docker info >/dev/null 2>&1; then
        print_error "Docker is not running. Please start Docker."
        exit 1
    fi
}

# Function to check if solution exists
check_solution() {
    if [ ! -f "Source/MCPHub.sln" ]; then
        print_error "Solution file not found. Please run 'Scripts/scaffold.sh' first."
        exit 1
    fi
}

# Function to check if infrastructure is running
check_infrastructure() {
    print_status "Checking infrastructure services..."
    
    local services=("postgres" "redis" "elasticsearch" "qdrant" "rabbitmq")
    local all_healthy=true
    
    for service in "${services[@]}"; do
        if ! docker-compose ps "$service" | grep -q "Up"; then
            print_warning "Service $service is not running"
            all_healthy=false
        fi
    done
    
    if [ "$all_healthy" = false ]; then
        print_warning "Some infrastructure services are not running. Starting them now..."
        docker-compose up -d postgres redis elasticsearch qdrant rabbitmq jaeger prometheus grafana pgadmin redis-commander mailhog seq portainer traefik
        
        print_status "Waiting for services to be healthy..."
        sleep 30
    fi
}

# Function to run database migrations
run_migrations() {
    print_status "Running database migrations..."
    
    if [ -d "Source/Data" ]; then
        cd Source/Data
        dotnet ef database update --verbose
        cd ../..
        print_success "Database migrations completed"
    else
        print_warning "Data project not found. Skipping migrations."
    fi
}

# Function to build the solution
build_solution() {
    print_status "Building solution..."
    
    cd Source
    dotnet restore
    dotnet build --configuration Debug --no-restore
    cd ..
    
    print_success "Solution built successfully"
}

# Function to start the Aspire AppHost
start_aspire() {
    print_status "Starting Aspire AppHost..."
    
    cd Source/AppHost
    
    # Set environment variables
    export ASPNETCORE_ENVIRONMENT=Development
    export DOTNET_ENVIRONMENT=Development
    export DOTNET_HOTRELOAD_ENABLED=true
    export DOTNET_USE_POLLING_FILE_WATCHER=true
    export ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL=http://localhost:18080
    
    # Start the AppHost
    dotnet run --configuration Debug --launch-profile "Development"
}

# Function to start individual services
start_services() {
    print_status "Starting application services..."
    
    # Start services in background
    docker-compose up -d webapp publicapi securityservice searchservice
    
    print_success "Application services started"
}

# Function to display service status
display_status() {
    print_status "Checking service status..."
    
    echo
    echo "Service Status:"
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    
    # Check Docker services
    docker-compose ps
    
    echo
    echo "Service URLs:"
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    echo "🌐 Web Application:     http://mcphub.local"
    echo "🔗 Public API:          http://api.mcphub.local"
    echo "🔒 Security Service:    http://security.mcphub.local"
    echo "🔍 Search Service:      http://search.mcphub.local"
    echo "📊 Aspire Dashboard:    http://localhost:18080"
    echo "📊 Traefik Dashboard:   http://traefik.mcphub.local"
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    echo
}

# Function to follow logs
follow_logs() {
    print_status "Following logs..."
    
    # Follow logs for all services
    docker-compose logs -f
}

# Function to cleanup
cleanup() {
    print_status "Cleaning up..."
    
    # Stop all services
    docker-compose down
    
    print_success "Cleanup completed"
}

# Function to show help
show_help() {
    echo "MCP Hub Development Server"
    echo
    echo "Usage: $0 [OPTIONS]"
    echo
    echo "Options:"
    echo "  -h, --help          Show this help message"
    echo "  -s, --status        Show service status"
    echo "  -l, --logs          Follow logs"
    echo "  -c, --clean         Clean up and stop services"
    echo "  -b, --build         Build solution before starting"
    echo "  -m, --migrate       Run database migrations"
    echo "  --aspire            Start using Aspire AppHost"
    echo "  --docker            Start using Docker Compose"
    echo
    echo "Examples:"
    echo "  $0                  Start development server"
    echo "  $0 --build          Build and start development server"
    echo "  $0 --status         Show service status"
    echo "  $0 --logs           Follow logs"
    echo "  $0 --clean          Stop all services"
    echo
}

# Parse command line arguments
BUILD_SOLUTION=false
RUN_MIGRATIONS=false
SHOW_STATUS=false
FOLLOW_LOGS=false
CLEAN_UP=false
USE_ASPIRE=false
USE_DOCKER=true

while [[ $# -gt 0 ]]; do
    case $1 in
        -h|--help)
            show_help
            exit 0
            ;;
        -s|--status)
            SHOW_STATUS=true
            shift
            ;;
        -l|--logs)
            FOLLOW_LOGS=true
            shift
            ;;
        -c|--clean)
            CLEAN_UP=true
            shift
            ;;
        -b|--build)
            BUILD_SOLUTION=true
            shift
            ;;
        -m|--migrate)
            RUN_MIGRATIONS=true
            shift
            ;;
        --aspire)
            USE_ASPIRE=true
            USE_DOCKER=false
            shift
            ;;
        --docker)
            USE_DOCKER=true
            USE_ASPIRE=false
            shift
            ;;
        *)
            print_error "Unknown option: $1"
            show_help
            exit 1
            ;;
    esac
done

# Handle cleanup
if [ "$CLEAN_UP" = true ]; then
    cleanup
    exit 0
fi

# Handle status
if [ "$SHOW_STATUS" = true ]; then
    display_status
    exit 0
fi

# Handle logs
if [ "$FOLLOW_LOGS" = true ]; then
    follow_logs
    exit 0
fi

# Trap cleanup on exit
trap cleanup EXIT

# Main execution
main() {
    print_status "Starting MCP Hub development server..."
    
    # Check prerequisites
    check_docker
    check_solution
    check_infrastructure
    
    # Run migrations if requested
    if [ "$RUN_MIGRATIONS" = true ]; then
        run_migrations
    fi
    
    # Build solution if requested
    if [ "$BUILD_SOLUTION" = true ]; then
        build_solution
    fi
    
    # Start services based on mode
    if [ "$USE_ASPIRE" = true ]; then
        print_status "Starting in Aspire mode..."
        start_aspire
    elif [ "$USE_DOCKER" = true ]; then
        print_status "Starting in Docker mode..."
        start_services
        display_status
        
        # Keep script running
        print_status "Press Ctrl+C to stop services..."
        while true; do
            sleep 1
        done
    fi
}

# Run main function
main "$@"