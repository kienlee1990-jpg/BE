namespace KPITrackerAPI.DTOs.TheoDoiThucHienKPI
{
    public class TheoDoiThucHienKPIDto
    {
        public long Id { get; set; }

        public long ChiTietGiaoChiTieuId { get; set; }
        public string? TenChiTieu { get; set; }
        public string? TenDonViNhan { get; set; }

        public long KyBaoCaoKPIId { get; set; }
        public string? MaKy { get; set; }
        public string? TenKy { get; set; }
        public string? LoaiKy { get; set; }
        public int Nam { get; set; }
        public int? SoKy { get; set; }

        public decimal? GiaTriDauKy { get; set; }
        public decimal? GiaTriPhatSinhTrongKy { get; set; }
        public decimal? GiaTriThucHienTrongKy { get; set; }
        public decimal? GiaTriCuoiKy { get; set; }
        public decimal? GiaTriLuyKe { get; set; }
        public decimal? GiaTriPhatSinhLuyKe { get; set; }

        public string? NhanXet { get; set; }
        public string TrangThai { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? MaChiTieu { get; set; }
    }
}
