using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KPITrackerAPI.Migrations
{
    public partial class AddTyLeSoSanhFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF COL_LENGTH('dbo.ChiTietGiaoChiTieu', 'KieuSoSanh') IS NULL
                BEGIN
                    ALTER TABLE [dbo].[ChiTietGiaoChiTieu]
                    ADD [KieuSoSanh] nvarchar(50) NULL;
                END
                """
            );

            migrationBuilder.Sql(
                """
                IF COL_LENGTH('dbo.TheoDoiThucHienKPI', 'GiaTriPhatSinhTrongKy') IS NULL
                BEGIN
                    ALTER TABLE [dbo].[TheoDoiThucHienKPI]
                    ADD [GiaTriPhatSinhTrongKy] decimal(18,2) NULL;
                END
                """
            );

            migrationBuilder.Sql(
                """
                IF COL_LENGTH('dbo.TheoDoiThucHienKPI', 'GiaTriPhatSinhLuyKe') IS NULL
                BEGIN
                    ALTER TABLE [dbo].[TheoDoiThucHienKPI]
                    ADD [GiaTriPhatSinhLuyKe] decimal(18,2) NULL;
                END
                """
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF COL_LENGTH('dbo.TheoDoiThucHienKPI', 'GiaTriPhatSinhLuyKe') IS NOT NULL
                BEGIN
                    ALTER TABLE [dbo].[TheoDoiThucHienKPI]
                    DROP COLUMN [GiaTriPhatSinhLuyKe];
                END
                """
            );

            migrationBuilder.Sql(
                """
                IF COL_LENGTH('dbo.TheoDoiThucHienKPI', 'GiaTriPhatSinhTrongKy') IS NOT NULL
                BEGIN
                    ALTER TABLE [dbo].[TheoDoiThucHienKPI]
                    DROP COLUMN [GiaTriPhatSinhTrongKy];
                END
                """
            );

            migrationBuilder.Sql(
                """
                IF COL_LENGTH('dbo.ChiTietGiaoChiTieu', 'KieuSoSanh') IS NOT NULL
                BEGIN
                    ALTER TABLE [dbo].[ChiTietGiaoChiTieu]
                    DROP COLUMN [KieuSoSanh];
                END
                """
            );
        }
    }
}
