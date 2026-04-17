using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KPITrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class SyncTieuChiDanhGiaWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Intentionally left blank.
            // The actual schema/data synchronization for these columns is handled
            // in 20260416173000_AddTieuChiDanhGiaToChiTietVaNguong to avoid
            // duplicate column creation on existing databases.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Intentionally left blank.
        }
    }
}
