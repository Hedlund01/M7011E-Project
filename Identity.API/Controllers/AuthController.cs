using Identity.API.Constants;
using Identity.API.Models;
using Identity.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AuthService _authService;
    private readonly JWTSettings _jwtSettings;
    private readonly RefreshTokenService _refreshTokenService;
    

    public AuthController(UserManager<IdentityUser> userManager, AuthService authService, IConfiguration configuration, RefreshTokenService refreshTokenService)
    {
        _userManager = userManager;
        _authService = authService;
        _refreshTokenService = refreshTokenService;
        _jwtSettings = configuration.GetSection("Jwt").Get<JWTSettings>()!;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            return Unauthorized("Invalid login attempt."); 
        
        
        
        var accessToken = await _authService.GenerateJwtTokenAsync(user.Email!);
        var refreshToken = _authService.GenerateRefreshToken();
        
        var rt = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpirationDate = DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        };
        
        _refreshTokenService.SaveRefreshTokenAsync(rt);
        
        return Ok(new TokenRequest{ AccessToken = accessToken, RefreshToken = refreshToken });
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
        
        await _userManager.AddToRoleAsync(user, Role.User );

        var accessToken = await _authService.GenerateJwtTokenAsync(user.Email!);
        var refreshToken = _authService.GenerateRefreshToken();
        
        var rt = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpirationDate = DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        };
        
        _refreshTokenService.SaveRefreshTokenAsync(rt);
        return Ok(new TokenRequest{ AccessToken = accessToken, RefreshToken = refreshToken , ExpiresIn = _jwtSettings.AccessTokenExpirationMinutes });
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
        
        _refreshTokenService.SaveRefreshTokenAsync(rt);
        
        return Ok(new TokenRequest{ AccessToken = accessToken, RefreshToken = refreshToken });
    }
    
    [Authorize(Roles = Role.Admin)]
    [HttpGet("admin")]
    public IActionResult Admin()
    {
        return Ok("Admin");
    }
   
}
