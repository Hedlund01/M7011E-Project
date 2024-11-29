           using Identity.API.Data;
           using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class AccountController: ControllerBase
{
    
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
 
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        return Ok(user);
    }
    
    [HttpPost("setRole")]
    public async Task<IActionResult> SetRole([FromBody] SetRoleModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, roles);
        await _userManager.AddToRoleAsync(user, model.Role);
        return Ok();
    }
}

public class SetRoleModel
{
    public string UserId { get; set; }
    public string Role { get; set; }
}