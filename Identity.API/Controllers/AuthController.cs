using Identity.API.Models;
using Identity.API.Services;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Constants;
using SharedLib.Models.DTOs;

namespace Identity.API.Controllers;

/// <summary>
/// Controller for handling authentication-related actions.
/// </summary>
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AuthService _authService;
    private readonly JWTSettings _jwtSettings;
    private readonly RefreshTokenService _refreshTokenService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="userManager">The user manager.</param>
    /// <param name="authService">The authentication service.</param>
    /// <param name="refreshTokenService">The refresh token service.</param>
    /// <param name="publishEndpoint">The publish endpoint for messaging.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="jwtSettings">The JWT settings.</param>
    public AuthController(UserManager<IdentityUser> userManager, AuthService authService,
        RefreshTokenService refreshTokenService,
        IPublishEndpoint publishEndpoint, ILogger<AuthController> logger,
        JWTSettings jwtSettings
    )
    {
        _userManager = userManager;
        _authService = authService;
        _refreshTokenService = refreshTokenService;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
        _jwtSettings = jwtSettings;
    }

    /// <summary>
    /// Handles user login.
    /// </summary>
    /// <param name="model">The login model containing user credentials.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the login attempt.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
        {
            if (user is not null)
            {
                await _userManager.AccessFailedAsync(user);
            }

            return Unauthorized("Invalid login attempt.");
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            return BadRequest($"Account locked until {await _userManager.GetLockoutEndDateAsync(user)}");
        }

        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            await _publishEndpoint.Publish<EmailVerification>(new()
            {
                Email = user.Email,
                Token = await _userManager.GenerateEmailConfirmationTokenAsync(user)
            });

            return BadRequest("Email not confirmed, verification email sent");
        }

        await _publishEndpoint.Publish<TwoFactorEmail>(new()
        {
            Email = user.Email!,
            Token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email")
        });

        return Ok("Verification email sent");
    }

    /// <summary>
    /// Handles two-factor authentication.
    /// </summary>
    /// <param name="model">The two-factor model containing the token and email.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the two-factor authentication.</returns>
    [HttpPost("two-factor")]
    public async Task<IActionResult> TwoFactor([FromBody] TwoFactorModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return BadRequest("User not found");
        }

        var result = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", model.Token);
        if (!result)
        {
            await _userManager.AccessFailedAsync(user);
            return BadRequest("Invalid token");
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        var accessToken = await _authService.GenerateJwtTokenAsync(user.Email!);
        var refreshToken = _authService.GenerateRefreshToken();

        var rt = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpirationDate = DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        };

        _refreshTokenService.SaveRefreshTokenAsync(rt);

        return Ok(new TokenRequest { AccessToken = accessToken, RefreshToken = refreshToken });
    }

    /// <summary>
    /// Handles user registration.
    /// </summary>
    /// <param name="model">The registration model containing user details.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the registration attempt.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var user = new IdentityUser { Email = model.Email, UserName = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            return BadRequest("Internal error");
        }

        await _userManager.AddToRoleAsync(user, Role.User);

        await _publishEndpoint.Publish<EmailVerification>(new()
        {
            Email = user.Email,
            Token = await _userManager.GenerateEmailConfirmationTokenAsync(user)
        });

        return Ok("Verification email sent");
    }

    /// <summary>
    /// Confirms the user's email.
    /// </summary>
    /// <param name="model">The confirm email model containing the token and email.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the email confirmation.</returns>
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return BadRequest("User not found");
        }

        var result = await _userManager.ConfirmEmailAsync(user, model.Token);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        await _userManager.SetTwoFactorEnabledAsync(user, true);

        var accessToken = await _authService.GenerateJwtTokenAsync(user.Email!);
        var refreshToken = _authService.GenerateRefreshToken();

        var rt = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpirationDate = DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        };

        await _refreshTokenService.SaveRefreshTokenAsync(rt);

        return Ok(new TokenRequest { AccessToken = accessToken, RefreshToken = refreshToken });
    }

    /// <summary>
    /// Refreshes the JWT token.
    /// </summary>
    /// <param name="model">The refresh token request model containing the refresh token.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the token refresh.</returns>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest model)
    {
        var rt = await _refreshTokenService.GetRefreshTokenAsync(model.RefreshToken);
        if (rt == null)
        {
            return BadRequest("Invalid refresh token");
        }

        if (rt.ExpirationDate < DateTime.Now)
        {
            return BadRequest("Refresh token expired");
        }

        var user = await _userManager.FindByIdAsync(rt.UserId.ToString());
        if (user == null)
        {
            return BadRequest("User not found");
        }

        _refreshTokenService.RemoveRefreshToken(model.RefreshToken);

        var accessToken = await _authService.GenerateJwtTokenAsync(user.Email!);
        var refreshToken = _authService.GenerateRefreshToken();

        rt.Token = refreshToken;
        rt.ExpirationDate = DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpirationDays);

        await _refreshTokenService.SaveRefreshTokenAsync(rt);

        return Ok(new TokenRequest { AccessToken = accessToken, RefreshToken = refreshToken });
    }

    /// <summary>
    /// Endpoint accessible only by admin users.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the admin action.</returns>
    [Authorize(Roles = Role.Admin)]
    [HttpGet("admin")]
    public IActionResult Admin()
    {
        return Ok("Admin");
    }
}