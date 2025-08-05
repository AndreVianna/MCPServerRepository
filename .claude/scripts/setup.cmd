@echo off
setlocal enabledelayedexpansion

REM ============================================================================
REM Claude Code Setup Script for Windows (Cleaned Up Version)
REM This script handles API key configuration and automatically configures MCP servers
REM ============================================================================

REM ========== INITIALIZATION ==========

REM Get project root directory (two levels up from script location)
set "SCRIPT_DIR=%~dp0"
pushd "%SCRIPT_DIR%\..\.."
set "PROJECT_ROOT=%CD%"
popd

REM Configuration files
set "ENV_FILE=%PROJECT_ROOT%\.env"
set "MCPS_FILE=%PROJECT_ROOT%\.claude\mcps.json"
set "LOCAL_MCPS_FILE=%PROJECT_ROOT%\.claude\mcps.local.json"

REM Script state variables
set "DEBUG=0"
set "QUIET=0"
set "EXISTING_SERVERS="
set "DEFINED_SERVERS="

REM Parse command line arguments
if "%~1"=="--debug" set "DEBUG=1"
if "%~1"=="-d" set "DEBUG=1"
if "%~2"=="--debug" set "DEBUG=1"
if "%~2"=="-d" set "DEBUG=1"
if "%~1"=="--quiet" set "QUIET=1"
if "%~1"=="-q" set "QUIET=1"
if "%~2"=="--quiet" set "QUIET=1"
if "%~2"=="-q" set "QUIET=1"

goto :main

REM ========== OUTPUT HELPER FUNCTIONS ==========

:echo_success
powershell -ExecutionPolicy Bypass -Command "Write-Host '[SUCCESS] %*' -ForegroundColor Green" 2>nul || echo [SUCCESS] %*
exit /b 0

:echo_error
powershell -ExecutionPolicy Bypass -Command "Write-Host '[ERROR] %*' -ForegroundColor Red" 2>nul || echo [ERROR] %*
exit /b 0

:echo_warning
powershell -ExecutionPolicy Bypass -Command "Write-Host '[WARNING] %*' -ForegroundColor Yellow" 2>nul || echo [WARNING] %*
exit /b 0

:echo_info
powershell -ExecutionPolicy Bypass -Command "Write-Host '[INFO] %*' -ForegroundColor Cyan" 2>nul || echo [INFO] %*
exit /b 0

:debug_msg
if "%DEBUG%"=="1" echo [DEBUG] %*
exit /b 0

REM ========== MAIN EXECUTION ==========

:main
call :debug_msg Script started with DEBUG=%DEBUG%
call :debug_msg PROJECT_ROOT=%PROJECT_ROOT%
call :debug_msg MCPS_FILE=%MCPS_FILE%
call :debug_msg LOCAL_MCPS_FILE=%LOCAL_MCPS_FILE%

REM Phase 1: Check dependencies
call :check_dependencies
if %ERRORLEVEL% neq 0 exit /b 1

REM Phase 2: Check Docker/Podman
call :check_docker
if %ERRORLEVEL% neq 0 exit /b 1

REM Phase 3: Configure MCP servers
call :configure_mcp_servers
if %ERRORLEVEL% neq 0 exit /b 1

REM Phase 4: Clean up orphaned servers
call :cleanup_orphaned_servers
if %ERRORLEVEL% neq 0 exit /b 1

if not "%QUIET%"=="1" call :echo_success Setup completed successfully.
exit /b 0

REM ========== PHASE 1: DEPENDENCY CHECKS ==========

:check_dependencies
call :debug_msg Checking dependencies

REM Check tree command (always available on Windows)
call :debug_msg Checking 'tree' command availability
where tree >nul 2>&1
if %ERRORLEVEL%==0 (
    call :debug_msg 'tree' command is available
    if not "%QUIET%"=="1" call :echo_success 'tree' command is configured correctly
) else (
    if not "%QUIET%"=="1" call :echo_error 'tree' command not found!
    if not "%QUIET%"=="1" call :echo_warning This is unusual for Windows. Please check your PATH environment variable.
    exit /b 1
)

REM Check Claude CLI availability
where claude >nul 2>&1
if %ERRORLEVEL% neq 0 (
    if not "%QUIET%"=="1" call :echo_error Claude CLI not available!
    exit /b 1
)

exit /b 0

REM ========== PHASE 2: DOCKER/PODMAN CHECK ==========

:check_docker
call :debug_msg Checking Docker/Podman installation and configuration

REM Try Docker first
where docker >nul 2>&1
if %ERRORLEVEL%==0 (
    call :debug_msg Docker is installed
    docker version >nul 2>&1
    if %ERRORLEVEL%==0 (
        call :debug_msg Docker is working correctly
        if not "%QUIET%"=="1" call :echo_success Docker is configured correctly
        exit /b 0
    ) else (
        if not "%QUIET%"=="1" call :echo_warning Docker is installed but not running properly
        if not "%QUIET%"=="1" call :echo_info Try starting Docker Desktop application
    )
)

REM Try Podman as alternative
where podman >nul 2>&1
if %ERRORLEVEL%==0 (
    call :debug_msg Podman is installed
    podman version >nul 2>&1
    if %ERRORLEVEL%==0 (
        call :debug_msg Podman is working correctly
        call :debug_msg Setting up 'docker' alias for podman
        doskey docker=podman $*
        if not "%QUIET%"=="1" call :echo_success Podman is configured correctly with docker alias
        exit /b 0
    ) else (
        call :debug_msg Podman installed but not working, attempting to fix
        call :fix_podman
        exit /b %ERRORLEVEL%
    )
)

if not "%QUIET%"=="1" call :echo_error Neither Docker nor Podman found or working!
if not "%QUIET%"=="1" call :echo_warning Docker or Podman is required for integration testing and containerized workflows!
if not "%QUIET%"=="1" call :echo_info Please install Docker Desktop: https://www.docker.com/products/docker-desktop/
if not "%QUIET%"=="1" call :echo_info Or install Podman: https://github.com/containers/podman/blob/main/docs/tutorials/podman-for-windows.md
exit /b 1

:fix_podman
call :debug_msg Attempting to fix Podman configuration
podman machine list 2>nul | findstr /i "default" >nul 2>&1
if %ERRORLEVEL%==0 (
    call :debug_msg Default machine exists, checking if running
    podman machine list 2>nul | findstr /i "default.*running" >nul 2>&1
    if %ERRORLEVEL% neq 0 (
        if not "%QUIET%"=="1" call :echo_info Starting Podman machine
        podman machine start 2>&1
        if %ERRORLEVEL%==0 (
            if not "%QUIET%"=="1" call :echo_success Successfully started Podman machine
            timeout /t 3 /nobreak >nul 2>&1
            podman version >nul 2>&1
            if %ERRORLEVEL%==0 (
                call :debug_msg Setting up 'docker' alias for podman
                doskey docker=podman $*
                if not "%QUIET%"=="1" call :echo_success Podman is now working correctly with docker alias
                exit /b 0
            )
        )
    )
) else (
    if not "%QUIET%"=="1" call :echo_info Creating default Podman machine
    podman machine init 2>&1
    if %ERRORLEVEL%==0 (
        podman machine start 2>&1
        if %ERRORLEVEL%==0 (
            if not "%QUIET%"=="1" call :echo_success Successfully created and started Podman machine
            timeout /t 3 /nobreak >nul 2>&1
            podman version >nul 2>&1
            if %ERRORLEVEL%==0 (
                call :debug_msg Setting up 'docker' alias for podman
                doskey docker=podman $*
                if not "%QUIET%"=="1" call :echo_success Podman is now working correctly with docker alias
                exit /b 0
            )
        )
    )
)

if not "%QUIET%"=="1" call :echo_error Failed to fix Podman configuration
exit /b 1

REM ========== PHASE 3: MCP SERVER CONFIGURATION ==========

:configure_mcp_servers
call :debug_msg Starting MCP server configuration

REM Get existing servers
call :debug_msg Reading existing MCP servers
call :get_existing_servers
if %ERRORLEVEL% neq 0 exit /b 1

REM Configure project servers
if exist "%MCPS_FILE%" (
    call :debug_msg Reading Project MCP server configuration from '%MCPS_FILE%'
    call :configure_servers_from_file "%MCPS_FILE%" "project"
    if %ERRORLEVEL% neq 0 exit /b 1
    if not "%QUIET%"=="1" call :echo_success Project MCP servers configured correctly
) else (
    if not "%QUIET%"=="1" call :echo_warning File not found: %MCPS_FILE%. Skipping project configuration.
)

REM Configure local servers
if exist "%LOCAL_MCPS_FILE%" (
    call :debug_msg Reading Local MCP server configuration from '%LOCAL_MCPS_FILE%'
    call :configure_servers_from_file "%LOCAL_MCPS_FILE%" "local"
    if %ERRORLEVEL% neq 0 exit /b 1
    if not "%QUIET%"=="1" call :echo_success Local MCP servers configured correctly
) else (
    if not "%QUIET%"=="1" call :echo_warning File not found: %LOCAL_MCPS_FILE%. Skipping local configuration.
)

exit /b 0

REM ========== PHASE 4: CLEANUP ORPHANED SERVERS ==========

:cleanup_orphaned_servers
call :debug_msg Starting cleanup of orphaned MCP servers

for %%a in (%EXISTING_SERVERS%) do (
    set "SERVER=%%a"
    call :debug_msg Checking if server '!SERVER!' is defined
    echo " %DEFINED_SERVERS% " | findstr " !SERVER! " >nul
    if !ERRORLEVEL! neq 0 (
        if not "%QUIET%"=="1" call :echo_warning Server '!SERVER!' is orphaned. Removing...
        claude mcp remove "!SERVER!" >nul 2>&1
        if !ERRORLEVEL!==0 (
            if not "%QUIET%"=="1" call :echo_success Successfully removed orphaned server '!SERVER!'
        ) else (
            if not "%QUIET%"=="1" call :echo_error Failed to remove orphaned server '!SERVER!'
            exit /b 1
        )
    ) else (
        call :debug_msg Server '!SERVER!' is defined. Keeping.
    )
)

call :debug_msg Orphaned MCP server cleanup completed
exit /b 0

REM ========== HELPER FUNCTIONS ==========

:get_existing_servers
set "EXISTING_SERVERS="
for /f "tokens=1 delims=:" %%a in ('claude mcp list 2^>nul ^| findstr /r "^[a-zA-Z0-9_-]*:"') do (
    if "!EXISTING_SERVERS!"=="" (
        set "EXISTING_SERVERS=%%a"
    ) else (
        set "EXISTING_SERVERS=!EXISTING_SERVERS! %%a"
    )
)
call :debug_msg Existing servers: %EXISTING_SERVERS%
exit /b 0

:configure_servers_from_file
set "CONFIG_FILE=%~1"
set "CONFIG_TYPE=%~2"

for /f "usebackq delims=" %%a in (`powershell -ExecutionPolicy Bypass -Command "try { (Get-Content '%CONFIG_FILE%' | ConvertFrom-Json).PSObject.Properties.Name } catch { Write-Host 'ERROR_PARSING_JSON' }"`) do (
    set "SERVER_NAME=%%a"
    
    REM Skip empty server names or error messages
    if not "!SERVER_NAME!"=="" (
        if "!SERVER_NAME!"=="ERROR_PARSING_JSON" (
            if not "%QUIET%"=="1" call :echo_error Failed to parse JSON from config file: %CONFIG_FILE%
            exit /b 1
        ) else (
            REM Add to defined servers list
            if "!DEFINED_SERVERS!"=="" (
                set "DEFINED_SERVERS=!SERVER_NAME!"
            ) else (
                set "DEFINED_SERVERS=!DEFINED_SERVERS! !SERVER_NAME!"
            )
            
            call :process_mcp_server "!SERVER_NAME!" "%CONFIG_FILE%" "%CONFIG_TYPE%"
            if !ERRORLEVEL! neq 0 exit /b 1
        )
    )
)

exit /b 0

:process_mcp_server
set "SERVER_NAME=%~1"
set "CONFIG_FILE=%~2" 
set "SERVER_TYPE=%~3"

call :debug_msg Processing %SERVER_TYPE% server: '%SERVER_NAME%'

REM Check if server already exists and remove it
echo " %EXISTING_SERVERS% " | findstr " %SERVER_NAME% " >nul
if %ERRORLEVEL%==0 (
    call :debug_msg Updating server '%SERVER_NAME%'
    claude mcp remove "%SERVER_NAME%" >nul 2>&1
) else (
    call :debug_msg Server '%SERVER_NAME%' does not exist, adding
)

REM Get server configuration
call :debug_msg Extracting configuration for server '%SERVER_NAME%'
set "SERVER_CONFIG="
set "PARSE_ERROR="
for /f "usebackq delims=" %%b in (`powershell -ExecutionPolicy Bypass -File "%SCRIPT_DIR%parse-mcp-config.ps1" -serverName "%SERVER_NAME%" -mcpsFile "%CONFIG_FILE%" -projectRoot "%PROJECT_ROOT%" 2^>^&1`) do (
    set "CONFIG_LINE=%%b"
    if "!CONFIG_LINE:~0,5!"=="ERROR" (
        set "PARSE_ERROR=!CONFIG_LINE!"
    ) else (
        set "SERVER_CONFIG=!CONFIG_LINE!"
    )
)

REM Check for PowerShell parsing errors
if not "%PARSE_ERROR%"=="" (
    if not "%QUIET%"=="1" call :echo_error PowerShell config parsing failed for server '%SERVER_NAME%': %PARSE_ERROR%
    exit /b 1
)

if "%SERVER_CONFIG%"=="" (
    if not "%QUIET%"=="1" call :echo_error No configuration found for server '%SERVER_NAME%'
    exit /b 1
)

REM Add the server
call :debug_msg Adding server %SERVER_NAME%
call :debug_msg Server config: '%SERVER_CONFIG%'

REM Escape double quotes in the JSON for proper command line usage
set "ESCAPED_CONFIG=%SERVER_CONFIG:"=\"%"

REM Add the server with proper error handling
set "ADD_RESULT=0"
if "%DEBUG%"=="1" (
    call :debug_msg Executing: claude mcp add-json "%SERVER_NAME%" "%ESCAPED_CONFIG%" --debug
    claude mcp add-json "%SERVER_NAME%" "%ESCAPED_CONFIG%" --debug
    set "ADD_RESULT=!ERRORLEVEL!"
) else (
    claude mcp add-json "%SERVER_NAME%" "%ESCAPED_CONFIG%" >nul 2>&1
    set "ADD_RESULT=!ERRORLEVEL!"
)

if !ADD_RESULT!==0 (
    call :debug_msg Successfully added or updated server '%SERVER_NAME%'
) else (
    if not "%QUIET%"=="1" call :echo_error Failed to add server '%SERVER_NAME%' ^(exit code: !ADD_RESULT!^)
    if not "%QUIET%"=="1" call :echo_error Server config was: '%SERVER_CONFIG%'
    exit /b 1
)

exit /b 0