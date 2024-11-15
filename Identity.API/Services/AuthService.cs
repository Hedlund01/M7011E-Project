using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SharedLib.Constants;

namespace Identity.API.Services;

public class AuthService(IConfiguration configuration, UserManager<IdentityUser> userManager, JWTSettings settings)
{
    public async Task<string> GenerateJwtTokenAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            throw new Exception("User not found");
        }
        var roles = await userManager.GetRolesAsync(user);
        
        if (user is null )
        {
            throw new Exception("User not found");
        }
        
        var claims = new []
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, roles.FirstOrDefault(Role.User))
        };

       
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(settings.AccessTokenExpirationMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    
    
}