Feature: Web Application User Experience
  In order to discover and manage MCP packages through a user-friendly interface
  As a package consumer or publisher
  I want to use the web application for all package-related activities

  Background:
    Given the MCP Hub web application is running
    And the database is initialized with test data

  @web @homepage
  Scenario: Homepage displays correctly
    When I visit the homepage
    Then I should see the MCP Hub branding
    And I should see the main search bar
    And I should see featured packages section
    And I should see trending packages
    And I should see category navigation
    And I should see platform statistics

  @web @search @basic
  Scenario: Basic package search from homepage
    Given there are packages containing "file manager" in their name or description
    When I visit the homepage
    And I enter "file manager" in the search bar
    And I click the search button
    Then I should be redirected to the search results page
    And I should see packages matching "file manager"
    And each package result should display name, description, and trust tier
    And I should see download counts and ratings

  @web @search @filters
  Scenario: Advanced search with filters
    Given there are packages in various categories and trust tiers
    When I visit the search page
    And I enter "data" in the search query
    And I select "data" category filter
    And I select "Community Trusted" as minimum trust tier
    And I click search
    Then I should see only packages matching all criteria
    And the active filters should be clearly displayed
    And I should be able to remove individual filters

  @web @search @sorting
  Scenario: Search results sorting
    Given there are packages with varying popularity and dates
    When I perform a search for "*"
    Then I should see sort options for:
      | Sort Option  | Description        |
      | Relevance    | Default sorting    |
      | Downloads    | Most downloaded    |
      | Recent       | Recently updated   |
      | Name         | Alphabetical       |
      | Rating       | Highest rated      |
    And when I change the sort option
    Then the results should reorder accordingly

  @web @search @pagination
  Scenario: Search results pagination
    Given there are 50 packages in the registry
    When I search for "*" with page size 10
    Then I should see 10 packages on the first page
    And I should see pagination controls showing page 1 of 5
    And when I click "Next" or page 2
    Then I should see the next 10 packages
    And the URL should reflect the current page

  @web @package @detail
  Scenario: Package detail page
    Given there is a package "@anthropic/file-organizer" with full details
    When I navigate to the package detail page
    Then I should see comprehensive package information:
      | Section          | Content                               |
      | Header           | Name, publisher, trust tier, version |
      | Description      | Full package description              |
      | Installation     | Command and instructions              |
      | Capabilities     | Tools, resources, prompts             |
      | Security         | Security score and scan results       |
      | Statistics       | Downloads, stars, contributors        |
      | Versions         | Version history and changelogs        |
    And I should see an installation command that I can copy
    And I should see security scan results and trust tier explanation

  @web @package @security
  Scenario: Package security information display
    Given there is a package with security scan results
    When I view the package detail page
    Then I should see the security section displaying:
      | Security Info    | Details                           |
      | Trust Tier       | Current tier with badge           |
      | Security Score   | Numerical score out of 10         |
      | Last Scanned     | Date of most recent scan          |
      | Vulnerabilities  | Count by severity level           |
      | Scan History     | Previous scan results             |
    And I should be able to view detailed security report
    And vulnerabilities should be clearly categorized by severity

  @web @package @versions
  Scenario: Package version history
    Given there is a package with multiple versions
    When I view the package versions tab
    Then I should see all versions listed chronologically
    And each version should show:
      | Version Info     | Details                           |
      | Version Number   | Semantic version                  |
      | Release Date     | When version was published        |
      | Download Count   | Downloads for this version        |
      | Release Notes    | Changes and improvements          |
      | Security Status  | Scan results for this version     |
    And I should be able to view installation commands for specific versions

  @web @authentication @login
  Scenario: User login process
    Given there is a registered user with email "test@example.com"
    When I click the "Sign In" button
    Then I should see the login form
    And when I enter valid credentials
    And I submit the login form
    Then I should be redirected to my dashboard or previous page
    And I should see my user menu in the header
    And I should see logout option

  @web @authentication @registration
  Scenario: User registration process
    When I click "Sign Up" from the login page
    Then I should see the registration form with fields:
      | Field           | Type       | Required |
      | Email           | Email      | Yes      |
      | Password        | Password   | Yes      |
      | Confirm Password| Password   | Yes      |
      | First Name      | Text       | Yes      |
      | Last Name       | Text       | Yes      |
      | Terms Agreement | Checkbox   | Yes      |
    And when I fill in valid registration details
    And I submit the form
    Then I should see a confirmation message about email verification
    And I should receive a verification email

  @web @authentication @logout
  Scenario: User logout
    Given I am logged in as a user
    When I click on my user menu
    And I click "Logout"
    Then I should be logged out
    And I should be redirected to the homepage
    And I should see "Sign In" option instead of user menu

  @web @dashboard @publisher
  Scenario: Publisher dashboard overview
    Given I am logged in as a publisher with packages
    When I navigate to my publisher dashboard
    Then I should see dashboard sections:
      | Section      | Content                           |
      | Overview     | Package count, total downloads    |
      | My Packages  | List of published packages        |
      | Analytics    | Download trends and statistics    |
      | Security     | Security alerts and scan status   |
      | Profile      | Publisher profile information     |
    And I should see options to publish new packages
    And I should see quick actions for package management

  @web @dashboard @packages
  Scenario: Publisher package management
    Given I am logged in as a publisher with packages
    When I navigate to "My Packages" section
    Then I should see a table of my packages with columns:
      | Column       | Information                       |
      | Name         | Package name with link            |
      | Version      | Latest version number             |
      | Trust Tier   | Current trust tier                |
      | Downloads    | Total download count              |
      | Status       | Active, deprecated, etc.          |
      | Actions      | Edit, view, manage options        |
    And I should be able to filter and sort packages
    And I should be able to perform bulk actions

  @web @dashboard @analytics
  Scenario: Publisher analytics dashboard
    Given I am logged in as a publisher with popular packages
    When I navigate to the analytics section
    Then I should see analytics charts and metrics:
      | Metric Type      | Visualization                     |
      | Download Trends  | Line chart over time              |
      | Popular Packages | Bar chart of top packages         |
      | Geographic Data  | Map of downloads by region        |
      | User Engagement  | Package ratings and feedback      |
      | Trust Tier Dist  | Pie chart of packages by tier     |
    And I should be able to select different time ranges
    And I should be able to export analytics data

  @web @dashboard @security
  Scenario: Security alerts and notifications
    Given I am logged in as a publisher with packages under security review
    When I navigate to the security section
    Then I should see security alerts:
      | Alert Type       | Information                       |
      | Scan Failures    | Packages with failed scans        |
      | New Vulnerabilities| Recently discovered issues      |
      | Policy Violations| Packages violating policies      |
      | Trust Tier Changes| Recent tier promotions/demotions |
    And each alert should have clear action items
    And I should be able to acknowledge or dismiss alerts

  @web @publishing @new
  Scenario: Publish new package workflow
    Given I am logged in as a publisher
    When I click "Publish New Package"
    Then I should see the package publishing wizard:
      | Step             | Content                           |
      | Package Upload   | File upload and manifest validation|
      | Package Details  | Name, description, categories     |
      | Capabilities     | Tools, resources, prompts config  |
      | Security Review  | Security policy acknowledgment    |
      | Publication      | Final review and publish          |
    And I should be guided through each step with validation
    And I should see progress indicators

  @web @publishing @validation
  Scenario: Package publishing validation
    Given I am in the package publishing workflow
    When I upload a package with invalid manifest
    Then I should see validation errors clearly displayed
    And I should see specific guidance on fixing issues
    And I should not be able to proceed until issues are resolved
    And I should see examples of correct manifest format

  @web @publishing @preview
  Scenario: Package publication preview
    Given I have completed the package publishing wizard
    When I reach the final review step
    Then I should see a preview of how the package will appear:
      | Preview Section  | Content                           |
      | Package Card     | How it appears in search results  |
      | Detail Page      | Full package detail preview       |
      | Installation     | Installation command preview      |
      | Security Info    | Initial security status           |
    And I should be able to go back and edit any information
    And I should see estimated time for security scanning

  @web @responsive @mobile
  Scenario: Mobile responsive design
    Given I am using a mobile device
    When I visit various pages of the application
    Then all pages should display correctly on mobile:
      | Page Type        | Mobile Adaptations                |
      | Homepage         | Collapsible navigation, stacked layout|
      | Search Results   | Touch-friendly filters, card layout|
      | Package Detail   | Tabbed information, readable text |
      | Dashboard        | Drawer navigation, responsive tables|
      | Forms            | Touch-optimized inputs, validation|
    And all interactive elements should be touch-friendly
    And text should be readable without zooming

  @web @accessibility @compliance
  Scenario: Accessibility compliance
    When I navigate through the application using keyboard only
    Then all interactive elements should be reachable via Tab key
    And focus indicators should be clearly visible
    And I should be able to complete all user flows without a mouse
    And screen reader announcements should be appropriate
    And color contrast should meet WCAG AA guidelines
    And images should have appropriate alt text

  @web @performance @loading
  Scenario: Page performance and loading
    When I navigate to different pages
    Then initial page load should complete within 2 seconds
    And subsequent navigation should feel instant
    And search results should load within 1 second
    And large package lists should implement virtual scrolling
    And images and assets should load progressively
    And the application should work offline for cached content

  @web @integration @links
  Scenario: Integration with CLI and external tools
    Given I am viewing a package detail page
    Then I should see clear integration guidance:
      | Integration      | Information                       |
      | CLI Installation | Copy-paste command with mcpm      |
      | API Access       | REST API endpoints and examples   |
      | Documentation    | Links to comprehensive docs       |
      | GitHub/GitLab    | Links to source repositories      |
      | Support          | Contact and support information   |
    And CLI commands should include proper authentication if needed
    And I should see links to IDE integrations and extensions

  @web @notifications @realtime
  Scenario Outline: Real-time notifications for different events
    Given I am logged in as a publisher
    When <event_trigger>
    Then I should receive real-time notifications
    And notifications should appear in the application header
    And I should be able to manage notification preferences
    And I should receive email notifications for critical issues

    Examples:
      | event_trigger                           |
      | security scans complete for my packages |
      | new vulnerabilities are discovered      |
      | my packages receive reviews or ratings  |

  @web @search @suggestions
  Scenario: Search suggestions and autocomplete
    Given there are packages in the registry
    When I start typing in the search box
    Then I should see autocomplete suggestions including:
      | Suggestion Type  | Examples                          |
      | Package Names    | Matching package names            |
      | Publishers       | Publisher names and organizations |
      | Categories       | Available categories              |
      | Popular Terms    | Frequently searched terms         |
    And suggestions should be ranked by relevance and popularity
    And I should be able to navigate suggestions with keyboard
    And clicking a suggestion should perform the search