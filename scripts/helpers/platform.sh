#!/bin/bash

# platform.sh - Cross-platform utility functions
# Version: 1.0
# Description: Provides platform detection, path normalization, validation, and utility functions

if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    log_error "platform-helper.sh should be sourced, not executed directly"
    exit 1
fi
if [[ -n "${PLATFORM_HELPER:-}" ]]; then
    return 0
fi
export PLATFORM_HELPER=1

# shellcheck source=.scripts/helpers/error.sh
source "helpers/error.sh"
# shellcheck source=.scripts/helpers/output.sh
source "helpers/output.sh"
# shellcheck source=.scripts/helpers/log.sh
source "helpers/log.sh"

normalize_path() {
    local path="$1"
    # Convert Windows paths to Unix-style for WSL/Git Bash
    if [[ "$path" =~ ^[A-Za-z]: ]]; then
        path="/${path:0:1}${path:2}"
        path="${path//\\\\//}"
    fi
    echo "$path"
}

get_platform() {
    if [[ -f /proc/version ]] && grep -qi microsoft /proc/version; then
        echo "WSL"
    else
        log_error "Only WSL2 is supported"
        exit_with "$NOT_SUPPORTED_PLATFORM"
    fi
}

# Get WSL2 temporary directory from environment
get_temp_dir() {
    echo "${TMP:-/tmp}"
}

# Check if a command exists in PATH
check_command_exists() {
    local cmd="$1"
    if command -v "$cmd" >/dev/null 2>&1; then
        return 0
    else
        return 1
    fi
}

# Validate if a directory exists
directory_exists() {
    local dir="$1"
    local dir_name="${2:-Directory}"
    
    if [[ ! -d "$dir" ]]; then
        log_error "$dir_name does not exist: $dir"
        return "$NO_INPUT_ERROR"
    fi
    return "$SUCCESS"
}

# Validate if a file exists
file_exists() {
    local file="$1"
    local file_name="${2:-File}"
    
    if [[ ! -f "$file" ]]; then
        log_error "$file_name does not exist: $file"
        return "$NO_INPUT_ERROR"
    fi
    return "$SUCCESS"
}

# Validate if a path is readable
is_path_readable() {
    local path="$1"
    local path_name="${2:-Path}"
    
    if [[ ! -r "$path" ]]; then
        log_error "$path_name is not readable: $path"
        return "$UNAUTHORIZED"
    fi
    return "$SUCCESS"
}

# Validate if a path is writable
is_path_writable() {
    local path="$1"
    local path_name="${2:-Path}"
    
    if [[ ! -w "$path" ]]; then
        log_error "$path_name is not writable: $path"
        return "$UNAUTHORIZED"
    fi
    return "$SUCCESS"
}

is_running_in_container() {
    # Check for Docker
    if [[ -f /.dockerenv ]]; then
        return 0
    fi
    
    # Check for container-specific cgroup
    if [[ -f /proc/1/cgroup ]] && grep -q "docker\|lxc\|kubepods" /proc/1/cgroup 2>/dev/null; then
        return 0
    fi
    
    # Check for Podman environment variable
    if [[ "${container:-}" == "podman" ]]; then
        return 0
    fi
    
    return 1
}

# Development environment detection

# Check if running in WSL
is_wsl() {
    [[ "$(get_platform)" == "WSL" ]]
}

# Check if running on macOS
is_macos() {
    [[ "$(get_platform)" == "Darwin" ]]
}

# WSL2-only environment - other platform functions removed

# Check if Git is available and we're in a Git repository
is_git_repository() {
    check_command_exists "git" && git rev-parse --git-dir >/dev/null 2>&1
}

# Get Git repository root
get_git_root() {
    if is_git_repository; then
        git rev-parse --show-toplevel
    else
        return "$DATA_FORMAT_ERROR"
    fi
}

# Tool validation functions

# Validate Java installation
validate_java() {
    local required_version="${1:-17}"
    
    if ! check_command_exists "java"; then
        log_error "Java is not installed or not in PATH"
        return "$PREREQUISITE_ERROR"
    fi
    
    local java_version
    java_version="$(java -version 2>&1 | head -n1 | cut -d'"' -f2 | cut -d'.' -f1)"
    
    if [[ "$java_version" -lt "$required_version" ]]; then
        log_error "Java $required_version or higher is required (found: $java_version)"
        return "$PREREQUISITE_ERROR"
    fi
    
    return "$SUCCESS"
}

# Validate Maven installation
validate_maven() {
    if ! check_command_exists "mvn"; then
        log_error "Maven is not installed or not in PATH"
        return "$PREREQUISITE_ERROR"
    fi
    return "$SUCCESS"
}

# Validate Node.js installation
validate_node() {
    local required_version="${1:-16}"
    
    if ! check_command_exists "node"; then
        log_error "Node.js is not installed or not in PATH"
        return "$PREREQUISITE_ERROR"
    fi
    
    local node_version
    node_version="$(node --version | sed 's/v//' | cut -d'.' -f1)"
    
    if [[ "$node_version" -lt "$required_version" ]]; then
        log_error "Node.js $required_version or higher is required (found: $node_version)"
        return "$PREREQUISITE_ERROR"
    fi
    
    return "$SUCCESS"
}

# Network and connectivity functions

# Check if a host is reachable
check_host_reachable() {
    local host="$1"
    local timeout="${2:-5}"
    
    if check_command_exists "ping"; then
        if is_macos; then
            ping -c 1 -W "$timeout" "$host" >/dev/null 2>&1
        else
            ping -c 1 -W "$timeout" "$host" >/dev/null 2>&1
        fi
    elif check_command_exists "nc"; then
        nc -z -w "$timeout" "$host" 80 >/dev/null 2>&1
    else
        return "$COMMAND_NOT_FOUND"
    fi
}

# Check if internet connectivity is available
check_internet_connectivity() {
    check_host_reachable "8.8.8.8" 3 || check_host_reachable "1.1.1.1" 3
}

# Utility functions

# Create directory with proper permissions
create_directory() {
    local dir="$1"
    local mode="${2:-755}"
    
    if [[ ! -d "$dir" ]]; then
        mkdir -p "$dir" || return "$CANNOT_CREATE_FOLDER"
        chmod "$mode" "$dir" || return "$UNAUTHORIZED"
    fi
    return "$SUCCESS"
}

# Exit with error message and code
exit_with_error() {
    local message="$1"
    local exit_code="${2:-$GENERAL_ERROR}"
    
    log_error "$message"
    exit_with "$exit_code"
}

# Get script directory (works with symlinks)
get_script_dir() {
    local source="${1:-${BASH_SOURCE[0]}}"
    while [[ -L "$source" ]]; do
        local dir
        dir="$(cd -P "$(dirname "$source")" && pwd)"
        source="$(readlink "$source")"
        [[ "$source" != /* ]] && source="$dir/$source"
    done
    cd -P "$(dirname "$source")" && pwd
}

# Get absolute path
get_absolute_path() {
    local path="$1"
    cd "$(dirname "$path")" && pwd -P || return "$NO_INPUT_ERROR"
}

# RWP (Ross Video Platform) dependency management

# Auto-detection of RWP dependencies
# Sets USE_LOCAL global variable based on RWP availability
# Returns: 0 on success, 1 on configuration error
detect_rwp_mode() {
    # Input validation
    if [[ -z "$RWP_PATH" ]]; then
        log_warning "RWP_PATH not set - using remote dependencies by default"
        USE_LOCAL=false
        return 0
    fi
    
    # Check if local RWP is available and built
    local rwp_repo_path="$RWP_PATH/releng/com.rossvideo.rwp.releng.update/target/repository"

    if [[ -d "$RWP_PATH" && -d "$rwp_repo_path" ]]; then
        USE_LOCAL=true
        log_info "Auto-detected local RWP dependencies at $RWP_PATH"
        log_debug "RWP P2 repository found: $rwp_repo_path"
        return 0
    else
        USE_LOCAL=false
        log_info "Using remote RWP dependencies (local not available or not built)"
        if [[ -d "$RWP_PATH" && ! -d "$rwp_repo_path" ]]; then
            log_info "Tip: Build RWP locally for faster builds: cd $RWP_PATH && ./rwp.sh build --target"
        fi
        return 0
    fi
}

# RWP dependency validation - checks if local RWP repository is properly built
# Parameters: None (uses global USE_LOCAL and RWP_PATH variables)
# Returns: 0 if valid or not using local, 1 if local RWP is required but missing
check_rwp_repository() {
    if [[ "$USE_LOCAL" != "true" ]]; then
        return 0  # Skip check if not using local RWP
    fi

    # Input validation
    if [[ -z "$RWP_PATH" ]]; then
        log_error "USE_LOCAL=true but RWP_PATH not set"
        return "$INVALID_CONFIGURATIONURATION_ERROR"
    fi

    local rwp_repo_path="$RWP_PATH/releng/com.rossvideo.rwp.releng.update/target/repository"

    if [[ ! -d "$rwp_repo_path" ]]; then
        log_error "RWP P2 repository not found: $rwp_repo_path"
        log_error "RWP must be built first to provide dependencies for Streamline"
        log_error ""
        log_error "Please run the following commands:"
        log_error "  cd $RWP_PATH"
        log_error "  ./rwp.sh build --target"
        log_error ""
        log_error "This will create the P2 repository that Streamline depends on"
        return "$PREREQUISITE_ERROR"
    fi

    log_debug "RWP P2 repository found: $rwp_repo_path"
    return 0
}

# Platform-specific validation functions
# (Moved from validation.sh for proper separation of responsibilities)

# Validate DNS configuration for corporate network
validate_dns_configuration() {
    local hosts=("srvottodrepo01.rossvideo.com" "srvottdockreg02.rossvideo.com" "rwp-docs.rossvideo.com")
    
    log_info "Validating DNS configuration for corporate network"
    
    local failed_hosts=()
    for host in "${hosts[@]}"; do
        if check_host_reachable "$host" 3; then
            log_debug "✓ DNS resolution successful: $host"
        else
            log_warning "✗ DNS resolution failed: $host"
            failed_hosts+=("$host")
        fi
    done
    
    if [[ ${#failed_hosts[@]} -eq 0 ]]; then
        log_success "DNS configuration validated successfully"
        return "$SUCCESS"
    else
        log_warning "DNS resolution failed for ${#failed_hosts[@]} hosts"
        return "$REMOTE_CONNECTION_ERROR"
    fi
}

# Validate single directory existence
__try_validate_directory() {
    local dir="$1"
    local description="${2:-Directory}"
    
    if [[ -z "$dir" ]]; then
        log_error "Directory path required"
        return "$USAGE_ERROR"
    fi
    
    if [[ -d "$dir" ]]; then
        log_debug "✓ Found: $description ($dir)"
        return "$SUCCESS"
    else
        log_warning "⚠ Missing: $description ($dir)"
        return "$NO_INPUT_ERROR"
    fi
}

# Check directory exists (streamline.sh compatibility)
check_directory_exists() {
    local dir="$1"
    if [[ -d "$dir" ]]; then
        log_debug "✓ Found: $dir"
        return 0
    else
        log_warning "⚠ Missing: $dir"
        return 1
    fi
}

# Validate directory structure
validate_directory_structure() {
    local project_root="$1"
    local required_dirs=("${@:2}")
    local default_dirs=("ProjectRoot" "InstallRoot" "scripts")
    
    if [[ -z "$project_root" ]]; then
        log_error "Project root directory required"
        return "$USAGE_ERROR"
    fi
    
    if [[ ${#required_dirs[@]} -eq 0 ]]; then
        required_dirs=("${default_dirs[@]}")
    fi
    
    log_info "Validating directory structure: $project_root"
    
    # Validate project root exists
    if ! directory_exists "$project_root" "Project root"; then
        return "$NO_INPUT_ERROR"
    fi
    
    local missing_dirs=()
    for dir in "${required_dirs[@]}"; do
        local full_path="$project_root/$dir"
        if [[ -d "$full_path" ]]; then
            log_debug "✓ Directory exists: $dir"
        else
            log_warning "✗ Directory missing: $dir"
            missing_dirs+=("$dir")
        fi
    done
    
    if [[ ${#missing_dirs[@]} -eq 0 ]]; then
        log_success "Directory structure validated successfully"
        return "$SUCCESS"
    else
        log_error "Missing directories: ${missing_dirs[*]}"
        return "$NO_INPUT_ERROR"
    fi
}

# Network connectivity validation
validate_network_connectivity() {
    local test_hosts=("8.8.8.8" "1.1.1.1" "github.com")
    
    if [[ ${#test_hosts[@]} -eq 0 ]]; then
        test_hosts=("${default_hosts[@]}")
    fi
    
    log_info "Validating network connectivity"
    
    local successful_tests=0
    for host in "${test_hosts[@]}"; do
        if check_host_reachable "$host" 5; then
            log_debug "✓ Network connectivity successful: $host"
            ((successful_tests++))
        else
            log_warning "✗ Network connectivity failed: $host"
        fi
    done
    
    if [[ $successful_tests -gt 0 ]]; then
        log_success "Network connectivity validated ($successful_tests/${#test_hosts[@]} hosts reachable)"
        return "$SUCCESS"
    else
        log_error "Network connectivity validation failed (no hosts reachable)"
        return "$REMOTE_CONNECTION_ERROR"
    fi
}

# Platform-specific test functions
# (Moved from validation.sh for proper separation of responsibilities)

# Test platform detection
test_platform_detection() {
    log_info "Testing platform detection"
    
    local platform
    platform="$(get_platform)"
    
    if [[ "$platform" != "Unknown" ]]; then
        log_success "Platform Detection: Detected: $platform"
        return "$SUCCESS"
    else
        log_error "Platform Detection: Unknown platform"
        return "$GENERAL_ERROR"
    fi
}

# Test directory structure
test_directory_structure() {
    local project_root="$1"
    
    log_info "Testing directory structure: $project_root"
    
    if validate_directory_structure "$project_root"; then
        log_success "Directory Structure: All required directories found"
        return "$SUCCESS"
    else
        log_error "Directory Structure: Missing required directories"
        return "$NO_INPUT_ERROR"
    fi
}

# Test network connectivity
test_network_connectivity() {
    log_info "Testing network connectivity"
    
    if validate_network_connectivity; then
        log_success "Network Connectivity: Internet access available"
        return "$SUCCESS"
    else
        log_warning "Network Connectivity: Limited or no internet access"
        return "$REMOTE_CONNECTION_ERROR"
    fi
}