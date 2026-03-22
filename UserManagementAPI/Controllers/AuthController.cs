using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UserManagementAPI.Data;              // 🔥 QUAN TRỌNG
using UserManagementAPI.DTOs;
using UserManagementAPI.DTOs.Auth;
using UserManagementAPI.Entities;
using UserManagementAPI.Interfaces;
using UserManagementAPI.Services;

namespace UserManagementAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context; // 🔥 thêm dòng này

    public AuthController(
        IAuthService authService,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context) // 🔥 inject vào đây
    {
        _authService = authService;
        _userManager = userManager;
        _context = context; // 🔥 gán vào đây
    }

    // ================= REGISTER =================
    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return Created(string.Empty, result);
    }

    // ================= LOGIN =================
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(result);
    }

    // ================= REFRESH TOKEN =================
    [HttpPost("refresh-token")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
    {
        var result = await _authService.RefreshTokenAsync(dto.RefreshToken);
        return Ok(result);
    }

    // ================= LOGOUT =================
    [Authorize]
    [HttpDelete("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto dto)
    {
        await _authService.LogoutAsync(User, dto.RefreshToken);
        return NoContent();
    }

    // ================= GET CURRENT USER =================
    [Authorize]

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("Invalid token");

        // Khai báo kiểu rõ ràng cho tuple
        (string? id, string? email, string? fullName, List<string> roles, List<string> permissions)
            = await _authService.GetCurrentUserAsync(userId);

        if (id == null)
            return NotFound("User not found");

        return Ok(new
        {
            UserId = id,
            Email = email,
            FullName = fullName,
            Roles = roles,
            Permissions = permissions
        });
    }

    // ================= FORGOT PASSWORD =================
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] RequestPasswordResetDto dto)
    {
        await _authService.RequestPasswordResetAsync(dto);
        return Ok("Reset password email sent");
    }

    // ================= RESET PASSWORD =================
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        await _authService.ResetPasswordAsync(dto);
        return NoContent();
    }
}