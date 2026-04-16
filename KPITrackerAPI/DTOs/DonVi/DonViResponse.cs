namespace KPITrackerAPI.DTOs.DonVi
{
    public class DonViDto
    {
        public long Id { get; set; }
        public string MaDonVi { get; set; } = string.Empty;
        public string TenDonVi { get; set; } = string.Empty;
        public string LoaiDonVi { get; set; } = string.Empty;

        public long? DonViChaId { get; set; }
        public string? TenDonViCha { get; set; }

        public string TrangThai { get; set; } = string.Empty;
        public string? DiaChi { get; set; }
        public string? NguoiDaiDien { get; set; }
        public string? SoDienThoai { get; set; }
        public string? Email { get; set; }
        public string? GhiChu { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
