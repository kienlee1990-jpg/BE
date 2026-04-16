using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KPITrackerAPI.Entities
{
    [Table("DonVi")]
    public class DonVi
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string MaDonVi { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string TenDonVi { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string LoaiDonVi { get; set; } = string.Empty;

        public long? DonViChaId { get; set; }

        [MaxLength(30)]
        public string TrangThai { get; set; } = "HOAT_DONG";

        [MaxLength(255)]
        public string? DiaChi { get; set; }

        [MaxLength(100)]
        public string? NguoiDaiDien { get; set; }

        [MaxLength(50)]
        public string? SoDienThoai { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        public string? GhiChu { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey(nameof(DonViChaId))]
        public DonVi? DonViCha { get; set; }
    }
}
