#!/bin/bash

# validation.sh - Validation and testing utility functions
# Version: 1.0
# Description: Provides comprehensive validation functions for tools, infrastructure, and prerequisites

if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    echo "Error: validation-helper.sh should be sourced, not executed directly"
    exit 1
fi
if [[ -n "${VALIDATION_HELPER:-}" ]]; then
    return 0
fi
export VALIDATION_HELPER=1

# shellcheck source=.scripts/helpers/error.sh
source "helpers/error.sh"
# shellcheck source=.scripts/helpers/output.sh
source "helpers/output.sh"
# shellcheck source=.scripts/helpers/log.sh
source "helpers/log.sh"
# shellcheck source=.scripts/helpers/platform.sh
source "helpers/platform.sh"

VALIDATION_TIMEOUT="${VALIDATION_TIMEOUT:-30}"
VALIDATION_VERBOSE="${VALIDATION_VERBOSE:-false}"
VALIDATION_FAIL_FAST="${VALIDATION_FAIL_FAST:-false}"

__validate_single_tool() {
    local tool_name="$1"
    local pattern="$2"
    local success_message="$3"
    local validation_output="$4"
    local is_required="${5:-true}"
    
    if [[ -z "$tool_name" || -z "$pattern" ]]; then
        log_error "Tool name and pattern required for validation"
        return "$USAGE_ERROR"
    fi
    
    if [[ -z "$success_message" ]]; then
        log_error "Success message required for validation"
        return "$USAGE_ERROR"
    fi
    
    if [[ -z "$validation_output" ]]; then
        log_error "Validation output required for tool validation"
        return "$USAGE_ERROR"
    fi
    
    if echo "$validation_output" | grep -q "$pattern"; then
        log_success "$success_message"
        return "$SUCCESS"
    else
        if [[ "$is_required" == "true" ]]; then
            log_error "$tool_name validation failed"
            if [[ "$VALIDATION_FAIL_FAST" == "true" ]]; then
                return "$VALIDATION_ERROR"
            fi
            return "$VALIDATION_ERROR"
        else
            log_warning "$tool_name not found (optional)"
            return "$SUCCESS"
        fi
    fi
}

__show_validation_result() {
    local test_name="$1"
    local result="$2"
    local message="$3"
    
    case "$result" in
        "success"|"pass"|"0")
            output_check "$test_name: $message"
            ;;
        "warning"|"warn")
            output_warning "$test_name: $message"
            ;;
        "error"|"fail"|*)
            output_cross "$test_name: $message"
            ;;
    esac
}

__format_validation_results() {
    local validation_output="$1"
    local validation_title="${2:-Tool Validation Results}"
    
    if [[ "$VALIDATION_VERBOSE" == "true" ]]; then
        log_info "$validation_title:"
        echo "$validation_output" | while IFS= read -r line; do
            if [[ -n "$line" ]]; then
                log_info "  $line"
            fi
        done
    fi
}

set_validation_config() {
    local key="$1"
    local value="$2"
    
    case "$key" in
        "timeout")
            VALIDATION_TIMEOUT="$value"
            ;;
        "verbose")
            VALIDATION_VERBOSE="$value"
            ;;
        "fail_fast")
            VALIDATION_FAIL_FAST="$value"
            ;;
        *)
            log_error "Unknown validation configuration key: $key"
            return "$USAGE_ERROR"
            ;;
    esac
}

validate_with_timeout() {
    local timeout="$1"
    local description="$2"
    shift 2
    local validation_function="$1"
    local validation_args=("${@:2}")
    
    log_info "Running validation with timeout: $description (${timeout}s)"
    
    # Run validation in background
    "$validation_function" "${validation_args[@]}" &
    local validation_pid=$!
    
    # Wait with timeout
    local elapsed=0
    while kill -0 "$validation_pid" 2>/dev/null && [[ $elapsed -lt $timeout ]]; do
        sleep 1
        ((elapsed++))
    done
    
    if kill -0 "$validation_pid" 2>/dev/null; then
        # Validation still running - kill it
        kill -TERM "$validation_pid" 2>/dev/null
        wait "$validation_pid" 2>/dev/null || true
        log_error "Validation timed out: $description"
        return "$TIMEOUT_ERROR"
    else
        # Validation completed - get result
        wait "$validation_pid"
        return $?
    fi
}