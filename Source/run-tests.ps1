# MCP Hub Test Runner Script (PowerShell)
# This script provides various test execution options

param(
    [Parameter(Position=0)]
    [string]$Command = "help"
)

Write-Host "MCP Hub Test Runner" -ForegroundColor Green
Write-Host "===================" -ForegroundColor Green

function Show-Usage {
    Write-Host "Usage: .\run-tests.ps1 [OPTION]"
    Write-Host "Options:"
    Write-Host "  all                 Run all tests"
    Write-Host "  unit                Run only unit tests"
    Write-Host "  integration         Run only integration tests"
    Write-Host "  database            Run only database tests"
    Write-Host "  e2e                 Run only end-to-end tests"
    Write-Host "  coverage            Run tests with coverage report"
    Write-Host "  coverage-html       Run tests with HTML coverage report"
    Write-Host "  watch               Run tests in watch mode"
    Write-Host "  clean               Clean test artifacts"
    Write-Host "  help                Show this help message"
}

function Invoke-AllTests {
    Write-Host "Running all tests..." -ForegroundColor Yellow
    dotnet test --verbosity normal
}

function Invoke-UnitTests {
    Write-Host "Running unit tests..." -ForegroundColor Yellow
    dotnet test --filter Category=Unit --verbosity normal
}

function Invoke-IntegrationTests {
    Write-Host "Running integration tests..." -ForegroundColor Yellow
    dotnet test --filter Category=Integration --verbosity normal
}

function Invoke-DatabaseTests {
    Write-Host "Running database tests..." -ForegroundColor Yellow
    dotnet test --filter Category=Database --verbosity normal
}

function Invoke-E2ETests {
    Write-Host "Running end-to-end tests..." -ForegroundColor Yellow
    dotnet test --filter Category=EndToEnd --verbosity normal
}

function Invoke-CoverageTests {
    Write-Host "Running tests with coverage..." -ForegroundColor Yellow
    dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings --verbosity normal
    Write-Host "Coverage report generated. Check TestResults directory for coverage files." -ForegroundColor Green
}

function Invoke-CoverageHtmlTests {
    Write-Host "Running tests with HTML coverage report..." -ForegroundColor Yellow
    dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=html /p:CoverletOutput=./coverage/ --verbosity normal
    Write-Host "HTML coverage report generated in ./coverage/ directory." -ForegroundColor Green
}

function Invoke-WatchTests {
    Write-Host "Running tests in watch mode..." -ForegroundColor Yellow
    dotnet watch test --verbosity normal
}

function Clear-TestArtifacts {
    Write-Host "Cleaning test artifacts..." -ForegroundColor Yellow
    Get-ChildItem -Path . -Recurse -Directory -Name "TestResults" | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
    Get-ChildItem -Path . -Recurse -Directory -Name "coverage" | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
    Get-ChildItem -Path . -Recurse -File -Name "*.trx" | Remove-Item -Force -ErrorAction SilentlyContinue
    Get-ChildItem -Path . -Recurse -File -Name "*.coverage" | Remove-Item -Force -ErrorAction SilentlyContinue
    Write-Host "Test artifacts cleaned." -ForegroundColor Green
}

# Main script logic
switch ($Command.ToLower()) {
    "all" {
        Invoke-AllTests
    }
    "unit" {
        Invoke-UnitTests
    }
    "integration" {
        Invoke-IntegrationTests
    }
    "database" {
        Invoke-DatabaseTests
    }
    "e2e" {
        Invoke-E2ETests
    }
    "coverage" {
        Invoke-CoverageTests
    }
    "coverage-html" {
        Invoke-CoverageHtmlTests
    }
    "watch" {
        Invoke-WatchTests
    }
    "clean" {
        Clear-TestArtifacts
    }
    default {
        Show-Usage
    }
}

Write-Host "Test execution completed!" -ForegroundColor Green