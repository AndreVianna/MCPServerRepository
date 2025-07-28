using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MCPHub.PublicApi.Controllers;

namespace MCPHub.PublicApi.UnitTests.Controllers;

/// <summary>
/// Unit tests for AuthController
/// </summary>
public class AuthControllerTests
{
    private readonly AuthController _controller;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthController> _logger;

    public AuthControllerTests()
    {
        _userManager = Substitute.For<UserManager<ApplicationUser>>(
            Substitute.For<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        _signInManager = Substitute.For<SignInManager<ApplicationUser>>(
            _userManager, Substitute.For<IHttpContextAccessor>(), 
            Substitute.For<IUserClaimsPrincipalFactory<ApplicationUser>>(), null, null, null, null);
        _jwtService = Substitute.For<IJwtService>();
        _logger = Substitute.For<ILogger<AuthController>>();

        _controller = new AuthController(_userManager, _signInManager, _jwtService, _logger);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsSuccessResult()
    {
        // Arrange
        var request = new LoginRequest 
        { 
            Email = "test@example.com", 
            Password = "TestPassword123!" 
        };
        
        var user = CreateTestUser();
        var accessToken = "test-access-token";
        var refreshToken = "test-refresh-token";

        _userManager.FindByEmailAsync(request.Email).Returns(user);
        _signInManager.CheckPasswordSignInAsync(user, request.Password, true)
            .Returns(Microsoft.AspNetCore.Identity.SignInResult.Success);
        _jwtService.GenerateAccessTokenAsync(user, Arg.Any<CancellationToken>())
            .Returns(accessToken);
        _jwtService.GenerateRefreshTokenAsync(user, Arg.Any<CancellationToken>())
            .Returns(refreshToken);

        // Act
        var result = await _controller.Login(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        var authResult = okResult.Value.Should().BeOfType<AuthenticationResult>().Subject;
        
        authResult.IsSuccess.Should().BeTrue();
        authResult.User.Should().Be(user);
        authResult.AccessToken.Should().Be(accessToken);
        authResult.RefreshToken.Should().Be(refreshToken);
        authResult.ExpiresAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Login_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new LoginRequest 
        { 
            Email = "nonexistent@example.com", 
            Password = "TestPassword123!" 
        };

        _userManager.FindByEmailAsync(request.Email).Returns((ApplicationUser?)null);

        // Act
        var result = await _controller.Login(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = (BadRequestObjectResult)result;
        var authResult = badRequestResult.Value.Should().BeOfType<AuthenticationResult>().Subject;
        
        authResult.IsSuccess.Should().BeFalse();
        authResult.ErrorMessage.Should().Be("Invalid email or password");
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsBadRequest()
    {
        // Arrange
        var request = new LoginRequest 
        { 
            Email = "test@example.com", 
            Password = "WrongPassword" 
        };
        
        var user = CreateTestUser();

        _userManager.FindByEmailAsync(request.Email).Returns(user);
        _signInManager.CheckPasswordSignInAsync(user, request.Password, true)
            .Returns(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        // Act
        var result = await _controller.Login(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = (BadRequestObjectResult)result;
        var authResult = badRequestResult.Value.Should().BeOfType<AuthenticationResult>().Subject;
        
        authResult.IsSuccess.Should().BeFalse();
        authResult.ErrorMessage.Should().Be("Invalid email or password");
    }

    [Fact]
    public async Task Login_WithLockedOutAccount_ReturnsBadRequest()
    {
        // Arrange
        var request = new LoginRequest 
        { 
            Email = "test@example.com", 
            Password = "TestPassword123!" 
        };
        
        var user = CreateTestUser();

        _userManager.FindByEmailAsync(request.Email).Returns(user);
        _signInManager.CheckPasswordSignInAsync(user, request.Password, true)
            .Returns(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);

        // Act
        var result = await _controller.Login(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = (BadRequestObjectResult)result;
        var authResult = badRequestResult.Value.Should().BeOfType<AuthenticationResult>().Subject;
        
        authResult.IsSuccess.Should().BeFalse();
        authResult.ErrorMessage.Should().Be("Account is locked out");
    }

    [Fact]
    public async Task RefreshToken_WithValidToken_ReturnsSuccessResult()
    {
        // Arrange
        var request = new RefreshTokenRequest { RefreshToken = "valid-refresh-token" };
        var userId = Guid.NewGuid();
        var user = CreateTestUser();
        var newAccessToken = "new-access-token";
        var newRefreshToken = "new-refresh-token";

        _jwtService.ValidateRefreshTokenAsync(request.RefreshToken, Arg.Any<CancellationToken>())
            .Returns(userId);
        _userManager.FindByIdAsync(userId.ToString()).Returns(user);
        _jwtService.GenerateAccessTokenAsync(user, Arg.Any<CancellationToken>())
            .Returns(newAccessToken);
        _jwtService.GenerateRefreshTokenAsync(user, Arg.Any<CancellationToken>())
            .Returns(newRefreshToken);
        _jwtService.RevokeRefreshTokenAsync(request.RefreshToken, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _controller.RefreshToken(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        var tokenResult = okResult.Value.Should().BeOfType<TokenResult>().Subject;
        
        tokenResult.IsSuccess.Should().BeTrue();
        tokenResult.AccessToken.Should().Be(newAccessToken);
        tokenResult.RefreshToken.Should().Be(newRefreshToken);
        tokenResult.ExpiresAt.Should().NotBeNull();
    }

    [Fact]
    public async Task RefreshToken_WithInvalidToken_ReturnsBadRequest()
    {
        // Arrange
        var request = new RefreshTokenRequest { RefreshToken = "invalid-refresh-token" };

        _jwtService.ValidateRefreshTokenAsync(request.RefreshToken, Arg.Any<CancellationToken>())
            .Returns((Guid?)null);

        // Act
        var result = await _controller.RefreshToken(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = (BadRequestObjectResult)result;
        var tokenResult = badRequestResult.Value.Should().BeOfType<TokenResult>().Subject;
        
        tokenResult.IsSuccess.Should().BeFalse();
        tokenResult.ErrorMessage.Should().Be("Invalid refresh token");
    }

    [Fact]
    public async Task GetProfile_WithValidUser_ReturnsUserProfile()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateTestUser();
        user.Id = userId;

        // Mock the HttpContext and User claims
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new("sub", userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            }
        };

        _userManager.FindByIdAsync(userId.ToString()).Returns(user);

        // Act
        var result = await _controller.GetProfile(CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        var profileResult = okResult.Value.Should().BeOfType<UserProfileResult>().Subject;
        
        profileResult.IsSuccess.Should().BeTrue();
        profileResult.User.Should().Be(user);
    }

    [Fact]
    public async Task GetProfile_WithInvalidUserClaim_ReturnsBadRequest()
    {
        // Arrange
        var claims = new List<Claim>(); // No user ID claim
        var identity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            }
        };

        // Act
        var result = await _controller.GetProfile(CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = (BadRequestObjectResult)result;
        var profileResult = badRequestResult.Value.Should().BeOfType<UserProfileResult>().Subject;
        
        profileResult.IsSuccess.Should().BeFalse();
        profileResult.ErrorMessage.Should().Be("Invalid user context");
    }

    [Fact]
    public async Task Logout_WithValidUser_ReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new("sub", userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            }
        };

        // Act
        var result = await _controller.Logout(CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        var logoutResult = okResult.Value.Should().BeOfType<LogoutResult>().Subject;
        
        logoutResult.IsSuccess.Should().BeTrue();
    }

    private static ApplicationUser CreateTestUser()
    {
        return new ApplicationUser("testuser", "test@example.com")
        {
            Id = Guid.NewGuid(),
            UserName = "testuser",
            Email = "test@example.com",
            EmailConfirmed = true
        };
    }
}