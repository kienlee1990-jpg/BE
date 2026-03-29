using UserManagementAPI.Data;
using UserManagementAPI.DTOs;
using UserManagementAPI.DTOs.Auth;
using UserManagementAPI.Entities;
using UserManagementAPI.Helper;
using UserManagementAPI.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UserManagementAPI.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuthService> _logger;
    private readonly IPermissionService _permissionService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IConfiguration config,
        ApplicationDbContext context,
        ILogger<AuthService> logger,
        IPermissionService permissionService)
    {
        _userManager = userManager;
        _config = config;
        _context = context;
        _logger = logger;
        _permissionService = permissionService;
    }

    private int AccessTokenExpirationHours =>
        int.Parse(_config["Jwt:AccessTokenExpirationHours"] ?? "3");

    private int RefreshTokenExpirationDays =>
        int.Parse(_config["Jwt:RefreshTokenExpirationDays"] ?? "7");

    private const string DefaultRole = "Customer";

    // ================= REGISTER =================
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            Email = dto.Email,
            UserName = dto.UserName,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new ApplicationException(string.Join(",", result.Errors.Select(x => x.Description)));

        //await _userManager.AddToRoleAsync(user, DefaultRole);

        _logger.LogInformation("User registered: {Email}", user.Email);

        return await GenerateAuthResponse(user);
    }

    // ================= LOGIN =================
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        // ❌ Sai tài khoản hoặc mật khẩu
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
        {
            _logger.LogWarning("Login failed for {Email}", dto.Email);
            throw new Exception("INVALID_CREDENTIALS");
        }

        // 🚫 Tài khoản bị khóa
        if (!user.IsActive)
        {
            _logger.LogWarning("Login blocked (inactive account): {Email}", user.Email);
            throw new Exception("ACCOUNT_INACTIVE");
        }

        // ✅ Thành công
        _logger.LogInformation("User logged in: {Email}", user.Email);

        return await GenerateAuthResponse(user);
    }

    // ================= CORE TOKEN GENERATION =================
    private async Task<AuthResponseDto> GenerateAuthResponse(ApplicationUser user)
    {
        var accessToken = await GenerateJwtAsync(user);

        // Revoke old refresh tokens
        var oldTokens = await _context.RefreshTokens
            .Where(x => x.UserId == user.Id && !x.IsRevoked)
            .ToListAsync();
        foreach (var t in oldTokens)
        {
            t.IsRevoked = true;
            t.RevokedAt = DateTime.UtcNow;
        }

        var refreshToken = new RefreshToken
        {
            Token = TokenHelper.GenerateRefreshToken(),
            Expires = DateTime.UtcNow.AddDays(RefreshTokenExpirationDays),
            UserId = user.Id
        };
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            Expiration = DateTime.UtcNow.AddHours(AccessTokenExpirationHours)
        };
    }

    // ================= JWT =================
    private async Task<string> GenerateJwtAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var permissions = await _permissionService.GetPermissionsAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? "")
        };

        // Role claims
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        // Permission claims
        claims.AddRange(permissions.Select(p => new Claim("permission", p)));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(AccessTokenExpirationHours),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // ================= REFRESH TOKEN =================
    public async Task<AuthResponseDto> RefreshTokenAsync(string token)
    {
        var refreshToken = await _context.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == token);

        if (refreshToken == null || refreshToken.IsRevoked || refreshToken.Expires < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid refresh token");

        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTime.UtcNow;

        _logger.LogInformation("Refresh token used for user {UserId}", refreshToken.UserId);
        return await GenerateAuthResponse(refreshToken.User!);
    }

    // ================= LOGOUT =================
    public async Task LogoutAsync(ClaimsPrincipal userPrincipal, string refreshToken)
    {
        var userId = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Invalid token");

        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == refreshToken);

        if (token == null)
            throw new ApplicationException("Refresh token not found");
        if (token.UserId != userId)
            throw new UnauthorizedAccessException("Forbidden");

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} logged out", userId);
    }

    // ================= FORGOT / RESET PASSWORD =================
    public async Task RequestPasswordResetAsync(RequestPasswordResetDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null) return;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(token);
        var resetLink = $"{_config["App:ClientUrl"]}/reset-password?email={dto.Email}&token={encodedToken}";

        _logger.LogInformation("Password reset requested for {Email}", dto.Email);
        // TODO: inject EmailService để gửi mail
        Console.WriteLine(resetLink);
    }

    public async Task ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            throw new ApplicationException("Invalid request");

        var decodedToken = Uri.UnescapeDataString(dto.Token);
        var result = await _userManager.ResetPasswordAsync(user, decodedToken, dto.NewPassword);

        if (!result.Succeeded)
            throw new ApplicationException(string.Join(",", result.Errors.Select(x => x.Description)));

        _logger.LogInformation("Password reset successful for {Email}", dto.Email);
    }

    // ================= GET ME =================
    public async Task<(string UserId, string Email, string FullName, List<string> Roles, List<string> Permissions)> GetCurrentUserAsync(string userId)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return (null!, null!, null!, new List<string>(), new List<string>());

        // Lấy role
        var roles = (await _userManager.GetRolesAsync(user)).ToList();

        // Lấy permission chỉ IsGranted = true
        var permissions = await _context.UserPermissions
            .Where(up => up.UserId == userId && up.IsGranted)
            .Join(_context.Permissions,
                  up => up.PermissionId,
                  p => p.Id,
                  (up, p) => p.Name)
            .Distinct()
            .ToListAsync();

        return (user.Id, user.Email, user.FullName, roles, permissions);
    }
}