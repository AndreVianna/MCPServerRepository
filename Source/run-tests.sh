#!/bin/bash

# MCP Hub Test Runner Script
# This script provides various test execution options

set -e

echo "MCP Hub Test Runner"
echo "==================="

# Function to display usage
usage() {
    echo "Usage: $0 [OPTION]"
    echo "Options:"
    echo "  all                 Run all tests"
    echo "  unit                Run only unit tests"
    echo "  integration         Run only integration tests"
    echo "  database            Run only database tests"
    echo "  e2e                 Run only end-to-end tests"
    echo "  coverage            Run tests with coverage report"
    echo "  coverage-html       Run tests with HTML coverage report"
    echo "  watch               Run tests in watch mode"
    echo "  clean               Clean test artifacts"
    echo "  help                Show this help message"
}

# Function to run all tests
run_all_tests() {
    echo "Running all tests..."
    dotnet test --verbosity normal
}

# Function to run unit tests
run_unit_tests() {
    echo "Running unit tests..."
    dotnet test --filter Category=Unit --verbosity normal
}

# Function to run integration tests
run_integration_tests() {
    echo "Running integration tests..."
    dotnet test --filter Category=Integration --verbosity normal
}

# Function to run database tests
run_database_tests() {
    echo "Running database tests..."
    dotnet test --filter Category=Database --verbosity normal
}

# Function to run end-to-end tests
run_e2e_tests() {
    echo "Running end-to-end tests..."
    dotnet test --filter Category=EndToEnd --verbosity normal
}

# Function to run tests with coverage
run_coverage_tests() {
    echo "Running tests with coverage..."
    dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings --verbosity normal
    echo "Coverage report generated. Check TestResults directory for coverage files."
}

# Function to run tests with HTML coverage report
run_coverage_html_tests() {
    echo "Running tests with HTML coverage report..."
    dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=html /p:CoverletOutput=./coverage/ --verbosity normal
    echo "HTML coverage report generated in ./coverage/ directory."
}

# Function to run tests in watch mode
run_watch_tests() {
    echo "Running tests in watch mode..."
    dotnet watch test --verbosity normal
}

# Function to clean test artifacts
clean_test_artifacts() {
    echo "Cleaning test artifacts..."
    find . -name "TestResults" -type d -exec rm -rf {} + 2>/dev/null || true
    find . -name "coverage" -type d -exec rm -rf {} + 2>/dev/null || true
    find . -name "*.trx" -delete 2>/dev/null || true
    find . -name "*.coverage" -delete 2>/dev/null || true
    echo "Test artifacts cleaned."
}

# Main script logic
case "${1:-help}" in
    "all")
        run_all_tests
        ;;
    "unit")
        run_unit_tests
        ;;
    "integration")
        run_integration_tests
        ;;
    "database")
        run_database_tests
        ;;
    "e2e")
        run_e2e_tests
        ;;
    "coverage")
        run_coverage_tests
        ;;
    "coverage-html")
        run_coverage_html_tests
        ;;
    "watch")
        run_watch_tests
        ;;
    "clean")
        clean_test_artifacts
        ;;
    "help"|*)
        usage
        ;;
esac

echo "Test execution completed!"