namespace KPI_Tracker_API.Models.DTOs.DanhGiaKPI
{
    public class DanhGiaKPIDto
    {
        public long Id { get; set; }
        public long ChiTietGiaoChiTieuId { get; set; }
        public long KyBaoCaoKPIId { get; set; }

        public string? MaChiTieu { get; set; }
        public string? TenChiTieu { get; set; }
        public string? TenDonViNhan { get; set; }

        public string? MaKy { get; set; }
        public string? TenKy { get; set; }
        public string? LoaiKy { get; set; }
        public int Nam { get; set; }
        public int? SoKy { get; set; }

        public decimal? GiaTriMucTieu { get; set; }
        public decimal? GiaTriDauKy { get; set; }
        public decimal? GiaTriCuoiKy { get; set; }
        public decimal? GiaTriCungKyNamTruoc { get; set; }

        public decimal? ChenhLechSoVoiDauKy { get; set; }
        public decimal? TyLeTangTruongSoVoiDauKy { get; set; }
        public decimal? ChenhLechSoVoiCungKyNamTruoc { get; set; }
        public decimal? TyLeTangTruongSoVoiCungKyNamTruoc { get; set; }
        public decimal? TyLeHoanThanh { get; set; }

        public string? XepLoai { get; set; }
        public string? KetQua { get; set; }
        public string? NhanXetDanhGia { get; set; }
        public string? NguoiDanhGia { get; set; }
        public DateTime? NgayDanhGia { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}