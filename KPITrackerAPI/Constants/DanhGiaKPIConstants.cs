namespace KPITrackerAPI.Constants
{
    public static class DanhGiaKPIConstants
    {
        public static class XepLoai
        {
            public const string KhongHoanThanh = "KHONG_HOAN_THANH";
            public const string ChuaHoanThanh = "CHUA_HOAN_THANH";
            public const string HoanThanh = "HOAN_THANH";
            public const string HoanThanhVuotMuc = "HOAN_THANH_VUOT_MUC";
            public const string ChuaDanhGia = "CHUA_DANH_GIA";
            public const string ChuaCauHinh = "CHUA_CAU_HINH";
        }

        public static class DieuKienThoiHan
        {
            public const string MacDinh = "MAC_DINH";
            public const string ChuaDenHan = "CHUA_DEN_HAN";
            public const string DaDenHan = "DA_DEN_HAN";
        }

        public static IReadOnlyCollection<string> AllowedXepLoai { get; } = new[]
        {
            XepLoai.KhongHoanThanh,
            XepLoai.ChuaHoanThanh,
            XepLoai.HoanThanh,
            XepLoai.HoanThanhVuotMuc
        };

        public static IReadOnlyCollection<string> AllowedDieuKienThoiHan { get; } = new[]
        {
            DieuKienThoiHan.MacDinh,
            DieuKienThoiHan.ChuaDenHan,
            DieuKienThoiHan.DaDenHan
        };

        public static string GetDisplayLabel(string? xepLoai)
        {
            return NormalizeCode(xepLoai) switch
            {
                XepLoai.KhongHoanThanh => "Không hoàn thành",
                XepLoai.ChuaHoanThanh => "Chưa hoàn thành",
                XepLoai.HoanThanh => "Hoàn thành",
                XepLoai.HoanThanhVuotMuc => "Hoàn thành vượt mức",
                XepLoai.ChuaDanhGia => "Chưa đánh giá",
                XepLoai.ChuaCauHinh => "Chưa cấu hình",
                _ => string.IsNullOrWhiteSpace(xepLoai) ? "Chưa xác định" : xepLoai.Trim()
            };
        }

        public static string GetThoiHanDisplayLabel(string? dieuKienThoiHan)
        {
            return NormalizeCode(dieuKienThoiHan) switch
            {
                DieuKienThoiHan.ChuaDenHan => "Chưa đến hạn",
                DieuKienThoiHan.DaDenHan => "Đã đến hạn",
                _ => "Mặc định"
            };
        }

        public static bool LaTrangThaiHoanThanh(string? xepLoai)
        {
            var normalized = NormalizeCode(xepLoai);
            return normalized == XepLoai.HoanThanh || normalized == XepLoai.HoanThanhVuotMuc;
        }

        public static int GetSeverityRank(string? xepLoai)
        {
            return NormalizeCode(xepLoai) switch
            {
                XepLoai.KhongHoanThanh => 0,
                XepLoai.ChuaHoanThanh => 1,
                XepLoai.HoanThanh => 2,
                XepLoai.HoanThanhVuotMuc => 3,
                XepLoai.ChuaDanhGia => -1,
                XepLoai.ChuaCauHinh => -2,
                _ => -3
            };
        }

        public static string NormalizeCode(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Trim().ToUpperInvariant().Replace(' ', '_');
        }
    }
}
