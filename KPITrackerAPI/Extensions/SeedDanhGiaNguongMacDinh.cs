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
            .OrderBy(x => x.DieuKienThoiHan)
            .ThenBy(x => x.TuTyLe)
            .ToListAsync();

        var expectedRows = BuildExpectedRows();
        if (GlobalRowsMatchExpected(globalRows, expectedRows))
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

    private static List<CauHinhNguongDanhGiaKPI> BuildExpectedRows()
    {
        return new List<CauHinhNguongDanhGiaKPI>
        {
            new()
            {
                TuTyLe = 0m,
                DenTyLe = 99.99m,
                XepLoai = DanhGiaKPIConstants.XepLoai.ChuaHoanThanh,
                DieuKienThoiHan = DanhGiaKPIConstants.DieuKienThoiHan.ChuaDenHan,
                Diem = 0m,
                GhiChu = "Chỉ tiêu chưa đạt nhưng vẫn còn thời gian thực hiện."
            },
            new()
            {
                TuTyLe = 0m,
                DenTyLe = 99.99m,
                XepLoai = DanhGiaKPIConstants.XepLoai.KhongHoanThanh,
                DieuKienThoiHan = DanhGiaKPIConstants.DieuKienThoiHan.DaDenHan,
                Diem = 0m,
                GhiChu = "Chỉ tiêu không đạt khi đã hết thời hạn giao."
            },
            new()
            {
                TuTyLe = 100m,
                DenTyLe = 100m,
                XepLoai = DanhGiaKPIConstants.XepLoai.HoanThanh,
                DieuKienThoiHan = DanhGiaKPIConstants.DieuKienThoiHan.MacDinh,
                Diem = 100m,
                GhiChu = "Chỉ tiêu hoàn thành đúng mức yêu cầu."
            },
            new()
            {
                TuTyLe = 100.01m,
                DenTyLe = 999999.99m,
                XepLoai = DanhGiaKPIConstants.XepLoai.HoanThanhVuotMuc,
                DieuKienThoiHan = DanhGiaKPIConstants.DieuKienThoiHan.MacDinh,
                Diem = 120m,
                GhiChu = "Chỉ tiêu hoàn thành vượt mức yêu cầu."
            }
        };
    }

    private static bool GlobalRowsMatchExpected(
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
