using KPITrackerAPI.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KPITrackerAPI.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260417190000_AddNhomThiDuaTables")]
    public partial class AddNhomThiDuaTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'dbo.NhomThiDua', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.NhomThiDua (
                        Id bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        MaNhom nvarchar(50) NOT NULL,
                        TenNhom nvarchar(255) NOT NULL,
                        MoTa nvarchar(max) NULL,
                        TrangThai nvarchar(30) NOT NULL CONSTRAINT DF_NhomThiDua_TrangThai DEFAULT N'HOAT_DONG',
                        CreatedAt datetime2 NOT NULL,
                        UpdatedAt datetime2 NULL
                    );

                    CREATE UNIQUE INDEX IX_NhomThiDua_MaNhom ON dbo.NhomThiDua(MaNhom);
                    CREATE UNIQUE INDEX IX_NhomThiDua_TenNhom ON dbo.NhomThiDua(TenNhom);
                END
            ");

            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'dbo.NhomThiDuaDonVi', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.NhomThiDuaDonVi (
                        NhomThiDuaId bigint NOT NULL,
                        DonViId bigint NOT NULL,
                        CreatedAt datetime2 NOT NULL,
                        CONSTRAINT PK_NhomThiDuaDonVi PRIMARY KEY (NhomThiDuaId, DonViId),
                        CONSTRAINT FK_NhomThiDuaDonVi_NhomThiDua_NhomThiDuaId FOREIGN KEY (NhomThiDuaId) REFERENCES dbo.NhomThiDua(Id) ON DELETE CASCADE,
                        CONSTRAINT FK_NhomThiDuaDonVi_DonVi_DonViId FOREIGN KEY (DonViId) REFERENCES dbo.DonVi(Id)
                    );

                    CREATE INDEX IX_NhomThiDuaDonVi_DonViId ON dbo.NhomThiDuaDonVi(DonViId);
                END
            ");

            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'dbo.NhomThiDuaChiTieu', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.NhomThiDuaChiTieu (
                        NhomThiDuaId bigint NOT NULL,
                        DanhMucChiTieuId bigint NOT NULL,
                        CreatedAt datetime2 NOT NULL,
                        CONSTRAINT PK_NhomThiDuaChiTieu PRIMARY KEY (NhomThiDuaId, DanhMucChiTieuId),
                        CONSTRAINT FK_NhomThiDuaChiTieu_NhomThiDua_NhomThiDuaId FOREIGN KEY (NhomThiDuaId) REFERENCES dbo.NhomThiDua(Id) ON DELETE CASCADE,
                        CONSTRAINT FK_NhomThiDuaChiTieu_DanhMucChiTieu_DanhMucChiTieuId FOREIGN KEY (DanhMucChiTieuId) REFERENCES dbo.DanhMucChiTieus(Id)
                    );

                    CREATE INDEX IX_NhomThiDuaChiTieu_DanhMucChiTieuId ON dbo.NhomThiDuaChiTieu(DanhMucChiTieuId);
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'dbo.NhomThiDuaChiTieu', N'U') IS NOT NULL
                    DROP TABLE dbo.NhomThiDuaChiTieu;

                IF OBJECT_ID(N'dbo.NhomThiDuaDonVi', N'U') IS NOT NULL
                    DROP TABLE dbo.NhomThiDuaDonVi;

                IF OBJECT_ID(N'dbo.NhomThiDua', N'U') IS NOT NULL
                    DROP TABLE dbo.NhomThiDua;
            ");
        }
    }
}
