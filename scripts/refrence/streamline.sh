#!/bin/bash

# Streamline CLI - Comprehensive development workflow script
# Version: 6.0
# Description: Router script that delegates to modular command implementations

set -e  # Exit on any error
set -o pipefail  # Exit on pipe failures

# Change to script directory for correct relative path resolution
cd "$(dirname "${BASH_SOURCE[0]}")"

# shellcheck source=.scripts/helpers/error.sh
source "helpers/error.sh"
# shellcheck source=.scripts/helpers/output.sh
source "helpers/output.sh"
# shellcheck source=.scripts/helpers/log.sh
source "helpers/log.sh"
# shellcheck source=.scripts/helpers/platform.sh
source "helpers/platform.sh"

# shellcheck source=.scripts/commands/base.sh
source "commands/base.sh"

# Initialize script paths (now that normalize_path is available)
SCRIPT_PATH="$(normalize_path "$(pwd)")"
export SCRIPT_PATH

# Basic path configuration
REPOSITORY_ROOT="$(dirname "$SCRIPT_PATH")"
RWP_PATH="${REPOSITORY_ROOT}/RWP/rwp"
CACHE_ROOT="${REPOSITORY_ROOT}/.cache"
export REPOSITORY_ROOT RWP_PATH CACHE_ROOT

# Configuration management sourced conditionally by commands that need it

# shellcheck source=.scripts/helpers/metrics.sh
source "helpers/metrics.sh"
# shellcheck source=.scripts/helpers/validation.sh
source "helpers/validation.sh"
# shellcheck source=.scripts/helpers/container.sh
source "helpers/container.sh"
# shellcheck source=.scripts/helpers/process.sh
source "helpers/process.sh"
# shellcheck source=.scripts/utilities/maven.sh
source "utilities/maven.sh"

# Legacy variables for backward compatibility

# Initialize signal handling with cleanup
setup_signal_handlers
register_cleanup_function "cleanup_timers"

# Add emergency signal handling for container cleanup
emergency_container_cleanup() {
    local running_containers
    running_containers=$(docker ps -q --filter "name=streamline-" 2>/dev/null || true)
    if [[ -n "$running_containers" ]]; then
        echo "Emergency: Stopping running streamline containers..." >&2
        echo "$running_containers" | xargs -r docker stop 2>/dev/null || true
    fi
}

# Register emergency cleanup for various signal scenarios
trap 'emergency_container_cleanup; exit 130' INT
trap 'emergency_container_cleanup; exit 143' TERM

# Initialize container variables for export
container_init

# Main help function
show_help() {
    # If a command is provided, show help for that command
    if [[ "$COMMAND" != "" ]]; then
        __show_command_help "$COMMAND"
        exit_with "$SUCCESS"
    fi

    cat << EOF
Streamline CLI - Ross Streamline Pro Development Tool

USAGE:
    ./streamline.sh [-h|--help]
    ./streamline.sh [global options] <command> [command options]

COMMANDS:
    init        Initialize development container
    build       Build the Streamline project
    run         Start the Streamline application
    test        Execute tests
    doctor      Validate development environment

GLOBAL OPTIONS:
    -h, --help                 Show help information
    -l, --log-level <level>    Set verbosity level: 
                                 3, q, quiet: no-output,
                                 2, m, minimal: show only errors and completed steps,
                                 1, v, verbose: show verbose output (all except debug and metrics),
                                 0, d, debug: show all messages

EXAMPLES:
    ./streamline.sh                   # Show this help
    ./streamline.sh init              # Initialize container
    ./streamline.sh build             # Fast build (auto-detects dependencies)
    ./streamline.sh -l d build        # Fast build (auto-detects dependencies) with detailed output
    ./streamline.sh build -a          # Complete rebuild with frontend
    ./streamline.sh build -r          # Force remote dependencies
    ./streamline.sh test -t SomeClassTests  # Run specific test
    ./streamline.sh test -r           # Build first, then run
    ./streamline.sh test -c           # Generate coverage report
    ./streamline.sh run -r            # Build first, then run
    ./streamline.sh doctor                   # Validate environment

PREREQUISITES:
    - Docker or Podman installed
    - Run './streamline.sh init' first to create development container
    - Corporate DNS configured (/etc/hosts):
      10.0.200.4 srvottdockreg01 srvottdockreg01.rossvideo.com

For command-specific help, use: ./streamline.sh COMMAND --help
EOF
}

# Global variables
COMMAND=""

# Parse global arguments only
__parse_options() {
    # First pass: check for immediate help request
    if [[ $# -eq 0 ]]; then
        show_help
        exit_with "$SUCCESS"
    fi

    # Second pass: parse global options (must come before command)
    while [[ $# -gt 0 ]]; do
        case $1 in
            -l|--log-level)
                set_log_level "$2"
                shift 2
                ;;
            -h|--help)
                # If no command set yet, show main help
                if [[ -z "$COMMAND" ]]; then
                    show_help
                    exit_with "$SUCCESS"
                else
                    # Command-specific help will be handled by the command
                    break
                fi
                ;;
            init|build|run|test|doctor)
                COMMAND="$1"
                shift
                # Check if next argument is help request
                if [[ "$1" == "--help" || "$1" == "-h" ]]; then
                    __show_command_help "$COMMAND"
                    exit_with "$SUCCESS"
                fi
                break
                ;;
            -*)
                log_error "Unknown global option: $1"
                log_info "Use ./streamline.sh --help for available options"
                exit_with "$GENERAL_ERROR"
                ;;
            *)
                log_error "Unknown command: $1"
                log_info "Use ./streamline.sh --help for available commands"
                exit_with "$GENERAL_ERROR"
                ;;
        esac
    done

    # If no command is provided, show help
    if [[ -z "$COMMAND" ]]; then
        show_help
        exit_with "$SUCCESS"
    fi

    # Return remaining arguments for command processing
    REMAINING_ARGS=("$@")
}

# Route help requests to appropriate command
__show_command_help() {
    local command="$1"

    case "$command" in
        init)
            # shellcheck source=.scripts/commands/init.sh
            source "commands/init.sh"
            show_help
            ;;
        build)
            # shellcheck source=.scripts/commands/build.sh
            source "commands/build.sh"
            show_help
            ;;
        run)
            # shellcheck source=.scripts/commands/run.sh
            source "commands/run.sh"
            show_help
            ;;
        test)
            # shellcheck source=.scripts/commands/test.sh
            source "commands/test.sh"
            show_help
            ;;
        doctor)
            # shellcheck source=.scripts/commands/doctor.sh
            source "commands/doctor.sh"
            show_help
            ;;
        *)
            show_help
            ;;
    esac
}

main() {
    __parse_options "$@"
    
    log_debug "Executing: Command=$COMMAND, Verbose Level=$LOG_LEVEL"
    
    export SCRIPT_PATH REPOSITORY_ROOT RWP_PATH CACHE_ROOT CONTAINER_ENGINE CONTAINER_IMAGE
    case "$COMMAND" in
        init)
            # shellcheck source=.scripts/commands/init.sh
            source "commands/init.sh"
            ;;
        build)
            # shellcheck source=.scripts/commands/build.sh
            source "commands/build.sh"
            ;;
        run)
            # shellcheck source=.scripts/commands/run.sh
            source "commands/run.sh"
            ;;
        test)
            # shellcheck source=.scripts/commands/test.sh
            source "commands/test.sh"
            ;;
        doctor)
            # shellcheck source=.scripts/commands/doctor.sh
            source "commands/doctor.sh"
            ;;
        *)
            log_error "Unknown command: $COMMAND"
            log_info "Use ./streamline.sh --help for available commands"
            exit_with "$GENERAL_ERROR"
            ;;
    esac
    execute_command "$COMMAND" "${REMAINING_ARGS[@]}"
    exit_with "$SUCCESS"
}

main "$@"