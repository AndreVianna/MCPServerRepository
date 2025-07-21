#!/bin/bash

# project.sh - MCP Hub project CLI tool
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

# Project dependency mapping (in build order)
declare -a PROJECT_BUILD_ORDER=(
    "Domain"
    "Core"
    "Common"
    "Storage"
    "Data"
    "Data.MigrationService"
    "AuthenticationService"
    "SecurityService"
    "SearchService"
    "CommandLineApp"
    "PublicApi"
    "WebApp"
    "AppHost"
)

# Project path mapping
declare -A PROJECT_PATHS=(
    ["Domain"]="Source/Domain/MCPHub.Domain.csproj"
    ["Core"]="Source/Core/MCPHub.Core.csproj"
    ["Common"]="Source/Common/MCPHub.Common.csproj"
    ["Storage"]="Source/Data/MCPHub.Storage.csproj"
    ["Data"]="Source/Data/MCPHub.Data.csproj"
    ["Data.MigrationService"]="Source/Data.MigrationService/MCPHub.Data.MigrationService.csproj"
    ["SecurityService"]="Source/SecurityService/MCPHub.SecurityService.csproj"
    ["AuthenticationService"]="Source/AuthenticationService/MCPHub.AuthenticationService.csproj"
    ["CommandLineApp"]="Source/CommandLineApp/MCPHub.CommandLineApp.csproj"
    ["SearchService"]="Source/SearchService/MCPHub.SearchService.csproj"
    ["CommandLineApp"]="Source/CommandLineApp/MCPHub.CommandLineApp.csproj"
    ["PublicApi"]="Source/PublicApi/MCPHub.PublicApi.csproj"
    ["WebApp"]="Source/WebApp/MCPHub.WebApp.csproj"
    ["AppHost"]="Source/AppHost/MCPHub.AppHost.csproj"
    ["Domain.UnitTests"]="Source/Domain.UnitTests/MCPHub.Domain.UnitTests.csproj"
    ["Core.UnitTests"]="Source/Core.UnitTests/MCPHub.Core.UnitTests.csproj"
    ["Common.UnitTests"]="Source/Common.UnitTests/MCPHub.Common.UnitTests.csproj"
    ["Data.UnitTests"]="Source/Data.UnitTests/MCPHub.Data.UnitTests.csproj"
)

# Test project path mapping
declare -A TEST_PROJECT_PATHS=(
    ["Domain"]="Source/Domain.UnitTests/MCPHub.Domain.UnitTests.csproj"
    ["Core"]="Source/Core.UnitTests/MCPHub.Core.UnitTests.csproj"
    ["Common"]="Source/Common.UnitTests/MCPHub.Common.UnitTests.csproj"
    ["Data"]="Source/Data.UnitTests/MCPHub.Data.UnitTests.csproj"
)

# Project display names
declare -A PROJECT_DISPLAY_NAMES=(
    ["Domain"]="MCPHub.Domain"
    ["Core"]="MCPHub.Core"
    ["Common"]="MCPHub.Common"
    ["Storage"]="MCPHub.Storage"
    ["Data"]="MCPHub.Data"
    ["Data.MigrationService"]="MCPHub.Data.MigrationService"
    ["AuthenticationService"]="MCPHub.AuthenticationService"
    ["SecurityService"]="MCPHub.SecurityService"
    ["SearchService"]="MCPHub.SearchService"
    ["CommandLineApp"]="MCPHub.CommandLineApp"
    ["PublicApi"]="MCPHub.PublicApi"
    ["WebApp"]="MCPHub.WebApp"
    ["AppHost"]="MCPHub.AppHost"
    ["Domain.UnitTests"]="MCPHub.Domain.UnitTests"
    ["Core.UnitTests"]="MCPHub.Core.UnitTests"
    ["Common.UnitTests"]="MCPHub.Common.UnitTests"
    ["Data.UnitTests"]="MCPHub.Data.UnitTests"
    ["Data.MigrationService.UnitTests"]="MCPHub.Data.MigrationService.UnitTests"
    ["CommandLineApp.UnitTests"]="MCPHub.CommandLineApp.UnitTests"
    ["PublicApi.UnitTests"]="MCPHub.PublicApi.UnitTests"
    ["SecurityService.UnitTests"]="MCPHub.SecurityService.UnitTests"
    ["SearchService.UnitTests"]="MCPHub.SearchService.UnitTests"
    ["WebApp.UnitTests"]="MCPHub.WebApp.UnitTests"
)

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
MCP Hub Project CLI Tool - project.sh

USAGE:
    ./project.sh [-h|--help] [global options] <command> [command options]

COMMANDS:
    init        Initialize development container
    build       Build the MCP Hub project
    test        Execute tests
    lint        Format code using dotnet format
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
    ./project.sh init                                    # Initialize container
    ./project.sh list                                    # List available projects
    ./project.sh build                                   # Build MCP Hub solution
    ./project.sh build --clean                           # Clean release build
    ./project.sh build --project Domain                  # Build Domain project only
    ./project.sh build --project Domain.UnitTests        # Build Domain unit tests only
    ./project.sh test                                    # Run all tests
    ./project.sh test --project Core                     # Run Core project tests only
    ./project.sh test --filter "Name~Domain"             # Run domain tests only
    ./project.sh test --collect "XPlat Code Coverage"    # Run tests with coverage
    ./project.sh lint                                    # Format entire solution
    ./project.sh lint --project Domain                   # Format Domain project only
    ./project.sh lint --verify                           # Verify formatting without changes
    ./project.sh run                                     # Start the applications
    ./project.sh run --environment Development           # Start in dev environment
    ./project.sh doctor                                  # Validate environment

PREREQUISITES:
    - Podman installed and configured
    - Run './project.sh init' first to create development container
    - .NET 9 SDK and runtime (handled by container)
    - PostgreSQL, Redis, Elasticsearch (handled by .NET Aspire)

For command-specific help, use: ./project.sh COMMAND --help
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

# List all available projects
list_projects() {
    log_info "Available MCP Hub projects:"
    echo

    for i in "${!PROJECT_BUILD_ORDER[@]}"; do
        local project="${PROJECT_BUILD_ORDER[$i]}"
        local display_name="${PROJECT_DISPLAY_NAMES[$project]}"
        local project_path="${PROJECT_PATHS[$project]}"
        printf "  %2d. %-30s (%s)\n" "$((i+1))" "$display_name" "$project_path"
    done
    echo
}

# Validate project name
validate_project() {
    local project="$1"
    if [[ -z "${PROJECT_PATHS[$project]:-}" ]]; then
        log_error "Unknown project: $project"
        log_info "Available projects:"
        for p in "${PROJECT_BUILD_ORDER[@]}"; do
            log_info "  - $p"
        done
        return 1
    fi

    local project_path="${PROJECT_PATHS[$project]}"
    if [[ ! -f "$PROJECT_ROOT/$project_path" ]]; then
        log_error "Project file not found: $PROJECT_ROOT/$project_path"
        return 1
    fi

    return 0
}

# Build individual project
build_single_project() {
    local project="$1"
    local config="${BUILD_CONFIG:-Debug}"

    # Validate project
    if ! validate_project "$project"; then
        exit_with "$VALIDATION_ERROR"
    fi

    local project_path="${PROJECT_PATHS[$project]}"
    local display_name="${PROJECT_DISPLAY_NAMES[$project]}"

    log_info "Building $display_name..."

    # Ensure container exists
    if ! check_container_exists; then
        log_info "Container not found, initializing..."
        init_container
    fi

    # Set build arguments
    local build_args=("dotnet" "build" "$project_path" "--configuration" "$config")

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

    # Build the project
    log_info "Building $display_name with configuration: $config"
    if run_container_async "${build_args[@]}"; then
        log_success "$display_name built successfully"
    else
        log_error "Failed to build $display_name"
        return $?
    fi
}

# Build the MCP Hub project
build_project() {
    # Check if we should build a specific project
    if [[ -n "${BUILD_PROJECT:-}" ]]; then
        build_single_project "$BUILD_PROJECT"
        return
    fi

    # Default: build entire solution
    log_info "Building MCP Hub solution..."

    # Ensure container exists
    if ! check_container_exists; then
        log_info "Container not found, initializing..."
        init_container
    fi

    # Set build configuration
    local config="${BUILD_CONFIG:-Debug}"
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
    if run_container_async "${build_args[@]}"; then
        log_success "MCP Hub solution built successfully"
    else
        log_error "Failed to build MCP Hub solution"
        return $?
    fi
}

# Test individual project
test_single_project() {
    local project="$1"

    # Validate project
    if ! validate_project "$project"; then
        exit_with "$VALIDATION_ERROR"
    fi

    # Check if test project exists
    if [[ -z "${TEST_PROJECT_PATHS[$project]:-}" ]]; then
        log_error "No test project found for: $project"
        return "$VALIDATION_ERROR"
    fi

    local test_project_path="${TEST_PROJECT_PATHS[$project]}"
    local display_name="${PROJECT_DISPLAY_NAMES[$project]}"

    log_info "Running tests for $display_name..."

    # Ensure container exists
    if ! check_container_exists; then
        log_info "Container not found, initializing..."
        init_container
    fi

    # Set test configuration
    local config="${BUILD_CONFIG:-Debug}"
    local test_args=("dotnet" "test" "$test_project_path" "--configuration" "$config" "--no-build")

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
        local log_level="$(get_log_level)"
        case $log_level in
            0)
                test_args+=("--verbosity" "detailed")
                ;;
            1)
                test_args+=("--verbosity" "normal")
                ;;
            2)
                test_args+=("--verbosity" "minimal")
                ;;
            *)
                test_args+=("--verbosity" "quiet")
                ;;
        esac
    fi

    # Run tests for the project
    log_info "Running unit tests for $display_name with configuration: $config"
    log_info "Test command: ${test_args[*]}"
    if run_container_async "${test_args[@]}"; then
        log_success "$display_name tests completed successfully"
    else
        log_error "$display_name tests failed"
        return $?
    fi
}

# Run tests
run_tests() {
    # Check if we should test a specific project
    if [[ -n "${TEST_PROJECT:-}" ]]; then
        test_single_project "$TEST_PROJECT"
        return
    fi

    log_info "Running MCP Hub tests..."

    # Ensure container exists
    if ! check_container_exists; then
        log_info "Container not found, initializing..."
        init_container
    fi

    # Set test configuration
    local config="${BUILD_CONFIG:-Debug}"
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
    if run_container_async "${test_args[@]}"; then
        log_success "All tests completed successfully"
    else
        log_error "Tests failed"
        return $?
    fi
}

# Format code using dotnet format
lint_project() {
    # Check if we should lint a specific project
    if [[ -n "${LINT_PROJECT:-}" ]]; then
        lint_single_project "$LINT_PROJECT"
        return
    fi

    # Default: lint entire solution
    log_info "Formatting MCP Hub solution..."

    # Ensure container exists
    if ! check_container_exists; then
        log_info "Container not found, initializing..."
        init_container
    fi

    # Set lint arguments
    local lint_args=("dotnet" "format" "Source/MCPHub.sln")

    # Add verify flag if specified
    if [[ "${LINT_VERIFY:-false}" == "true" ]]; then
        lint_args+=("--verify-no-changes")
        log_info "Verifying code formatting without making changes..."
    else
        log_info "Formatting code with .editorconfig rules..."
    fi

    # Add verbosity if specified
    if [[ -n "${LINT_VERBOSITY:-}" ]]; then
        lint_args+=("--verbosity" "$LINT_VERBOSITY")
    fi

    # Format the solution
    log_info "Running dotnet format on solution..."
    if [[ "${LINT_VERIFY:-false}" == "true" ]]; then
        # Verify-only mode: check formatting without fixing
        if run_container_async "${lint_args[@]}"; then
            log_success "Code formatting verification completed"
        else
            log_error "Code formatting verification failed"
            return $?
        fi
    else
        # Format mode: check first, then fix if needed
        local verify_args=("dotnet" "format" "Source/MCPHub.sln" "--verify-no-changes")
        if [[ -n "${LINT_VERBOSITY:-}" ]]; then
            verify_args+=("--verbosity" "$LINT_VERBOSITY")
        fi

        # First check if formatting is needed (suppress output for cleaner UX)
        local verify_exit_code=0
        if ! run_container_async "${verify_args[@]}" >/dev/null 2>&1; then
            verify_exit_code=$?
        fi

        # Apply formatting and capture output
        local format_output
        local format_exit_code=0

        # Run format command and capture output
        format_output=$(run_container_async "${lint_args[@]}" 2>&1)
        format_exit_code=$?

        # Display the format output to the user
        if [[ -n "$format_output" ]]; then
            echo "$format_output"
        fi

        # Check if format command succeeded
        if [[ $format_exit_code -eq 0 ]]; then
            # Format command succeeded, but check for unfixable issues
            if echo "$format_output" | grep -q "Unable to fix"; then
                # Found unfixable issues in output
                log_error "Solution has formatting issues that could not be fixed"
                return 1
            elif [[ $verify_exit_code -eq 1 ]]; then
                # Issues were found initially, check if they're all fixed now
                if run_container_async "${verify_args[@]}" >/dev/null 2>&1; then
                    # All issues were fixed
                    log_error "Solution had formatting issues that were fixed"
                    return 1
                else
                    # Some issues remain unfixed
                    log_error "Solution has formatting issues that could not be fixed"
                    return 1
                fi
            else
                # No issues found
                log_success "Code formatting completed successfully"
            fi
        else
            # Format command failed
            log_error "Code formatting failed"
            return $format_exit_code
        fi
    fi
}

# Format individual project
lint_single_project() {
    local project="$1"

    # Validate project
    if ! validate_project "$project"; then
        exit_with "$VALIDATION_ERROR"
    fi

    local project_path="${PROJECT_PATHS[$project]}"
    local display_name="${PROJECT_DISPLAY_NAMES[$project]}"

    log_info "Formatting $display_name..."

    # Ensure container exists
    if ! check_container_exists; then
        log_info "Container not found, initializing..."
        init_container
    fi

    # Set lint arguments
    local lint_args=("dotnet" "format" "$project_path")

    # Add verify flag if specified
    if [[ "${LINT_VERIFY:-false}" == "true" ]]; then
        lint_args+=("--verify-no-changes")
        log_info "Verifying formatting for $display_name..."
    else
        log_info "Formatting $display_name with .editorconfig rules..."
    fi

    # Add verbosity if specified
    if [[ -n "${LINT_VERBOSITY:-}" ]]; then
        lint_args+=("--verbosity" "$LINT_VERBOSITY")
    fi

    # Format the project
    if [[ "${LINT_VERIFY:-false}" == "true" ]]; then
        # Verify-only mode: check formatting without fixing
        if run_container_async "${lint_args[@]}"; then
            log_success "$display_name formatting verification completed"
        else
            log_error "$display_name formatting verification failed"
            return $?
        fi
    else
        # Format mode: check first, then fix if needed
        local verify_args=("dotnet" "format" "$project_path" "--verify-no-changes")
        if [[ -n "${LINT_VERBOSITY:-}" ]]; then
            verify_args+=("--verbosity" "$LINT_VERBOSITY")
        fi

        # First check if formatting is needed (suppress output for cleaner UX)
        local verify_exit_code=0
        if ! run_container_async "${verify_args[@]}" >/dev/null 2>&1; then
            verify_exit_code=$?
        fi

        # Apply formatting and capture output
        local format_output
        local format_exit_code=0

        # Run format command and capture output
        format_output=$(run_container_async "${lint_args[@]}" 2>&1)
        format_exit_code=$?

        # Display the format output to the user
        if [[ -n "$format_output" ]]; then
            echo "$format_output"
        fi

        # Check if format command succeeded
        if [[ $format_exit_code -eq 0 ]]; then
            # Format command succeeded, but check for unfixable issues
            if echo "$format_output" | grep -q "Unable to fix"; then
                # Found unfixable issues in output
                log_error "$display_name has formatting issues that could not be fixed"
                return 1
            elif [[ $verify_exit_code -eq 1 ]]; then
                # Issues were found initially, check if they're all fixed now
                if run_container_async "${verify_args[@]}" >/dev/null 2>&1; then
                    # All issues were fixed
                    log_error "$display_name had formatting issues that were fixed"
                    return 1
                else
                    # Some issues remain unfixed
                    log_error "$display_name has formatting issues that could not be fixed"
                    return 1
                fi
            else
                # No issues found
                log_success "$display_name formatted successfully"
            fi
        else
            # Format command failed
            log_error "$display_name formatting failed"
            return $format_exit_code
        fi
    fi
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
    local config="${BUILD_CONFIG:-Debug}"
    local run_args=("dotnet" "run" "--project" "Source/AppHost/MCPHub.AppHost.csproj" "--configuration" "$config")

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
    if run_container_async "${run_args[@]}"; then
        log_success "MCP Hub applications started successfully"
    else
        log_error "Failed to start MCP Hub applications"
        return $?
    fi
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
        log_info "Run './project.sh init' to create the container"
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
                else
                    show_command_help "$COMMAND"
                fi
                exit_with "$SUCCESS"
                break
                ;;
            --force-rebuild)
                FORCE_REBUILD="true"
                shift
                ;;
            --clean)
                BUILD_CLEAN="true"
                shift
                ;;
            --release)
                BUILD_CONFIG="Release"
                shift
                ;;
            --no-restore)
                NO_RESTORE="true"
                shift
                ;;
            --project)
                BUILD_PROJECT="$2"
                LINT_PROJECT="$2"
                TEST_PROJECT="$2"
                shift 2
                ;;
            --tree)
                PROJECT_TREE="true"
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
            --verify)
                LINT_VERIFY="true"
                shift
                ;;
            --verbosity)
                LINT_VERBOSITY="$2"
                shift 2
                ;;
            init|list|build|test|lint|run|doctor)
                COMMAND="$1"
                shift
                ;;
            -*)
                log_error "Unknown global option: $1"
                log_info "Use ./project.sh --help for available options"
                exit_with "$GENERAL_ERROR"
                ;;
            *)
                log_error "Unknown command: $1"
                log_info "Use ./project.sh --help for available commands"
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
        list)
            cat << EOF
List available projects

USAGE:
    ./project.sh list

OPTIONS:
    --tree    Show projects in the dependency tree order

DESCRIPTION:
    Lists all available projects

EXAMPLES:
    ./project.sh list
    ./project.sh list --tree
EOF
            ;;
        init)
            cat << EOF
Initialize development container

USAGE:
    ./project.sh init [--force-rebuild]

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
    ./project.sh build [--clean] [--release] [--no-restore] [--project <name>]

OPTIONS:
    --clean                Force rebuild (equivalent to dotnet build --force)
    --release              Build in Release configuration instead of Debug
    --no-restore           Skip package restore during build
    --project <name>       Build specific project only

EXAMPLES:
    ./project.sh build                           # Build entire solution
    ./project.sh build --project Domain         # Build Domain project only
    ./project.sh build --project Common --release # Build Common project in release

DESCRIPTION:
    Builds the MCP Hub solution or individual projects:
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
    ./project.sh test [--project <name>] [--filter <filter>] [--collect <collector>] [--logger <logger>]

OPTIONS:
    --project <name>        Test specific project only
    --filter <filter>       Filter tests to run (e.g., "Name~Domain")
    --collect <collector>   Collect data using specified collector (e.g., "XPlat Code Coverage")
    --logger <logger>       Use specified logger (e.g., "trx", "html")

EXAMPLES:
    ./project.sh test                           # Run all tests
    ./project.sh test --project Core            # Run Core project tests only
    ./project.sh test --project Domain         # Run Domain project tests only

DESCRIPTION:
    Runs all unit tests for the MCP Hub project:
    - Domain tests
    - Application service tests
    - Web application tests
    - CLI tool tests
    - Integration tests
EOF
            ;;
        lint)
            cat << EOF
Format code using dotnet format

USAGE:
    ./project.sh lint [--project <name>] [--verify] [--verbosity <level>]

OPTIONS:
    --project <name>        Format specific project only
    --verify                Verify formatting without making changes
    --verbosity <level>     Set verbosity level (quiet, minimal, normal, detailed, diagnostic)

DESCRIPTION:
    Formats code using dotnet format with .editorconfig rules:
    - Applies consistent code formatting
    - Enforces coding standards
    - Uses .editorconfig configuration
    - Can format entire solution or specific projects

EXAMPLES:
    ./project.sh lint                           # Format entire solution
    ./project.sh lint --project Domain         # Format Domain project only
    ./project.sh lint --verify                 # Check formatting without changes
    ./project.sh lint --verbosity detailed     # Format with detailed output
EOF
            ;;
        run)
            cat << EOF
Start MCP Hub applications

USAGE:
    ./project.sh run [--environment <env>] [--launch-profile <profile>]

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
    ./project.sh doctor

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
        list)
            list_projects
            ;;
        build)
            build_project
            ;;
        test)
            run_tests
            ;;
        lint)
            lint_project
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

    if execute_command "$COMMAND" "${REMAINING_ARGS[@]}"; then
        exit_with "$SUCCESS"
    else
        local exit_code=$?
        # Don't show additional error messages for commands that handle their own error reporting
        exit "$exit_code"
    fi
}

# Run main function
main "$@"
