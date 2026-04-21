using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using KPITrackerAPI.Data;              // ?? QUAN TR?NG
using KPITrackerAPI.DTOs;
using KPITrackerAPI.DTOs.Auth;
using KPITrackerAPI.Entities;
using KPITrackerAPI.Interfaces;
using KPITrackerAPI.Services;

namespace KPITrackerAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context; // ?? thêm dòng này

    public AuthController(
        IAuthService authService,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context) // ?? inject vào dây
    {
        _authService = authService;
        _userManager = userManager;
        _context = context; // ?? gán vào dây
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
        try
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            // ?? Tài kho?n b? khóa
            if (ex.Message == "ACCOUNT_INACTIVE")
            {
                return StatusCode(403, new
                {
                    code = "ACCOUNT_INACTIVE",
                    message = "Tài kho?n dã b? khóa"
                });
            }

            // ? Sai tài kho?n / m?t kh?u
            if (ex.Message == "INVALID_CREDENTIALS")
            {
                return Unauthorized(new
                {
                    code = "INVALID_CREDENTIALS",
                    message = "Sai tài kho?n ho?c m?t kh?u"
                });
            }

            // ?? L?i khác
            return StatusCode(500, new
            {
                code = "SERVER_ERROR",
                message = "L?i h? th?ng"
            });
        }
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

        // Khai báo ki?u rõ ràng cho tuple
        (string? id, string? userName, string? email, string? fullName, long? donViId, string? donVi, string? maDonVi, List<string> roles, List<string> permissions, List<string> rolePermissions)
            = await _authService.GetCurrentUserAsync(userId);

        if (id == null)
            return NotFound("User not found");

        return Ok(new
        {
            UserId = id,
            UserName = userName,
            Email = email,
            FullName = fullName,
            DonViId = donViId,
            DonVi = donVi,
            MaDonVi = maDonVi,
            Roles = roles,
            Permissions = permissions,
            RolePermissions = rolePermissions
        });
    }

    // ================= FORGOT PASSWORD =================
    [HttpPost("forgot-password")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ForgotPassword([FromBody] RequestPasswordResetDto dto)
    {
        var result = await _authService.RequestPasswordResetAsync(dto);
        return Ok(new
        {
            Email = result.Email,
            Token = result.Token
        });
    }

    // ================= RESET PASSWORD =================
    [HttpPost("reset-password")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        await _authService.ResetPasswordAsync(dto);
        return NoContent();
    }
}

