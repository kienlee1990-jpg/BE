namespace KPI_Tracker_API.Responses
{
    public class CauHinhNguongDanhGiaKPIResponse
    {
        public long Id { get; set; }
        public long? DanhMucChiTieuId { get; set; }
        public string? TenChiTieu { get; set; }
        public decimal TuTyLe { get; set; }
        public decimal DenTyLe { get; set; }
        public string XepLoai { get; set; } = string.Empty;
        public decimal? Diem { get; set; }
        public string? GhiChu { get; set; }
    }
}