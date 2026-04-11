using System.ComponentModel.DataAnnotations;

namespace KPI_Tracker_API.Models.DTOs.ChiTietGiaoChiTieu
{
    public class CreateChiTietGiaoChiTieuDto
    {
        [Required]
        public long DotGiaoChiTieuId { get; set; }

        [Required]
        public long DanhMucChiTieuId { get; set; }

        [Required]
        public long DonViNhanId { get; set; }
        [MaxLength(30)]
        public string? TanSuatBaoCao { get; set; }

        public long? DonViThucHienChinhId { get; set; }

        public decimal? GiaTriMucTieu { get; set; }

        public string? GiaTriMucTieuText { get; set; }

        public long? ChiTietGiaoChaId { get; set; }

        public string? GhiChu { get; set; }

        public int? ThuTuHienThi { get; set; }

        public string? TrangThai { get; set; }

        public string? CreatedBy { get; set; }
    }
}