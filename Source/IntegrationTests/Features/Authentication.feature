Feature: Authentication and Authorization API
  In order to secure the MCP Hub platform
  As a user or system administrator
  I want to authenticate users and control access to resources

  Background:
    Given the MCP Hub API is running
    And the database is initialized with test data

  @api @auth @login
  Scenario: Successful user login
    Given there is a registered user with email "test@example.com" and password "SecurePass123!"
    When I login with email "test@example.com" and password "SecurePass123!"
    Then I should receive a successful response
    And the response should contain a valid JWT access token
    And the response should contain a refresh token
    And the response should contain user profile information
    And the token should have an expiration time

  @api @auth @login @invalid
  Scenario: Login with invalid credentials
    Given there is a registered user with email "test@example.com"
    When I login with email "test@example.com" and password "WrongPassword"
    Then I should receive a "Bad Request" response
    And the response should contain an error message about invalid credentials
    And no tokens should be provided

  @api @auth @login @nonexistent
  Scenario: Login with non-existent user
    When I login with email "nonexistent@example.com" and password "AnyPassword123!"
    Then I should receive a "Bad Request" response
    And the response should contain an error message about invalid credentials
    And no tokens should be provided

  @api @auth @login @lockout
  Scenario: Account lockout after multiple failed attempts
    Given there is a registered user with email "test@example.com"
    When I attempt to login with wrong password 5 times
    Then the account should be locked out
    And subsequent login attempts should return "Bad Request" with lockout message
    And the user should not be able to login even with correct password

  @api @auth @refresh
  Scenario: Refresh expired access token
    Given I have a valid refresh token from a previous login
    When I request a token refresh using the refresh token
    Then I should receive a successful response
    And the response should contain a new JWT access token
    And the response should contain a new refresh token
    And the old refresh token should be invalidated

  @api @auth @refresh @invalid
  Scenario: Refresh with invalid token
    When I request a token refresh using an invalid refresh token
    Then I should receive a "Bad Request" response
    And the response should contain an error message about invalid token
    And no new tokens should be provided

  @api @auth @refresh @expired
  Scenario: Refresh with expired token
    Given I have an expired refresh token
    When I request a token refresh using the expired token
    Then I should receive a "Bad Request" response
    And the response should contain an error message about invalid token
    And no new tokens should be provided

  @api @auth @logout
  Scenario: User logout
    Given I am authenticated with a valid access token
    When I logout
    Then I should receive a successful response
    And the refresh token should be invalidated
    And subsequent requests with the access token should eventually fail

  @api @auth @profile
  Scenario: Get user profile with valid token
    Given I am authenticated as user "test@example.com"
    When I request my user profile
    Then I should receive a successful response
    And the response should contain my user information
    And the response should include email, name, and account details

  @api @auth @profile @unauthorized
  Scenario: Get user profile without authentication
    Given I am not authenticated
    When I request user profile information
    Then I should receive an "Unauthorized" response
    And no user information should be provided

  @api @auth @profile @invalid
  Scenario: Get user profile with invalid token
    Given I have an invalid or malformed JWT token
    When I request my user profile
    Then I should receive an "Unauthorized" response
    And no user information should be provided

  @api @auth @registration @email
  Scenario: User registration with email verification
    Given I have valid registration details
    When I register a new account with email "newuser@example.com"
    Then I should receive a "Created" response
    And a verification email should be sent
    And the account should be created but not yet verified
    And I should not be able to login until verification

  @api @auth @registration @duplicate
  Scenario: Registration with existing email
    Given there is a registered user with email "existing@example.com"
    When I attempt to register with the same email "existing@example.com"
    Then I should receive a "Bad Request" response
    And the response should indicate that the email is already taken
    And no new account should be created

  @api @auth @registration @validation
  Scenario Outline: Registration with invalid data
    When I attempt to register with the following data:
      | Field    | Value      |
      | email    | <email>    |
      | password | <password> |
      | name     | <name>     |
    Then I should receive a "Bad Request" response
    And the response should contain validation errors for "<field>"
    And no account should be created

    Examples:
      | email           | password    | name | field    |
      | invalid-email   | ValidPass1! | John | email    |
      | test@valid.com  | weak        | John | password |
      | test@valid.com  | ValidPass1! |      | name     |

  @api @auth @roles @publisher
  Scenario: Publisher role authorization
    Given I am authenticated with role "Publisher"
    When I attempt to create a new package
    Then I should be authorized to perform the action
    And the package creation should proceed

  @api @auth @roles @user
  Scenario: Regular user authorization limits
    Given I am authenticated with role "User" only
    When I attempt to create a new package
    Then I should receive a "Forbidden" response
    And the package should not be created

  @api @auth @roles @admin
  Scenario: Admin role permissions
    Given I am authenticated with role "Admin"
    When I attempt to access admin-only endpoints
    Then I should be authorized for all admin operations
    And I should be able to manage any user's resources

  @api @auth @apikey
  Scenario: API key authentication
    Given I have a valid API key for my account
    When I make requests using the API key in headers
    Then I should be authenticated successfully
    And I should have the same permissions as token-based auth

  @api @auth @apikey @invalid
  Scenario: Invalid API key authentication
    When I make requests using an invalid API key
    Then I should receive an "Unauthorized" response
    And the request should be rejected

  @api @auth @apikey @revoked
  Scenario: Revoked API key authentication
    Given I have an API key that has been revoked
    When I make requests using the revoked API key
    Then I should receive an "Unauthorized" response
    And the request should be rejected

  @api @auth @security @bruteforce
  Scenario: Brute force protection
    Given there is a registered user
    When multiple failed login attempts occur from the same IP
    Then rate limiting should be applied
    And subsequent requests should be temporarily blocked
    And legitimate users from other IPs should not be affected

  @api @auth @security @token
  Scenario: JWT token security validation
    Given I have a valid JWT token
    When I modify the token payload or signature
    And I make requests with the tampered token
    Then I should receive an "Unauthorized" response
    And the request should be rejected

  @api @auth @security @expiration
  Scenario: Token expiration handling
    Given I have a JWT token that has expired
    When I make requests with the expired token
    Then I should receive an "Unauthorized" response
    And I should be prompted to refresh or re-authenticate

  @api @auth @multifactor
  Scenario: Multi-factor authentication setup
    Given I am authenticated as a user
    When I enable multi-factor authentication
    Then I should be able to set up TOTP
    And subsequent logins should require the second factor
    And backup codes should be provided

  @api @auth @multifactor @login
  Scenario: Login with multi-factor authentication
    Given I have multi-factor authentication enabled
    When I login with email and password
    Then I should be prompted for the second factor
    And login should only succeed with valid TOTP code
    And invalid codes should be rejected

  @api @auth @password @reset
  Scenario: Password reset request
    Given there is a registered user with email "user@example.com"
    When I request a password reset for "user@example.com"
    Then I should receive a successful response
    And a password reset email should be sent
    And the reset token should be valid for limited time

  @api @auth @password @change
  Scenario: Password change with valid current password
    Given I am authenticated as a user
    When I change my password from "OldPass123!" to "NewPass456!"
    Then I should receive a successful response
    And I should be able to login with the new password
    And the old password should no longer work

  @api @auth @session @concurrent
  Scenario: Concurrent session management
    Given I am logged in on multiple devices
    When I logout from one device
    Then only that device's session should be terminated
    And other devices should remain authenticated
    And I should be able to logout from all devices

  @api @auth @audit @logging
  Scenario: Authentication event logging
    Given audit logging is enabled
    When I perform various authentication actions
    Then all authentication events should be logged
    And logs should include IP address, user agent, and timestamp
    And failed attempts should be recorded with details