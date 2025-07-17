#!/bin/bash

# error.sh - Standard exit codes
# Version: 1.0
# Description: Provides standardized exit codes for consistent error handling across projects

if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    echo "Error: error.sh should be sourced, not executed directly"
    exit 1
fi
if [[ -n "${ERROR_HELPER:-}" ]]; then
    return 0
fi
export ERROR_HELPER=1

readonly __MAXIMUM_CODE=255
readonly __FATAL_SIGNAL_BASE=192
readonly SUCCESS=0
readonly GENERAL_ERROR=1

# Custom application exit codes
readonly UNAUTHORIZED=8               # Permission denied
readonly USAGE_ERROR=9                # Command line usage error
readonly NO_INPUT_ERROR=10            # Cannot open input
readonly DATA_FORMAT_ERROR=11         # Data format error
readonly VALIDATION_ERROR=12          # Input validation failed
readonly SOFTWARE_ERROR=13            # Internal software error
readonly NOT_SUPPORTED_PLATFORM=14    # OS not supported
readonly CANNOT_CREATE_FOLDER=15      # Can't create (user) output file
readonly INVALID_CONFIGURATION=16     # Configuration error
readonly PREREQUISITE_ERROR=17        # Prerequisites not met
readonly REMOTE_CONNECTION_ERROR=18   # Network connectivity issue
readonly TIMEOUT_ERROR=19             # Operation timed out
readonly CONTAINER_ERROR=20           # Container runtime error
readonly COMMAND_NOT_FOUND=21         # Command invoked cannot be found

# Command specific exit codes (reseve 10 codes for each command)
readonly LINT_ERROR=50                # Linting/code quality check failed
readonly BUILD_ERROR=60               # Build/compilation failed
readonly TEST_ERROR=70                # Test execution failed
readonly RUN_ERROR=80                 # Application run failed

# User interrupt signal code
readonly USER_INTERRUPT=128           # Script terminated by Ctrl+C


# Function to get human-readable error description
show_error_message() {
    local exit_code="$1"
    
    # Input validation
    if ! [[ "$exit_code" =~ ^[0-9]+$ ]]; then
        echo "Unknown error: $exit_code"
    fi
    
    case "$exit_code" in
        "$SUCCESS")                     echo "Success" ;;
        "$GENERAL_ERROR")               echo "General error" ;;
        "$UNAUTHORIZED")                echo "Permission denied" ;;
        "$USAGE_ERROR")                 echo "Command line usage error" ;;
        "$NO_INPUT_ERROR")              echo "Cannot open input" ;;
        "$DATA_FORMAT_ERROR")           echo "Data format error" ;;
        "$VALIDATION_ERROR")            echo "Input validation failed" ;;
        "$SOFTWARE_ERROR")              echo "Internal software error" ;;
        "$NOT_SUPPORTED_PLATFORM")      echo "OS not supported" ;;
        "$CANNOT_CREATE_FOLDER")        echo "Can't create output file" ;;
        "$INVALID_CONFIGURATION")       echo "Configuration error" ;;
        "$PREREQUISITE_ERROR")          echo "Prerequisites not met" ;;
        "$REMOTE_CONNECTION_ERROR")     echo "Network connectivity issue" ;;
        "$TIMEOUT_ERROR")               echo "Operation timed out" ;;
        "$CONTAINER_ERROR")             echo "Container runtime error" ;;
        "$COMMAND_NOT_FOUND")           echo "Command not found" ;;
        "$BUILD_ERROR")                 echo "Build/compilation failed" ;;
        "$TEST_ERROR")                  echo "Test execution failed" ;;
        "$LINT_ERROR")                  echo "Linting/code quality check failed" ;;
        "$RUN_ERROR")                   echo "Application run failed" ;;
        "$USER_INTERRUPT")              echo "User interrupt (Ctrl+C)" ;;
        *)
            if [[ "$exit_code" -gt "$__FATAL_SIGNAL_BASE" && "$exit_code" -le "$__MAXIMUM_CODE" ]]; then
                echo "Fatal signal detected: $exit_code."
            else
                echo "Unknown error: $exit_code."
            fi
            ;;
    esac
}

# Function to exit with error code and optional message
exit_with() {
    local exit_code="$1"
    
    # Input validation for exit code
    if [[ -z "$exit_code" ]]; then
        exit_code="$SUCCESS"
    fi
    
    if [[ "$exit_code" -ne "$SUCCESS" ]]; then
        show_error_message "$exit_code"
    fi
    exit "$exit_code"
}

ensure_success() {
    local exit_code="$1"
    if [[ "$exit_code" -eq "$SUCCESS" ]]; then
        exit_with "$exit_code"
    fi
}
