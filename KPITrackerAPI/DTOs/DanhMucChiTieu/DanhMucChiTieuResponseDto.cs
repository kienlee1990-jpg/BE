namespace KPITrackerAPI.DTOs.DanhMucChiTieu
{
    public class DanhMucChiTieuResponseDto
    {
        public long Id { get; set; }
        public string MaChiTieu { get; set; } = string.Empty;
        public string TenChiTieu { get; set; } = string.Empty;
        public string NguonChiTieu { get; set; } = string.Empty;
        public string LoaiChiTieu { get; set; } = string.Empty;
        public string CapApDung { get; set; } = string.Empty;
        public string? LinhVucNghiepVu { get; set; }
        public string? DonViTinh { get; set; }
        public string? MoTa { get; set; }
        public string? HuongDanTinhToan { get; set; }
        public bool CoChoPhepPhanRa { get; set; }
        public string TrangThaiSuDung { get; set; } = string.Empty;
        public DateTime? NgayHieuLuc { get; set; }
        public DateTime? NgayHetHieuLuc { get; set; }
        public string? DieuKienHoanThanh { get; set; }
        public string? DieuKienKhongHoanThanh { get; set; }
        public decimal? TyLePhanTramMucTieu { get; set; }
        public string? LoaiMocSoSanh { get; set; }
        public string? ChieuSoSanh { get; set; }
        public long? ChiTieuChaId { get; set; }
        public int? ThuTuHienThi { get; set; }
        public bool BatBuocDatTatCaTieuChiCon { get; set; }
        public List<DanhMucChiTieuResponseDto> TieuChiDanhGias { get; set; } = [];
        public DateTime CreatedAt { get; set; }
    }
}
