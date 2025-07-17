#!/bin/bash

# cache.sh - Simple cache management for Maven dependencies
# Version: 2.0
# Description: Lightweight cache management with single cache directory

if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    echo "Error: cache.sh should be sourced, not executed directly"
    exit 1
fi
if [[ -n "${CACHE_HELPER:-}" ]]; then
    return 0
fi
export CACHE_HELPER=1

# shellcheck source=.scripts/helpers/error.sh
source "helpers/error.sh"
# shellcheck source=.scripts/helpers/log.sh
source "helpers/log.sh"
# shellcheck source=.scripts/helpers/platform.sh
source "helpers/platform.sh"

# Cache configuration
CACHE_ENABLED="${CACHE_ENABLED:-true}"
CACHE_TTL_DAYS="${CACHE_TTL_DAYS:-30}"

# Initialize cache system
cache_init() {
    log_debug "Initializing cache system"
    
    # Set cache root directory
    if [[ -z "${CACHE_ROOT:-}" ]]; then
        CACHE_ROOT="${SCRIPT_PATH:-.}/.cache"
    fi
    
    # Create cache directory
    if ! create_directory "$CACHE_ROOT"; then
        log_error "Failed to create cache directory: $CACHE_ROOT"
        return "$CANNOT_CREATE_FOLDER"
    fi
    
    # Create Maven repository subdirectory
    if ! create_directory "$CACHE_ROOT/repository"; then
        log_error "Failed to create Maven repository directory: $CACHE_ROOT/repository"
        return "$CANNOT_CREATE_FOLDER"
    fi
    
    log_debug "Cache system initialized: $CACHE_ROOT"
    return "$SUCCESS"
}

# Clean cache
cache_clean() {
    local force="${1:-false}"
    
    if [[ "$CACHE_ENABLED" != "true" ]]; then
        log_info "Cache disabled, skipping cleanup"
        return "$SUCCESS"
    fi
    
    if [[ ! -d "$CACHE_ROOT" ]]; then
        log_info "Cache directory does not exist: $CACHE_ROOT"
        return "$SUCCESS"
    fi
    
    # Get cache size before cleaning
    local size_before
    size_before=$(cache_get_size "")
    
    log_info "Cleaning cache: $CACHE_ROOT"
    
    if [[ "$force" == "true" ]]; then
        # Force clean - remove everything
        rm -rf "${CACHE_ROOT:?}"/* 2>/dev/null || {
            log_warning "Failed to clean cache completely"
            return "$IO_ERROR"
        }
    else
        # Smart cleanup - remove old snapshots and failed downloads
        if command -v find >/dev/null 2>&1; then
            # Remove snapshot artifacts older than TTL
            find "$CACHE_ROOT" -name "*SNAPSHOT*" -mtime +"$CACHE_TTL_DAYS" -delete 2>/dev/null || true
            # Remove failed download markers
            find "$CACHE_ROOT" -name "*.lastUpdated" -delete 2>/dev/null || true
            find "$CACHE_ROOT" -name "_remote.repositories" -delete 2>/dev/null || true
        fi
    fi
    
    local size_after
    size_after=$(cache_get_size "")
    
    log_success "Cache cleaned: $size_before -> $size_after"
    return "$SUCCESS"
}

# Get cache size (see legacy compatibility section for full implementation)

# Get cache status
cache_get_status() {
    if [[ ! -d "$CACHE_ROOT" ]]; then
        echo "not_initialized"
    elif [[ ! -r "$CACHE_ROOT" || ! -w "$CACHE_ROOT" ]]; then
        echo "permission_error"
    else
        echo "healthy"
    fi
}

# Check cache health
cache_health() {
    log_info "Checking cache health"
    
    local issues=0
    
    # Check if cache directory exists and is accessible
    if [[ ! -d "$CACHE_ROOT" ]]; then
        log_warning "Cache directory does not exist: $CACHE_ROOT"
        ((issues++))
    elif [[ ! -r "$CACHE_ROOT" || ! -w "$CACHE_ROOT" ]]; then
        log_warning "Cache directory has permission issues: $CACHE_ROOT"
        ((issues++))
    fi
    
    # Check disk space
    if command -v df >/dev/null 2>&1; then
        local available_space
        available_space=$(df "$CACHE_ROOT" 2>/dev/null | awk 'NR==2 {print $4}' || echo "0")
        local min_space=1048576  # 1GB in KB
        
        if [[ "$available_space" -lt "$min_space" ]]; then
            log_warning "Low disk space available for cache: ${available_space}KB"
            ((issues++))
        fi
    fi
    
    # Report results
    if [[ $issues -eq 0 ]]; then
        log_success "Cache health check passed"
    else
        log_error "Cache health check failed with $issues issues"
    fi
    
    return "$issues"
}

# Show cache status
cache_status() {
    echo "Cache Status"
    echo "============"
    echo "Enabled: $CACHE_ENABLED"
    echo "Directory: ${CACHE_ROOT:-not set}"
    echo "Size: $(cache_get_size "")"
    echo "Status: $(cache_get_status)"
    echo "TTL Days: $CACHE_TTL_DAYS"
    
    if [[ -d "$CACHE_ROOT" ]]; then
        echo "Repository: $CACHE_ROOT/repository"
        if command -v find >/dev/null 2>&1; then
            local file_count
            file_count=$(find "$CACHE_ROOT" -type f 2>/dev/null | wc -l || echo "0")
            echo "Files: $file_count"
        fi
    fi
}

# Legacy compatibility functions for existing code
cache_init_type() {
    local cache_type="${1:-}"
    
    if [[ "$cache_type" == "maven" ]]; then
        cache_init
    else
        log_warning "Cache type '$cache_type' not supported in simplified cache system"
        return "$SUCCESS"
    fi
}

cache_clean_type() {
    local cache_type="${1:-}"
    local force="${2:-false}"
    
    if [[ "$cache_type" == "maven" ]]; then
        cache_clean "$force"
    else
        log_warning "Cache type '$cache_type' not supported in simplified cache system"
        return "$SUCCESS"
    fi
}

cache_get_size() {
    local cache_type="${1:-}"
    
    if [[ -n "$cache_type" && "$cache_type" != "maven" ]]; then
        # For non-maven cache types, return 0 (legacy compatibility)
        echo "0"
    else
        # For maven or no cache type specified, return actual size
        if [[ ! -d "$CACHE_ROOT" ]]; then
            echo "0"
            return
        fi
        
        if command -v du >/dev/null 2>&1; then
            du -sh "$CACHE_ROOT" 2>/dev/null | cut -f1 || echo "0"
        else
            echo "unknown"
        fi
    fi
}

cache_get_total_size() {
    cache_get_size ""
}

cache_get_type_status() {
    local cache_type="${1:-}"
    
    if [[ -n "$cache_type" && "$cache_type" != "maven" ]]; then
        # For non-maven cache types, return not_initialized (legacy compatibility)
        echo "not_initialized"
    else
        cache_get_status
    fi
}

log_debug "Cache management helper loaded successfully"