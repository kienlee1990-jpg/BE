using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KPITrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddChiTieuPhanRaAndNgayBatDau : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NgayGiao",
                table: "DotGiaoChiTieu",
                newName: "NgayBatDau");

            migrationBuilder.AddColumn<bool>(
                name: "BatBuocDatTatCaTieuChiCon",
                table: "DanhMucChiTieus",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ChiTieuChaId",
                table: "DanhMucChiTieus",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ThuTuHienThi",
                table: "DanhMucChiTieus",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucChiTieus_ChiTieuChaId",
                table: "DanhMucChiTieus",
                column: "ChiTieuChaId");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhMucChiTieus_DanhMucChiTieus_ChiTieuChaId",
                table: "DanhMucChiTieus",
                column: "ChiTieuChaId",
                principalTable: "DanhMucChiTieus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhMucChiTieus_DanhMucChiTieus_ChiTieuChaId",
                table: "DanhMucChiTieus");

            migrationBuilder.DropIndex(
                name: "IX_DanhMucChiTieus_ChiTieuChaId",
                table: "DanhMucChiTieus");

            migrationBuilder.DropColumn(
                name: "BatBuocDatTatCaTieuChiCon",
                table: "DanhMucChiTieus");

            migrationBuilder.DropColumn(
                name: "ChiTieuChaId",
                table: "DanhMucChiTieus");

            migrationBuilder.DropColumn(
                name: "ThuTuHienThi",
                table: "DanhMucChiTieus");

            migrationBuilder.RenameColumn(
                name: "NgayBatDau",
                table: "DotGiaoChiTieu",
                newName: "NgayGiao");
        }
    }
}
