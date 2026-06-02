using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KPITrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDanhMucChiTieuDonViChuTri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DonViChuTriId",
                table: "DanhMucChiTieus",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucChiTieus_DonViChuTriId",
                table: "DanhMucChiTieus",
                column: "DonViChuTriId");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhMucChiTieus_DonVi_DonViChuTriId",
                table: "DanhMucChiTieus",
                column: "DonViChuTriId",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhMucChiTieus_DonVi_DonViChuTriId",
                table: "DanhMucChiTieus");

            migrationBuilder.DropIndex(
                name: "IX_DanhMucChiTieus_DonViChuTriId",
                table: "DanhMucChiTieus");

            migrationBuilder.DropColumn(
                name: "DonViChuTriId",
                table: "DanhMucChiTieus");
        }
    }
}
