#!/bin/bash

# output.sh - Colorized output utility functions
# Version: 1.0
# Description: Provides standardized colorized output functions for consistent CLI presentation

if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    echo "Error: output.sh should be sourced, not executed directly"
    exit 1
fi
if [[ -n "${OUTPUT_HELPER:-}" ]]; then
    return 0
fi
export OUTPUT_HELPER=1

# Color definitions - ANSI escape codes for terminal colors
readonly _RED='\033[0;31m'
readonly _GREEN='\033[0;32m'
readonly _YELLOW='\033[1;33m'
readonly _BLUE='\033[0;34m'
readonly _PURPLE='\033[0;35m'
readonly _CYAN='\033[0;36m'
readonly _WHITE='\033[1;37m'
readonly _BOLD='\033[1m'
readonly _DIM='\033[2m'
readonly _NO_COLOR='\033[0m'  # No Color/Reset

# Raw colorized output functions (no level control, no prefixes)
output_red() {
    echo -e "${_RED}$1${_NO_COLOR}"
}

output_green() {
    echo -e "${_GREEN}$1${_NO_COLOR}"
}

output_yellow() {
    echo -e "${_YELLOW}$1${_NO_COLOR}"
}

output_blue() {
    echo -e "${_BLUE}$1${_NO_COLOR}"
}

output_purple() {
    echo -e "${_PURPLE}$1${_NO_COLOR}"
}

output_cyan() {
    echo -e "${_CYAN}$1${_NO_COLOR}"
}

output_white() {
    echo -e "${_WHITE}$1${_NO_COLOR}"
}

output_bold() {
    echo -e "${_BOLD}$1${_NO_COLOR}"
}

output_dim() {
    echo -e "${_DIM}$1${_NO_COLOR}"
}

# Progress indicators
output_spinner() {
    local pid="$1"
    local delay=0.1
    local spinstr="|/-\\"
    while ps -p "$pid" > /dev/null 2>&1; do
        for i in $(seq 0 3); do
            printf "\r${_BLUE}[%c]${_NO_COLOR} Working..." "${spinstr:$i:1}"
            sleep $delay
        done
    done
    printf "\r"
}

output_progress_bar() {
    local current="$1"
    local total="$2"
    local width="${3:-50}"
    
    # Input validation
    if [[ -z "$current" || -z "$total" ]]; then
        echo "ERROR: current and total parameters required for progress bar" >&2
        return "$USAGE_ERROR"
    fi
    
    # Validate numeric format
    if ! [[ "$current" =~ ^[0-9]+$ ]] || ! [[ "$total" =~ ^[0-9]+$ ]]; then
        echo "ERROR: current and total must be numeric" >&2
        return "$USAGE_ERROR"
    fi
    
    # Validate width if provided
    if ! [[ "$width" =~ ^[0-9]+$ ]] || [[ "$width" -lt 1 ]]; then
        echo "ERROR: width must be a positive number" >&2
        return "$USAGE_ERROR"
    fi
    
    # Prevent division by zero
    if [[ "$total" -eq 0 ]]; then
        echo "ERROR: total cannot be zero for progress calculation" >&2
        return "$USAGE_ERROR"
    fi
    
    # Validate current <= total
    if [[ "$current" -gt "$total" ]]; then
        echo "ERROR: current ($current) cannot exceed total ($total)" >&2
        return "$USAGE_ERROR"
    fi
    
    local percent=$((current * 100 / total))
    local filled=$((width * current / total))
    local empty=$((width - filled))
    
    printf "\n%s[" "$_BLUE"
    printf "%*s" "$filled" '#'
    printf "%s" "$_RED"
    printf "%*s" "$empty" '-'
    printf "] %d%%${_NO_COLOR}" "$percent"
}

# Table output helpers
output_table_row() {
    local col1="$1"
    local col2="$2" 
    local col3="$3"
    printf "%-20s %-30s %s\n" "$col1" "$col2" "$col3"
}

output_table_header() {
    output_bold "$(output_table_row "$1" "$2" "$3")"
    output_horizontal_line "-" 60
}

# Status indicators with symbols
output_check() {
    echo -e "${_GREEN}✓${_NO_COLOR} $1"
}

output_cross() {
    echo -e "${_RED}✗${_NO_COLOR} $1"
}

output_arrow() {
    echo -e "${_BLUE}→${_NO_COLOR} $1"
}

output_bullet() {
    echo -e "${_CYAN}•${_NO_COLOR} $1"
}

# Step indicator for multi-step processes
output_step() {
    local step_num="$1"
    local step_desc="$2"
    echo -e "${_BOLD}[$step_num]${_NO_COLOR} $step_desc"
}

# Section header with customizable character
output_section_header() {
    local title="$1"
    local char="${2:-=}"
    local length=${#title}
    local line=""
    
    # Build separator line
    for ((i=0; i<length+4; i++)); do
        line+="$char"
    done
    
    echo -e "${_BOLD}$line${_NO_COLOR}"
    echo -e "${_BOLD}  $title${_NO_COLOR}"
    echo -e "${_BOLD}$line${_NO_COLOR}"
}

# Horizontal line for tables and separation
output_horizontal_line() {
    local char="${1:--}"
    local width="${2:-50}"
    local line=""
    
    for ((i=0; i<width; i++)); do
        line+="$char"
    done
    
    echo "$line"
}
