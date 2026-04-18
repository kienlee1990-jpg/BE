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

        public static class TieuChiDanhGia
        {
            public const string DinhTinh = "DINH_TINH";
            public const string DinhLuongTichLuy = "DINH_LUONG_TICH_LUY";
            public const string DinhLuongSoSanh = "DINH_LUONG_SO_SANH";
        }

        public static class LoaiMocSoSanh
        {
            public const string DauKy = "DAU_KY";
            public const string CungKy = "CUNG_KY";
            public const string KyTruoc = "KY_TRUOC";
            public const string TongNamTruoc = "TONG_NAM_TRUOC";
        }

        public static class KieuSoSanh
        {
            public const string ChenhLech = "CHENH_LECH";
            public const string TyLe = "TY_LE";
        }

        public static class ChieuSoSanh
        {
            public const string Tang = "TANG";
            public const string Giam = "GIAM";
        }

        public static class QuyTacDanhGia
        {
            public const string MacDinh = "MAC_DINH";
            public const string DatToiThieu = "DAT_TOI_THIEU";
            public const string KhongVuotNguong = "KHONG_VUOT_NGUONG";
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

        public static IReadOnlyCollection<string> AllowedTieuChiDanhGia { get; } = new[]
        {
            TieuChiDanhGia.DinhTinh,
            TieuChiDanhGia.DinhLuongTichLuy,
            TieuChiDanhGia.DinhLuongSoSanh
        };

        public static IReadOnlyCollection<string> AllowedLoaiMocSoSanh { get; } = new[]
        {
            LoaiMocSoSanh.DauKy,
            LoaiMocSoSanh.CungKy,
            LoaiMocSoSanh.KyTruoc,
            LoaiMocSoSanh.TongNamTruoc
        };

        public static IReadOnlyCollection<string> AllowedKieuSoSanh { get; } = new[]
        {
            KieuSoSanh.ChenhLech,
            KieuSoSanh.TyLe
        };

        public static IReadOnlyCollection<string> AllowedChieuSoSanh { get; } = new[]
        {
            ChieuSoSanh.Tang,
            ChieuSoSanh.Giam
        };

        public static IReadOnlyCollection<string> AllowedQuyTacDanhGia { get; } = new[]
        {
            QuyTacDanhGia.MacDinh,
            QuyTacDanhGia.DatToiThieu,
            QuyTacDanhGia.KhongVuotNguong
        };

        public static IReadOnlyCollection<string> DinhTinhHoanThanhOptions { get; } = new[]
        {
            "KHONG_XAY_RA",
            "DAM_BAO",
            "DAT_100"
        };

        public static IReadOnlyCollection<string> DinhTinhKhongHoanThanhOptions { get; } = new[]
        {
            "XAY_RA",
            "KHONG_DAM_BAO",
            "KHONG_DAT"
        };

        public static IReadOnlyCollection<string> AllowedDinhTinhOptions { get; } = new[]
        {
            "KHONG_XAY_RA",
            "DAM_BAO",
            "DAT_100",
            "XAY_RA",
            "KHONG_DAM_BAO",
            "KHONG_DAT"
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

        public static string GetQuyTacDanhGiaDisplayLabel(string? quyTacDanhGia)
        {
            return NormalizeCode(quyTacDanhGia) switch
            {
                QuyTacDanhGia.DatToiThieu => "Đạt tối thiểu",
                QuyTacDanhGia.KhongVuotNguong => "Không vượt ngưỡng",
                _ => "Mặc định"
            };
        }

        public static string GetKieuSoSanhDisplayLabel(string? kieuSoSanh)
        {
            return NormalizeCode(kieuSoSanh) switch
            {
                KieuSoSanh.TyLe => "Tỷ lệ thực hiện",
                _ => "Chênh lệch theo mốc"
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
