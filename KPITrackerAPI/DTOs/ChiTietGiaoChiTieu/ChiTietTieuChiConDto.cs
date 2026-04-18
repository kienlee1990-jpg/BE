using System.ComponentModel.DataAnnotations;

namespace KPITrackerAPI.DTOs.ChiTietGiaoChiTieu
{
    public class ChiTietTieuChiConDto
    {
        [Required]
        public long DanhMucChiTieuId { get; set; }

        public decimal? GiaTriMucTieu { get; set; }
        public string? GiaTriMucTieuText { get; set; }
        public decimal? GiaTriDauKyCoDinh { get; set; }
        public string? TieuChiDanhGia { get; set; }
        public string? LoaiMocSoSanh { get; set; }
        public string? KieuSoSanh { get; set; }
        public string? ChieuSoSanh { get; set; }
        public string? QuyTacDanhGia { get; set; }
        public string? GhiChu { get; set; }
        public int? ThuTuHienThi { get; set; }
    }
}
