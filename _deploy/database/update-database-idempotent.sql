IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [FullName] nvarchar(200) NOT NULL,
        [Address] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [IsActive] bit NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE TABLE [DanhMucChiTieus] (
        [Id] bigint NOT NULL IDENTITY,
        [MaChiTieu] nvarchar(50) NOT NULL,
        [TenChiTieu] nvarchar(500) NOT NULL,
        [NguonChiTieu] nvarchar(50) NOT NULL,
        [LoaiChiTieu] nvarchar(50) NOT NULL,
        [CapApDung] nvarchar(50) NOT NULL,
        [LinhVucNghiepVu] nvarchar(255) NULL,
        [DonViTinh] nvarchar(50) NULL,
        [MoTa] nvarchar(max) NULL,
        [HuongDanTinhToan] nvarchar(max) NULL,
        [CoChoPhepPhanRa] bit NOT NULL,
        [TrangThaiSuDung] nvarchar(50) NOT NULL,
        [NgayHieuLuc] datetime2 NULL,
        [NgayHetHieuLuc] datetime2 NULL,
        [DieuKienHoanThanh] nvarchar(max) NULL,
        [DieuKienKhongHoanThanh] nvarchar(max) NULL,
        [TyLePhanTramMucTieu] decimal(18,2) NULL,
        [LoaiMocSoSanh] nvarchar(50) NULL,
        [ChieuSoSanh] nvarchar(50) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_DanhMucChiTieus] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE TABLE [DonVi] (
        [Id] bigint NOT NULL IDENTITY,
        [MaDonVi] nvarchar(50) NOT NULL,
        [TenDonVi] nvarchar(255) NOT NULL,
        [LoaiDonVi] nvarchar(30) NOT NULL,
        [DonViChaId] bigint NULL,
        [TrangThai] nvarchar(30) NOT NULL,
        [DiaChi] nvarchar(255) NULL,
        [NguoiDaiDien] nvarchar(100) NULL,
        [SoDienThoai] nvarchar(50) NULL,
        [Email] nvarchar(100) NULL,
        [GhiChu] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_DonVi] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DonVi_DonVi_DonViChaId] FOREIGN KEY ([DonViChaId]) REFERENCES [DonVi] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE TABLE [DotGiaoChiTieu] (
        [Id] bigint NOT NULL IDENTITY,
        [MaDotGiao] nvarchar(50) NOT NULL,
        [TenDotGiao] nvarchar(255) NOT NULL,
        [NamApDung] int NOT NULL,
        [NguonDotGiao] nvarchar(50) NOT NULL,
        [CapGiao] nvarchar(50) NOT NULL,
        [DonViGiaoId] bigint NOT NULL,
        [NgayGiao] datetime2 NOT NULL,
        [TrangThai] nvarchar(50) NOT NULL,
        [GhiChu] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_DotGiaoChiTieu] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE TABLE [KyBaoCaoKPI] (
        [Id] bigint NOT NULL IDENTITY,
        [MaKy] nvarchar(50) NOT NULL,
        [TenKy] nvarchar(255) NOT NULL,
        [LoaiKy] nvarchar(30) NOT NULL,
        [Nam] int NOT NULL,
        [SoKy] int NULL,
        [TuNgay] datetime2 NOT NULL,
        [DenNgay] datetime2 NOT NULL,
        [NgayDauKy] datetime2 NOT NULL,
        [NgayCuoiKy] datetime2 NOT NULL,
        [TrangThai] nvarchar(30) NOT NULL,
        [GhiChu] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] nvarchar(100) NULL,
        [UpdatedBy] nvarchar(100) NULL,
        CONSTRAINT [PK_KyBaoCaoKPI] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE TABLE [Permissions] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(200) NOT NULL,
        CONSTRAINT [PK_Permissions] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE TABLE [RefreshTokens] (
        [Id] int NOT NULL IDENTITY,
        [Token] nvarchar(450) NOT NULL,
        [Expires] datetime2 NOT NULL,
        [IsRevoked] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [RevokedAt] datetime2 NULL,
        [ReplacedByToken] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RefreshTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE TABLE [CauHinhNguongDanhGiaKPI] (
        [Id] bigint NOT NULL IDENTITY,
        [DanhMucChiTieuId] bigint NULL,
        [TuTyLe] decimal(18,2) NOT NULL,
        [DenTyLe] decimal(18,2) NOT NULL,
        [XepLoai] nvarchar(50) NOT NULL,
        [Diem] decimal(18,2) NULL,
        [GhiChu] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_CauHinhNguongDanhGiaKPI] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CauHinhNguongDanhGiaKPI_DanhMucChiTieus_DanhMucChiTieuId] FOREIGN KEY ([DanhMucChiTieuId]) REFERENCES [DanhMucChiTieus] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE TABLE [ChiTietGiaoChiTieu] (
        [Id] bigint NOT NULL IDENTITY,
        [DotGiaoChiTieuId] bigint NOT NULL,
        [DanhMucChiTieuId] bigint NOT NULL,
        [DonViNhanId] bigint NOT NULL,
        [DonViThucHienChinhId] bigint NULL,
        [GiaTriMucTieu] decimal(18,2) NULL,
        [GiaTriMucTieuText] nvarchar(max) NULL,
        [ChiTietGiaoChaId] bigint NULL,
        [GhiChu] nvarchar(max) NULL,
        [ThuTuHienThi] int NULL,
        [TrangThai] nvarchar(50) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] nvarchar(100) NULL,
        [UpdatedBy] nvarchar(100) NULL,
        CONSTRAINT [PK_ChiTietGiaoChiTieu] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ChiTietGiaoChiTieu_ChiTietGiaoChiTieu_ChiTietGiaoChaId] FOREIGN KEY ([ChiTietGiaoChaId]) REFERENCES [ChiTietGiaoChiTieu] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ChiTietGiaoChiTieu_DanhMucChiTieus_DanhMucChiTieuId] FOREIGN KEY ([DanhMucChiTieuId]) REFERENCES [DanhMucChiTieus] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ChiTietGiaoChiTieu_DonVi_DonViNhanId] FOREIGN KEY ([DonViNhanId]) REFERENCES [DonVi] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ChiTietGiaoChiTieu_DonVi_DonViThucHienChinhId] FOREIGN KEY ([DonViThucHienChinhId]) REFERENCES [DonVi] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ChiTietGiaoChiTieu_DotGiaoChiTieu_DotGiaoChiTieuId] FOREIGN KEY ([DotGiaoChiTieuId]) REFERENCES [DotGiaoChiTieu] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE TABLE [RolePermissions] (
        [RoleId] nvarchar(450) NOT NULL,
        [PermissionId] int NOT NULL,
        CONSTRAINT [PK_RolePermissions] PRIMARY KEY ([RoleId], [PermissionId]),
        CONSTRAINT [FK_RolePermissions_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RolePermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permissions] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE TABLE [UserPermissions] (
        [UserId] nvarchar(450) NOT NULL,
        [PermissionId] int NOT NULL,
        [IsGranted] bit NOT NULL,
        CONSTRAINT [PK_UserPermissions] PRIMARY KEY ([UserId], [PermissionId]),
        CONSTRAINT [FK_UserPermissions_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserPermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permissions] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE TABLE [DanhGiaKPI] (
        [Id] bigint NOT NULL IDENTITY,
        [ChiTietGiaoChiTieuId] bigint NOT NULL,
        [KyBaoCaoKPIId] bigint NOT NULL,
        [GiaTriMucTieu] decimal(18,2) NULL,
        [GiaTriDauKy] decimal(18,2) NULL,
        [GiaTriCuoiKy] decimal(18,2) NULL,
        [GiaTriCungKyNamTruoc] decimal(18,2) NULL,
        [ChenhLechSoVoiDauKy] decimal(18,2) NULL,
        [TyLeTangTruongSoVoiDauKy] decimal(18,2) NULL,
        [ChenhLechSoVoiCungKyNamTruoc] decimal(18,2) NULL,
        [TyLeTangTruongSoVoiCungKyNamTruoc] decimal(18,2) NULL,
        [TyLeHoanThanh] decimal(18,2) NULL,
        [XepLoai] nvarchar(50) NULL,
        [KetQua] nvarchar(50) NULL,
        [NhanXetDanhGia] nvarchar(max) NULL,
        [NguoiDanhGia] nvarchar(100) NULL,
        [NgayDanhGia] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_DanhGiaKPI] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DanhGiaKPI_ChiTietGiaoChiTieu_ChiTietGiaoChiTieuId] FOREIGN KEY ([ChiTietGiaoChiTieuId]) REFERENCES [ChiTietGiaoChiTieu] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DanhGiaKPI_KyBaoCaoKPI_KyBaoCaoKPIId] FOREIGN KEY ([KyBaoCaoKPIId]) REFERENCES [KyBaoCaoKPI] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE TABLE [TheoDoiThucHienKPI] (
        [Id] bigint NOT NULL IDENTITY,
        [ChiTietGiaoChiTieuId] bigint NOT NULL,
        [KyBaoCaoKPIId] bigint NOT NULL,
        [GiaTriDauKy] decimal(18,2) NULL,
        [GiaTriThucHienTrongKy] decimal(18,2) NULL,
        [GiaTriCuoiKy] decimal(18,2) NULL,
        [GiaTriLuyKe] decimal(18,2) NULL,
        [NhanXet] nvarchar(max) NULL,
        [TrangThai] nvarchar(30) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] nvarchar(100) NULL,
        [UpdatedBy] nvarchar(100) NULL,
        CONSTRAINT [PK_TheoDoiThucHienKPI] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TheoDoiThucHienKPI_ChiTietGiaoChiTieu_ChiTietGiaoChiTieuId] FOREIGN KEY ([ChiTietGiaoChiTieuId]) REFERENCES [ChiTietGiaoChiTieu] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TheoDoiThucHienKPI_KyBaoCaoKPI_KyBaoCaoKPIId] FOREIGN KEY ([KyBaoCaoKPIId]) REFERENCES [KyBaoCaoKPI] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE INDEX [IX_CauHinhNguongDanhGiaKPI_DanhMucChiTieuId] ON [CauHinhNguongDanhGiaKPI] ([DanhMucChiTieuId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE INDEX [IX_ChiTietGiaoChiTieu_ChiTietGiaoChaId] ON [ChiTietGiaoChiTieu] ([ChiTietGiaoChaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE INDEX [IX_ChiTietGiaoChiTieu_DanhMucChiTieuId] ON [ChiTietGiaoChiTieu] ([DanhMucChiTieuId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE INDEX [IX_ChiTietGiaoChiTieu_DonViNhanId] ON [ChiTietGiaoChiTieu] ([DonViNhanId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE INDEX [IX_ChiTietGiaoChiTieu_DonViThucHienChinhId] ON [ChiTietGiaoChiTieu] ([DonViThucHienChinhId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ChiTietGiaoChiTieu_DotGiaoChiTieuId_DanhMucChiTieuId_DonViNhanId] ON [ChiTietGiaoChiTieu] ([DotGiaoChiTieuId], [DanhMucChiTieuId], [DonViNhanId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DanhGiaKPI_ChiTietGiaoChiTieuId_KyBaoCaoKPIId] ON [DanhGiaKPI] ([ChiTietGiaoChiTieuId], [KyBaoCaoKPIId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE INDEX [IX_DanhGiaKPI_KyBaoCaoKPIId] ON [DanhGiaKPI] ([KyBaoCaoKPIId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DanhMucChiTieus_MaChiTieu] ON [DanhMucChiTieus] ([MaChiTieu]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE INDEX [IX_DonVi_DonViChaId] ON [DonVi] ([DonViChaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DonVi_MaDonVi] ON [DonVi] ([MaDonVi]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DotGiaoChiTieu_MaDotGiao] ON [DotGiaoChiTieu] ([MaDotGiao]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE INDEX [IX_KyBaoCaoKPI_LoaiKy_Nam_SoKy] ON [KyBaoCaoKPI] ([LoaiKy], [Nam], [SoKy]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE UNIQUE INDEX [IX_KyBaoCaoKPI_MaKy] ON [KyBaoCaoKPI] ([MaKy]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Permissions_Name] ON [Permissions] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RefreshTokens_Token] ON [RefreshTokens] ([Token]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE INDEX [IX_RolePermissions_PermissionId] ON [RolePermissions] ([PermissionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TheoDoiThucHienKPI_ChiTietGiaoChiTieuId_KyBaoCaoKPIId] ON [TheoDoiThucHienKPI] ([ChiTietGiaoChiTieuId], [KyBaoCaoKPIId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE INDEX [IX_TheoDoiThucHienKPI_KyBaoCaoKPIId] ON [TheoDoiThucHienKPI] ([KyBaoCaoKPIId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    CREATE INDEX [IX_UserPermissions_PermissionId] ON [UserPermissions] ([PermissionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410135250_KPITracker'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260410135250_KPITracker', N'8.0.12');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410201546_AddTanSuatBaoCaoToChiTietGiaoChiTieu'
)
BEGIN
    ALTER TABLE [ChiTietGiaoChiTieu] ADD [TanSuatBaoCao] nvarchar(30) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260410201546_AddTanSuatBaoCaoToChiTietGiaoChiTieu'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260410201546_AddTanSuatBaoCaoToChiTietGiaoChiTieu', N'8.0.12');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260413215806_AddNgayKetThucToDotGiaoChiTieu'
)
BEGIN
    ALTER TABLE [DotGiaoChiTieu] ADD [NgayKetThuc] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260413215806_AddNgayKetThucToDotGiaoChiTieu'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260413215806_AddNgayKetThucToDotGiaoChiTieu', N'8.0.12');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260415104747_AddChiTieuPhanRaAndNgayBatDau'
)
BEGIN
    EXEC sp_rename N'[DotGiaoChiTieu].[NgayGiao]', N'NgayBatDau', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260415104747_AddChiTieuPhanRaAndNgayBatDau'
)
BEGIN
    ALTER TABLE [DanhMucChiTieus] ADD [BatBuocDatTatCaTieuChiCon] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260415104747_AddChiTieuPhanRaAndNgayBatDau'
)
BEGIN
    ALTER TABLE [DanhMucChiTieus] ADD [ChiTieuChaId] bigint NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260415104747_AddChiTieuPhanRaAndNgayBatDau'
)
BEGIN
    ALTER TABLE [DanhMucChiTieus] ADD [ThuTuHienThi] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260415104747_AddChiTieuPhanRaAndNgayBatDau'
)
BEGIN
    CREATE INDEX [IX_DanhMucChiTieus_ChiTieuChaId] ON [DanhMucChiTieus] ([ChiTieuChaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260415104747_AddChiTieuPhanRaAndNgayBatDau'
)
BEGIN
    ALTER TABLE [DanhMucChiTieus] ADD CONSTRAINT [FK_DanhMucChiTieus_DanhMucChiTieus_ChiTieuChaId] FOREIGN KEY ([ChiTieuChaId]) REFERENCES [DanhMucChiTieus] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260415104747_AddChiTieuPhanRaAndNgayBatDau'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260415104747_AddChiTieuPhanRaAndNgayBatDau', N'8.0.12');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260415133000_AddDieuKienThoiHanToCauHinhNguongDanhGiaKPI'
)
BEGIN
    ALTER TABLE [CauHinhNguongDanhGiaKPI] ADD [DieuKienThoiHan] nvarchar(30) NOT NULL DEFAULT N'MAC_DINH';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260415133000_AddDieuKienThoiHanToCauHinhNguongDanhGiaKPI'
)
BEGIN

                    UPDATE CauHinhNguongDanhGiaKPI
                    SET DieuKienThoiHan = CASE
                        WHEN XepLoai = 'CHUA_HOAN_THANH' THEN 'CHUA_DEN_HAN'
                        WHEN XepLoai = 'KHONG_HOAN_THANH' THEN 'DA_DEN_HAN'
                        ELSE 'MAC_DINH'
                    END
                
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260415133000_AddDieuKienThoiHanToCauHinhNguongDanhGiaKPI'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260415133000_AddDieuKienThoiHanToCauHinhNguongDanhGiaKPI', N'8.0.12');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416081028_AddGiaTriDauKyCoDinhToChiTietGiaoChiTieu'
)
BEGIN
    ALTER TABLE [ChiTietGiaoChiTieu] ADD [GiaTriDauKyCoDinh] decimal(18,2) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416081028_AddGiaTriDauKyCoDinhToChiTietGiaoChiTieu'
)
BEGIN

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
                
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416081028_AddGiaTriDauKyCoDinhToChiTietGiaoChiTieu'
)
BEGIN

                    UPDATE ct
                    SET ct.GiaTriDauKyCoDinh = 0
                    FROM ChiTietGiaoChiTieu AS ct
                    INNER JOIN DanhMucChiTieus AS dm ON dm.Id = ct.DanhMucChiTieuId
                    WHERE ct.GiaTriDauKyCoDinh IS NULL
                      AND UPPER(ISNULL(dm.LoaiChiTieu, '')) <> 'DINH_TINH';
                
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416081028_AddGiaTriDauKyCoDinhToChiTietGiaoChiTieu'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260416081028_AddGiaTriDauKyCoDinhToChiTietGiaoChiTieu', N'8.0.12');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416154736_SyncTieuChiDanhGiaWorkflow'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260416154736_SyncTieuChiDanhGiaWorkflow', N'8.0.12');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416173000_AddTieuChiDanhGiaToChiTietVaNguong'
)
BEGIN

                    IF COL_LENGTH('dbo.ChiTietGiaoChiTieu', 'TieuChiDanhGia') IS NULL
                        ALTER TABLE dbo.ChiTietGiaoChiTieu ADD TieuChiDanhGia nvarchar(50) NULL;

                    IF COL_LENGTH('dbo.ChiTietGiaoChiTieu', 'LoaiMocSoSanh') IS NULL
                        ALTER TABLE dbo.ChiTietGiaoChiTieu ADD LoaiMocSoSanh nvarchar(50) NULL;

                    IF COL_LENGTH('dbo.ChiTietGiaoChiTieu', 'ChieuSoSanh') IS NULL
                        ALTER TABLE dbo.ChiTietGiaoChiTieu ADD ChieuSoSanh nvarchar(50) NULL;

                    IF COL_LENGTH('dbo.CauHinhNguongDanhGiaKPI', 'TieuChiDanhGia') IS NULL
                        ALTER TABLE dbo.CauHinhNguongDanhGiaKPI ADD TieuChiDanhGia nvarchar(50) NULL;
                
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416173000_AddTieuChiDanhGiaToChiTietVaNguong'
)
BEGIN

                    UPDATE ct
                    SET
                        ct.TieuChiDanhGia = CASE
                            WHEN UPPER(ISNULL(dm.LoaiChiTieu, '')) IN ('DINH_TINH', 'DINH_LUONG_TICH_LUY', 'DINH_LUONG_SO_SANH')
                                THEN UPPER(dm.LoaiChiTieu)
                            ELSE NULL
                        END,
                        ct.LoaiMocSoSanh = CASE
                            WHEN UPPER(ISNULL(dm.LoaiChiTieu, '')) = 'DINH_LUONG_SO_SANH'
                                THEN UPPER(dm.LoaiMocSoSanh)
                            ELSE NULL
                        END,
                        ct.ChieuSoSanh = CASE
                            WHEN UPPER(ISNULL(dm.LoaiChiTieu, '')) = 'DINH_LUONG_SO_SANH'
                                THEN UPPER(dm.ChieuSoSanh)
                            ELSE NULL
                        END
                    FROM ChiTietGiaoChiTieu AS ct
                    INNER JOIN DanhMucChiTieus AS dm ON dm.Id = ct.DanhMucChiTieuId
                    WHERE ct.TieuChiDanhGia IS NULL;
                
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416173000_AddTieuChiDanhGiaToChiTietVaNguong'
)
BEGIN

                    UPDATE cfg
                    SET cfg.TieuChiDanhGia = UPPER(dm.LoaiChiTieu)
                    FROM CauHinhNguongDanhGiaKPI AS cfg
                    INNER JOIN DanhMucChiTieus AS dm ON dm.Id = cfg.DanhMucChiTieuId
                    WHERE cfg.TieuChiDanhGia IS NULL
                      AND UPPER(ISNULL(dm.LoaiChiTieu, '')) IN ('DINH_TINH', 'DINH_LUONG_TICH_LUY', 'DINH_LUONG_SO_SANH');
                
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416173000_AddTieuChiDanhGiaToChiTietVaNguong'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260416173000_AddTieuChiDanhGiaToChiTietVaNguong', N'8.0.12');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260417161500_AddQuyTacDanhGiaToChiTietVaNguong'
)
BEGIN

                    IF COL_LENGTH('dbo.ChiTietGiaoChiTieu', 'QuyTacDanhGia') IS NULL
                        ALTER TABLE dbo.ChiTietGiaoChiTieu ADD QuyTacDanhGia nvarchar(50) NULL;

                    IF COL_LENGTH('dbo.CauHinhNguongDanhGiaKPI', 'QuyTacDanhGia') IS NULL
                        ALTER TABLE dbo.CauHinhNguongDanhGiaKPI ADD QuyTacDanhGia nvarchar(50) NULL;
                
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260417161500_AddQuyTacDanhGiaToChiTietVaNguong'
)
BEGIN

                    UPDATE ChiTietGiaoChiTieu
                    SET QuyTacDanhGia = CASE
                        WHEN UPPER(ISNULL(TieuChiDanhGia, '')) = 'DINH_TINH' THEN 'MAC_DINH'
                        ELSE 'DAT_TOI_THIEU'
                    END
                    WHERE QuyTacDanhGia IS NULL;
                
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260417161500_AddQuyTacDanhGiaToChiTietVaNguong'
)
BEGIN

                    UPDATE CauHinhNguongDanhGiaKPI
                    SET QuyTacDanhGia = CASE
                        WHEN UPPER(ISNULL(TieuChiDanhGia, '')) = 'DINH_TINH' THEN 'MAC_DINH'
                        ELSE 'DAT_TOI_THIEU'
                    END
                    WHERE QuyTacDanhGia IS NULL;
                
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260417161500_AddQuyTacDanhGiaToChiTietVaNguong'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260417161500_AddQuyTacDanhGiaToChiTietVaNguong', N'8.0.12');
END;
GO

COMMIT;
GO

