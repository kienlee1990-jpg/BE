using UserManagementAPI.DTOs.User;
using UserManagementAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace UserManagementAPI.Controllers;

[ApiController]
[Route("api/users")]
[Authorize] // Tất cả endpoint đều require login
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }


    // 🟡 ADMIN or OWNER - Cập nhật thông tin user
    

    // 🟢 CURRENT USER PROFILE - Lấy profile của chính mình
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