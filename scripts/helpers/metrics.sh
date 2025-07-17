#!/bin/bash

# metrics.sh - Metric helper utility functions
# Version: 1.0
# Description: Provides standardized timing operations and performance metrics

if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    echo "Error: metrics.sh should be sourced, not executed directly"
    exit 1
fi
if [[ -n "${METRICS_HELPER:-}" ]]; then
    return 0
fi
export METRICS_HELPER=1

# shellcheck source=.scripts/helpers/error.sh
source "helpers/error.sh"
# shellcheck source=.scripts/helpers/output.sh
source "helpers/output.sh"
# shellcheck source=.scripts/helpers/log.sh
source "helpers/log.sh"

# Timer storage - support multiple named timers
declare -A TIMER_START_TIMES
declare -A TIMER_DESCRIPTIONS
TIMER_DEFAULT_NAME="default"

# Basic timer functions

# Start a timer
start_timer() {
    local timer_name="${1:-$TIMER_DEFAULT_NAME}"
    local description="${2:-Timer}"
    
    TIMER_START_TIMES["$timer_name"]=$(date +%s.%3N)
    TIMER_DESCRIPTIONS["$timer_name"]="$description"
    
    log_debug "Started timer '$timer_name': $description"
}

# End a timer and log the duration
end_timer() {
    local timer_name="${1:-$TIMER_DEFAULT_NAME}"
    
    if [[ -z "${TIMER_START_TIMES[$timer_name]:-}" ]]; then
        log_warning "Timer '$timer_name' was not started"
        return "$USAGE_ERROR"
    fi
    
    local start_time="${TIMER_START_TIMES[$timer_name]}"
    local end_time
    end_time=$(date +%s.%3N)
    local duration
    duration=$(echo "$end_time - $start_time" | bc -l 2>/dev/null || echo "0")
    
    local description="${TIMER_DESCRIPTIONS[$timer_name]}"
    
    # Format duration appropriately
    local formatted_duration
    formatted_duration=$(format_duration "$duration")
    
    log_metric "$description completed in $formatted_duration"
    
    # Clean up timer data
    unset "TIMER_START_TIMES[$timer_name]"
    unset "TIMER_DESCRIPTIONS[$timer_name]"
    
    # Return duration for programmatic use
    echo "$duration"
}

# Get elapsed time without ending timer
get_elapsed_time() {
    local timer_name="${1:-$TIMER_DEFAULT_NAME}"
    
    if [[ -z "${TIMER_START_TIMES[$timer_name]:-}" ]]; then
        log_warning "Timer '$timer_name' was not started"
        return "$USAGE_ERROR"
    fi
    
    local start_time="${TIMER_START_TIMES[$timer_name]}"
    local current_time
    current_time=$(date +%s.%3N)
    local elapsed
    elapsed=$(echo "$current_time - $start_time" | bc -l 2>/dev/null || echo "0")
    
    echo "$elapsed"
}

# Format duration for human-readable output
format_duration() {
    local duration="$1"
    
    # Input validation
    if [[ -z "$duration" ]]; then
        echo "0s"
        return "$SUCCESS"
    fi
    
    # Validate numeric format (integer or decimal)
    if ! [[ "$duration" =~ ^[0-9]+\.?[0-9]*$ ]]; then
        echo "0s"
        return "$SUCCESS"
    fi
    
    # Handle negative durations
    if [[ "${duration%.*}" -lt 0 ]]; then
        echo "0s"
        return "$SUCCESS"
    fi
    
    # Handle bc dependency gracefully
    if ! command -v bc >/dev/null 2>&1; then
        # Fallback to basic integer arithmetic
        local seconds=${duration%.*}
        if [[ $seconds -lt 60 ]]; then
            echo "${seconds}s"
        elif [[ $seconds -lt 3600 ]]; then
            local minutes=$((seconds / 60))
            local remaining_seconds=$((seconds % 60))
            echo "${minutes}m ${remaining_seconds}s"
        else
            local hours=$((seconds / 3600))
            local remaining_minutes=$(((seconds % 3600) / 60))
            local remaining_seconds=$((seconds % 60))
            echo "${hours}h ${remaining_minutes}m ${remaining_seconds}s"
        fi
        return
    fi
    
    # Use bc for precise calculations
    local total_seconds
    total_seconds=$(echo "scale=0; $duration / 1" | bc)
    
    if [[ $(echo "$duration < 1" | bc) -eq 1 ]]; then
        # Less than 1 second - show milliseconds
        local milliseconds
        milliseconds=$(echo "scale=0; $duration * 1000" | bc)
        echo "${milliseconds}ms"
    elif [[ $total_seconds -lt 60 ]]; then
        # Less than 1 minute - show seconds with decimal
        local formatted_seconds
        formatted_seconds=$(echo "scale=1; $duration" | bc)
        echo "${formatted_seconds}s"
    elif [[ $total_seconds -lt 3600 ]]; then
        # Less than 1 hour - show minutes and seconds
        local minutes
        minutes=$(echo "scale=0; $total_seconds / 60" | bc)
        local remaining_seconds
        remaining_seconds=$(echo "scale=0; $total_seconds % 60" | bc)
        echo "${minutes}m ${remaining_seconds}s"
    else
        # 1 hour or more - show hours, minutes, and seconds
        local hours
        hours=$(echo "scale=0; $total_seconds / 3600" | bc)
        local remaining_minutes
        remaining_minutes=$(echo "scale=0; ($total_seconds % 3600) / 60" | bc)
        local remaining_seconds
        remaining_seconds=$(echo "scale=0; $total_seconds % 60" | bc)
        echo "${hours}h ${remaining_minutes}m ${remaining_seconds}s"
    fi
}

# Advanced timer functions

# Time a command execution
time_command() {
    local timer_name="$1"
    local description="$2"
    shift 2
    local command_array=("$@")
    
    if [[ ${#command_array[@]} -eq 0 ]]; then
        log_error "No command provided for timing"
        return "$USAGE_ERROR"
    fi
    
    start_timer "$timer_name" "$description"
    
    local exit_code=0
    "${command_array[@]}" || exit_code=$?
    
    local duration
    duration=$(end_timer "$timer_name")
    
    if [[ $exit_code -eq 0 ]]; then
        log_success "$description completed successfully in $(format_duration "$duration")"
    else
        log_error "$description failed in $(format_duration "$duration") (exit code: $exit_code)"
    fi
    
    return $exit_code
}

# Time a function execution
time_function() {
    local timer_name="$1"
    local description="$2"
    local function_name="$3"
    shift 3
    local function_args=("$@")
    
    start_timer "$timer_name" "$description"
    
    local exit_code=0
    "$function_name" "${function_args[@]}" || exit_code=$?
    
    local duration
    duration=$(end_timer "$timer_name")
    
    if [[ $exit_code -eq 0 ]]; then
        log_success "$description completed successfully in $(format_duration "$duration")"
    else
        log_error "$description failed in $(format_duration "$duration") (exit code: $exit_code)"
    fi
    
    return $exit_code
}

# Time a code block (using eval)
time_block() {
    local timer_name="$1"
    local description="$2"
    local code_block="$3"
    
    start_timer "$timer_name" "$description"
    
    local exit_code=0
    eval "$code_block" || exit_code=$?
    
    local duration
    duration=$(end_timer "$timer_name")
    
    if [[ $exit_code -eq 0 ]]; then
        log_success "$description completed successfully in $(format_duration "$duration")"
    else
        log_error "$description failed in $(format_duration "$duration") (exit code: $exit_code)"
    fi
    
    return $exit_code
}

# Timer management functions

# List all active timers
list_active_timers() {
    if [[ ${#TIMER_START_TIMES[@]} -eq 0 ]]; then
        log_info "No active timers"
        return
    fi
    
    log_info "Active timers:"
    for timer_name in "${!TIMER_START_TIMES[@]}"; do
        local elapsed
        elapsed=$(get_elapsed_time "$timer_name")
        local formatted_elapsed
        formatted_elapsed=$(format_duration "$elapsed")
        local description="${TIMER_DESCRIPTIONS[$timer_name]}"
        log_info "  $timer_name: $description (running for $formatted_elapsed)"
    done
}

# Stop and clear all timers
clear_all_timers() {
    local count=${#TIMER_START_TIMES[@]}
    
    if [[ $count -gt 0 ]]; then
        log_debug "Clearing $count active timers"
        TIMER_START_TIMES=()
        TIMER_DESCRIPTIONS=()
    fi
}

# Stop a specific timer without logging
stop_timer() {
    local timer_name="${1:-$TIMER_DEFAULT_NAME}"
    
    if [[ -n "${TIMER_START_TIMES[$timer_name]:-}" ]]; then
        unset "TIMER_START_TIMES[$timer_name]"
        unset "TIMER_DESCRIPTIONS[$timer_name]"
        log_debug "Stopped timer '$timer_name'"
    else
        log_warning "Timer '$timer_name' was not active"
        return "$USAGE_ERROR"
    fi
}

# Check if a timer is active
is_timer_active() {
    local timer_name="${1:-$TIMER_DEFAULT_NAME}"
    [[ -n "${TIMER_START_TIMES[$timer_name]:-}" ]]
}

# Performance measurement utilities

# Measure memory usage of a command
measure_memory_usage() {
    local description="$1"
    shift
    local command_array=("$@")
    
    if ! command -v time >/dev/null 2>&1; then
        log_warning "GNU time command not available, skipping memory measurement"
        "${command_array[@]}"
        return $?
    fi
    
    log_info "Measuring memory usage: $description"
    
    local temp_file
    temp_file=$(mktemp)
    
    local exit_code=0
    /usr/bin/time -v "${command_array[@]}" 2>"$temp_file" || exit_code=$?
    
    if [[ -f "$temp_file" ]]; then
        local max_memory
        max_memory=$(grep "Maximum resident set size" "$temp_file" | awk '{print $6}')
        if [[ -n "$max_memory" ]]; then
            local memory_mb
            memory_mb=$(echo "scale=2; $max_memory / 1024" | bc -l 2>/dev/null || echo "$max_memory")
            log_metrics "Peak memory usage: ${memory_mb}MB"
        fi
        rm -f "$temp_file"
    fi
    
    return $exit_code
}

# Benchmark a command multiple times
benchmark_command() {
    local iterations="$1"
    local description="$2"
    shift 2
    local command_array=("$@")
    
    if [[ "$iterations" -lt 1 ]]; then
        log_error "Invalid iteration count: $iterations"
        return "$USAGE_ERROR"
    fi
    
    log_info "Benchmarking: $description ($iterations iterations)"
    
    local total_duration=0
    local successful_runs=0
    local failed_runs=0
    
    for ((i=1; i<=iterations; i++)); do
        local timer_name="benchmark_$i"
        start_timer "$timer_name" "Iteration $i"
        
        local exit_code=0
        "${command_array[@]}" >/dev/null 2>&1 || exit_code=$?
        
        local duration
        duration=$(end_timer "$timer_name" 2>/dev/null)
        
        if [[ $exit_code -eq 0 ]]; then
            ((successful_runs++))
            total_duration=$(echo "$total_duration + $duration" | bc -l 2>/dev/null || echo "$total_duration")
        else
            ((failed_runs++))
        fi
    done
    
    if [[ $successful_runs -gt 0 ]]; then
        local average_duration
        average_duration=$(echo "scale=3; $total_duration / $successful_runs" | bc -l 2>/dev/null || echo "0")
        local formatted_average
        formatted_average=$(format_duration "$average_duration")
        
        log_metrics "Benchmark results: $successful_runs/$iterations successful runs"
        log_metrics "Average execution time: $formatted_average"
        
        if [[ $failed_runs -gt 0 ]]; then
            log_warning "Failed runs: $failed_runs/$iterations"
        fi
    else
        log_error "All benchmark iterations failed"
        return "$TEST_ERROR"
    fi
}

# Cleanup function for timer helper
cleanup_timers() {
    clear_all_timers
}

# Auto-cleanup on script exit (if enabled)
if [[ "${TIMER_AUTO_CLEANUP:-true}" == "true" ]]; then
    trap cleanup_timers EXIT
fi