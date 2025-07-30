Feature: Security Scanning and Trust Tier Management
  In order to ensure platform security and user trust
  As a security administrator or package consumer
  I want packages to be automatically scanned and assigned appropriate trust tiers

  Background:
    Given the MCP Hub API is running
    And the security scanning service is available
    And the database is initialized with test data

  @api @security @scanning @automatic
  Scenario: Automatic security scan on package publish
    Given I am authenticated as a publisher
    And I have a clean package manifest and files
    When I publish a new package
    Then the package should be created successfully
    And a security scan should be automatically initiated
    And the scan status should be "In Progress"
    And I should receive a scan ID for tracking

  @api @security @scanning @completion
  Scenario: Security scan completion with clean results
    Given I have published a package that is being scanned
    When the security scan completes successfully
    Then the package should have scan results attached
    And the package should be assigned a trust tier based on scan results
    And the security score should be calculated and stored
    And the package status should be updated to reflect scan completion

  @api @security @scanning @vulnerabilities
  Scenario: Security scan detects vulnerabilities
    Given I have published a package with known security issues
    When the security scan completes
    Then the scan should detect the security vulnerabilities
    And each vulnerability should have severity, description, and recommendations
    And the package trust tier should reflect the security issues
    And the package should be flagged for review if critical issues found

  @api @security @scanning @static
  Scenario: Static code analysis scanning
    Given I have a package with various code patterns
    When the static analysis scan runs
    Then it should analyze code for security patterns
    And it should detect potential vulnerabilities like:
      | Vulnerability Type    | Pattern                    |
      | Code Injection       | eval(), Function()         |
      | Path Traversal       | ../, \..\                  |
      | Hardcoded Secrets    | API keys, passwords        |
      | Unsafe Dependencies  | Known vulnerable packages  |
    And findings should be categorized by severity
    And recommendations should be provided for remediation

  @api @security @scanning @dynamic
  Scenario: Dynamic security analysis in sandbox
    Given I have a package that needs dynamic analysis
    When the dynamic security scan runs
    Then the package should be executed in a secure sandbox
    And the scan should monitor runtime behavior including:
      | Behavior           | Monitoring             |
      | Network Activity   | Outbound connections   |
      | File System        | File read/write access |
      | System Calls       | OS-level interactions  |
      | Memory Usage       | Resource consumption   |
    And suspicious behavior should be flagged
    And the execution should be terminated if malicious activity detected

  @api @security @scanning @dependencies
  Scenario: Dependency vulnerability scanning
    Given I have a package with external dependencies
    When the dependency scan runs
    Then it should check all dependencies against vulnerability databases
    And it should identify packages with known CVEs
    And it should calculate transitive dependency risks
    And it should recommend dependency updates where available
    And the overall security score should factor in dependency risks

  @api @security @trusttier @progression
  Scenario: Trust tier progression based on scan results
    Given I have packages with different security scan results
    Then packages should be assigned trust tiers as follows:
      | Security Score | Vulnerabilities | Trust Tier        |
      | 9.0-10.0      | None Critical   | Security Audited  |
      | 7.0-8.9       | None High       | Community Trusted |
      | 5.0-6.9       | Some Medium     | Community Trusted |
      | 0.0-4.9       | Critical/High   | Unverified        |
    And trust tier changes should be logged in audit trail
    And package consumers should be notified of tier changes

  @api @security @trusttier @manual
  Scenario: Manual trust tier promotion
    Given I am authenticated as a security administrator
    And there is a package with "Community Trusted" tier
    When I manually promote the package to "Certified" tier
    Then the package trust tier should be updated
    And a manual review audit entry should be created
    And the promotion should include reviewer notes and timestamp
    And package users should be notified of the tier upgrade

  @api @security @trusttier @demotion
  Scenario: Trust tier demotion due to new vulnerabilities
    Given there is a "Security Audited" package
    When new vulnerabilities are discovered in the package
    And a rescan reveals critical security issues
    Then the package should be automatically demoted to appropriate tier
    And all users of the package should be notified
    And the package should be flagged for immediate attention
    And remediation guidance should be provided to the publisher

  @api @security @scanning @rescan
  Scenario: Manual package rescan request
    Given I am the publisher of a package
    And my package has been updated to fix security issues
    When I request a manual rescan of my package
    Then a new security scan should be initiated
    And the previous scan results should be archived
    And I should receive notification when the rescan completes
    And the trust tier should be re-evaluated based on new results

  @api @security @scanning @scheduled
  Scenario: Scheduled security rescans
    Given there are packages in the registry older than 30 days
    When the scheduled security scan job runs
    Then packages should be selected for rescanning based on:
      | Criteria              | Priority |
      | High download count   | High     |
      | Recent vulnerability  | High     |
      | Age since last scan   | Medium   |
      | Trust tier level      | Medium   |
    And rescans should be distributed over time to manage load
    And scan results should be compared with previous scans

  @api @security @policy @enforcement
  Scenario: Security policy enforcement
    Given there are security policies defined for different trust tiers
    When a package scan completes
    Then the package should be evaluated against relevant policies
    And packages violating policies should be:
      | Policy Violation      | Action                    |
      | Critical vulnerability| Immediate suspension      |
      | High vulnerability    | Warning and review flag   |
      | Policy non-compliance | Trust tier restriction   |
      | License incompatibility| Publisher notification   |
    And policy violations should be logged and tracked

  @api @security @scanning @metadata
  Scenario: Security scan metadata and reporting
    Given I have packages with completed security scans
    When I request security scan details
    Then the response should include comprehensive metadata:
      | Field                 | Description                |
      | scan_id              | Unique scan identifier     |
      | scan_type            | Static, dynamic, dependency|
      | scan_timestamp       | When scan was performed    |
      | scan_duration        | Time taken for scan        |
      | scanner_version      | Version of scanning tools  |
      | findings_count       | Number of issues found     |
      | false_positive_count | Issues marked as FP        |
    And historical scan data should be available for comparison

  @api @security @scanning @performance
  Scenario: Security scan performance optimization
    Given there are multiple packages queued for scanning
    When the security scanning system processes the queue
    Then scans should be prioritized by:
      | Priority Factor       | Weight |
      | Package popularity    | High   |
      | Publisher reputation  | High   |
      | Package complexity    | Medium |
      | Previous scan results | Medium |
    And parallel scanning should be used to optimize throughput
    And scan results should be cached to avoid redundant scans

  @api @security @scanning @falsepositives
  Scenario: False positive management
    Given I am a package publisher with scan results containing false positives
    When I mark specific findings as false positives
    Then the findings should be marked as dismissed
    And the security score should be recalculated
    And the false positive markers should be preserved across rescans
    And security administrators should be able to review FP claims

  @api @security @scanning @integration
  Scenario: Integration with external security databases
    When the security scanning runs
    Then it should integrate with external vulnerability databases:
      | Database      | Purpose                    |
      | CVE           | Common vulnerabilities     |
      | NVD           | National vulnerability DB  |
      | OSV           | Open source vulnerabilities|
      | GHSA          | GitHub security advisories |
    And database updates should trigger rescans of affected packages
    And new vulnerability data should be continuously synchronized

  @api @security @trusttier @consumer
  Scenario: Trust tier filtering for package consumers
    Given there are packages with various trust tiers
    When I search for packages with minimum trust tier "Community Trusted"
    Then only packages meeting the trust tier requirement should be returned
    And the search results should clearly display trust tier information
    And packages below the threshold should be excluded from results
    And clear explanations should be provided for trust tier meanings

  @api @security @scanning @notification
  Scenario: Security scan notifications
    Given I am a publisher with packages in the registry
    When security scans complete for my packages
    Then I should receive notifications for:
      | Event Type            | Notification Method |
      | Scan completion       | Email, API webhook  |
      | New vulnerabilities   | Email, dashboard    |
      | Trust tier changes    | Email, API webhook  |
      | Policy violations     | Email, urgent flag  |
    And notifications should include actionable information
    And I should be able to configure notification preferences

  @api @security @compliance @reporting
  Scenario: Security compliance reporting
    Given I am a security administrator
    When I request security compliance reports
    Then the reports should include:
      | Report Section        | Information                |
      | Scan Coverage         | % of packages scanned      |
      | Vulnerability Trends  | New vs resolved issues     |
      | Trust Tier Distribution| Breakdown by tier         |
      | Policy Compliance     | Violations by policy       |
      | Response Times        | Time to address issues     |
    And reports should be available in multiple formats
    And historical data should be included for trend analysis