namespace KPITrackerAPI.DTOs.NhomThiDua
{
    public class NhomThiDuaChiTieuDto
    {
        public long DanhMucChiTieuId { get; set; }
        public string MaChiTieu { get; set; } = string.Empty;
        public string TenChiTieu { get; set; } = string.Empty;
        public long? ChiTieuChaId { get; set; }
        public string? TenChiTieuCha { get; set; }
    }
}
