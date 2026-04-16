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
        public string? GhiChu { get; set; }
        public int? ThuTuHienThi { get; set; }
    }
}
