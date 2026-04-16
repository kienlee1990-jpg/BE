using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace KPITrackerAPI.Entities;

public class ApplicationUser : IdentityUser
{
    // ================= Navigation =================
    public ICollection<RefreshToken> RefreshTokens { get; set; }
        = new List<RefreshToken>();

    // ?? OPTIONAL: n?u mu?n query nhanh quy?n user
    public ICollection<UserPermission> UserPermissions { get; set; }
        = new List<UserPermission>();

    // ================= Thông tin thêm =================
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // ?? OPTIONAL nâng cao (an di?m cao)
    public bool IsActive { get; set; } = true;
}
