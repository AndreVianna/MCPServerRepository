Feature: Package Management API
  In order to manage MCP packages effectively
  As a package publisher or consumer
  I want to be able to create, search, and manage packages through the API

  Background:
    Given the MCP Hub API is running
    And the database is initialized with test data

  @api @packages @search
  Scenario: Search for packages with basic query
    Given there are 10 packages in the registry
    When I search for packages with query "file"
    Then I should receive a successful response
    And the response should contain packages matching "file"
    And each package should have required fields populated

  @api @packages @search @filtering
  Scenario: Search packages with category filtering
    Given there are packages in the following categories:
      | Category | Count |
      | ai       | 5     |
      | data     | 3     |
      | files    | 4     |
      | web      | 2     |
    When I search for packages with query "*" and category "ai"
    Then I should receive a successful response
    And all returned packages should be in category "ai"
    And the total count should be 5

  @api @packages @search @trusttier
  Scenario: Search packages with trust tier filtering
    Given there are packages with the following trust tiers:
      | Trust Tier       | Count |
      | Unverified      | 3     |
      | Community       | 4     |
      | Security Audited| 2     |
      | Certified       | 1     |
    When I search for packages with minimum trust tier "Community"
    Then I should receive a successful response
    And all returned packages should have trust tier "Community" or higher
    And the total count should be 7

  @api @packages @search @pagination
  Scenario: Search packages with pagination
    Given there are 25 packages in the registry
    When I search for packages with query "*" and page size 10 and page 2
    Then I should receive a successful response
    And the response should contain 10 packages
    And the pagination headers should indicate page 2 of 3 pages
    And the total count should be 25

  @api @packages @search @sorting
  Scenario Outline: Search packages with different sorting options
    Given there are packages with varying statistics
    When I search for packages sorted by "<sortField>" in "<direction>" order
    Then I should receive a successful response
    And the packages should be sorted by "<sortField>" in "<direction>" order

    Examples:
      | sortField | direction  |
      | name      | ascending  |
      | name      | descending |
      | downloads | descending |
      | created   | descending |

  @api @packages @retrieve
  Scenario: Get package by ID
    Given there is a package with name "test-package"
    When I request the package by its ID
    Then I should receive a successful response
    And the response should contain the package details
    And the package should have versions and security information

  @api @packages @retrieve
  Scenario: Get package by name
    Given there is a package with name "@anthropic/file-organizer"
    When I request the package by name "@anthropic/file-organizer"
    Then I should receive a successful response
    And the response should contain the package details
    And the package name should be "@anthropic/file-organizer"

  @api @packages @retrieve @notfound
  Scenario: Get non-existent package by ID
    When I request a package with non-existent ID
    Then I should receive a "Not Found" response
    And the response should contain an appropriate error message

  @api @packages @retrieve @notfound
  Scenario: Get non-existent package by name
    When I request a package with name "non-existent-package"
    Then I should receive a "Not Found" response
    And the response should contain an appropriate error message

  @api @packages @publisher
  Scenario: Get packages by publisher
    Given there are publishers with packages:
      | Publisher | Package Count |
      | anthropic | 3            |
      | openai    | 2            |
      | google    | 1            |
    When I request packages for publisher "anthropic"
    Then I should receive a successful response
    And the response should contain 3 packages
    And all packages should belong to publisher "anthropic"

  @api @packages @create @authentication
  Scenario: Create package as authenticated user
    Given I am authenticated as a publisher
    And I have a valid package manifest
    When I create a new package
    Then I should receive a "Created" response
    And the response should contain the created package
    And the package should be assigned to my publisher account
    And a security scan should be automatically triggered

  @api @packages @create @authentication
  Scenario: Create package without authentication
    Given I am not authenticated
    And I have a valid package manifest
    When I attempt to create a new package
    Then I should receive an "Unauthorized" response
    And no package should be created

  @api @packages @create @validation
  Scenario: Create package with invalid manifest
    Given I am authenticated as a publisher
    And I have an invalid package manifest with missing required fields
    When I attempt to create a new package
    Then I should receive a "Bad Request" response
    And the response should contain validation error messages
    And no package should be created

  @api @packages @update @authentication
  Scenario: Update package as owner
    Given I am authenticated as a publisher
    And I have a package that I own
    When I update the package with new information
    Then I should receive a successful response
    And the package should be updated with the new information
    And an audit entry should be created

  @api @packages @update @authorization
  Scenario: Update package as different user
    Given I am authenticated as a publisher
    And there is a package owned by a different publisher
    When I attempt to update the package
    Then I should receive a "Forbidden" response
    And the package should not be modified

  @api @packages @delete @authentication
  Scenario: Delete package as owner
    Given I am authenticated as a publisher
    And I have a package that I own
    When I delete the package
    Then I should receive a "No Content" response
    And the package should be removed from the registry

  @api @packages @delete @notfound
  Scenario: Delete non-existent package
    Given I am authenticated as a publisher
    When I attempt to delete a non-existent package
    Then I should receive a "Not Found" response

  @api @packages @search @advanced
  Scenario: Advanced search with multiple filters
    Given there are packages with various attributes
    When I perform an advanced search with the following criteria:
      | Field           | Value           |
      | query           | file manager    |
      | categories      | files,tools     |
      | trustTier       | Community       |
      | sortBy          | downloads       |
      | sortDirection   | descending      |
      | page            | 1               |
      | pageSize        | 10              |
    Then I should receive a successful response
    And the packages should match the search criteria
    And the packages should be sorted by downloads in descending order
    And the response should include search metadata

  @api @packages @security
  Scenario: Package security scan integration
    Given I am authenticated as a publisher
    And I have created a new package
    When the security scan completes
    Then the package should have security scan results
    And the package should have a trust tier assigned
    And the security score should be calculated

  @api @packages @versions
  Scenario: Package version management
    Given I am authenticated as a publisher
    And I have a package with multiple versions
    When I request the package details
    Then the response should include all versions
    And versions should be sorted by creation date
    And each version should have download statistics

  @api @packages @performance
  Scenario: Search performance with large dataset
    Given there are 1000 packages in the registry
    When I search for packages with any query
    Then the response should be returned within 500ms
    And the search should use appropriate database indexes
    And memory usage should remain within acceptable limits

  @api @packages @concurrent
  Scenario: Concurrent package operations
    Given I am authenticated as a publisher
    When I perform 10 concurrent package searches
    And I perform 5 concurrent package creations
    Then all operations should complete successfully
    And no race conditions should occur
    And data consistency should be maintained