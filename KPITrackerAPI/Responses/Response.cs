namespace KPITrackerAPI.Responses
{
    public class KyBaoCaoKPIResponse
    {
        public int Id { get; set; }
        public string MaKy { get; set; } = string.Empty;
        public string TenKy { get; set; } = string.Empty;
        public string LoaiKy { get; set; } = string.Empty;
        public int Nam { get; set; }
        public int? SoKy { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public DateTime NgayDauKy { get; set; }
        public DateTime NgayCuoiKy { get; set; }
        public string? TrangThai { get; set; }
        public string? GhiChu { get; set; }
    }



    public class DanhGiaKPIResponse
    {
        public long Id { get; set; }
        public long GiaoChiTieuId { get; set; }
        public int KyBaoCaoKPIId { get; set; }

        public string? TenKy { get; set; }
        public string? TenDonVi { get; set; }
        public string? TenChiTieu { get; set; }

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
    }

   
}
