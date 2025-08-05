# ============================================================================
# Logger.ps1 - Centralized logging utilities for Claude Code Setup Script
# ============================================================================

function Write-Success {
    param([string]$Message)
    try {
        Write-Host "[SUCCESS] $Message" -ForegroundColor Green
    } catch {
        Write-Output "[SUCCESS] $Message"
    }
}

function Write-Error {
    param([string]$Message)
    try {
        Write-Host "[ERROR] $Message" -ForegroundColor Red
    } catch {
        Write-Output "[ERROR] $Message"
    }
}

function Write-Warning {
    param([string]$Message)
    try {
        Write-Host "[WARNING] $Message" -ForegroundColor Yellow
    } catch {
        Write-Output "[WARNING] $Message"
    }
}

function Write-Info {
    param([string]$Message)
    try {
        Write-Host "[INFO] $Message" -ForegroundColor Cyan
    } catch {
        Write-Output "[INFO] $Message"
    }
}

function Write-Debug {
    param([string]$Message, [bool]$DebugEnabled = $false)
    if ($DebugEnabled) {
        Write-Output "[DEBUG] $Message"
    }
}