using System.ComponentModel.DataAnnotations;

namespace KPI_Tracker_API.Models.DTOs.ChiTietGiaoChiTieu
{
    public class ChiTietGiaoChiTieuDto
    {
        public long Id { get; set; }
        public long DotGiaoChiTieuId { get; set; }
        public string? TenDotGiaoChiTieu { get; set; }

        public long DanhMucChiTieuId { get; set; }
        public string? TenDanhMucChiTieu { get; set; }
        [MaxLength(30)]
        public string? TanSuatBaoCao { get; set; }

        public long DonViNhanId { get; set; }
        public string? TenDonViNhan { get; set; }

        public long? DonViThucHienChinhId { get; set; }
        public string? TenDonViThucHienChinh { get; set; }

        public decimal? GiaTriMucTieu { get; set; }
        public string? GiaTriMucTieuText { get; set; }

        public long? ChiTietGiaoChaId { get; set; }

        public string? GhiChu { get; set; }
        public int? ThuTuHienThi { get; set; }
        public string? TrangThai { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}