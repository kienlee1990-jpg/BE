using KPITrackerAPI.Constants;
using KPITrackerAPI.Data;
using KPITrackerAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace KPITrackerAPI.Extensions;

public static class SeedDanhGiaNguongMacDinh
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        var globalRows = await context.CauHinhNguongDanhGiaKPIs
            .Where(x => x.DanhMucChiTieuId == null)
            .OrderBy(x => x.TieuChiDanhGia)
            .ThenBy(x => x.QuyTacDanhGia)
            .ThenBy(x => x.DieuKienThoiHan)
            .ThenBy(x => x.TuTyLe)
            .ToListAsync();

        var expectedRows = BuildExpectedRowsByCriterion();
        if (GlobalRowsMatchExpectedByCriterion(globalRows, expectedRows))
        {
            return;
        }

        if (globalRows.Count > 0)
        {
            context.CauHinhNguongDanhGiaKPIs.RemoveRange(globalRows);
            await context.SaveChangesAsync();
        }

        context.CauHinhNguongDanhGiaKPIs.AddRange(expectedRows);
        await context.SaveChangesAsync();
    }

    private static List<CauHinhNguongDanhGiaKPI> BuildExpectedRowsByCriterion()
    {
        var rows = new List<CauHinhNguongDanhGiaKPI>();

        AddStandardRows(
            rows,
            DanhGiaKPIConstants.TieuChiDanhGia.DinhTinh,
            DanhGiaKPIConstants.QuyTacDanhGia.MacDinh,
            includeVuotMuc: false);
        AddStandardRows(
            rows,
            DanhGiaKPIConstants.TieuChiDanhGia.DinhLuongTichLuy,
            DanhGiaKPIConstants.QuyTacDanhGia.DatToiThieu,
            includeVuotMuc: true);
        AddStandardRows(
            rows,
            DanhGiaKPIConstants.TieuChiDanhGia.DinhLuongTichLuy,
            DanhGiaKPIConstants.QuyTacDanhGia.KhongVuotNguong,
            includeVuotMuc: false);
        AddStandardRows(
            rows,
            DanhGiaKPIConstants.TieuChiDanhGia.DinhLuongSoSanh,
            DanhGiaKPIConstants.QuyTacDanhGia.DatToiThieu,
            includeVuotMuc: true);
        AddStandardRows(
            rows,
            DanhGiaKPIConstants.TieuChiDanhGia.DinhLuongSoSanh,
            DanhGiaKPIConstants.QuyTacDanhGia.KhongVuotNguong,
            includeVuotMuc: false);

        return rows
            .OrderBy(x => x.TieuChiDanhGia)
            .ThenBy(x => x.QuyTacDanhGia)
            .ThenBy(x => x.DieuKienThoiHan)
            .ThenBy(x => x.TuTyLe)
            .ToList();
    }

    private static void AddStandardRows(
        ICollection<CauHinhNguongDanhGiaKPI> rows,
        string tieuChiDanhGia,
        string quyTacDanhGia,
        bool includeVuotMuc)
    {
        rows.Add(new CauHinhNguongDanhGiaKPI
        {
            TieuChiDanhGia = tieuChiDanhGia,
            QuyTacDanhGia = quyTacDanhGia,
            TuTyLe = 0m,
            DenTyLe = 99.99m,
            XepLoai = DanhGiaKPIConstants.XepLoai.ChuaHoanThanh,
            DieuKienThoiHan = DanhGiaKPIConstants.DieuKienThoiHan.ChuaDenHan,
            Diem = 0m,
            GhiChu = "Chi tieu chua dat nhung van con thoi gian thuc hien."
        });

        rows.Add(new CauHinhNguongDanhGiaKPI
        {
            TieuChiDanhGia = tieuChiDanhGia,
            QuyTacDanhGia = quyTacDanhGia,
            TuTyLe = 0m,
            DenTyLe = 99.99m,
            XepLoai = DanhGiaKPIConstants.XepLoai.KhongHoanThanh,
            DieuKienThoiHan = DanhGiaKPIConstants.DieuKienThoiHan.DaDenHan,
            Diem = 0m,
            GhiChu = "Chi tieu khong dat khi da het thoi han giao."
        });

        rows.Add(new CauHinhNguongDanhGiaKPI
        {
            TieuChiDanhGia = tieuChiDanhGia,
            QuyTacDanhGia = quyTacDanhGia,
            TuTyLe = 100m,
            DenTyLe = 100m,
            XepLoai = DanhGiaKPIConstants.XepLoai.HoanThanh,
            DieuKienThoiHan = DanhGiaKPIConstants.DieuKienThoiHan.MacDinh,
            Diem = 100m,
            GhiChu = "Chi tieu hoan thanh dung muc yeu cau."
        });

        if (!includeVuotMuc)
        {
            return;
        }

        rows.Add(new CauHinhNguongDanhGiaKPI
        {
            TieuChiDanhGia = tieuChiDanhGia,
            QuyTacDanhGia = quyTacDanhGia,
            TuTyLe = 100.01m,
            DenTyLe = 999999.99m,
            XepLoai = DanhGiaKPIConstants.XepLoai.HoanThanhVuotMuc,
            DieuKienThoiHan = DanhGiaKPIConstants.DieuKienThoiHan.MacDinh,
            Diem = 120m,
            GhiChu = "Chi tieu hoan thanh vuot muc yeu cau."
        });
    }

    private static bool GlobalRowsMatchExpectedByCriterion(
        IReadOnlyList<CauHinhNguongDanhGiaKPI> actualRows,
        IReadOnlyList<CauHinhNguongDanhGiaKPI> expectedRows)
    {
        if (actualRows.Count != expectedRows.Count)
        {
            return false;
        }

        for (var i = 0; i < expectedRows.Count; i += 1)
        {
            var actual = actualRows[i];
            var expected = expectedRows[i];

            if (actual.DanhMucChiTieuId != null ||
                actual.TieuChiDanhGia != expected.TieuChiDanhGia ||
                actual.QuyTacDanhGia != expected.QuyTacDanhGia ||
                actual.TuTyLe != expected.TuTyLe ||
                actual.DenTyLe != expected.DenTyLe ||
                actual.XepLoai != expected.XepLoai ||
                actual.DieuKienThoiHan != expected.DieuKienThoiHan)
            {
                return false;
            }
        }

        return true;
    }
}
