#!/bin/bash

# log.sh - Structured logging utility functions
# Version: 1.0
# Description: Provides structured logging with levels, timestamps, and context using output.sh foundation

if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    echo "Error: log.sh should be sourced, not executed directly"
    exit 1
fi
if [[ -n "${LOG_HELPER:-}" ]]; then
    return 0
fi
export LOG_HELPER=1

# shellcheck source=.scripts/helpers/error.sh
source "helpers/error.sh"
# shellcheck source=.scripts/helpers/output.sh
source "helpers/output.sh"

_TIMESTAMP_FORMAT="%Y-%m-%d_%H:%M:%S"

readonly LOG_LEVEL_DEBUG=0
readonly LOG_LEVEL_VERBOSE=1
readonly LOG_LEVEL_MINIMAL=2
readonly LOG_LEVEL_QUIET=3

MINIMUM_LOG_LEVEL="${MINIMUM_LOG_LEVEL:-$LOG_LEVEL_MINIMAL}"

# Set logging verbosity level
set_log_level() {
    local level="$1"
    case "$level" in
        "silent"|"quiet"|"3")
            MINIMUM_LOG_LEVEL="$LOG_LEVEL_QUIET"
            ;;
        "minimal"|"m"|"2")
            MINIMUM_LOG_LEVEL="$LOG_LEVEL_MINIMAL"
            ;;
        "verbose"|"v"|"1")
            MINIMUM_LOG_LEVEL="$LOG_LEVEL_VERBOSE"
            ;;
        "debug"|"d"|"0")
            MINIMUM_LOG_LEVEL="$LOG_LEVEL_DEBUG"
            ;;
        *)
            echo "ERROR: Invalid log level: $level. Use: 0=debug, 1=verbose, 2=minimal, 3=quiet" >&2
            return 1
            ;;
    esac
    export MINIMUM_LOG_LEVEL
}

get_log_level() {
    echo "$MINIMUM_LOG_LEVEL"
}

__log() {
    local level="$1"
    local type="$2"
    local message="$3"
    local context="${4:-$LOG_CONTEXT}"
    
    if [[ "$level" -lt "$MINIMUM_LOG_LEVEL" ]]; then
        return 0
    fi
    
    local formatted_message="$message"

    if [[ -n "$context" ]]; then
        formatted_message="$context => $formatted_message"
    fi

    case "$type" in
        "trace")
            formatted_message="[${_DIM}TRACE${_NO_COLOR}] $formatted_message"
            ;;
        "debug")
            formatted_message="[${_PURPLE}DEBUG${_NO_COLOR}] $formatted_message"
            ;;
        "info")
            formatted_message="[${_CYAN}INFO${_NO_COLOR}] $formatted_message"
            ;;
        "warning")
            formatted_message="[${_YELLOW}WARNING${_NO_COLOR}] $formatted_message"
            ;;
        "success")
            formatted_message="[${_GREEN}SUCCESS${_NO_COLOR}] $formatted_message"
            ;;
        "error")
            formatted_message="[${_RED}ERROR${_NO_COLOR}] $formatted_message"
            ;;
        "metric")
            formatted_message="[${_BLUE}METRIC${_NO_COLOR}] $formatted_message"
            ;;
        *)
            formatted_message="[$type] $formatted_message"
            ;;
    esac

    local timestamp
    timestamp="$(date +"$_TIMESTAMP_FORMAT")"
    formatted_message="$timestamp $formatted_message"
    
    echo -e "$formatted_message" >&2
    
    # Output to file if specified
    if [[ -n "$LOG_FILE" ]]; then
        if ! echo "$(date +"$_TIMESTAMP_FORMAT") [$context] $message" >> "$LOG_FILE" 2>/dev/null; then
            # Fallback to stderr if log file write fails
            echo "WARNING: Failed to write to log file: $LOG_FILE" >&2
        fi
    fi
}

log_trace() {
    local message="$1"
    local context="${2:-$LOG_CONTEXT}"
    __log "0" "trace" "$1" "$2"
}

log_debug() {
    local message="$1"
    local context="${2:-$LOG_CONTEXT}"
    __log "0" "debug" "$1" "$2"
}

log_info() {
    local message="$1"
    local context="${2:-$LOG_CONTEXT}"
    __log "1" "info" "$1" "$2"
}

log_warning() {
    local message="$1"
    local context="${2:-$LOG_CONTEXT}"
    __log "1" "warning" "$1" "$2"
}

log_success() {
    local message="$1"
    local context="${2:-$LOG_CONTEXT}"
    __log "2" "success" "$1" "$2"
}

log_error() {
    local message="$1"
    local context="${2:-$LOG_CONTEXT}"
    __log "2" "error" "$1" "$2"
}

log_metric() {
    local message="$1"
    local context="${2:-$LOG_CONTEXT}"
    __log "0" "metric" "$1" "$2"
}
