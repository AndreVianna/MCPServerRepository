using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MCPHub.PublicApi.UnitTests.Services;

/// <summary>
/// Unit tests for JwtService
/// </summary>
public class JwtServiceTests {
    private readonly JwtService _jwtService;
    private readonly ILogger<JwtService> _logger;
    private readonly JwtOptions _jwtOptions;

    public JwtServiceTests() {
        _logger = Substitute.For<ILogger<JwtService>>();

        _jwtOptions = new JwtOptions {
            SecretKey = "TEST_SECRET_KEY_THAT_IS_LONG_ENOUGH_FOR_HMAC_SHA256_ALGORITHM_1234567890",
            Issuer = "MCPHub.Test",
            Audience = "MCPHub.Test.Clients",
            AccessTokenExpirationMinutes = 15,
            RefreshTokenExpirationDays = 7,
            ClockSkewMinutes = 5,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };

        var options = Substitute.For<IOptions<JwtOptions>>();
        options.Value.Returns(_jwtOptions);

        _jwtService = new JwtService(options, _logger);
    }

    [Fact]
    public async Task GenerateAccessTokenAsync_WithValidUser_ShouldReturnValidJwtToken() {
        // Arrange
        var user = CreateTestUser();

        // Act
        var token = await _jwtService.GenerateAccessTokenAsync(user);

        // Assert
        token.Should().NotBeNullOrEmpty();

        var tokenHandler = new JwtSecurityTokenHandler();
        var jsonToken = tokenHandler.ReadJwtToken(token);

        jsonToken.Issuer.Should().Be(_jwtOptions.Issuer);
        jsonToken.Audiences.Should().Contain(_jwtOptions.Audience);
        jsonToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Id.ToString());
        jsonToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == user.Email);
        jsonToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == user.UserName);
    }

    [Fact]
    public async Task GenerateAccessTokenAsync_WithPublisherUser_ShouldIncludePublisherClaim() {
        // Arrange
        var user = CreateTestUser();
        user.IsPublisher = true;

        // Act
        var token = await _jwtService.GenerateAccessTokenAsync(user);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jsonToken = tokenHandler.ReadJwtToken(token);

        jsonToken.Claims.Should().Contain(c => c.Type == "is_publisher" && c.Value == "true");
    }

    [Fact]
    public async Task GenerateAccessTokenAsync_WithDisplayName_ShouldIncludeDisplayNameClaim() {
        // Arrange
        var user = CreateTestUser();
        user.DisplayName = "Test Display Name";

        // Act
        var token = await _jwtService.GenerateAccessTokenAsync(user);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jsonToken = tokenHandler.ReadJwtToken(token);

        jsonToken.Claims.Should().Contain(c => c.Type == "display_name" && c.Value == "Test Display Name");
    }

    [Fact]
    public async Task GenerateAccessTokenAsync_WithNullUser_ShouldThrowArgumentNullException()
        // Act & Assert
        => await FluentActions.Invoking(() => _jwtService.GenerateAccessTokenAsync(null!))
            .Should().ThrowAsync<ArgumentNullException>();

    [Fact]
    public async Task GenerateRefreshTokenAsync_WithValidUser_ShouldReturnBase64String() {
        // Arrange
        var user = CreateTestUser();

        // Act
        var refreshToken = await _jwtService.GenerateRefreshTokenAsync(user);

        // Assert
        refreshToken.Should().NotBeNullOrEmpty();
        refreshToken.Length.Should().BeGreaterThan(0);

        // Should be valid base64
        FluentActions.Invoking(() => Convert.FromBase64String(refreshToken))
            .Should().NotThrow();
    }

    [Fact]
    public async Task GenerateRefreshTokenAsync_WithNullUser_ShouldThrowArgumentNullException()
        // Act & Assert
        => await FluentActions.Invoking(() => _jwtService.GenerateRefreshTokenAsync(null!))
            .Should().ThrowAsync<ArgumentNullException>();

    [Fact]
    public async Task ValidateAccessTokenAsync_WithValidToken_ShouldReturnClaimsPrincipal() {
        // Arrange
        var user = CreateTestUser();
        var token = await _jwtService.GenerateAccessTokenAsync(user);

        // Act
        var principal = await _jwtService.ValidateAccessTokenAsync(token);

        // Assert
        principal.Should().NotBeNull();
        principal!.FindFirst(JwtRegisteredClaimNames.Sub)?.Value.Should().Be(user.Id.ToString());
        principal.FindFirst(ClaimTypes.Email)?.Value.Should().Be(user.Email);
        principal.FindFirst(ClaimTypes.Name)?.Value.Should().Be(user.UserName);
    }

    [Fact]
    public async Task ValidateAccessTokenAsync_WithInvalidToken_ShouldReturnNull() {
        // Arrange
        var invalidToken = "invalid.jwt.token";

        // Act
        var principal = await _jwtService.ValidateAccessTokenAsync(invalidToken);

        // Assert
        principal.Should().BeNull();
    }

    [Fact]
    public async Task ValidateAccessTokenAsync_WithEmptyToken_ShouldReturnNull() {
        // Act
        var principal = await _jwtService.ValidateAccessTokenAsync(string.Empty);

        // Assert
        principal.Should().BeNull();
    }

    [Fact]
    public async Task ValidateAccessTokenAsync_WithExpiredToken_ShouldReturnNull() {
        // Arrange
        var user = CreateTestUser();
        var expiredOptions = new JwtOptions {
            SecretKey = _jwtOptions.SecretKey,
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            AccessTokenExpirationMinutes = -1, // Expired token
            RefreshTokenExpirationDays = 7,
            ClockSkewMinutes = 0 // No clock skew to ensure token is expired
        };

        var options = Substitute.For<IOptions<JwtOptions>>();
        options.Value.Returns(expiredOptions);
        var expiredJwtService = new JwtService(options, _logger);

        var expiredToken = await expiredJwtService.GenerateAccessTokenAsync(user);

        // Act
        var principal = await _jwtService.ValidateAccessTokenAsync(expiredToken);

        // Assert
        principal.Should().BeNull();
    }

    [Fact]
    public async Task RevokeRefreshTokenAsync_WithValidToken_ShouldReturnTrue() {
        // Arrange
        var user = CreateTestUser();
        var refreshToken = await _jwtService.GenerateRefreshTokenAsync(user);

        // Act
        var result = await _jwtService.RevokeRefreshTokenAsync(refreshToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task RevokeRefreshTokenAsync_WithEmptyToken_ShouldReturnFalse() {
        // Act
        var result = await _jwtService.RevokeRefreshTokenAsync(string.Empty);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetUserIdFromToken_WithValidToken_ShouldReturnUserId() {
        // Arrange
        var user = CreateTestUser();
        var token = await _jwtService.GenerateAccessTokenAsync(user);

        // Act
        var userId = _jwtService.GetUserIdFromToken(token);

        // Assert
        userId.Should().Be(user.Id);
    }

    [Fact]
    public void GetUserIdFromToken_WithInvalidToken_ShouldReturnNull() {
        // Arrange
        var invalidToken = "invalid.jwt.token";

        // Act
        var userId = _jwtService.GetUserIdFromToken(invalidToken);

        // Assert
        userId.Should().BeNull();
    }

    [Fact]
    public void GetUserIdFromToken_WithEmptyToken_ShouldReturnNull() {
        // Act
        var userId = _jwtService.GetUserIdFromToken(string.Empty);

        // Assert
        userId.Should().BeNull();
    }

    [Fact]
    public async Task GetTokenExpiration_WithValidToken_ShouldReturnExpirationTime() {
        // Arrange
        var user = CreateTestUser();
        var beforeGeneration = DateTime.UtcNow;
        var token = await _jwtService.GenerateAccessTokenAsync(user);
        var expectedExpiration = beforeGeneration.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);

        // Act
        var expiration = _jwtService.GetTokenExpiration(token);

        // Assert
        expiration.Should().NotBeNull();
        expiration!.Value.Should().BeCloseTo(expectedExpiration, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void GetTokenExpiration_WithInvalidToken_ShouldReturnNull() {
        // Arrange
        var invalidToken = "invalid.jwt.token";

        // Act
        var expiration = _jwtService.GetTokenExpiration(invalidToken);

        // Assert
        expiration.Should().BeNull();
    }

    [Fact]
    public void GetTokenExpiration_WithEmptyToken_ShouldReturnNull() {
        // Act
        var expiration = _jwtService.GetTokenExpiration(string.Empty);

        // Assert
        expiration.Should().BeNull();
    }

    [Fact]
    public async Task ValidateRefreshTokenAsync_WithRevokedToken_ShouldReturnNull() {
        // Arrange
        var user = CreateTestUser();
        var refreshToken = await _jwtService.GenerateRefreshTokenAsync(user);
        await _jwtService.RevokeRefreshTokenAsync(refreshToken);

        // Act
        var userId = await _jwtService.ValidateRefreshTokenAsync(refreshToken);

        // Assert
        userId.Should().BeNull();
    }

    [Fact]
    public async Task ValidateRefreshTokenAsync_WithEmptyToken_ShouldReturnNull() {
        // Act
        var userId = await _jwtService.ValidateRefreshTokenAsync(string.Empty);

        // Assert
        userId.Should().BeNull();
    }

    private static ApplicationUser CreateTestUser() => new("testuser", "test@example.com") {
        Id = Guid.NewGuid(),
        UserName = "testuser",
        Email = "test@example.com",
        EmailConfirmed = true
    };
}