using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KPITrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddNgayKetThucToDotGiaoChiTieu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NgayKetThuc",
                table: "DotGiaoChiTieu",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NgayKetThuc",
                table: "DotGiaoChiTieu");
        }
    }
}

