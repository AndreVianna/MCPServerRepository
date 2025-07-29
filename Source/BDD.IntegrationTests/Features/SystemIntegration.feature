Feature: End-to-End System Integration and Performance
  In order to ensure the MCP Hub platform works as a complete system
  As a system administrator or quality assurance engineer
  I want to verify all components work together seamlessly with acceptable performance

  Background:
    Given the complete MCP Hub system is running
    And all services are healthy and connected
    And the database is initialized with realistic test data

  @integration @e2e @lifecycle
  Scenario: Complete package lifecycle from publish to install
    Given I am a publisher with valid credentials
    And I have a complete MCP package ready for publishing
    When I publish the package through the web interface
    Then the package should be uploaded successfully
    And a security scan should be automatically initiated
    And I should receive confirmation with package details
    And when the security scan completes
    Then the package should be assigned an appropriate trust tier
    And the package should appear in search results
    And when a user searches for the package via CLI
    Then they should find the package with correct information
    And when they install the package via CLI
    Then the installation should complete successfully
    And the package should be functional in their MCP environment

  @integration @e2e @multiuser
  Scenario: Multi-user concurrent operations
    Given there are 10 authenticated publishers
    And there are 50 authenticated users
    When publishers simultaneously publish different packages
    And users simultaneously search and install packages
    Then all operations should complete successfully
    And there should be no data corruption or race conditions
    And system performance should remain within acceptable limits
    And all audit trails should be correctly maintained

  @integration @security @scanning
  Scenario: End-to-end security scanning workflow
    Given I publish a package with mixed security characteristics
    When the security scanning service processes the package
    Then it should perform comprehensive analysis including:
      | Analysis Type     | Verification                        |
      | Static Analysis   | Code patterns, vulnerabilities     |
      | Dynamic Analysis  | Runtime behavior in sandbox        |
      | Dependency Scan   | Third-party vulnerability check     |
      | Permission Audit  | Required permissions assessment     |
    And the scan results should be stored correctly
    And the package trust tier should be calculated accurately
    And security alerts should be sent if critical issues found
    And the package status should reflect security findings

  @integration @trusttier @progression
  Scenario: Trust tier progression through system lifecycle
    Given I publish a new package
    Then it should start with "Unverified" trust tier
    And when initial security scan completes successfully
    Then it should be promoted to "Community Trusted" if scores are good
    And when the package gains popularity and positive reviews
    Then it should be eligible for "Security Audited" tier
    And when I request manual security review
    Then security administrators should be notified
    And when manual review is completed favorably
    Then the package should be promoted to "Certified" tier
    And all tier changes should be properly audited and logged

  @integration @api @web @cli
  Scenario: Cross-platform consistency
    Given there is a package in the registry
    When I retrieve package information via REST API
    And I view the same package in the web application
    And I get package info via CLI tool
    Then all three interfaces should show identical information:
      | Information       | Consistency Check                   |
      | Package Name      | Exactly matching across platforms   |
      | Version           | Same version number and details     |
      | Trust Tier        | Identical tier and security score   |
      | Download Count    | Same statistics                     |
      | Security Status   | Identical scan results              |
      | Installation      | Same installation commands          |
    And all timestamps should be consistent across platforms

  @integration @search @indexing
  Scenario: Real-time search index updates
    Given the search service is running
    When I publish a new package
    Then the package should appear in search results within 30 seconds
    And when I update package information
    Then search results should reflect changes within 60 seconds
    And when I delete a package
    Then it should be removed from search results within 30 seconds
    And search suggestions should be updated accordingly
    And category and tag filters should include new packages immediately

  @integration @caching @performance
  Scenario: Multi-layer caching system
    Given the caching system is operational
    When I request popular package information
    Then the first request should hit the database
    And subsequent requests should be served from cache
    And response times should improve significantly
    And when package information changes
    Then cache should be invalidated appropriately
    And all cache layers should remain consistent
    And cache hit ratios should meet performance targets

  @integration @monitoring @observability
  Scenario: System monitoring and alerting
    Given monitoring and alerting systems are configured
    When various system events occur:
      | Event Type            | Expected Monitoring                 |
      | High CPU Usage        | Performance alert triggered         |
      | Database Slow Query   | Query performance alert             |
      | Security Scan Failure | Security team notification          |
      | Authentication Error  | Security monitoring alert           |
      | Package Upload Spike  | Capacity planning notification      |
    Then appropriate alerts should be generated
    And metrics should be collected and stored
    And dashboards should reflect real-time system status
    And incident response procedures should be triggered

  @integration @backup @recovery
  Scenario: Backup and disaster recovery
    Given the system has operational data
    When automated backup procedures run
    Then all critical data should be backed up:
      | Data Type             | Backup Verification                 |
      | Package Metadata      | Complete package information        |
      | User Accounts         | All user profiles and preferences   |
      | Security Scan Results | Full scan history and findings      |
      | Audit Trails          | Complete audit and activity logs    |
      | Configuration         | System and service configurations   |
    And backup integrity should be verified
    And when disaster recovery is tested
    Then system should be restorable within RTO requirements
    And data should be consistent after recovery

  @integration @scaling @loadbalancing
  Scenario: System scaling and load distribution
    Given the system is running with load balancers
    When system load increases significantly
    Then additional service instances should be started automatically
    And load should be distributed evenly across instances
    And no user should experience service disruption
    And when load decreases
    Then excess instances should be scaled down appropriately
    And cost optimization should be maintained
    And all scaling events should be logged and monitored

  @integration @authentication @authorization
  Scenario: End-to-end authentication and authorization
    Given users with different roles exist in the system
    When they access various system components:
      | User Role         | Allowed Operations                  |
      | Anonymous User    | Browse, search packages             |
      | Registered User   | Browse, search, download, rate      |
      | Publisher         | All user operations plus publish    |
      | Administrator     | All operations plus user management |
    Then access control should be consistently enforced
    And JWT tokens should work across all services
    And session management should be secure and consistent
    And audit logs should capture all authentication events

  @integration @messaging @events
  Scenario: Event-driven architecture integration
    Given the event messaging system is operational
    When domain events are published:
      | Event                     | Expected Subscribers                |
      | PackagePublished          | Search indexer, security scanner   |
      | SecurityScanCompleted     | Trust tier calculator, notifications|
      | UserRegistered            | Email service, analytics           |
      | TrustTierUpdated          | Search index, package consumers    |
    Then all subscribers should receive events reliably
    And event processing should be idempotent
    And failed events should be retried appropriately
    And event ordering should be maintained where required

  @integration @storage @cdn
  Scenario: Package storage and content delivery
    Given the storage and CDN systems are configured
    When packages are uploaded
    Then they should be stored securely with proper access controls
    And package downloads should be served from CDN
    And download performance should meet SLA requirements
    And storage costs should be optimized through tiering
    And when packages are deleted
    Then all storage references should be cleaned up
    And CDN cache should be invalidated appropriately

  @performance @api @response
  Scenario: API performance requirements
    Given the system is under normal load
    When API requests are made
    Then response times should meet performance requirements:
      | Endpoint Type         | Performance Target                  |
      | Package Search        | 95th percentile < 200ms             |
      | Package Info          | 95th percentile < 100ms             |
      | Package Upload        | Complete within 30 seconds         |
      | Authentication        | 95th percentile < 50ms              |
      | User Registration     | Complete within 5 seconds          |
    And all endpoints should handle expected concurrent load
    And resource utilization should remain within limits
    And performance metrics should be continuously monitored

  @performance @database @optimization
  Scenario: Database performance and optimization
    Given the database contains realistic data volumes
    When database queries are executed under load
    Then query performance should meet requirements:
      | Query Type            | Performance Target                  |
      | Package Search        | < 50ms average execution time       |
      | User Authentication   | < 10ms average execution time       |
      | Package Retrieval     | < 20ms average execution time       |
      | Analytics Queries     | < 500ms for complex aggregations    |
    And database indexes should be utilized effectively
    And connection pooling should optimize resource usage
    And slow queries should be identified and optimized

  @performance @memory @resources
  Scenario: System resource utilization
    Given the system is running all components
    When operating under normal load
    Then resource utilization should be within limits:
      | Resource Type         | Utilization Target                  |
      | CPU Usage             | < 70% average, < 90% peak           |
      | Memory Usage          | < 80% average, < 95% peak           |
      | Disk I/O              | < 80% capacity                      |
      | Network Bandwidth     | < 70% capacity                      |
    And resource usage should scale predictably with load
    And memory leaks should not occur over time
    And garbage collection should not cause performance issues

  @performance @concurrent @users
  Scenario: Concurrent user performance
    Given the system supports multiple concurrent users
    When 1000 users perform various operations simultaneously
    Then the system should handle the load without degradation
    And no user should experience response times > 5 seconds
    And all operations should complete successfully
    And system stability should be maintained
    And resource utilization should remain within acceptable ranges

  @reliability @availability @uptime
  Scenario: System reliability and availability
    Given the system has been running continuously
    When monitored over a 24-hour period
    Then system availability should be > 99.9%
    And any downtime should be planned maintenance only
    And service recovery should be automatic where possible
    And mean time to recovery (MTTR) should be < 5 minutes
    And all availability metrics should be tracked and reported

  @reliability @failover @redundancy
  Scenario: Failover and redundancy testing
    Given the system has redundant components
    When individual components fail:
      | Component Failure     | Expected Behavior                   |
      | Database Node         | Automatic failover to standby      |
      | API Service Instance  | Load balancer routes to healthy     |
      | Cache Instance        | Graceful degradation, data rebuild |
      | Storage Service       | Failover to backup storage          |
    Then system should continue operating normally
    And users should not experience service interruption
    And failed components should be automatically recovered
    And all failover events should be logged and monitored

  @security @penetration @testing
  Scenario: Security penetration testing scenarios
    Given the system is deployed with security measures
    When security testing is performed:
      | Security Test         | Verification                        |
      | SQL Injection         | All inputs properly sanitized       |
      | XSS Attacks          | Content properly encoded/escaped    |
      | CSRF Protection       | All state changes protected         |
      | Authentication Bypass | No unauthorized access possible     |
      | Authorization Bypass  | Role-based access properly enforced |
      | Package Malware       | Malicious packages detected/blocked |
    Then all security tests should pass
    And no vulnerabilities should be exploitable
    And security events should be properly logged
    And incident response should be triggered for attacks

  @security @encryption @compliance
  Scenario: Data encryption and compliance
    Given the system handles sensitive data
    When data is processed and stored
    Then all data should be encrypted:
      | Data Type             | Encryption Requirement              |
      | Data in Transit       | TLS 1.3 encryption                  |
      | Data at Rest          | AES-256 encryption                  |
      | User Passwords        | Bcrypt with appropriate cost factor |
      | API Tokens            | Cryptographically secure storage    |
      | Package Contents      | Integrity checksums and signatures  |
    And encryption keys should be properly managed
    And compliance requirements should be met
    And security audits should verify encryption implementation

  @integration @analytics @reporting
  Scenario: Analytics and reporting system
    Given the analytics system is collecting data
    When users interact with the platform
    Then usage analytics should be captured:
      | Analytics Type        | Data Collected                      |
      | Package Downloads     | Count, user demographics, trends    |
      | Search Queries        | Terms, results, conversion rates    |
      | User Engagement       | Session duration, page views       |
      | Performance Metrics   | Response times, error rates         |
      | Security Events       | Scan results, threat detection      |
    And data should be aggregated and processed correctly
    And reports should be generated automatically
    And dashboard visualizations should be accurate and current

  @integration @compliance @audit
  Scenario: Regulatory compliance and audit trail
    Given the system must maintain compliance
    When auditable events occur
    Then comprehensive audit trails should be maintained:
      | Audit Category        | Required Information                |
      | User Authentication   | Login/logout, failed attempts       |
      | Data Access           | Who accessed what data when         |
      | Configuration Changes | What was changed by whom            |
      | Security Events       | Scans, alerts, policy violations    |
      | Data Modifications    | All CRUD operations with details    |
    And audit logs should be tamper-proof
    And compliance reports should be generated automatically
    And audit data should be retained per regulatory requirements

  @integration @migration @upgrade
  Scenario: System migration and upgrade procedures
    Given a system upgrade is required
    When migration procedures are executed
    Then all data should be migrated correctly
    And system functionality should be preserved
    And performance should be maintained or improved
    And rollback procedures should be available
    And migration should complete within maintenance window
    And all components should be verified after upgrade
    And users should experience minimal disruption

  @integration @capacity @planning
  Scenario: Capacity planning and growth management
    Given the system usage is growing
    When capacity thresholds are approached
    Then capacity alerts should be triggered
    And scaling recommendations should be provided
    And cost projections should be calculated
    And growth trends should be analyzed
    And infrastructure changes should be planned proactively
    And business impact should be assessed and communicated