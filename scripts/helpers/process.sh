#!/bin/bash

# process.sh - Signal handling and process management utility functions
# Version: 1.0
# Description: Provides standardized signal handling, process management, and cleanup operations

if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    echo "Error: process-helper.sh should be sourced, not executed directly"
    exit 1
fi
if [[ -n "${PROCESS_HELPER:-}" ]]; then
    return 0
fi
export PROCESS_HELPER=1

# shellcheck source=.scripts/helpers/error.sh
source "helpers/error.sh"
# shellcheck source=.scripts/helpers/output.sh
source "helpers/output.sh"
# shellcheck source=.scripts/helpers/log.sh
source "helpers/log.sh"

# Signal handling configuration
SIGNAL_HANDLING_ENABLED="${SIGNAL_HANDLING_ENABLED:-true}"
SIGNAL_CLEANUP_ON_EXIT="${SIGNAL_CLEANUP_ON_EXIT:-true}"
SIGNAL_VERBOSE_CLEANUP="${SIGNAL_VERBOSE_CLEANUP:-false}"

# Process tracking
declare -A TRACKED_PROCESSES
declare -a CLEANUP_FUNCTIONS
declare -a HOOKS

# Core signal handling functions

# Handle interrupt signals (SIGINT, SIGTERM)
handle_interrupt() {
    local signal="${1:-SIGINT}"
    local exit_code="${2:-$USER_INTERRUPT}"
    
    log_debug "handle_interrupt called with signal=$signal"
    
    if [[ "$SIGNAL_VERBOSE_CLEANUP" == "true" ]]; then
        log_warning "Received $signal signal"
        log_info "Initiating graceful shutdown..."
    fi
    
    log_debug "About to stop tracked processes"
    # Stop tracked processes
    stop_tracked_processes
    
    log_debug "About to run cleanup functions"
    # Run cleanup functions
    run_cleanup_functions
    
    log_debug "About to exit with code $exit_code"
    # Exit with appropriate code
    exit_with "$exit_code"
}

# Handle exit cleanup
handle_exit() {
    local exit_code=$?
    
    if [[ "$SIGNAL_VERBOSE_CLEANUP" == "true" ]]; then
        log_debug "Exit cleanup triggered with code: $exit_code"
    fi
    
    # Stop any remaining tracked processes
    stop_tracked_processes
    
    # Run cleanup functions
    run_cleanup_functions
    
    # Run exit hooks
    run_exit_hooks
    
    # Don't change the exit code
    exit_with "$exit_code"
}

# Setup signal handlers
setup_signal_handlers() {
    local enable_interrupt="${1:-true}"
    local enable_exit="${2:-true}"
    
    if [[ "$SIGNAL_HANDLING_ENABLED" != "true" ]]; then
        log_debug "Signal handling is disabled"
        return
    fi
    
    if [[ "$enable_interrupt" == "true" ]]; then
        trap 'handle_interrupt SIGINT' SIGINT
        trap 'handle_interrupt SIGTERM' SIGTERM
        log_debug "Interrupt signal handlers installed"
    fi
    
    if [[ "$enable_exit" == "true" && "$SIGNAL_CLEANUP_ON_EXIT" == "true" ]]; then
        trap 'handle_exit' EXIT
        log_debug "Exit cleanup handler installed"
    fi
}

# Remove signal handlers
remove_signal_handlers() {
    trap - SIGINT SIGTERM EXIT
    log_debug "Signal handlers removed"
}

# Process tracking functions

# Track a process for cleanup
track_process() {
    local process_name="$1"
    local process_pid="$2"
    local kill_signal="${3:-TERM}"
    
    if [[ -z "$process_name" || -z "$process_pid" ]]; then
        log_error "Process name and PID required for tracking"
        return "$USAGE_ERROR"
    fi
    
    if ! kill -0 "$process_pid" 2>/dev/null; then
        log_warning "Process $process_pid is not running, not tracking"
        return "$GENERAL_ERROR"
    fi
    
    TRACKED_PROCESSES["$process_name"]="$process_pid:$kill_signal"
    log_debug "Tracking process: $process_name (PID: $process_pid, signal: $kill_signal)"
}

# Stop tracking a process
untrack_process() {
    local process_name="$1"
    
    if [[ -n "${TRACKED_PROCESSES[$process_name]:-}" ]]; then
        unset "TRACKED_PROCESSES[$process_name]"
        log_debug "Stopped tracking process: $process_name"
    else
        log_warning "Process not tracked: $process_name"
    fi
}

# Stop a specific tracked process
stop_tracked_process() {
    local process_name="$1"
    local force="${2:-false}"
    
    if [[ -z "${TRACKED_PROCESSES[$process_name]:-}" ]]; then
        log_warning "Process not tracked: $process_name"
        return "$GENERAL_ERROR"
    fi
    
    local process_info="${TRACKED_PROCESSES[$process_name]}"
    local process_pid="${process_info%%:*}"
    local kill_signal="${process_info##*:}"
    
    if ! kill -0 "$process_pid" 2>/dev/null; then
        log_debug "Process $process_name (PID: $process_pid) already stopped"
        untrack_process "$process_name"
        return "$SUCCESS"
    fi
    
    log_debug "Stopping process: $process_name (PID: $process_pid)"
    
    # Try graceful shutdown first
    if kill -"$kill_signal" "$process_pid" 2>/dev/null; then
        # Wait for graceful shutdown
        local timeout=5
        while [[ $timeout -gt 0 ]] && kill -0 "$process_pid" 2>/dev/null; do
            sleep 1
            ((timeout--))
        done
        
        # Force kill if still running
        if kill -0 "$process_pid" 2>/dev/null; then
            if [[ "$force" == "true" ]]; then
                log_warning "Force killing process: $process_name (PID: $process_pid)"
                kill -KILL "$process_pid" 2>/dev/null || true
            else
                log_warning "Process $process_name (PID: $process_pid) did not respond to $kill_signal"
            fi
        fi
    fi
    
    untrack_process "$process_name"
}

# Stop all tracked processes
stop_tracked_processes() {
    local force="${1:-false}"
    
    if [[ ${#TRACKED_PROCESSES[@]} -eq 0 ]]; then
        return "$SUCCESS"
    fi
    
    if [[ "$SIGNAL_VERBOSE_CLEANUP" == "true" ]]; then
        log_info "Stopping ${#TRACKED_PROCESSES[@]} tracked processes..."
    fi
    
    for process_name in "${!TRACKED_PROCESSES[@]}"; do
        stop_tracked_process "$process_name" "$force"
    done
}

# List tracked processes
list_tracked_processes() {
    if [[ ${#TRACKED_PROCESSES[@]} -eq 0 ]]; then
        log_info "No processes are being tracked"
        return
    fi
    
    log_info "Tracked processes:"
    for process_name in "${!TRACKED_PROCESSES[@]}"; do
        local process_info="${TRACKED_PROCESSES[$process_name]}"
        local process_pid="${process_info%%:*}"
        local kill_signal="${process_info##*:}"
        
        if kill -0 "$process_pid" 2>/dev/null; then
            log_info "  $process_name: PID $process_pid (signal: $kill_signal) [RUNNING]"
        else
            log_info "  $process_name: PID $process_pid (signal: $kill_signal) [STOPPED]"
        fi
    done
}

# Cleanup functions management

# Register a cleanup function
register_cleanup_function() {
    local function_name="$1"
    
    if [[ -z "$function_name" ]]; then
        log_error "Cleanup function name required"
        return "$USAGE_ERROR"
    fi
    
    if ! type "$function_name" >/dev/null 2>&1; then
        log_error "Cleanup function not found: $function_name"
        return "$GENERAL_ERROR"
    fi
    
    CLEANUP_FUNCTIONS+=("$function_name")
    log_debug "Registered cleanup function: $function_name"
}

# Unregister a cleanup function
unregister_cleanup_function() {
    local function_name="$1"
    local new_cleanup_functions=()
    
    for func in "${CLEANUP_FUNCTIONS[@]}"; do
        if [[ "$func" != "$function_name" ]]; then
            new_cleanup_functions+=("$func")
        fi
    done
    
    CLEANUP_FUNCTIONS=("${new_cleanup_functions[@]}")
    log_debug "Unregistered cleanup function: $function_name"
}

# Run all cleanup functions
run_cleanup_functions() {
    log_debug "run_cleanup_functions called, ${#CLEANUP_FUNCTIONS[@]} functions registered"
    
    if [[ ${#CLEANUP_FUNCTIONS[@]} -eq 0 ]]; then
        log_debug "No cleanup functions to run"
        return "$SUCCESS"
    fi
    
    if [[ "$SIGNAL_VERBOSE_CLEANUP" == "true" ]]; then
        log_debug "Running ${#CLEANUP_FUNCTIONS[@]} cleanup functions..."
    fi
    
    for cleanup_func in "${CLEANUP_FUNCTIONS[@]}"; do
        log_debug "Running cleanup function: $cleanup_func"
        if type "$cleanup_func" >/dev/null 2>&1; then
            "$cleanup_func" || log_warning "Cleanup function failed: $cleanup_func"
        else
            log_warning "Cleanup function not found: $cleanup_func"
        fi
    done
    
    log_debug "All cleanup functions completed"
}

# Exit hooks management

# Register an exit hook
register_exit_hook() {
    local hook_function="$1"
    
    if [[ -z "$hook_function" ]]; then
        log_error "Exit hook function name required"
        return "$USAGE_ERROR"
    fi
    
    if ! type "$hook_function" >/dev/null 2>&1; then
        log_error "Exit hook function not found: $hook_function"
        return "$GENERAL_ERROR"
    fi
    
    HOOKS+=("$hook_function")
    log_debug "Registered exit hook: $hook_function"
}

# Run all exit hooks
run_exit_hooks() {
    if [[ ${#HOOKS[@]} -eq 0 ]]; then
        return "$SUCCESS"
    fi
    
    if [[ "$SIGNAL_VERBOSE_CLEANUP" == "true" ]]; then
        log_debug "Running ${#HOOKS[@]} exit hooks..."
    fi
    
    for exit_hook in "${HOOKS[@]}"; do
        if type "$exit_hook" >/dev/null 2>&1; then
            log_debug "Running exit hook: $exit_hook"
            "$exit_hook" || log_warning "Exit hook failed: $exit_hook"
        else
            log_warning "Exit hook function not found: $exit_hook"
        fi
    done
}

# Utility functions

# Check if a process is running
is_process_running() {
    local pid="$1"
    kill -0 "$pid" 2>/dev/null
}

# Wait for a process to finish with timeout
wait_for_process() {
    local pid="$1"
    local timeout="${2:-30}"
    local check_interval="${3:-1}"
    
    local elapsed=0
    while is_process_running "$pid" && [[ $elapsed -lt $timeout ]]; do
        sleep "$check_interval"
        elapsed=$((elapsed + check_interval))
    done
    
    if is_process_running "$pid"; then
        log_warning "Process $pid still running after ${timeout}s timeout"
        return "$TIMEOUT_ERROR"
    else
        log_debug "Process $pid finished"
        return "$SUCCESS"
    fi
}

# Kill process tree (process and all children)
kill_process_tree() {
    local pid="$1"
    local signal="${2:-TERM}"
    
    if ! is_process_running "$pid"; then
        log_debug "Process $pid is not running"
        return "$SUCCESS"
    fi
    
    # Get all child processes
    local children
    children=$(pgrep -P "$pid" 2>/dev/null || true)
    
    # Kill children first
    for child_pid in $children; do
        kill_process_tree "$child_pid" "$signal"
    done
    
    # Kill the parent process
    log_debug "Killing process $pid with signal $signal"
    kill -"$signal" "$pid" 2>/dev/null || true
}

# Send signal to process with retry
signal_process_with_retry() {
    local pid="$1"
    local signal="$2"
    local retries="${3:-3}"
    local delay="${4:-1}"
    
    for ((i=1; i<=retries; i++)); do
        if kill -"$signal" "$pid" 2>/dev/null; then
            log_debug "Signal $signal sent to process $pid (attempt $i)"
            return "$SUCCESS"
        else
            if [[ $i -lt $retries ]]; then
                log_debug "Failed to send signal $signal to process $pid (attempt $i/$retries), retrying..."
                sleep "$delay"
            fi
        fi
    done
    
    log_warning "Failed to send signal $signal to process $pid after $retries attempts"
    return "$GENERAL_ERROR"
}

# Configuration functions

# Enable signal handling
enable_signal_handling() {
    SIGNAL_HANDLING_ENABLED="true"
    setup_signal_handlers "true" "true"
    log_debug "Signal handling enabled"
}

# Disable signal handling
disable_signal_handling() {
    SIGNAL_HANDLING_ENABLED="false"
    remove_signal_handlers
    log_debug "Signal handling disabled"
}

# Enable verbose cleanup logging
enable_verbose_cleanup() {
    SIGNAL_VERBOSE_CLEANUP="true"
    log_debug "Verbose cleanup logging enabled"
}

# Disable verbose cleanup logging
disable_verbose_cleanup() {
    SIGNAL_VERBOSE_CLEANUP="false"
    log_debug "Verbose cleanup logging disabled"
}

# Emergency cleanup function
emergency_cleanup() {
    log_error "Emergency cleanup initiated"
    
    # Force stop all tracked processes
    stop_tracked_processes "true"
    
    # Run cleanup functions (ignore failures)
    for cleanup_func in "${CLEANUP_FUNCTIONS[@]}"; do
        "$cleanup_func" 2>/dev/null || true
    done
    
    log_error "Emergency cleanup completed"
}

# Initialize signal handling (called automatically when sourced)
if [[ "$SIGNAL_HANDLING_ENABLED" == "true" ]]; then
    setup_signal_handlers "true" "true"
fi