using UserManagementAPI.DTOs;
using UserManagementAPI.DTOs.Auth;
using System.Security.Claims;

namespace UserManagementAPI.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task RequestPasswordResetAsync(RequestPasswordResetDto dto);
    Task ResetPasswordAsync(ResetPasswordDto dto);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(ClaimsPrincipal user, string refreshToken);
    Task<(string UserId, string Email, string FullName, List<string> Roles, List<string> Permissions)> GetCurrentUserAsync(string userId);
}