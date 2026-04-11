using System.ComponentModel.DataAnnotations;

namespace KPI_Tracker_API.Models.DTOs.DanhGiaKPI
{
    public class UpdateDanhGiaKPIDto
    {
        [Required]
        public long ChiTietGiaoChiTieuId { get; set; }

        [Required]
        public long KyBaoCaoKPIId { get; set; }

        public decimal? GiaTriMucTieu { get; set; }
        public decimal? GiaTriDauKy { get; set; }
        public decimal? GiaTriCuoiKy { get; set; }
        public decimal? GiaTriCungKyNamTruoc { get; set; }

        public decimal? ChenhLechSoVoiDauKy { get; set; }
        public decimal? TyLeTangTruongSoVoiDauKy { get; set; }

        public decimal? ChenhLechSoVoiCungKyNamTruoc { get; set; }
        public decimal? TyLeTangTruongSoVoiCungKyNamTruoc { get; set; }

        public decimal? TyLeHoanThanh { get; set; }

        [MaxLength(50)]
        public string? XepLoai { get; set; }

        [MaxLength(50)]
        public string? KetQua { get; set; }

        public string? NhanXetDanhGia { get; set; }

        [MaxLength(100)]
        public string? NguoiDanhGia { get; set; }

        public DateTime? NgayDanhGia { get; set; }
    }
}