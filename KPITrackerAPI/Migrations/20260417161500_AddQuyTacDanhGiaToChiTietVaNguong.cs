using KPITrackerAPI.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KPITrackerAPI.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260417161500_AddQuyTacDanhGiaToChiTietVaNguong")]
    public partial class AddQuyTacDanhGiaToChiTietVaNguong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF COL_LENGTH('dbo.ChiTietGiaoChiTieu', 'QuyTacDanhGia') IS NULL
                    ALTER TABLE dbo.ChiTietGiaoChiTieu ADD QuyTacDanhGia nvarchar(50) NULL;

                IF COL_LENGTH('dbo.CauHinhNguongDanhGiaKPI', 'QuyTacDanhGia') IS NULL
                    ALTER TABLE dbo.CauHinhNguongDanhGiaKPI ADD QuyTacDanhGia nvarchar(50) NULL;
            ");

            migrationBuilder.Sql(@"
                UPDATE ChiTietGiaoChiTieu
                SET QuyTacDanhGia = CASE
                    WHEN UPPER(ISNULL(TieuChiDanhGia, '')) = 'DINH_TINH' THEN 'MAC_DINH'
                    ELSE 'DAT_TOI_THIEU'
                END
                WHERE QuyTacDanhGia IS NULL;
            ");

            migrationBuilder.Sql(@"
                UPDATE CauHinhNguongDanhGiaKPI
                SET QuyTacDanhGia = CASE
                    WHEN UPPER(ISNULL(TieuChiDanhGia, '')) = 'DINH_TINH' THEN 'MAC_DINH'
                    ELSE 'DAT_TOI_THIEU'
                END
                WHERE QuyTacDanhGia IS NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF COL_LENGTH('dbo.CauHinhNguongDanhGiaKPI', 'QuyTacDanhGia') IS NOT NULL
                    ALTER TABLE dbo.CauHinhNguongDanhGiaKPI DROP COLUMN QuyTacDanhGia;

                IF COL_LENGTH('dbo.ChiTietGiaoChiTieu', 'QuyTacDanhGia') IS NOT NULL
                    ALTER TABLE dbo.ChiTietGiaoChiTieu DROP COLUMN QuyTacDanhGia;
            ");
        }
    }
}
