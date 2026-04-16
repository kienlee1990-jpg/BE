using KPITrackerAPI.Entities;

public class RefreshToken
{
    public int Id { get; set; }

    public string Token { get; set; } = string.Empty;

    public DateTime Expires { get; set; }

    public bool IsRevoked { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // ? NÊN THÊM
    public DateTime? RevokedAt { get; set; }

    // ? (optional nhung r?t x?n)
    public string? ReplacedByToken { get; set; }

    // FK
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
}
