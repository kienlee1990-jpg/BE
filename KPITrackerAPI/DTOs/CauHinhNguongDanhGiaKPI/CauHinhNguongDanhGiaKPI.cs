namespace KPITrackerAPI.DTOs.CauHinhNguongDanhGiaKPI
{
    public class CreateCauHinhNguongDanhGiaKPIDto
    {
        public long? DanhMucChiTieuId { get; set; }
        public decimal TuTyLe { get; set; }
        public decimal DenTyLe { get; set; }
        public string XepLoai { get; set; } = string.Empty;
        public string DieuKienThoiHan { get; set; } = string.Empty;
        public decimal? Diem { get; set; }
        public string? GhiChu { get; set; }
    }

    public class UpdateCauHinhNguongDanhGiaKPIDto : CreateCauHinhNguongDanhGiaKPIDto
    {
    }

    public class CauHinhNguongDanhGiaKPIQueryDto
    {
        public long? DanhMucChiTieuId { get; set; }
        public string? Keyword { get; set; }
    }
}
