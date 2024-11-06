using Identity.API.Constants;
using Identity.API.Models;
using Identity.API.Services;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Models.DTOs;

namespace Identity.API.Controllers;

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


    public AuthController(UserManager<IdentityUser> userManager, AuthService authService, IConfiguration configuration,
        RefreshTokenService refreshTokenService,
        IPublishEndpoint publishEndpoint, ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _authService = authService;
        _refreshTokenService = refreshTokenService;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
        _jwtSettings = configuration.GetSection("Jwt").Get<JWTSettings>()!;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            return Unauthorized("Invalid login attempt.");


        await _publishEndpoint.Publish<SendEmailTwoFactorEmail>(new()
        {
            Email = user.Email,
            Token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email")
        });

        return Ok("Verification email sent");
    }
    
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
            return BadRequest("Invalid token");
        }

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

        await _publishEndpoint.Publish<SendEmailEmailVerification>(new()
        {
            Email = user.Email,
            Token = await _userManager.GenerateEmailConfirmationTokenAsync(user)
        });

        return Ok("Verification email sent");
    }

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

    [Authorize(Roles = Role.Admin)]
    [HttpGet("admin")]
    public IActionResult Admin()
    {
        return Ok("Admin");
    }
}