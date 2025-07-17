#!/bin/bash

# container.sh - Container management utility functions
# Version: 1.0
# Description: Provides parameterized container lifecycle management for Docker/Podman

if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    echo "Error: container.sh should be sourced, not executed directly"
    exit 1
fi
if [[ -n "${CONTAINER_HELPER:-}" ]]; then
    return 0
fi
export CONTAINER_HELPER=1

# shellcheck source=.scripts/helpers/error.sh
source "helpers/error.sh"
# shellcheck source=.scripts/helpers/output.sh
source "helpers/output.sh"
# shellcheck source=.scripts/helpers/log.sh
source "helpers/log.sh"
# shellcheck source=.scripts/helpers/platform.sh
source "helpers/platform.sh"

get_container_engine_name() {
    if check_command_exists "docker" && docker info >/dev/null 2>&1; then
        echo "docker"
        return "$SUCCESS"
    elif check_command_exists "podman" && podman info >/dev/null 2>&1; then
        echo "podman"
        return "$SUCCESS"
    else
        return "$COMMAND_NOT_FOUND"
    fi
}

# Container configuration
DOCKERFILE="$SCRIPT_PATH/dev.dockerfile"
CONTAINER_ENGINE="$(get_container_engine_name)"
CONTAINER_IMAGE="mcphub-dotnet-dev:latest"
CONTAINER_CLEANUP_ON_EXIT=true
CONTAINER_PID=""
CONTAINER_NAME=""

# Container lifecycle management

# Initialize container engine detection
container_init() {
    if [[ -z "$CONTAINER_ENGINE" ]]; then
        CONTAINER_ENGINE="$(get_container_engine_name)" || {
            exit_with_error "Container engine detection failed" "$PREREQUISITE_ERROR"
        }
        log_debug "Initialized container engine: $CONTAINER_ENGINE"
    fi

    export CONTAINER_ENGINE CONTAINER_IMAGE CONTAINER_PID CONTAINER_NAME
}

# Check if container image exists
check_container_exists() {
    container_init
    
    local image_exists
    image_exists=$($CONTAINER_ENGINE images -q "$CONTAINER_IMAGE" 2>/dev/null)
    [[ -n "$image_exists" ]]
}

ensure_container_engine() {
    log_debug "Detecting available container engine..."
    
    if [[ -z "$CONTAINER_ENGINE" ]]; then
        log_error "Container Engine: No accessible engine found (docker/podman required)"
        return "$COMMAND_NOT_FOUND"
    fi

    return "$SUCCESS"
}


# Check container health by running a simple command
check_container_health() {
    local test_command="echo 'Container health check passed'"
    container_init
    
    log_debug "Testing container health for image: $CONTAINER_IMAGE"
    
    if $CONTAINER_ENGINE run --rm "$CONTAINER_IMAGE" sh -c "$test_command" >/dev/null 2>&1; then
        log_debug "Container health check passed for: $CONTAINER_IMAGE"
        return "$SUCCESS"
    else
        log_debug "Container health check failed for: $CONTAINER_IMAGE"
        return "$CONTAINER_ERROR"
    fi
}

# Build container from Dockerfile
build_custom_container() {
    container_init
    file_exists "$DOCKERFILE" "Dockerfile"
    
    log_info "Building custom container: $CONTAINER_IMAGE"
    log_debug "Dockerfile: $DOCKERFILE"
    log_debug "Build context: $SCRIPT_PATH"
    
    local build_cmd=("$CONTAINER_ENGINE" "build" "-f" "$DOCKERFILE" "-t" "$CONTAINER_IMAGE")
    
    build_cmd+=("$SCRIPT_PATH")
    
    if "${build_cmd[@]}"; then
        log_success "Container built successfully: $CONTAINER_IMAGE"
        return "$SUCCESS"
    else
        log_error "Container build failed: $CONTAINER_IMAGE"
        return "$BUILD_ERROR"
    fi
}

# Container cleanup functions

# Stop and remove running containers based on image
container_cleanup_running() {
    container_init
    
    log_debug "Cleaning up running containers for image: $CONTAINER_IMAGE"
    
    local running_containers
    running_containers=$($CONTAINER_ENGINE ps -q --filter "ancestor=$CONTAINER_IMAGE" 2>/dev/null)
    
    if [[ -n "$running_containers" ]]; then
        log_info "Stopping and removing running containers for image: $CONTAINER_IMAGE"
        echo "$running_containers" | xargs -r "$CONTAINER_ENGINE" stop >/dev/null 2>&1
        echo "$running_containers" | xargs -r "$CONTAINER_ENGINE" rm >/dev/null 2>&1
        log_debug "Cleaned up running containers for: $CONTAINER_IMAGE"
    else
        log_debug "No running containers found for image: $CONTAINER_IMAGE"
    fi
}

# Remove container image
cleanup_container_image() {
    container_init
    
    log_debug "Removing container image: $CONTAINER_IMAGE"
    
    if check_container_exists; then
        if $CONTAINER_ENGINE rmi "$CONTAINER_IMAGE" >/dev/null 2>&1; then
            log_debug "Removed container image: $CONTAINER_IMAGE"
        else
            log_warning "Failed to remove container image: $CONTAINER_IMAGE"
        fi
    else
        log_debug "Container image not found, nothing to remove: $CONTAINER_IMAGE"
    fi
}

# Clean up dangling resources
container_cleanup_dangling() {
    container_init
    
    log_debug "Cleaning up dangling container resources"
    
    # Clean dangling images
    $CONTAINER_ENGINE image prune -f >/dev/null 2>&1 || true
    
    # Clean unused volumes if supported
    if $CONTAINER_ENGINE volume prune --help >/dev/null 2>&1; then
        $CONTAINER_ENGINE volume prune -f >/dev/null 2>&1 || true
    fi
    
    log_debug "Cleaned up dangling resources"
}

# Comprehensive container cleanup
container_cleanup() {
    local cleanup_running="${1:-true}"
    local cleanup_image="${2:-true}"
    local cleanup_dangling="${3:-true}"
    
    log_info "Starting container cleanup for: $CONTAINER_IMAGE"
    
    if [[ "$cleanup_running" == "true" ]]; then
        container_cleanup_running "$CONTAINER_IMAGE"
    fi
    
    if [[ "$cleanup_image" == "true" ]]; then
        cleanup_container_image "$CONTAINER_IMAGE"
    fi
    
    if [[ "$cleanup_dangling" == "true" ]]; then
        container_cleanup_dangling
    fi
    
    log_info "Container cleanup completed for: $CONTAINER_IMAGE"
}

# Run command in container with interrupt handling
run_container_async() {
    local cmd_array=("${@:1}")
    
    container_init
    
    if [[ ${#cmd_array[@]} -eq 0 ]]; then
        log_error "No command provided for container execution"
        return "$USAGE_ERROR"
    fi
    
    log_debug "Running command in container: $CONTAINER_IMAGE"
    log_debug "Command: ${cmd_array[*]}"
    
    # Generate unique container name for tracking
    local container_name
    container_name="mcphub-$$-$(date +%s)"
    
    # Build container run command
    local container_cmd
    container_cmd=("$CONTAINER_ENGINE" "run" "--rm" "--name" "$container_name")
    
    # Add signal handling - ensure signals are properly forwarded to container
    container_cmd+=("--init")
    
    # Add interactive TTY for colorization if stdout is a terminal
    if [[ -t 1 ]]; then
        container_cmd+=("-t")
        log_debug "Added TTY (-t) for colorization (stdout is a terminal)"
    else
        log_debug "No TTY added (stdout is not a terminal)"
    fi
    
    # Add environment variables
    if [[ -n "${CONTAINER_ENV:-}" ]]; then
        IFS=',' read -ra ENV_VARS <<< "$CONTAINER_ENV"
        for env_var in "${ENV_VARS[@]}"; do
            container_cmd+=("-e" "$env_var")
        done
    fi
    
    # Add timezone configuration to match host
    local host_timezone="${TZ:-}"
    if [[ -z "$host_timezone" ]]; then
        # Try to get timezone from various sources
        host_timezone="$(cat /etc/timezone 2>/dev/null || timedatectl show --property=Timezone --value 2>/dev/null || echo 'UTC')"
    fi
    container_cmd+=("-e" "TZ=$host_timezone")
    
    # Add timezone data volume mount for accurate timezone information
    if [[ -f "/etc/localtime" ]]; then
        container_cmd+=("-v" "/etc/localtime:/etc/localtime:ro")
    fi
    
    # Add volume mounts
    if [[ -n "${CONTAINER_VOLUMES:-}" ]]; then
        IFS=',' read -ra VOLUME_MOUNTS <<< "$CONTAINER_VOLUMES"
        for volume in "${VOLUME_MOUNTS[@]}"; do
            container_cmd+=("-v" "$volume")
        done
    fi
    
    # Add working directory
    if [[ -n "${CONTAINER_WORKDIR:-}" ]]; then
        container_cmd+=("-w" "$CONTAINER_WORKDIR")
    fi
    
    # Add image and command
    container_cmd+=("$CONTAINER_IMAGE")
    container_cmd+=("${cmd_array[@]}")
    
    log_debug "Full container command: ${container_cmd[*]}"
    
    # Set up immediate signal forwarding trap BEFORE starting container
    trap 'handle_container_interrupt' INT TERM
    
    # Execute with process tracking for interrupt handling
    "${container_cmd[@]}" &
    CONTAINER_PID=$!
    CONTAINER_NAME="$container_name"
    
    # Register container interrupt handler for cleanup
    register_cleanup_function "handle_container_interrupt"
    
    # Set up additional signal forwarding for faster response
    trap 'handle_container_interrupt; exit 130' INT
    trap 'handle_container_interrupt; exit 143' TERM
    
    # Wait for completion and capture exit code
    local exit_code=0
    if ! wait $CONTAINER_PID; then
        exit_code=$?
    fi
    
    # Remove the trap after wait completes
    trap - INT TERM
    
    # Cleanup - unregister handler and clear PID
    unregister_cleanup_function "handle_container_interrupt"
    CONTAINER_PID=""
    CONTAINER_NAME=""
    return $exit_code
}

# Run Maven command in container with specialized interrupt handling
run_maven_container_async() {
    local cmd_array=("${@:1}")
    
    container_init
    
    if [[ ${#cmd_array[@]} -eq 0 ]]; then
        log_error "No Maven command provided for container execution"
        return "$USAGE_ERROR"
    fi
    
    log_debug "Running Maven command in container: $CONTAINER_IMAGE"
    log_debug "Maven command: ${cmd_array[*]}"
    
    # Generate unique container name for tracking
    local container_name
    container_name="mcphub-dotnet-$$-$(date +%s)"
    
    # Build container run command
    local container_cmd
    container_cmd=("$CONTAINER_ENGINE" "run" "--rm" "--name" "$container_name")
    
    # Add signal handling - ensure signals are properly forwarded to container
    container_cmd+=("--init")
    
    # Add interactive TTY for colorization if stdout is a terminal
    if [[ -t 1 ]]; then
        container_cmd+=("-t")
        log_debug "Added TTY (-t) for Maven colorization (stdout is a terminal)"
    else
        log_debug "No TTY added for Maven (stdout is not a terminal)"
    fi
    
    # Add environment variables
    if [[ -n "${CONTAINER_ENV:-}" ]]; then
        IFS=',' read -ra ENV_VARS <<< "$CONTAINER_ENV"
        for env_var in "${ENV_VARS[@]}"; do
            container_cmd+=("-e" "$env_var")
        done
    fi
    
    # Add timezone configuration to match host
    local host_timezone="${TZ:-}"
    if [[ -z "$host_timezone" ]]; then
        # Try to get timezone from various sources
        host_timezone="$(cat /etc/timezone 2>/dev/null || timedatectl show --property=Timezone --value 2>/dev/null || echo 'UTC')"
    fi
    container_cmd+=("-e" "TZ=$host_timezone")
    
    # Add timezone data volume mount for accurate timezone information
    if [[ -f "/etc/localtime" ]]; then
        container_cmd+=("-v" "/etc/localtime:/etc/localtime:ro")
    fi
    
    # Add volume mounts
    if [[ -n "${CONTAINER_VOLUMES:-}" ]]; then
        IFS=',' read -ra VOLUME_MOUNTS <<< "$CONTAINER_VOLUMES"
        for volume in "${VOLUME_MOUNTS[@]}"; do
            container_cmd+=("-v" "$volume")
        done
    fi
    
    # Add working directory
    if [[ -n "${CONTAINER_WORKDIR:-}" ]]; then
        container_cmd+=("-w" "$CONTAINER_WORKDIR")
    fi
    
    # Add image and command
    container_cmd+=("$CONTAINER_IMAGE")
    container_cmd+=("${cmd_array[@]}")
    
    log_debug "Full Maven container command: ${container_cmd[*]}"
    
    # Set up Maven-specific signal forwarding trap BEFORE starting container
    trap 'handle_maven_interrupt' INT TERM
    
    # Execute with process tracking for interrupt handling
    "${container_cmd[@]}" &
    CONTAINER_PID=$!
    CONTAINER_NAME="$container_name"
    
    # Register Maven-specific interrupt handler for cleanup
    register_cleanup_function "handle_maven_interrupt"
    
    # Set up additional Maven-specific signal forwarding for faster response
    trap 'handle_maven_interrupt; exit 130' INT
    trap 'handle_maven_interrupt; exit 143' TERM
    
    # Wait for completion and capture exit code
    local exit_code=0
    if ! wait $CONTAINER_PID; then
        exit_code=$?
    fi
    
    # Remove the trap after wait completes
    trap - INT TERM
    
    # Cleanup - unregister handler and clear PID
    unregister_cleanup_function "handle_maven_interrupt"
    CONTAINER_PID=""
    CONTAINER_NAME=""
    return $exit_code
}

# Run command in container (simple version)
run_container_command() {
    local command="$1"
    local working_dir="${2:-}"
    local env_vars=("${@:3}")
    
    container_init
    
    local container_cmd=("$CONTAINER_ENGINE" "run" "--rm")
    
    # Add working directory if specified
    if [[ -n "$working_dir" ]]; then
        container_cmd+=("-w" "$working_dir")
    fi
    
    # Add environment variables
    for env_var in "${env_vars[@]}"; do
        container_cmd+=("-e" "$env_var")
    done
    
    container_cmd+=("$CONTAINER_IMAGE" "sh" "-c" "$command")
    
    "${container_cmd[@]}"
}

# Validate container tools comprehensively
validate_container_tools() {
    container_init
    __validate_container_tool_versions "$CONTAINER_IMAGE" "$CONTAINER_ENGINE"
}


# Utility functions

# Set container configuration
set_container_config() {
    local key="$1"
    local value="$2"
    
    case "$key" in
        "engine")
            CONTAINER_ENGINE="$value"
            ;;
        "image")
            CONTAINER_IMAGE="$value"
            ;;
        "env")
            CONTAINER_ENV="$value"
            ;;
        "volumes")
            CONTAINER_VOLUMES="$value"
            ;;
        "workdir")
            CONTAINER_WORKDIR="$value"
            ;;
        "cleanup")
            CONTAINER_CLEANUP_ON_EXIT="$value"
            ;;
        *)
            log_error "Unknown container configuration key: $key"
            return "$USAGE_ERROR"
            ;;
    esac
}

# Get container configuration
get_container_config() {
    local key="$1"
    
    case "$key" in
        "engine")
            echo "$CONTAINER_ENGINE"
            ;;
        "image")
            echo "$CONTAINER_IMAGE"
            ;;
        "env")
            echo "$CONTAINER_ENV"
            ;;
        "volumes")
            echo "$CONTAINER_VOLUMES"
            ;;
        "workdir")
            echo "$CONTAINER_WORKDIR"
            ;;
        "cleanup")
            echo "$CONTAINER_CLEANUP_ON_EXIT"
            ;;
        *)
            log_error "Unknown container configuration key: $key"
            return "$USAGE_ERROR"
            ;;
    esac
}

# Container interrupt handler
handle_container_interrupt() {
    log_debug "handle_container_interrupt called"
    log_debug "CONTAINER_NAME='${CONTAINER_NAME:-<empty>}'"
    log_debug "CONTAINER_PID='${CONTAINER_PID:-<empty>}'"
    
    if [[ -n "$CONTAINER_NAME" ]]; then
        log_debug "Stopping container by name: $CONTAINER_NAME"
        log_info "Interrupting container: $CONTAINER_NAME"
        
        # Step 1: Send SIGTERM directly to the container's main process
        log_debug "Sending SIGTERM to container main process"
        $CONTAINER_ENGINE kill -s TERM "$CONTAINER_NAME" 2>/dev/null || true
        
        # Step 2: Terminate all Maven/Java processes inside the container more aggressively
        log_debug "Terminating all Maven and Java processes inside container"
        $CONTAINER_ENGINE exec "$CONTAINER_NAME" sh -c "
            # Kill all Maven processes by command line pattern
            pkill -TERM -f \"org.apache.maven\" 2>/dev/null || true
            pkill -TERM -f \"maven-\" 2>/dev/null || true
            pkill -TERM -f \"mvn\" 2>/dev/null || true
            
            # Kill all Java processes
            pkill -TERM java 2>/dev/null || true
            
            # Kill download-related processes (wget, curl, etc.)
            pkill -TERM wget 2>/dev/null || true
            pkill -TERM curl 2>/dev/null || true
            
            # Kill any process tree related to Maven
            for pid in $(pgrep -f \"maven\|mvn\" 2>/dev/null || true); do
                kill -TERM \"$pid\" 2>/dev/null || true
                # Also kill all children of this process
                pkill -TERM -P \"$pid\" 2>/dev/null || true
            done
        " 2>/dev/null || true
        
        # Step 3: Give processes a brief moment to shutdown gracefully
        sleep 0.5
        
        # Step 4: Force kill everything if still running
        log_debug "Force killing any remaining processes"
        $CONTAINER_ENGINE exec "$CONTAINER_NAME" sh -c "
            # Force kill all Maven/Java processes
            pkill -KILL -f \"org.apache.maven\" 2>/dev/null || true
            pkill -KILL -f \"maven-\" 2>/dev/null || true
            pkill -KILL -f \"mvn\" 2>/dev/null || true
            pkill -KILL java 2>/dev/null || true
            pkill -KILL wget 2>/dev/null || true
            pkill -KILL curl 2>/dev/null || true
            
            # Kill any remaining process trees
            for pid in $(pgrep -f \"maven\|mvn\|java\" 2>/dev/null || true); do
                kill -KILL \"$pid\" 2>/dev/null || true
                pkill -KILL -P \"$pid\" 2>/dev/null || true
            done
        " 2>/dev/null || true
        
        # Step 5: Send SIGKILL to container main process
        log_debug "Sending SIGKILL to container main process"
        $CONTAINER_ENGINE kill -s KILL "$CONTAINER_NAME" 2>/dev/null || true
        
        # Step 6: Stop the container with minimal timeout
        if $CONTAINER_ENGINE stop -t 2 "$CONTAINER_NAME" 2>/dev/null; then
            log_debug "Container stopped successfully: $CONTAINER_NAME"
        else
            log_warning "Failed to stop container gracefully, forcing removal: $CONTAINER_NAME"
            # Force remove the container
            $CONTAINER_ENGINE rm -f "$CONTAINER_NAME" 2>/dev/null || true
        fi
    elif [[ -n "$CONTAINER_PID" ]]; then
        log_debug "Stopping container by PID: $CONTAINER_PID"
        log_info "Interrupting container process (PID: $CONTAINER_PID)"
        # Fallback: kill the shell process and any children
        kill -TERM "$CONTAINER_PID" 2>/dev/null || true
        # Give it a moment to terminate gracefully
        sleep 1
        # Force kill if still running
        if kill -0 "$CONTAINER_PID" 2>/dev/null; then
            kill -KILL "$CONTAINER_PID" 2>/dev/null || true
        fi
        log_debug "Container process interrupted: $CONTAINER_PID"
    else
        log_debug "No container name or PID, attempting emergency cleanup"
        # Emergency cleanup: stop any running mcphub containers
        log_warning "No container name or PID available, attempting emergency cleanup"
        local running_containers
        running_containers=$($CONTAINER_ENGINE ps -q --filter "name=mcphub-" 2>/dev/null || true)
        if [[ -n "$running_containers" ]]; then
            log_debug "Found running containers: $running_containers"
            log_info "Found running mcphub containers, stopping them..."
            echo "$running_containers" | xargs -r "$CONTAINER_ENGINE" stop 2>/dev/null || true
        fi
    fi
    
    # Wait for the shell process to finish if it exists
    if [[ -n "$CONTAINER_PID" ]]; then
        wait "$CONTAINER_PID" 2>/dev/null || true
        CONTAINER_PID=""
    fi
    
    CONTAINER_NAME=""
    log_debug "handle_container_interrupt finished"
}

# Maven-specific interrupt handler for execution phase
handle_maven_interrupt() {
    log_debug "handle_maven_interrupt called for Maven operations"
    log_debug "CONTAINER_NAME='${CONTAINER_NAME:-<empty>}'"
    log_debug "CONTAINER_PID='${CONTAINER_PID:-<empty>}'"
    
    if [[ -n "$CONTAINER_NAME" ]]; then
        log_info "Interrupting Maven build: $CONTAINER_NAME"
        
        # Step 1: Immediate signal injection to container main process
        log_debug "Sending immediate SIGTERM to container main process"
        $CONTAINER_ENGINE kill -s TERM "$CONTAINER_NAME" 2>/dev/null || true
        
        # Step 2: Aggressively terminate Maven and download processes
        log_debug "Terminating Maven processes and download operations"
        $CONTAINER_ENGINE exec "$CONTAINER_NAME" sh -c "
            # Target Maven-specific processes first
            pkill -TERM -f \"org.apache.maven.cli.MavenCli\" 2>/dev/null || true
            pkill -TERM -f \"org.apache.maven\" 2>/dev/null || true
            pkill -TERM -f \"maven-dependency\" 2>/dev/null || true
            pkill -TERM -f \"maven-compiler\" 2>/dev/null || true
            
            # Kill network download processes that Maven may spawn
            pkill -TERM -f \"aether\" 2>/dev/null || true
            pkill -TERM -f \"wagon\" 2>/dev/null || true
            pkill -TERM wget 2>/dev/null || true
            pkill -TERM curl 2>/dev/null || true
            
            # Kill all Java HTTP client connections
            pkill -TERM -f \"java.*http\" 2>/dev/null || true
            pkill -TERM -f \"HttpClient\" 2>/dev/null || true
            
            # Kill Maven wrapper and related processes
            pkill -TERM -f \"mvnw\|mvn\" 2>/dev/null || true
            
            # Kill all Java processes as final sweep
            pkill -TERM java 2>/dev/null || true
            
            # Terminate process trees for any Maven-related PIDs
            for pid in $(pgrep -f "maven\|mvn\|java" 2>/dev/null || true); do
                kill -TERM \"$pid\" 2>/dev/null || true
                pkill -TERM -P \"$pid\" 2>/dev/null || true
            done
        " 2>/dev/null || true
        
        # Step 3: Minimal grace period (Maven downloads need fast termination)
        sleep 0.3
        
        # Step 4: Force kill everything immediately
        log_debug "Force killing all remaining Maven processes"
        
        # Send SIGKILL to container main process in parallel
        $CONTAINER_ENGINE kill -s KILL "$CONTAINER_NAME" 2>/dev/null || true &
        
        # Force kill all processes inside container
        $CONTAINER_ENGINE exec "$CONTAINER_NAME" sh -c "
            pkill -KILL -f \"org.apache.maven\" 2>/dev/null || true
            pkill -KILL -f \"maven-\" 2>/dev/null || true
            pkill -KILL -f \"mvn\" 2>/dev/null || true
            pkill -KILL -f \"aether|wagon\" 2>/dev/null || true
            pkill -KILL -f \"HttpClient|http\" 2>/dev/null || true
            pkill -KILL wget 2>/dev/null || true
            pkill -KILL curl 2>/dev/null || true
            pkill -KILL java 2>/dev/null || true
            
            # Force kill any remaining process trees
            for pid in $(pgrep -f \"maven\|mvn\|java\|wget\|curl\" 2>/dev/null || true); do
                kill -KILL \"$pid\" 2>/dev/null || true
                pkill -KILL -P \"$pid\" 2>/dev/null || true
            done
        " 2>/dev/null || true &
        
        # Wait for parallel operations to complete
        wait
        
        # Step 5: Force stop container with minimal timeout
        if $CONTAINER_ENGINE stop -t 1 "$CONTAINER_NAME" 2>/dev/null; then
            log_debug "Maven container stopped successfully: $CONTAINER_NAME"
        else
            log_warning "Forcing Maven container removal: $CONTAINER_NAME"
            $CONTAINER_ENGINE rm -f "$CONTAINER_NAME" 2>/dev/null || true
        fi
    elif [[ -n "$CONTAINER_PID" ]]; then
        log_debug "Stopping Maven container by PID: $CONTAINER_PID"
        log_info "Interrupting Maven process (PID: $CONTAINER_PID)"
        
        # Kill the container process and all children immediately
        kill -TERM "$CONTAINER_PID" 2>/dev/null || true
        sleep 0.2
        kill -KILL "$CONTAINER_PID" 2>/dev/null || true
        
        # Kill all children of this process
        pkill -KILL -P "$CONTAINER_PID" 2>/dev/null || true
    else
        log_debug "No Maven container identifiers, attempting emergency cleanup"
        log_warning "Performing emergency Maven container cleanup"
        
        # Emergency cleanup for .NET containers
        local dotnet_containers
        dotnet_containers=$($CONTAINER_ENGINE ps -q --filter "name=mcphub-" 2>/dev/null || true)
        if [[ -n "$dotnet_containers" ]]; then
            log_info "Found .NET containers, force stopping..."
            echo "$dotnet_containers" | xargs -r "$CONTAINER_ENGINE" kill -s KILL 2>/dev/null || true
            echo "$dotnet_containers" | xargs -r "$CONTAINER_ENGINE" rm -f 2>/dev/null || true
        fi
    fi
    
    # Clean up container tracking variables
    if [[ -n "$CONTAINER_PID" ]]; then
        wait "$CONTAINER_PID" 2>/dev/null || true
        CONTAINER_PID=""
    fi
    
    CONTAINER_NAME=""
    log_debug "handle_maven_interrupt finished"
}

# Container cleanup on exit
cleanup_container_on_exit() {
    if [[ "$CONTAINER_CLEANUP_ON_EXIT" == "true" ]]; then
        handle_container_interrupt
        container_cleanup_dangling
    fi
}

# Container validation functions (moved from validation.sh to resolve circular dependency)

# Get tool versions from container
__get_container_tool_versions() {
    local image="$1"
    local container_engine="${2:-$CONTAINER_ENGINE}"
    
    if [[ -z "$image" ]]; then
        log_error "Container image required for tool version validation"
        return "$USAGE_ERROR"
    fi
    
    # Validate container engine
    if [[ "$container_engine" != "docker" && "$container_engine" != "podman" ]]; then
        log_error "Invalid container engine: $container_engine (must be docker or podman)"
        return "$USAGE_ERROR"
    fi
    
    # Validate image name format (basic check)
    if ! [[ "$image" =~ ^[a-zA-Z0-9][a-zA-Z0-9._/-]*[a-zA-Z0-9]*(:[a-zA-Z0-9._-]+)?$ ]]; then
        log_error "Invalid container image format: $image"
        return "$USAGE_ERROR"
    fi
    
    local validation_script="
    echo \"DotNet: \$(dotnet --version 2>/dev/null || echo 'not found')\"
    echo \"DotNet EF: \$(dotnet ef --version 2>/dev/null | head -1 | awk '{print \$5}' || echo 'not found')\"
    echo \"PostgreSQL: \$(psql --version 2>/dev/null | awk '{print \$3}' || echo 'not found')\"
    echo \"Redis: \$(redis-cli --version 2>/dev/null | awk '{print \$2}' || echo 'not found')\"
    echo \"Git: \$(git --version 2>/dev/null | awk '{print \$3}' || echo 'not found')\"
    echo \"Curl: \$(curl --version 2>/dev/null | head -1 | awk '{print \$2}' || echo 'not found')\"
    echo \"JQ: \$(jq --version 2>/dev/null || echo 'not found')\"
    "
    
    if ! $container_engine run --rm "$image" sh -c "$validation_script" 2>/dev/null; then
        log_error "Failed to get tool versions from container: $image"
        return "$CONTAINER_ERROR"
    fi
}

# Validate comprehensive tool versions in container
__validate_container_tool_versions() {
    local image="$1"
    local container_engine="${2:-$CONTAINER_ENGINE}"
    
    log_info "Validating tool versions in container: $image"
    
    local validation_output
    if ! validation_output="$(__get_container_tool_versions "$image" "$container_engine")"; then
        return "$CONTAINER_ERROR"
    fi
    
    local errors=0
    
    # Check for required tools
    if ! echo "$validation_output" | grep -q "DotNet:.*[0-9]"; then
        log_error "✗ .NET SDK not found or invalid version"
        ((errors++))
    else
        log_success "✓ .NET SDK available"
    fi
    
    if ! echo "$validation_output" | grep -q "Git:.*[0-9]"; then
        log_error "✗ Git not found or invalid version"
        ((errors++))
    else
        log_success "✓ Git available"
    fi
    
    # Check for optional tools (don't increment error count)
    if echo "$validation_output" | grep -q "DotNet EF:.*[0-9]"; then
        log_success "✓ Entity Framework CLI available"
    else
        log_info "○ Entity Framework CLI not found (optional)"
    fi
    
    if echo "$validation_output" | grep -q "PostgreSQL:.*[0-9]"; then
        log_success "✓ PostgreSQL client available"
    else
        log_info "○ PostgreSQL client not found (optional)"
    fi
    
    if echo "$validation_output" | grep -q "Redis:.*[0-9]"; then
        log_success "✓ Redis client available"
    else
        log_info "○ Redis client not found (optional)"
    fi
    
    if echo "$validation_output" | grep -q "Curl:.*[0-9]"; then
        log_success "✓ Curl available"
    else
        log_info "○ Curl not found (optional)"
    fi
    
    if echo "$validation_output" | grep -q "JQ:.*[0-9]"; then
        log_success "✓ JQ available"
    else
        log_info "○ JQ not found (optional)"
    fi
    
    if [[ $errors -eq 0 ]]; then
        log_success "All required tools validated successfully"
        return "$SUCCESS"
    else
        log_error "Tool validation failed ($errors errors)"
        return "$VALIDATION_ERROR"
    fi
}

validate_container_prerequisites() {
    local image="$1"
    local container_engine="${2:-$CONTAINER_ENGINE}"
    
    log_info "Validating container prerequisites"
    
    # Check container engine
    if ! command -v "$container_engine" >/dev/null 2>&1; then
        log_error "Container engine not found: $container_engine"
        return "$PREREQUISITE_ERROR"
    fi
    
    # Set container engine for container-helper.sh functions
    local original_engine="$CONTAINER_ENGINE"
    CONTAINER_ENGINE="$container_engine"
    
    # Check container exists
    if ! check_container_exists "$image"; then
        log_error "Container image not found: $image"
        CONTAINER_ENGINE="$original_engine"
        return "$CONTAINER_ERROR"
    fi
    
    # Check container health
    if ! check_container_health "$image"; then
        log_error "Container health check failed: $image"
        CONTAINER_ENGINE="$original_engine"
        return "$CONTAINER_ERROR"
    fi
    
    # Restore original container engine
    CONTAINER_ENGINE="$original_engine"
    
    # Validate tools in container
    if ! __validate_container_tool_versions "$image" "$container_engine"; then
        log_error "Container tool validation failed: $image"
        return "$VALIDATION_ERROR"
    fi
    
    log_success "Container prerequisites validated successfully"
    return "$SUCCESS"
}

test_container_engine() {
    log_info "Testing container engine detection"
    
    local engine
    engine="$(ensure_container_engine)"
    local result=$?
    
    if [[ $result -eq 0 ]]; then
        log_success "✓ Container Engine: Detected $engine"
        return "$SUCCESS"
    else
        log_error "✗ Container Engine: Detection failed"
        return "$PREREQUISITE_ERROR"
    fi
}

test_container_existence() {
    local image="$1"
    local container_engine="${2:-$CONTAINER_ENGINE}"
    
    log_info "Testing container existence: $image"
    
    # Set container engine for container-helper.sh functions
    local original_engine="$CONTAINER_ENGINE"
    CONTAINER_ENGINE="$container_engine"
    
    local result
    if check_container_exists "$image"; then
        log_success "✓ Container Existence: Image found: $image"
        result="$SUCCESS"
    else
        log_error "✗ Container Existence: Image not found: $image"
        result="$CONTAINER_ERROR"
    fi
    
    # Restore original container engine
    CONTAINER_ENGINE="$original_engine"
    return "$result"
}

# Test container health
test_container_health() {
    local image="$1"
    local container_engine="${2:-$CONTAINER_ENGINE}"
    
    log_info "Testing container health: $image"
    
    # Set container engine for container-helper.sh functions
    local original_engine="$CONTAINER_ENGINE"
    CONTAINER_ENGINE="$container_engine"
    
    local result
    if check_container_health "$image"; then
        log_success "✓ Container Health: Health check passed"
        result="$SUCCESS"
    else
        log_error "✗ Container Health: Health check failed"
        result="$CONTAINER_ERROR"
    fi
    
    # Restore original container engine
    CONTAINER_ENGINE="$original_engine"
    return "$result"
}