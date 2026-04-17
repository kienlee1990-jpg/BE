using KPITrackerAPI.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KPITrackerAPI.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260416173000_AddTieuChiDanhGiaToChiTietVaNguong")]
    public partial class AddTieuChiDanhGiaToChiTietVaNguong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF COL_LENGTH('dbo.ChiTietGiaoChiTieu', 'TieuChiDanhGia') IS NULL
                    ALTER TABLE dbo.ChiTietGiaoChiTieu ADD TieuChiDanhGia nvarchar(50) NULL;

                IF COL_LENGTH('dbo.ChiTietGiaoChiTieu', 'LoaiMocSoSanh') IS NULL
                    ALTER TABLE dbo.ChiTietGiaoChiTieu ADD LoaiMocSoSanh nvarchar(50) NULL;

                IF COL_LENGTH('dbo.ChiTietGiaoChiTieu', 'ChieuSoSanh') IS NULL
                    ALTER TABLE dbo.ChiTietGiaoChiTieu ADD ChieuSoSanh nvarchar(50) NULL;

                IF COL_LENGTH('dbo.CauHinhNguongDanhGiaKPI', 'TieuChiDanhGia') IS NULL
                    ALTER TABLE dbo.CauHinhNguongDanhGiaKPI ADD TieuChiDanhGia nvarchar(50) NULL;
            ");

            migrationBuilder.Sql(@"
                UPDATE ct
                SET
                    ct.TieuChiDanhGia = CASE
                        WHEN UPPER(ISNULL(dm.LoaiChiTieu, '')) IN ('DINH_TINH', 'DINH_LUONG_TICH_LUY', 'DINH_LUONG_SO_SANH')
                            THEN UPPER(dm.LoaiChiTieu)
                        ELSE NULL
                    END,
                    ct.LoaiMocSoSanh = CASE
                        WHEN UPPER(ISNULL(dm.LoaiChiTieu, '')) = 'DINH_LUONG_SO_SANH'
                            THEN UPPER(dm.LoaiMocSoSanh)
                        ELSE NULL
                    END,
                    ct.ChieuSoSanh = CASE
                        WHEN UPPER(ISNULL(dm.LoaiChiTieu, '')) = 'DINH_LUONG_SO_SANH'
                            THEN UPPER(dm.ChieuSoSanh)
                        ELSE NULL
                    END
                FROM ChiTietGiaoChiTieu AS ct
                INNER JOIN DanhMucChiTieus AS dm ON dm.Id = ct.DanhMucChiTieuId
                WHERE ct.TieuChiDanhGia IS NULL;
            ");

            migrationBuilder.Sql(@"
                UPDATE cfg
                SET cfg.TieuChiDanhGia = UPPER(dm.LoaiChiTieu)
                FROM CauHinhNguongDanhGiaKPI AS cfg
                INNER JOIN DanhMucChiTieus AS dm ON dm.Id = cfg.DanhMucChiTieuId
                WHERE cfg.TieuChiDanhGia IS NULL
                  AND UPPER(ISNULL(dm.LoaiChiTieu, '')) IN ('DINH_TINH', 'DINH_LUONG_TICH_LUY', 'DINH_LUONG_SO_SANH');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF COL_LENGTH('dbo.CauHinhNguongDanhGiaKPI', 'TieuChiDanhGia') IS NOT NULL
                    ALTER TABLE dbo.CauHinhNguongDanhGiaKPI DROP COLUMN TieuChiDanhGia;

                IF COL_LENGTH('dbo.ChiTietGiaoChiTieu', 'TieuChiDanhGia') IS NOT NULL
                    ALTER TABLE dbo.ChiTietGiaoChiTieu DROP COLUMN TieuChiDanhGia;

                IF COL_LENGTH('dbo.ChiTietGiaoChiTieu', 'LoaiMocSoSanh') IS NOT NULL
                    ALTER TABLE dbo.ChiTietGiaoChiTieu DROP COLUMN LoaiMocSoSanh;

                IF COL_LENGTH('dbo.ChiTietGiaoChiTieu', 'ChieuSoSanh') IS NOT NULL
                    ALTER TABLE dbo.ChiTietGiaoChiTieu DROP COLUMN ChieuSoSanh;
            ");
        }
    }
}
