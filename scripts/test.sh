#!/bin/bash

# MCP Hub Test Script
# This script runs the test suite for MCP Hub

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

# Function to check if solution exists
check_solution() {
    if [ ! -f "Source/MCPHub.sln" ]; then
        print_error "Solution file not found. Please run 'Scripts/scaffold.sh' first."
        exit 1
    fi
}

# Function to start test infrastructure
start_test_infrastructure() {
    print_status "Starting test infrastructure..."
    
    # Start test database and other services
    docker-compose -f docker-compose.test.yml up -d
    
    # Wait for services to be ready
    print_status "Waiting for test infrastructure to be ready..."
    sleep 30
    
    print_success "Test infrastructure started"
}

# Function to run unit tests
run_unit_tests() {
    print_status "Running unit tests..."
    
    cd Source
    
    # Run unit tests with coverage
    dotnet test \
        --configuration Release \
        --collect:"XPlat Code Coverage" \
        --results-directory ../TestResults/ \
        --logger trx \
        --logger "console;verbosity=detailed" \
        --filter "Category!=Integration&Category!=E2E" \
        --verbosity normal
    
    cd ..
    
    print_success "Unit tests completed"
}

# Function to run integration tests
run_integration_tests() {
    print_status "Running integration tests..."
    
    cd Source
    
    # Set test environment variables
    export ASPNETCORE_ENVIRONMENT=Testing
    export DOTNET_ENVIRONMENT=Testing
    export ConnectionStrings__DefaultConnection="Host=localhost;Database=mcphub_test;Username=mcphub_user;Password=mcphub_test_password"
    export ConnectionStrings__Redis="localhost:6379"
    export ConnectionStrings__Elasticsearch="http://localhost:9200"
    export ConnectionStrings__Qdrant="http://localhost:6333"
    export ConnectionStrings__RabbitMQ="amqp://guest:guest@localhost:5672"
    
    # Run integration tests
    dotnet test \
        --configuration Release \
        --collect:"XPlat Code Coverage" \
        --results-directory ../TestResults/ \
        --logger trx \
        --logger "console;verbosity=detailed" \
        --filter "Category=Integration" \
        --verbosity normal
    
    cd ..
    
    print_success "Integration tests completed"
}

# Function to run E2E tests
run_e2e_tests() {
    print_status "Running E2E tests..."
    
    # Start application services
    docker-compose up -d webapp publicapi securityservice searchservice
    
    # Wait for services to be ready
    print_status "Waiting for application services to be ready..."
    sleep 60
    
    # Run Playwright tests
    npx playwright test
    
    print_success "E2E tests completed"
}

# Function to run performance tests
run_performance_tests() {
    print_status "Running performance tests..."
    
    cd Source
    
    # Run performance tests if they exist
    if [ -d "PerformanceTests" ]; then
        cd PerformanceTests
        dotnet run --configuration Release
        cd ..
    else
        print_warning "Performance tests not found. Skipping..."
    fi
    
    cd ..
    
    print_success "Performance tests completed"
}

# Function to run security tests
run_security_tests() {
    print_status "Running security tests..."
    
    # Check for vulnerable packages
    cd Source
    dotnet list package --vulnerable --include-transitive
    
    # Run security scan if tool is available
    if command -v dotnet-sonarscanner >/dev/null 2>&1; then
        print_status "Running SonarQube analysis..."
        dotnet sonarscanner begin /k:"mcphub" /d:sonar.host.url="http://localhost:9000" /d:sonar.login="admin" /d:sonar.password="admin" || true
        dotnet build --configuration Release
        dotnet sonarscanner end /d:sonar.login="admin" /d:sonar.password="admin" || true
    fi
    
    cd ..
    
    print_success "Security tests completed"
}

# Function to generate test report
generate_test_report() {
    print_status "Generating test report..."
    
    # Generate coverage report
    if command -v reportgenerator >/dev/null 2>&1; then
        reportgenerator \
            -reports:"TestResults/*/coverage.cobertura.xml" \
            -targetdir:"TestResults/CoverageReport" \
            -reporttypes:"HtmlInline_AzurePipelines;Cobertura;JsonSummary"
        
        print_success "Coverage report generated at TestResults/CoverageReport/"
    else
        print_warning "reportgenerator not found. Skipping coverage report."
    fi
    
    # Display test summary
    if [ -f "TestResults/CoverageReport/Summary.json" ]; then
        print_status "Test Coverage Summary:"
        cat TestResults/CoverageReport/Summary.json | jq '.summary.linecoverage' 2>/dev/null || echo "Coverage data available in TestResults/CoverageReport/"
    fi
    
    print_success "Test report generated"
}

# Function to cleanup test environment
cleanup_test_environment() {
    print_status "Cleaning up test environment..."
    
    # Stop test infrastructure
    docker-compose -f docker-compose.test.yml down
    
    # Stop application services
    docker-compose down
    
    print_success "Test environment cleaned up"
}

# Function to show help
show_help() {
    echo "MCP Hub Test Runner"
    echo
    echo "Usage: $0 [OPTIONS]"
    echo
    echo "Options:"
    echo "  -h, --help          Show this help message"
    echo "  -u, --unit          Run unit tests only"
    echo "  -i, --integration   Run integration tests only"
    echo "  -e, --e2e           Run E2E tests only"
    echo "  -p, --performance   Run performance tests only"
    echo "  -s, --security      Run security tests only"
    echo "  -a, --all           Run all tests (default)"
    echo "  -c, --coverage      Generate coverage report"
    echo "  -w, --watch         Run tests in watch mode"
    echo "  --no-build          Skip building before running tests"
    echo "  --parallel          Run tests in parallel"
    echo "  --verbose           Enable verbose output"
    echo
    echo "Examples:"
    echo "  $0                  Run all tests"
    echo "  $0 --unit           Run unit tests only"
    echo "  $0 --integration    Run integration tests only"
    echo "  $0 --e2e            Run E2E tests only"
    echo "  $0 --coverage       Run tests and generate coverage report"
    echo "  $0 --watch          Run tests in watch mode"
    echo
}

# Parse command line arguments
RUN_UNIT=false
RUN_INTEGRATION=false
RUN_E2E=false
RUN_PERFORMANCE=false
RUN_SECURITY=false
RUN_ALL=true
GENERATE_COVERAGE=false
WATCH_MODE=false
NO_BUILD=false
PARALLEL=false
VERBOSE=false

while [[ $# -gt 0 ]]; do
    case $1 in
        -h|--help)
            show_help
            exit 0
            ;;
        -u|--unit)
            RUN_UNIT=true
            RUN_ALL=false
            shift
            ;;
        -i|--integration)
            RUN_INTEGRATION=true
            RUN_ALL=false
            shift
            ;;
        -e|--e2e)
            RUN_E2E=true
            RUN_ALL=false
            shift
            ;;
        -p|--performance)
            RUN_PERFORMANCE=true
            RUN_ALL=false
            shift
            ;;
        -s|--security)
            RUN_SECURITY=true
            RUN_ALL=false
            shift
            ;;
        -a|--all)
            RUN_ALL=true
            shift
            ;;
        -c|--coverage)
            GENERATE_COVERAGE=true
            shift
            ;;
        -w|--watch)
            WATCH_MODE=true
            shift
            ;;
        --no-build)
            NO_BUILD=true
            shift
            ;;
        --parallel)
            PARALLEL=true
            shift
            ;;
        --verbose)
            VERBOSE=true
            shift
            ;;
        *)
            print_error "Unknown option: $1"
            show_help
            exit 1
            ;;
    esac
done

# Function to run tests in watch mode
run_watch_mode() {
    print_status "Running tests in watch mode..."
    
    cd Source
    
    # Run tests in watch mode
    dotnet watch test \
        --configuration Debug \
        --logger "console;verbosity=detailed" \
        --filter "Category!=Integration&Category!=E2E"
    
    cd ..
}

# Trap cleanup on exit
trap cleanup_test_environment EXIT

# Main execution
main() {
    print_status "Starting MCP Hub test suite..."
    
    # Check prerequisites
    check_solution
    
    # Handle watch mode
    if [ "$WATCH_MODE" = true ]; then
        run_watch_mode
        exit 0
    fi
    
    # Create test results directory
    mkdir -p TestResults
    
    # Start test infrastructure
    start_test_infrastructure
    
    # Build solution if not skipped
    if [ "$NO_BUILD" = false ]; then
        print_status "Building solution..."
        cd Source
        dotnet restore
        dotnet build --configuration Release --no-restore
        cd ..
        print_success "Solution built"
    fi
    
    # Run tests based on options
    if [ "$RUN_ALL" = true ]; then
        run_unit_tests
        run_integration_tests
        run_e2e_tests
        run_performance_tests
        run_security_tests
    else
        if [ "$RUN_UNIT" = true ]; then
            run_unit_tests
        fi
        
        if [ "$RUN_INTEGRATION" = true ]; then
            run_integration_tests
        fi
        
        if [ "$RUN_E2E" = true ]; then
            run_e2e_tests
        fi
        
        if [ "$RUN_PERFORMANCE" = true ]; then
            run_performance_tests
        fi
        
        if [ "$RUN_SECURITY" = true ]; then
            run_security_tests
        fi
    fi
    
    # Generate test report
    generate_test_report
    
    print_success "All tests completed successfully!"
}

# Run main function
main "$@"