using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KPITrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddGiaTriDauKyCoDinhToChiTietGiaoChiTieu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "GiaTriDauKyCoDinh",
                table: "ChiTietGiaoChiTieu",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE ct
                SET ct.GiaTriDauKyCoDinh = firstTracking.GiaTriDauKy
                FROM ChiTietGiaoChiTieu AS ct
                OUTER APPLY (
                    SELECT TOP (1) td.GiaTriDauKy
                    FROM TheoDoiThucHienKPI AS td
                    INNER JOIN KyBaoCaoKPI AS ky ON ky.Id = td.KyBaoCaoKPIId
                    WHERE td.ChiTietGiaoChiTieuId = ct.Id
                    ORDER BY
                        ky.Nam,
                        CASE UPPER(ISNULL(ky.LoaiKy, ''))
                            WHEN 'THANG' THEN 1
                            WHEN 'QUY' THEN 2
                            WHEN '6THANG' THEN 3
                            WHEN 'NAM' THEN 4
                            ELSE 99
                        END,
                        ISNULL(ky.SoKy, 0),
                        td.Id
                ) AS firstTracking
                WHERE ct.GiaTriDauKyCoDinh IS NULL
                  AND firstTracking.GiaTriDauKy IS NOT NULL;
            ");

            migrationBuilder.Sql(@"
                UPDATE ct
                SET ct.GiaTriDauKyCoDinh = 0
                FROM ChiTietGiaoChiTieu AS ct
                INNER JOIN DanhMucChiTieus AS dm ON dm.Id = ct.DanhMucChiTieuId
                WHERE ct.GiaTriDauKyCoDinh IS NULL
                  AND UPPER(ISNULL(dm.LoaiChiTieu, '')) <> 'DINH_TINH';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GiaTriDauKyCoDinh",
                table: "ChiTietGiaoChiTieu");
        }
    }
}
