namespace KPI_Tracker_API.DTOs.DotGiaoChiTieu
{
    public class DotGiaoChiTieuResponseDto
    {
        public long Id { get; set; }
        public string MaDotGiao { get; set; } = string.Empty;
        public string TenDotGiao { get; set; } = string.Empty;
        public int NamApDung { get; set; }
        public string NguonDotGiao { get; set; } = string.Empty;
        public string CapGiao { get; set; } = string.Empty;
        public long DonViGiaoId { get; set; }
        public DateTime NgayGiao { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string? GhiChu { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}