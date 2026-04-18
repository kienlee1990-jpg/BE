namespace KPITrackerAPI.DTOs.NhomThiDua
{
    public class NhomThiDuaDto
    {
        public long Id { get; set; }
        public string MaNhom { get; set; } = string.Empty;
        public string TenNhom { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public List<NhomThiDuaDonViDto> DonVis { get; set; } = [];
        public List<NhomThiDuaChiTieuDto> ChiTieus { get; set; } = [];
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
