namespace KPITrackerAPI.DTOs.KyBaoCaoKPI
{
    public class KyBaoCaoKPIDto
    {
        public long Id { get; set; }
        public string MaKy { get; set; } = string.Empty;
        public string TenKy { get; set; } = string.Empty;
        public string LoaiKy { get; set; } = string.Empty;
        public int Nam { get; set; }
        public int? SoKy { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public DateTime NgayDauKy { get; set; }
        public DateTime NgayCuoiKy { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string? GhiChu { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
