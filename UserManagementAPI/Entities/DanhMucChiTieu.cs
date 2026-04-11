using System.ComponentModel.DataAnnotations;

namespace KPI_Tracker_API.Entities
{
    public class DanhMucChiTieu
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string MaChiTieu { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string TenChiTieu { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string NguonChiTieu { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LoaiChiTieu { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string CapApDung { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? LinhVucNghiepVu { get; set; }

        [MaxLength(50)]
        public string? DonViTinh { get; set; }

        public string? MoTa { get; set; }
        public string? HuongDanTinhToan { get; set; }

        public bool CoChoPhepPhanRa { get; set; } = true;

        [MaxLength(50)]
        public string TrangThaiSuDung { get; set; } = "DANG_AP_DUNG";

        public DateTime? NgayHieuLuc { get; set; }
        public DateTime? NgayHetHieuLuc { get; set; }

        public string? DieuKienHoanThanh { get; set; }
        public string? DieuKienKhongHoanThanh { get; set; }

        public decimal? TyLePhanTramMucTieu { get; set; }

        [MaxLength(50)]
        public string? LoaiMocSoSanh { get; set; }

        [MaxLength(50)]
        public string? ChieuSoSanh { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<ChiTietGiaoChiTieu> ChiTietGiaoChiTieux { get; set; } = new List<ChiTietGiaoChiTieu>();
    }
}