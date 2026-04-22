using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KPITrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucChiTieus",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaChiTieu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenChiTieu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NguonChiTieu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LoaiChiTieu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CapApDung = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LinhVucNghiepVu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DonViTinh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HuongDanTinhToan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoChoPhepPhanRa = table.Column<bool>(type: "bit", nullable: false),
                    TrangThaiSuDung = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayHieuLuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayHetHieuLuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DieuKienHoanThanh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DieuKienKhongHoanThanh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TyLePhanTramMucTieu = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LoaiMocSoSanh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChieuSoSanh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChiTieuChaId = table.Column<long>(type: "bigint", nullable: true),
                    ThuTuHienThi = table.Column<int>(type: "int", nullable: true),
                    BatBuocDatTatCaTieuChiCon = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucChiTieus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucChiTieus_DanhMucChiTieus_ChiTieuChaId",
                        column: x => x.ChiTieuChaId,
                        principalTable: "DanhMucChiTieus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DonVi",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDonVi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenDonVi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LoaiDonVi = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DonViChaId = table.Column<long>(type: "bigint", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NguoiDaiDien = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonVi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DonVi_DonVi_DonViChaId",
                        column: x => x.DonViChaId,
                        principalTable: "DonVi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DotGiaoChiTieu",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDotGiao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenDotGiao = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NamApDung = table.Column<int>(type: "int", nullable: false),
                    NguonDotGiao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CapGiao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DonViGiaoId = table.Column<long>(type: "bigint", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DotGiaoChiTieu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KyBaoCaoKPI",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenKy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LoaiKy = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Nam = table.Column<int>(type: "int", nullable: false),
                    SoKy = table.Column<int>(type: "int", nullable: true),
                    TuNgay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DenNgay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayDauKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCuoiKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KyBaoCaoKPI", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NhomThiDua",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNhom = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenNhom = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhomThiDua", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CauHinhNguongDanhGiaKPI",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DanhMucChiTieuId = table.Column<long>(type: "bigint", nullable: true),
                    TieuChiDanhGia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    QuyTacDanhGia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TuTyLe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DenTyLe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    XepLoai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DieuKienThoiHan = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Diem = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHinhNguongDanhGiaKPI", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CauHinhNguongDanhGiaKPI_DanhMucChiTieus_DanhMucChiTieuId",
                        column: x => x.DanhMucChiTieuId,
                        principalTable: "DanhMucChiTieus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DonViId = table.Column<long>(type: "bigint", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_DonVi_DonViId",
                        column: x => x.DonViId,
                        principalTable: "DonVi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietGiaoChiTieu",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DotGiaoChiTieuId = table.Column<long>(type: "bigint", nullable: false),
                    DanhMucChiTieuId = table.Column<long>(type: "bigint", nullable: false),
                    DonViNhanId = table.Column<long>(type: "bigint", nullable: false),
                    DonViThucHienChinhId = table.Column<long>(type: "bigint", nullable: true),
                    GiaTriMucTieu = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GiaTriMucTieuText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GiaTriDauKyCoDinh = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TieuChiDanhGia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LoaiMocSoSanh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    KieuSoSanh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChieuSoSanh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    QuyTacDanhGia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChiTietGiaoChaId = table.Column<long>(type: "bigint", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThuTuHienThi = table.Column<int>(type: "int", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TanSuatBaoCao = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietGiaoChiTieu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietGiaoChiTieu_ChiTietGiaoChiTieu_ChiTietGiaoChaId",
                        column: x => x.ChiTietGiaoChaId,
                        principalTable: "ChiTietGiaoChiTieu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTietGiaoChiTieu_DanhMucChiTieus_DanhMucChiTieuId",
                        column: x => x.DanhMucChiTieuId,
                        principalTable: "DanhMucChiTieus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTietGiaoChiTieu_DonVi_DonViNhanId",
                        column: x => x.DonViNhanId,
                        principalTable: "DonVi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTietGiaoChiTieu_DonVi_DonViThucHienChinhId",
                        column: x => x.DonViThucHienChinhId,
                        principalTable: "DonVi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTietGiaoChiTieu_DotGiaoChiTieu_DotGiaoChiTieuId",
                        column: x => x.DotGiaoChiTieuId,
                        principalTable: "DotGiaoChiTieu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NhomThiDuaChiTieu",
                columns: table => new
                {
                    NhomThiDuaId = table.Column<long>(type: "bigint", nullable: false),
                    DanhMucChiTieuId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhomThiDuaChiTieu", x => new { x.NhomThiDuaId, x.DanhMucChiTieuId });
                    table.ForeignKey(
                        name: "FK_NhomThiDuaChiTieu_DanhMucChiTieus_DanhMucChiTieuId",
                        column: x => x.DanhMucChiTieuId,
                        principalTable: "DanhMucChiTieus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NhomThiDuaChiTieu_NhomThiDua_NhomThiDuaId",
                        column: x => x.NhomThiDuaId,
                        principalTable: "NhomThiDua",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NhomThiDuaDonVi",
                columns: table => new
                {
                    NhomThiDuaId = table.Column<long>(type: "bigint", nullable: false),
                    DonViId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhomThiDuaDonVi", x => new { x.NhomThiDuaId, x.DonViId });
                    table.ForeignKey(
                        name: "FK_NhomThiDuaDonVi_DonVi_DonViId",
                        column: x => x.DonViId,
                        principalTable: "DonVi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NhomThiDuaDonVi_NhomThiDua_NhomThiDuaId",
                        column: x => x.NhomThiDuaId,
                        principalTable: "NhomThiDua",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissions",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    IsGranted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissions", x => new { x.UserId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_UserPermissions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DanhGiaKPI",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChiTietGiaoChiTieuId = table.Column<long>(type: "bigint", nullable: false),
                    KyBaoCaoKPIId = table.Column<long>(type: "bigint", nullable: false),
                    GiaTriMucTieu = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GiaTriDauKy = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GiaTriCuoiKy = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GiaTriCungKyNamTruoc = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ChenhLechSoVoiDauKy = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TyLeTangTruongSoVoiDauKy = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ChenhLechSoVoiCungKyNamTruoc = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TyLeTangTruongSoVoiCungKyNamTruoc = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TyLeHoanThanh = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    XepLoai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    KetQua = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NhanXetDanhGia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NguoiDanhGia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NgayDanhGia = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhGiaKPI", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhGiaKPI_ChiTietGiaoChiTieu_ChiTietGiaoChiTieuId",
                        column: x => x.ChiTietGiaoChiTieuId,
                        principalTable: "ChiTietGiaoChiTieu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DanhGiaKPI_KyBaoCaoKPI_KyBaoCaoKPIId",
                        column: x => x.KyBaoCaoKPIId,
                        principalTable: "KyBaoCaoKPI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TheoDoiThucHienKPI",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChiTietGiaoChiTieuId = table.Column<long>(type: "bigint", nullable: false),
                    KyBaoCaoKPIId = table.Column<long>(type: "bigint", nullable: false),
                    GiaTriDauKy = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GiaTriThucHienTrongKy = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GiaTriPhatSinhTrongKy = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GiaTriCuoiKy = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GiaTriLuyKe = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GiaTriPhatSinhLuyKe = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NhanXet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TheoDoiThucHienKPI", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TheoDoiThucHienKPI_ChiTietGiaoChiTieu_ChiTietGiaoChiTieuId",
                        column: x => x.ChiTietGiaoChiTieuId,
                        principalTable: "ChiTietGiaoChiTieu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TheoDoiThucHienKPI_KyBaoCaoKPI_KyBaoCaoKPIId",
                        column: x => x.KyBaoCaoKPIId,
                        principalTable: "KyBaoCaoKPI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DonViId",
                table: "AspNetUsers",
                column: "DonViId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CauHinhNguongDanhGiaKPI_DanhMucChiTieuId",
                table: "CauHinhNguongDanhGiaKPI",
                column: "DanhMucChiTieuId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietGiaoChiTieu_ChiTietGiaoChaId",
                table: "ChiTietGiaoChiTieu",
                column: "ChiTietGiaoChaId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietGiaoChiTieu_DanhMucChiTieuId",
                table: "ChiTietGiaoChiTieu",
                column: "DanhMucChiTieuId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietGiaoChiTieu_DonViNhanId",
                table: "ChiTietGiaoChiTieu",
                column: "DonViNhanId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietGiaoChiTieu_DonViThucHienChinhId",
                table: "ChiTietGiaoChiTieu",
                column: "DonViThucHienChinhId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietGiaoChiTieu_DotGiaoChiTieuId_DanhMucChiTieuId_DonViNhanId",
                table: "ChiTietGiaoChiTieu",
                columns: new[] { "DotGiaoChiTieuId", "DanhMucChiTieuId", "DonViNhanId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DanhGiaKPI_ChiTietGiaoChiTieuId_KyBaoCaoKPIId",
                table: "DanhGiaKPI",
                columns: new[] { "ChiTietGiaoChiTieuId", "KyBaoCaoKPIId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DanhGiaKPI_KyBaoCaoKPIId",
                table: "DanhGiaKPI",
                column: "KyBaoCaoKPIId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucChiTieus_ChiTieuChaId",
                table: "DanhMucChiTieus",
                column: "ChiTieuChaId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucChiTieus_MaChiTieu",
                table: "DanhMucChiTieus",
                column: "MaChiTieu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DonVi_DonViChaId",
                table: "DonVi",
                column: "DonViChaId");

            migrationBuilder.CreateIndex(
                name: "IX_DonVi_MaDonVi",
                table: "DonVi",
                column: "MaDonVi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DotGiaoChiTieu_MaDotGiao",
                table: "DotGiaoChiTieu",
                column: "MaDotGiao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KyBaoCaoKPI_LoaiKy_Nam_SoKy",
                table: "KyBaoCaoKPI",
                columns: new[] { "LoaiKy", "Nam", "SoKy" });

            migrationBuilder.CreateIndex(
                name: "IX_KyBaoCaoKPI_MaKy",
                table: "KyBaoCaoKPI",
                column: "MaKy",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NhomThiDua_MaNhom",
                table: "NhomThiDua",
                column: "MaNhom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NhomThiDua_TenNhom",
                table: "NhomThiDua",
                column: "TenNhom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NhomThiDuaChiTieu_DanhMucChiTieuId",
                table: "NhomThiDuaChiTieu",
                column: "DanhMucChiTieuId");

            migrationBuilder.CreateIndex(
                name: "IX_NhomThiDuaDonVi_DonViId",
                table: "NhomThiDuaDonVi",
                column: "DonViId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Name",
                table: "Permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_TheoDoiThucHienKPI_ChiTietGiaoChiTieuId_KyBaoCaoKPIId",
                table: "TheoDoiThucHienKPI",
                columns: new[] { "ChiTietGiaoChiTieuId", "KyBaoCaoKPIId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TheoDoiThucHienKPI_KyBaoCaoKPIId",
                table: "TheoDoiThucHienKPI",
                column: "KyBaoCaoKPIId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_PermissionId",
                table: "UserPermissions",
                column: "PermissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CauHinhNguongDanhGiaKPI");

            migrationBuilder.DropTable(
                name: "DanhGiaKPI");

            migrationBuilder.DropTable(
                name: "NhomThiDuaChiTieu");

            migrationBuilder.DropTable(
                name: "NhomThiDuaDonVi");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "TheoDoiThucHienKPI");

            migrationBuilder.DropTable(
                name: "UserPermissions");

            migrationBuilder.DropTable(
                name: "NhomThiDua");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "ChiTietGiaoChiTieu");

            migrationBuilder.DropTable(
                name: "KyBaoCaoKPI");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "DanhMucChiTieus");

            migrationBuilder.DropTable(
                name: "DotGiaoChiTieu");

            migrationBuilder.DropTable(
                name: "DonVi");
        }
    }
}
