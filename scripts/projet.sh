#!/bin/bash

# projet.sh - MCP Hub project CLI tool
# Version: 1.0
# Description: Initialize, build, and test the MCP Hub project in containers using Podman

set -e  # Exit on any error
set -o pipefail  # Exit on pipe failures

# Change to script directory for correct relative path resolution
cd "$(dirname "${BASH_SOURCE[0]}")"

# Source helper functions
# shellcheck source=./helpers/error.sh
source "helpers/error.sh"
# shellcheck source=./helpers/output.sh
source "helpers/output.sh"
# shellcheck source=./helpers/log.sh
source "helpers/log.sh"
# shellcheck source=./helpers/platform.sh
source "helpers/platform.sh"
# shellcheck source=./helpers/container.sh
source "helpers/container.sh"
# shellcheck source=./helpers/validation.sh
source "helpers/validation.sh"
# shellcheck source=./helpers/process.sh
source "helpers/process.sh"

# Initialize script paths
SCRIPT_PATH="$(normalize_path "$(pwd)")"
export SCRIPT_PATH

# Project configuration
PROJECT_ROOT="$(dirname "$SCRIPT_PATH")"
DOCKERFILE="$SCRIPT_PATH/dev.dockerfile"
CONTAINER_IMAGE="mcphub-dotnet-dev:latest"
CONTAINER_ENGINE="podman"
export PROJECT_ROOT DOCKERFILE CONTAINER_IMAGE CONTAINER_ENGINE

# Source container configuration
set_container_config "engine" "podman"
set_container_config "image" "$CONTAINER_IMAGE"
set_container_config "workdir" "/workspace"
set_container_config "volumes" "$PROJECT_ROOT:/workspace"
set_container_config "cleanup" "true"

# Initialize signal handling
setup_signal_handlers
register_cleanup_function "cleanup_container_on_exit"

# Main help function
show_help() {
    cat << EOF
MCP Hub Project CLI Tool - projet.sh

USAGE:
    ./projet.sh [-h|--help] [global options] <command> [command options]

COMMANDS:
    init        Initialize development container
    build       Build the MCP Hub project
    test        Execute tests
    run         Start the MCP Hub applications
    doctor      Validate development environment

GLOBAL OPTIONS:
    -h, --help                 Show help information
    -l, --log-level <level>    Set verbosity level: 
                                 3, q, quiet: no-output,
                                 2, m, minimal: show only errors and completed steps,
                                 1, v, verbose: show verbose output (all except debug),
                                 0, d, debug: show all messages

EXAMPLES:
    ./projet.sh init                                    # Initialize container
    ./projet.sh build                                   # Build MCP Hub solution
    ./projet.sh build --clean --debug                  # Clean debug build
    ./projet.sh test                                    # Run all tests
    ./projet.sh test --filter "Name~Domain"            # Run domain tests only
    ./projet.sh test --collect "XPlat Code Coverage"   # Run tests with coverage
    ./projet.sh run                                     # Start the applications
    ./projet.sh run --environment Development          # Start in dev environment
    ./projet.sh doctor                                  # Validate environment

PREREQUISITES:
    - Podman installed and configured
    - Run './projet.sh init' first to create development container
    - .NET 9 SDK and runtime (handled by container)
    - PostgreSQL, Redis, Elasticsearch (handled by .NET Aspire)

For command-specific help, use: ./projet.sh COMMAND --help
EOF
}

# Initialize development container
init_container() {
    log_info "Initializing MCP Hub development container..."
    
    # Check if Podman is available
    if ! command -v podman >/dev/null 2>&1; then
        log_error "Podman is not installed or not in PATH"
        log_info "Please install Podman to use this tool"
        exit_with "$PREREQUISITE_ERROR"
    fi
    
    # Check if Dockerfile exists
    if [[ ! -f "$DOCKERFILE" ]]; then
        log_error "Dockerfile not found: $DOCKERFILE"
        exit_with "$FILE_NOT_FOUND"
    fi
    
    # Build container if it doesn't exist or force rebuild
    if ! check_container_exists || [[ "${FORCE_REBUILD:-false}" == "true" ]]; then
        log_info "Building development container: $CONTAINER_IMAGE"
        build_custom_container
    else
        log_info "Development container already exists: $CONTAINER_IMAGE"
    fi
    
    # Validate container health
    if ! check_container_health; then
        log_error "Container health check failed"
        exit_with "$CONTAINER_ERROR"
    fi
    
    log_success "Development container initialized successfully"
}

# Build the MCP Hub project
build_project() {
    log_info "Building MCP Hub project..."
    
    # Ensure container exists
    if ! check_container_exists; then
        log_info "Container not found, initializing..."
        init_container
    fi
    
    # Set build configuration
    local config="${BUILD_CONFIG:-Release}"
    local build_args=("dotnet" "build" "Source/MCPHub.sln" "--configuration" "$config")
    
    # Add clean flag if specified
    if [[ "${BUILD_CLEAN:-false}" == "true" ]]; then
        build_args+=("--force")
        log_info "Performing clean build..."
    fi
    
    # Add no-restore flag if specified
    if [[ "${NO_RESTORE:-false}" == "true" ]]; then
        build_args+=("--no-restore")
        log_info "Skipping package restore..."
    fi
    
    # Build the solution
    log_info "Building .NET solution with configuration: $config"
    run_container_async "${build_args[@]}"
    
    log_success "MCP Hub project built successfully"
}

# Run tests
run_tests() {
    log_info "Running MCP Hub tests..."
    
    # Ensure container exists
    if ! check_container_exists; then
        log_info "Container not found, initializing..."
        init_container
    fi
    
    # Set test configuration
    local config="${BUILD_CONFIG:-Release}"
    local test_args=("dotnet" "test" "Source/MCPHub.sln" "--configuration" "$config" "--no-build")
    
    # Add filter if specified
    if [[ -n "${TEST_FILTER:-}" ]]; then
        test_args+=("--filter" "$TEST_FILTER")
        log_info "Running tests with filter: $TEST_FILTER"
    fi
    
    # Add collect option if specified
    if [[ -n "${TEST_COLLECT:-}" ]]; then
        test_args+=("--collect" "$TEST_COLLECT")
        log_info "Collecting: $TEST_COLLECT"
    fi
    
    # Add logger if specified
    if [[ -n "${TEST_LOGGER:-}" ]]; then
        test_args+=("--logger" "$TEST_LOGGER")
        log_info "Using logger: $TEST_LOGGER"
    else
        test_args+=("--verbosity" "normal")
    fi
    
    # Run tests
    log_info "Running unit tests with configuration: $config"
    run_container_async "${test_args[@]}"
    
    log_success "All tests completed successfully"
}

# Run the applications
run_applications() {
    log_info "Starting MCP Hub applications..."
    
    # Ensure container exists
    if ! check_container_exists; then
        log_info "Container not found, initializing..."
        init_container
    fi
    
    # Set run configuration
    local config="${BUILD_CONFIG:-Release}"
    local run_args=("dotnet" "run" "--project" "Source/AppHost/AppHost.csproj" "--configuration" "$config")
    
    # Add environment if specified
    if [[ -n "${RUN_ENVIRONMENT:-}" ]]; then
        run_args+=("--environment" "$RUN_ENVIRONMENT")
        log_info "Using environment: $RUN_ENVIRONMENT"
    fi
    
    # Add launch profile if specified
    if [[ -n "${LAUNCH_PROFILE:-}" ]]; then
        run_args+=("--launch-profile" "$LAUNCH_PROFILE")
        log_info "Using launch profile: $LAUNCH_PROFILE"
    fi
    
    # Start the .NET Aspire AppHost
    log_info "Starting .NET Aspire orchestration with configuration: $config"
    run_container_async "${run_args[@]}"
    
    log_success "MCP Hub applications started successfully"
}

# Validate development environment
validate_environment() {
    log_info "Validating MCP Hub development environment..."
    
    # Check Podman
    if ! command -v podman >/dev/null 2>&1; then
        log_error "✗ Podman not found"
        exit_with "$PREREQUISITE_ERROR"
    else
        log_success "✓ Podman available"
    fi
    
    # Check container
    if check_container_exists; then
        log_success "✓ Development container exists"
        
        # Validate container tools
        if validate_container_tools; then
            log_success "✓ Container tools validated"
        else
            log_error "✗ Container tool validation failed"
            exit_with "$VALIDATION_ERROR"
        fi
    else
        log_warning "⚠ Development container not found"
        log_info "Run './projet.sh init' to create the container"
    fi
    
    # Check project structure
    if [[ -f "$PROJECT_ROOT/Source/MCPHub.sln" ]]; then
        log_success "✓ MCP Hub solution found"
    else
        log_error "✗ MCP Hub solution not found"
        exit_with "$FILE_NOT_FOUND"
    fi
    
    log_success "Environment validation completed"
}

# Parse command line arguments
COMMAND=""
REMAINING_ARGS=()

parse_options() {
    # First pass: check for immediate help request
    if [[ $# -eq 0 ]]; then
        show_help
        exit_with "$SUCCESS"
    fi

    # Second pass: parse global options
    while [[ $# -gt 0 ]]; do
        case $1 in
            -l|--log-level)
                set_log_level "$2"
                shift 2
                ;;
            -h|--help)
                if [[ -z "$COMMAND" ]]; then
                    show_help
                    exit_with "$SUCCESS"
                else
                    break
                fi
                ;;
            --force-rebuild)
                FORCE_REBUILD="true"
                shift
                ;;
            --clean)
                BUILD_CLEAN="true"
                shift
                ;;
            --debug)
                BUILD_CONFIG="Debug"
                shift
                ;;
            --no-restore)
                NO_RESTORE="true"
                shift
                ;;
            --filter)
                TEST_FILTER="$2"
                shift 2
                ;;
            --collect)
                TEST_COLLECT="$2"
                shift 2
                ;;
            --logger)
                TEST_LOGGER="$2"
                shift 2
                ;;
            --environment)
                RUN_ENVIRONMENT="$2"
                shift 2
                ;;
            --launch-profile)
                LAUNCH_PROFILE="$2"
                shift 2
                ;;
            init|build|test|run|doctor)
                COMMAND="$1"
                shift
                if [[ "$1" == "--help" || "$1" == "-h" ]]; then
                    show_command_help "$COMMAND"
                    exit_with "$SUCCESS"
                fi
                break
                ;;
            -*)
                log_error "Unknown global option: $1"
                log_info "Use ./projet.sh --help for available options"
                exit_with "$GENERAL_ERROR"
                ;;
            *)
                log_error "Unknown command: $1"
                log_info "Use ./projet.sh --help for available commands"
                exit_with "$GENERAL_ERROR"
                ;;
        esac
    done

    # If no command is provided, show help
    if [[ -z "$COMMAND" ]]; then
        show_help
        exit_with "$SUCCESS"
    fi

    # Store remaining arguments
    REMAINING_ARGS=("$@")
}

# Show command-specific help
show_command_help() {
    local command="$1"
    
    case "$command" in
        init)
            cat << EOF
Initialize development container

USAGE:
    ./projet.sh init [--force-rebuild]

OPTIONS:
    --force-rebuild    Force rebuild of container even if it exists

DESCRIPTION:
    Creates and configures the development container with all required tools:
    - .NET 9 SDK and runtime
    - PostgreSQL client tools
    - Redis client tools
    - Git and development utilities
EOF
            ;;
        build)
            cat << EOF
Build the MCP Hub project

USAGE:
    ./projet.sh build [--clean] [--debug] [--no-restore]

OPTIONS:
    --clean         Force rebuild (equivalent to dotnet build --force)
    --debug         Build in Debug configuration instead of Release
    --no-restore    Skip package restore during build

DESCRIPTION:
    Builds the entire MCP Hub solution including:
    - Domain layer
    - Application services
    - Web applications
    - CLI tools
    - Tests
EOF
            ;;
        test)
            cat << EOF
Run MCP Hub tests

USAGE:
    ./projet.sh test [--filter <filter>] [--collect <collector>] [--logger <logger>]

OPTIONS:
    --filter <filter>       Filter tests to run (e.g., "Name~Domain")
    --collect <collector>   Collect data using specified collector (e.g., "XPlat Code Coverage")
    --logger <logger>       Use specified logger (e.g., "trx", "html")

DESCRIPTION:
    Runs all unit tests for the MCP Hub project:
    - Domain tests
    - Application service tests
    - Web application tests
    - CLI tool tests
    - Integration tests
EOF
            ;;
        run)
            cat << EOF
Start MCP Hub applications

USAGE:
    ./projet.sh run [--environment <env>] [--launch-profile <profile>]

OPTIONS:
    --environment <env>         Set the environment (e.g., "Development", "Production")
    --launch-profile <profile>  Use specified launch profile

DESCRIPTION:
    Starts the MCP Hub applications using .NET Aspire:
    - Web portal
    - Public API
    - CLI tools
    - Supporting services (PostgreSQL, Redis, etc.)
EOF
            ;;
        doctor)
            cat << EOF
Validate development environment

USAGE:
    ./projet.sh doctor

DESCRIPTION:
    Validates the development environment setup:
    - Podman installation
    - Container availability
    - Tool versions
    - Project structure
EOF
            ;;
        *)
            show_help
            ;;
    esac
}

# Execute command
execute_command() {
    local command="$1"
    shift
    
    case "$command" in
        init)
            init_container
            ;;
        build)
            build_project
            ;;
        test)
            run_tests
            ;;
        run)
            run_applications
            ;;
        doctor)
            validate_environment
            ;;
        *)
            log_error "Unknown command: $command"
            exit_with "$GENERAL_ERROR"
            ;;
    esac
}

# Main function
main() {
    parse_options "$@"
    
    log_debug "Executing: Command=$COMMAND, Args=${REMAINING_ARGS[*]}"
    
    execute_command "$COMMAND" "${REMAINING_ARGS[@]}"
    exit_with "$SUCCESS"
}

# Run main function
main "$@"