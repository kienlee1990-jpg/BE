using System.ComponentModel.DataAnnotations;

namespace KPITrackerAPI.DTOs.DanhMucChiTieu
{
    public class CreateDanhMucChiTieuDto
    {
        [Required]
        [MaxLength(50)]
        public string MaChiTieu { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string TenChiTieu { get; set; } = string.Empty;

        [Required]
        public string NguonChiTieu { get; set; } = string.Empty;

        [Required]
        public string LoaiChiTieu { get; set; } = string.Empty;

        [Required]
        public string CapApDung { get; set; } = string.Empty;

        public string? LinhVucNghiepVu { get; set; }
        public string? DonViTinh { get; set; }
        public string? MoTa { get; set; }
        public string? HuongDanTinhToan { get; set; }

        public bool CoChoPhepPhanRa { get; set; } = true;

        public DateTime? NgayHieuLuc { get; set; }
        public DateTime? NgayHetHieuLuc { get; set; }

        public string? DieuKienHoanThanh { get; set; }
        public string? DieuKienKhongHoanThanh { get; set; }

        public decimal? TyLePhanTramMucTieu { get; set; }
        public string? LoaiMocSoSanh { get; set; }
        public string? ChieuSoSanh { get; set; }

        public bool BatBuocDatTatCaTieuChiCon { get; set; } = true;

        public List<TieuChiDanhGiaDto> TieuChiDanhGias { get; set; } = [];
    }
}
