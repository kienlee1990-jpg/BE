using System.ComponentModel.DataAnnotations;

namespace KPI_Tracker_API.DTOs.DotGiaoChiTieu
{
    public class CreateDotGiaoChiTieuDto
    {
        [Required]
        [MaxLength(50)]
        public string MaDotGiao { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string TenDotGiao { get; set; } = string.Empty;

        [Required]
        public int NamApDung { get; set; }

        [Required]
        public string NguonDotGiao { get; set; } = string.Empty;

        [Required]
        public string CapGiao { get; set; } = string.Empty;

        [Required]
        public long DonViGiaoId { get; set; }

        [Required]
        public DateTime NgayGiao { get; set; }

        public string? GhiChu { get; set; }
    }
}