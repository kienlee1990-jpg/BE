using System.ComponentModel.DataAnnotations;

namespace KPI_Tracker_API.DTOs
{
    public class CreateDotGiaoChiTieuDto
    {
        [Required]
        [MaxLength(50)]
        public string MaDotGiao { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string TenDotGiao { get; set; } = string.Empty;

        public int NamApDung { get; set; }

        [Required]
        [MaxLength(50)]
        public string NguonDotGiao { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string CapGiao { get; set; } = string.Empty;

        public long DonViGiaoId { get; set; }

        public DateTime NgayGiao { get; set; }

        public DateTime? NgayKetThuc { get; set; }

        [MaxLength(50)]
        public string TrangThai { get; set; } = "DRAFT";

        public string? GhiChu { get; set; }
    }
}