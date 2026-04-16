using System.ComponentModel.DataAnnotations;

namespace KPITrackerAPI.DTOs.KyBaoCaoKPI
{
    public class UpdateKyBaoCaoKPIDto
    {
        [Required]
        [MaxLength(50)]
        public string MaKy { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string TenKy { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string LoaiKy { get; set; } = string.Empty;

        [Required]
        public int Nam { get; set; }

        public int? SoKy { get; set; }

        [Required]
        public DateTime TuNgay { get; set; }

        [Required]
        public DateTime DenNgay { get; set; }

        [Required]
        public DateTime NgayDauKy { get; set; }

        [Required]
        public DateTime NgayCuoiKy { get; set; }

        [MaxLength(30)]
        public string? TrangThai { get; set; }

        public string? GhiChu { get; set; }

        [MaxLength(100)]
        public string? UpdatedBy { get; set; }
    }
}
