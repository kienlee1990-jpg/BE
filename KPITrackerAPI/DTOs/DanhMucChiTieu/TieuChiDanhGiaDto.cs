using System.ComponentModel.DataAnnotations;

namespace KPITrackerAPI.DTOs.DanhMucChiTieu
{
    public class TieuChiDanhGiaDto
    {
        public long? Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string MaChiTieu { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string TenChiTieu { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LoaiChiTieu { get; set; } = string.Empty;

        public int? ThuTuHienThi { get; set; }

        [MaxLength(50)]
        public string? DonViTinh { get; set; }

        public string? MoTa { get; set; }
        public string? HuongDanTinhToan { get; set; }
        public string? DieuKienHoanThanh { get; set; }
        public string? DieuKienKhongHoanThanh { get; set; }
        public decimal? TyLePhanTramMucTieu { get; set; }

        [MaxLength(50)]
        public string? LoaiMocSoSanh { get; set; }

        [MaxLength(50)]
        public string? ChieuSoSanh { get; set; }
    }
}
