using System.ComponentModel.DataAnnotations;

namespace KPITrackerAPI.DTOs.DonVi
{
    public class CreateDonViDto
    {
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
        public string? TrangThai { get; set; }

        [MaxLength(255)]
        public string? DiaChi { get; set; }

        [MaxLength(100)]
        public string? NguoiDaiDien { get; set; }

        [MaxLength(50)]
        public string? SoDienThoai { get; set; }

        [MaxLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        public string? GhiChu { get; set; }
    }
}
