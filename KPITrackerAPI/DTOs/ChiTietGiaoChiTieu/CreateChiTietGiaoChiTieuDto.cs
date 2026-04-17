using System.ComponentModel.DataAnnotations;

namespace KPITrackerAPI.DTOs.ChiTietGiaoChiTieu
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

        public decimal? GiaTriDauKyCoDinh { get; set; }

        public string? TieuChiDanhGia { get; set; }

        public string? LoaiMocSoSanh { get; set; }

        public string? ChieuSoSanh { get; set; }

        public string? QuyTacDanhGia { get; set; }

        public long? ChiTietGiaoChaId { get; set; }

        public string? GhiChu { get; set; }

        public int? ThuTuHienThi { get; set; }

        public string? TrangThai { get; set; }

        public string? CreatedBy { get; set; }

        public List<ChiTietTieuChiConDto> TieuChiCon { get; set; } = [];
    }
}
