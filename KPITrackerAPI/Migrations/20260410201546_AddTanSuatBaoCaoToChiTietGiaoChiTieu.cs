using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KPITrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddTanSuatBaoCaoToChiTietGiaoChiTieu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TanSuatBaoCao",
                table: "ChiTietGiaoChiTieu",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TanSuatBaoCao",
                table: "ChiTietGiaoChiTieu");
        }
    }
}

