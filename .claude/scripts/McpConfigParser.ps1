param($serverName, $mcpsFile, $projectRoot)

try {
    # Validate input parameters
    if (-not $serverName -or -not $mcpsFile -or -not $projectRoot) {
        Write-Output 'ERROR_MISSING_PARAMETERS'
        exit 1
    }
    
    if (-not (Test-Path $mcpsFile)) {
        Write-Output 'ERROR_FILE_NOT_FOUND'
        exit 1
    }
    
    # Parse JSON configuration
    $json = Get-Content $mcpsFile -Raw | ConvertFrom-Json
    
    # Check if server exists in configuration
    if (-not $json.PSObject.Properties.Name.Contains($serverName)) {
        Write-Output 'ERROR_SERVER_NOT_FOUND'
        exit 1
    }
    
    # Extract and format server configuration
    $config = $json.$serverName | ConvertTo-Json -Compress -Depth 10
    
    # Replace $(pwd) with project root and normalize paths
    $projectRootForwardSlash = $projectRoot -replace '\\', '/'
    $config = $config -replace '\$\(pwd\)', $projectRootForwardSlash
    
    # Normalize all backslashes to forward slashes for better cross-platform compatibility
    $config = $config -replace '\\\\', '/'
    
    # Validate that the result is valid JSON
    try {
        $config | ConvertFrom-Json | Out-Null
    } catch {
        Write-Output 'ERROR_INVALID_JSON_OUTPUT'
        exit 1
    }
    
    # Output the configuration (use Write-Output for clean capture)
    Write-Output $config
} catch {
    Write-Output "ERROR_PARSING_CONFIG: $($_.Exception.Message)"
    exit 1
}