using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KPITrackerAPI.Entities;

public class ApplicationUser : IdentityUser
{
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();

    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    public long? DonViId { get; set; }

    [ForeignKey(nameof(DonViId))]
    public DonVi? DonVi { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}
