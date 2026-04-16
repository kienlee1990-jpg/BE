using KPITrackerAPI.DTOs.User;
using KPITrackerAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KPITrackerAPI.Controllers;

[ApiController]
[Route("api/users")]
[Authorize] // T?t c? endpoint d?u require login
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }


    // ?? ADMIN or OWNER - C?p nh?t thông tin user
    

    // ?? CURRENT USER PROFILE - L?y profile c?a chính mình
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
            return Unauthorized();

        var user = await _userService.GetUserByIdAsync(currentUserId);

        if (user == null)
            return NotFound();

        return Ok(user);
    }
}
