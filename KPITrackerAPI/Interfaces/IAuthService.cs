using KPITrackerAPI.DTOs;
using KPITrackerAPI.DTOs.Auth;
using System.Security.Claims;

namespace KPITrackerAPI.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<(string Email, string Token)> RequestPasswordResetAsync(RequestPasswordResetDto dto);
    Task ResetPasswordAsync(ResetPasswordDto dto);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(ClaimsPrincipal user, string refreshToken);
    Task<(string UserId, string UserName, string Email, string FullName, long? DonViId, string? DonVi, string? MaDonVi, List<string> Roles, List<string> Permissions, List<string> RolePermissions)> GetCurrentUserAsync(string userId);
}
