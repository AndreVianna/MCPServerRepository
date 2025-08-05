# ============================================================================
# Claude Code Setup Script for Windows (PowerShell Version)
# This script handles API key configuration and automatically configures MCP servers
# ============================================================================

param(
    [switch]$Debug,
    [switch]$Quiet
)

# ========== INITIALIZATION ==========

# Import utilities
. "$PSScriptRoot\Logger.ps1"

# Get project root directory (two levels up from script location)
$ProjectRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)

# Configuration files
$EnvFile = Join-Path $ProjectRoot ".env"
$McpsFile = Join-Path $ProjectRoot ".claude\mcps.json"
$LocalMcpsFile = Join-Path $ProjectRoot ".claude\mcps.local.json"

# Script state variables
$ExistingServers = @()
$DefinedServers = @()

# ========== MAIN EXECUTION ==========

function Main {
    Write-Debug "Script started with Debug=$Debug" -DebugEnabled $Debug
    Write-Debug "ProjectRoot=$ProjectRoot" -DebugEnabled $Debug
    Write-Debug "McpsFile=$McpsFile" -DebugEnabled $Debug
    Write-Debug "LocalMcpsFile=$LocalMcpsFile" -DebugEnabled $Debug

    try {
        # Phase 1: Check dependencies
        Test-Dependencies

        # Phase 2: Check Docker/Podman
        Test-DockerPodman

        # Phase 3: Configure MCP servers
        Initialize-McpServers

        # Phase 4: Clean up orphaned servers
        Remove-OrphanedServers

        if (-not $Quiet) {
            Write-Success "Setup completed successfully."
        }
        exit 0
    }
    catch {
        Write-Error "Setup failed: $($_.Exception.Message)"
        exit 1
    }
}

# ========== PHASE 1: DEPENDENCY CHECKS ==========

function Test-Dependencies {
    Write-Debug "Checking dependencies" -DebugEnabled $Debug

    # Check tree command (always available on Windows)
    Write-Debug "Checking 'tree' command availability" -DebugEnabled $Debug
    if (Get-Command tree -ErrorAction SilentlyContinue) {
        Write-Debug "'tree' command is available" -DebugEnabled $Debug
        if (-not $Quiet) {
            Write-Success "'tree' command is configured correctly"
        }
    } else {
        if (-not $Quiet) {
            Write-Error "'tree' command not found!"
            Write-Warning "This is unusual for Windows. Please check your PATH environment variable."
        }
        throw "Tree command not available"
    }

    # Check Claude CLI availability
    if (-not (Get-Command claude -ErrorAction SilentlyContinue)) {
        if (-not $Quiet) {
            Write-Error "Claude CLI not available!"
        }
        throw "Claude CLI not available"
    }
}

# ========== PHASE 2: DOCKER/PODMAN CHECK ==========

function Test-DockerPodman {
    Write-Debug "Checking Docker/Podman installation and configuration" -DebugEnabled $Debug

    # Try Docker first
    if (Get-Command docker -ErrorAction SilentlyContinue) {
        Write-Debug "Docker is installed" -DebugEnabled $Debug
        try {
            docker version | Out-Null
            Write-Debug "Docker is working correctly" -DebugEnabled $Debug
            if (-not $Quiet) {
                Write-Success "Docker is configured correctly"
            }
            return
        }
        catch {
            if (-not $Quiet) {
                Write-Warning "Docker is installed but not running properly"
                Write-Info "Try starting Docker Desktop application"
            }
        }
    }

    # Try Podman as alternative
    if (Get-Command podman -ErrorAction SilentlyContinue) {
        Write-Debug "Podman is installed" -DebugEnabled $Debug
        try {
            podman version | Out-Null
            Write-Debug "Podman is working correctly" -DebugEnabled $Debug
            Write-Debug "Setting up 'docker' alias for podman" -DebugEnabled $Debug
            # Note: PowerShell aliases are session-specific, unlike CMD doskey
            Set-Alias -Name docker -Value podman -Scope Global
            if (-not $Quiet) {
                Write-Success "Podman is configured correctly with docker alias"
            }
            return
        }
        catch {
            Write-Debug "Podman installed but not working, attempting to fix" -DebugEnabled $Debug
            Initialize-Podman
            return
        }
    }

    if (-not $Quiet) {
        Write-Error "Neither Docker nor Podman found or working!"
        Write-Warning "Docker or Podman is required for integration testing and containerized workflows!"
        Write-Info "Please install Docker Desktop: https://www.docker.com/products/docker-desktop/"
        Write-Info "Or install Podman: https://github.com/containers/podman/blob/main/docs/tutorials/podman-for-windows.md"
    }
    throw "Container runtime not available"
}

function Initialize-Podman {
    Write-Debug "Attempting to fix Podman configuration" -DebugEnabled $Debug
    
    try {
        $machines = podman machine list 2>$null
        if ($machines -match "default") {
            Write-Debug "Default machine exists, checking if running" -DebugEnabled $Debug
            if ($machines -notmatch "default.*running") {
                if (-not $Quiet) {
                    Write-Info "Starting Podman machine"
                }
                podman machine start 2>&1 | Out-Null
                if ($LASTEXITCODE -eq 0) {
                    if (-not $Quiet) {
                        Write-Success "Successfully started Podman machine"
                    }
                    Start-Sleep -Seconds 3
                    podman version | Out-Null
                    if ($LASTEXITCODE -eq 0) {
                        Write-Debug "Setting up 'docker' alias for podman" -DebugEnabled $Debug
                        Set-Alias -Name docker -Value podman -Scope Global
                        if (-not $Quiet) {
                            Write-Success "Podman is now working correctly with docker alias"
                        }
                        return
                    }
                }
            }
        } else {
            if (-not $Quiet) {
                Write-Info "Creating default Podman machine"
            }
            podman machine init 2>&1 | Out-Null
            if ($LASTEXITCODE -eq 0) {
                podman machine start 2>&1 | Out-Null
                if ($LASTEXITCODE -eq 0) {
                    if (-not $Quiet) {
                        Write-Success "Successfully created and started Podman machine"
                    }
                    Start-Sleep -Seconds 3
                    podman version | Out-Null
                    if ($LASTEXITCODE -eq 0) {
                        Write-Debug "Setting up 'docker' alias for podman" -DebugEnabled $Debug
                        Set-Alias -Name docker -Value podman -Scope Global
                        if (-not $Quiet) {
                            Write-Success "Podman is now working correctly with docker alias"
                        }
                        return
                    }
                }
            }
        }
    }
    catch {
        # Ignore errors in machine list check
    }

    if (-not $Quiet) {
        Write-Error "Failed to fix Podman configuration"
    }
    throw "Podman initialization failed"
}

# ========== PHASE 3: MCP SERVER CONFIGURATION ==========

function Initialize-McpServers {
    Write-Debug "Starting MCP server configuration" -DebugEnabled $Debug

    # Get existing servers
    Write-Debug "Reading existing MCP servers" -DebugEnabled $Debug
    Get-ExistingServers

    # Configure project servers
    if (Test-Path $McpsFile) {
        Write-Debug "Reading Project MCP server configuration from '$McpsFile'" -DebugEnabled $Debug
        Add-ServersFromFile -ConfigFile $McpsFile -ConfigType "project"
        if (-not $Quiet) {
            Write-Success "Project MCP servers configured correctly"
        }
    } else {
        if (-not $Quiet) {
            Write-Warning "File not found: $McpsFile. Skipping project configuration."
        }
    }

    # Configure local servers
    if (Test-Path $LocalMcpsFile) {
        Write-Debug "Reading Local MCP server configuration from '$LocalMcpsFile'" -DebugEnabled $Debug
        Add-ServersFromFile -ConfigFile $LocalMcpsFile -ConfigType "local"
        if (-not $Quiet) {
            Write-Success "Local MCP servers configured correctly"
        }
    } else {
        if (-not $Quiet) {
            Write-Warning "File not found: $LocalMcpsFile. Skipping local configuration."
        }
    }
}

function Get-ExistingServers {
    $script:ExistingServers = @()
    try {
        $output = claude mcp list 2>$null
        if ($output) {
            $servers = $output | Select-String "^[a-zA-Z0-9_-]*:" | ForEach-Object { 
                $_.Line.Split(':')[0] 
            }
            $script:ExistingServers = @($servers)
        }
    }
    catch {
        # Ignore errors - empty list is fine
    }
    Write-Debug "Existing servers: $($ExistingServers -join ' ')" -DebugEnabled $Debug
}

function Add-ServersFromFile {
    param(
        [string]$ConfigFile,
        [string]$ConfigType
    )

    try {
        $json = Get-Content $ConfigFile -Raw | ConvertFrom-Json
        $serverNames = $json.PSObject.Properties.Name

        foreach ($serverName in $serverNames) {
            if (-not [string]::IsNullOrWhiteSpace($serverName)) {
                # Add to defined servers list
                $script:DefinedServers += $serverName
                
                Add-McpServer -ServerName $serverName -ConfigFile $ConfigFile -ServerType $ConfigType
            }
        }
    }
    catch {
        if (-not $Quiet) {
            Write-Error "Failed to parse JSON from config file: $ConfigFile"
        }
        throw "JSON parsing failed"
    }
}

function Add-McpServer {
    param(
        [string]$ServerName,
        [string]$ConfigFile,
        [string]$ServerType
    )

    Write-Debug "Processing $ServerType server: '$ServerName'" -DebugEnabled $Debug

    # Check if server already exists and remove it
    if ($ExistingServers -contains $ServerName) {
        Write-Debug "Updating server '$ServerName'" -DebugEnabled $Debug
        claude mcp remove $ServerName 2>&1 | Out-Null
    } else {
        Write-Debug "Server '$ServerName' does not exist, adding" -DebugEnabled $Debug
    }

    # Get server configuration
    Write-Debug "Extracting configuration for server '$ServerName'" -DebugEnabled $Debug
    try {
        $configOutput = & powershell -ExecutionPolicy Bypass -File "$PSScriptRoot\McpConfigParser.ps1" -serverName $ServerName -mcpsFile $ConfigFile -projectRoot $ProjectRoot 2>&1
        
        $serverConfig = ""
        $parseError = ""
        
        foreach ($line in $configOutput) {
            if ($line -like "ERROR*") {
                $parseError = $line
            } else {
                $serverConfig = $line
            }
        }

        # Check for PowerShell parsing errors
        if ($parseError) {
            if (-not $Quiet) {
                Write-Error "PowerShell config parsing failed for server '$ServerName': $parseError"
            }
            throw "Config parsing failed"
        }

        if (-not $serverConfig) {
            if (-not $Quiet) {
                Write-Error "No configuration found for server '$ServerName'"
            }
            throw "No configuration found"
        }
    }
    catch {
        throw "Failed to extract server configuration: $($_.Exception.Message)"
    }

    # Add the server
    Write-Debug "Adding server $ServerName" -DebugEnabled $Debug
    Write-Debug "Server config: '$serverConfig'" -DebugEnabled $Debug

    try {
        # Use the exact same approach as the working CMD script - escape quotes and use cmd.exe
        $escapedConfig = $serverConfig -replace '"', '\"'
        
        if ($Debug) {
            Write-Debug "Executing: claude mcp add-json `"$ServerName`" `"$escapedConfig`" --debug" -DebugEnabled $Debug
            # Use cmd.exe like the working CMD script to avoid PowerShell quote handling issues
            $result = cmd.exe /c "claude mcp add-json `"$ServerName`" `"$escapedConfig`" --debug" 2>&1
        } else {
            $result = cmd.exe /c "claude mcp add-json `"$ServerName`" `"$escapedConfig`"" 2>&1
        }

        if ($LASTEXITCODE -eq 0) {
            Write-Debug "Successfully added or updated server '$ServerName'" -DebugEnabled $Debug
        } else {
            if (-not $Quiet) {
                Write-Error "Failed to add server '$ServerName' (exit code: $LASTEXITCODE)"
                Write-Error "Server config was: '$serverConfig'"
                if ($result) {
                    Write-Error "Command output: $($result -join "`n")"
                }
            }
            throw "Server addition failed"
        }
    }
    catch {
        throw "Failed to add MCP server: $($_.Exception.Message)"
    }
}

# ========== PHASE 4: CLEANUP ORPHANED SERVERS ==========

function Remove-OrphanedServers {
    Write-Debug "Starting cleanup of orphaned MCP servers" -DebugEnabled $Debug

    foreach ($server in $ExistingServers) {
        Write-Debug "Checking if server '$server' is defined" -DebugEnabled $Debug
        if ($DefinedServers -notcontains $server) {
            if (-not $Quiet) {
                Write-Warning "Server '$server' is orphaned. Removing..."
            }
            try {
                claude mcp remove $server 2>&1 | Out-Null
                if ($LASTEXITCODE -eq 0) {
                    if (-not $Quiet) {
                        Write-Success "Successfully removed orphaned server '$server'"
                    }
                } else {
                    if (-not $Quiet) {
                        Write-Error "Failed to remove orphaned server '$server'"
                    }
                    throw "Failed to remove orphaned server"
                }
            }
            catch {
                throw "Orphaned server removal failed: $($_.Exception.Message)"
            }
        } else {
            Write-Debug "Server '$server' is defined. Keeping." -DebugEnabled $Debug
        }
    }

    Write-Debug "Orphaned MCP server cleanup completed" -DebugEnabled $Debug
}

# ========== SCRIPT ENTRY POINT ==========

Main