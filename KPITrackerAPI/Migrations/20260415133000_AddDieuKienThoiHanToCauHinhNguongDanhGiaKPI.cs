using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KPITrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDieuKienThoiHanToCauHinhNguongDanhGiaKPI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DieuKienThoiHan",
                table: "CauHinhNguongDanhGiaKPI",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "MAC_DINH");

            migrationBuilder.Sql(@"
                UPDATE CauHinhNguongDanhGiaKPI
                SET DieuKienThoiHan = CASE
                    WHEN XepLoai = 'CHUA_HOAN_THANH' THEN 'CHUA_DEN_HAN'
                    WHEN XepLoai = 'KHONG_HOAN_THANH' THEN 'DA_DEN_HAN'
                    ELSE 'MAC_DINH'
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DieuKienThoiHan",
                table: "CauHinhNguongDanhGiaKPI");
        }
    }
}
