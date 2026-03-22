using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.Entities;

public class ApplicationUser : IdentityUser
{
    // ================= Navigation =================
    public ICollection<RefreshToken> RefreshTokens { get; set; }
        = new List<RefreshToken>();

    // 🔥 OPTIONAL: nếu muốn query nhanh quyền user
    public ICollection<UserPermission> UserPermissions { get; set; }
        = new List<UserPermission>();

    // ================= Thông tin thêm =================
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 🔥 OPTIONAL nâng cao (ăn điểm cao)
    public bool IsActive { get; set; } = true;
}