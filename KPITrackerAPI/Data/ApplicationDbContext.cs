using KPITrackerAPI.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KPITrackerAPI.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // ================= User Management =================
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    // ================= KPI =================
    public DbSet<DanhMucChiTieu> DanhMucChiTieus { get; set; }
    public DbSet<DotGiaoChiTieu> DotGiaoChiTieus { get; set; }
    public DbSet<ChiTietGiaoChiTieu> ChiTietGiaoChiTieus { get; set; }
    public DbSet<DonVi> DonVis { get; set; }
    public DbSet<NhomThiDua> NhomThiDuas { get; set; }
    public DbSet<NhomThiDuaDonVi> NhomThiDuaDonVis { get; set; }
    public DbSet<NhomThiDuaChiTieu> NhomThiDuaChiTieus { get; set; }
    public DbSet<KyBaoCaoKPI> KyBaoCaoKPIs { get; set; }
    public DbSet<TheoDoiThucHienKPI> TheoDoiThucHienKPIs { get; set; }
    public DbSet<DanhGiaKPI> DanhGiaKPIs { get; set; }
    public DbSet<CauHinhNguongDanhGiaKPI> CauHinhNguongDanhGiaKPIs { get; set; }

    // ================= RBAC =================
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // =====================================================
        // RefreshToken
        // =====================================================
        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.Token)
                  .IsUnique();

            entity.HasOne(x => x.User)
                  .WithMany(u => u.RefreshTokens)
                  .HasForeignKey(x => x.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // =====================================================
        // Permission
        // =====================================================
        builder.Entity<Permission>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Name)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.HasIndex(p => p.Name)
                  .IsUnique();
        });

        // =====================================================
        // RolePermission
        // =====================================================
        builder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });

            entity.HasOne(rp => rp.Role)
                  .WithMany()
                  .HasForeignKey(rp => rp.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(rp => rp.Permission)
                  .WithMany()
                  .HasForeignKey(rp => rp.PermissionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // =====================================================
        // UserPermission
        // =====================================================
        builder.Entity<UserPermission>(entity =>
        {
            entity.HasKey(up => new { up.UserId, up.PermissionId });

            entity.Property(up => up.IsGranted)
                  .IsRequired();

            entity.HasOne(up => up.User)
                  .WithMany(u => u.UserPermissions)
                  .HasForeignKey(up => up.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(up => up.Permission)
                  .WithMany()
                  .HasForeignKey(up => up.PermissionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.HasOne(x => x.DonVi)
                  .WithMany()
                  .HasForeignKey(x => x.DonViId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================
        // DanhMucChiTieu
        // =====================================================
        builder.Entity<DanhMucChiTieu>(entity =>
        {
            entity.HasIndex(x => x.MaChiTieu)
                  .IsUnique();

            entity.Property(x => x.TyLePhanTramMucTieu)
                  .HasColumnType("decimal(18,2)");

            entity.HasOne(x => x.ChiTieuCha)
                  .WithMany(x => x.TieuChiCons)
                  .HasForeignKey(x => x.ChiTieuChaId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================
        // DotGiaoChiTieu
        // =====================================================
        builder.Entity<DotGiaoChiTieu>(entity =>
        {
            entity.HasIndex(x => x.MaDotGiao)
                  .IsUnique();
        });

        // =====================================================
        // ChiTietGiaoChiTieu
        // =====================================================
        builder.Entity<ChiTietGiaoChiTieu>(entity =>
        {
            entity.Property(x => x.GiaTriMucTieu)
                  .HasColumnType("decimal(18,2)");

            entity.Property(x => x.GiaTriDauKyCoDinh)
                  .HasColumnType("decimal(18,2)");

            entity.Property(x => x.TieuChiDanhGia)
                  .HasMaxLength(50);

            entity.Property(x => x.LoaiMocSoSanh)
                  .HasMaxLength(50);

            entity.Property(x => x.KieuSoSanh)
                  .HasMaxLength(50);

            entity.Property(x => x.ChieuSoSanh)
                  .HasMaxLength(50);

            entity.Property(x => x.QuyTacDanhGia)
                  .HasMaxLength(50);

            entity.HasIndex(x => new
            {
                x.DotGiaoChiTieuId,
                x.DanhMucChiTieuId,
                x.DonViNhanId
            }).IsUnique();

            entity.HasOne(x => x.DotGiaoChiTieu)
                  .WithMany(x => x.ChiTietGiaoChiTieux)
                  .HasForeignKey(x => x.DotGiaoChiTieuId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.DanhMucChiTieu)
                  .WithMany(x => x.ChiTietGiaoChiTieux)
                  .HasForeignKey(x => x.DanhMucChiTieuId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.DonViNhan)
                  .WithMany()
                  .HasForeignKey(x => x.DonViNhanId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.DonViThucHienChinh)
                  .WithMany()
                  .HasForeignKey(x => x.DonViThucHienChinhId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.ChiTietGiaoCha)
                  .WithMany(x => x.ChiTietGiaoChiTieuCons)
                  .HasForeignKey(x => x.ChiTietGiaoChaId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================
        // DonVi
        // =====================================================
        builder.Entity<DonVi>(entity =>
        {
            entity.HasIndex(x => x.MaDonVi)
                  .IsUnique();

            entity.HasOne(x => x.DonViCha)
                  .WithMany()
                  .HasForeignKey(x => x.DonViChaId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================
        // NhomThiDua
        // =====================================================
        builder.Entity<NhomThiDua>(entity =>
        {
            entity.HasIndex(x => x.MaNhom)
                  .IsUnique();

            entity.HasIndex(x => x.TenNhom)
                  .IsUnique();
        });

        // =====================================================
        // NhomThiDuaDonVi
        // =====================================================
        builder.Entity<NhomThiDuaDonVi>(entity =>
        {
            entity.HasKey(x => new { x.NhomThiDuaId, x.DonViId });

            entity.HasOne(x => x.NhomThiDua)
                  .WithMany(x => x.NhomThiDuaDonVis)
                  .HasForeignKey(x => x.NhomThiDuaId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.DonVi)
                  .WithMany()
                  .HasForeignKey(x => x.DonViId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================
        // NhomThiDuaChiTieu
        // =====================================================
        builder.Entity<NhomThiDuaChiTieu>(entity =>
        {
            entity.HasKey(x => new { x.NhomThiDuaId, x.DanhMucChiTieuId });

            entity.HasOne(x => x.NhomThiDua)
                  .WithMany(x => x.NhomThiDuaChiTieus)
                  .HasForeignKey(x => x.NhomThiDuaId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.DanhMucChiTieu)
                  .WithMany()
                  .HasForeignKey(x => x.DanhMucChiTieuId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================
        // KyBaoCaoKPI
        // =====================================================
        builder.Entity<KyBaoCaoKPI>(entity =>
        {
            entity.HasIndex(x => x.MaKy)
                  .IsUnique();

            entity.HasIndex(x => new { x.LoaiKy, x.Nam, x.SoKy })
                  .IsUnique(false);
        });

        // =====================================================
        // TheoDoiThucHienKPI
        // =====================================================
        builder.Entity<TheoDoiThucHienKPI>(entity =>
        {
            entity.Property(x => x.GiaTriDauKy).HasColumnType("decimal(18,2)");
            entity.Property(x => x.GiaTriPhatSinhTrongKy).HasColumnType("decimal(18,2)");
            entity.Property(x => x.GiaTriThucHienTrongKy).HasColumnType("decimal(18,2)");
            entity.Property(x => x.GiaTriCuoiKy).HasColumnType("decimal(18,2)");
            entity.Property(x => x.GiaTriLuyKe).HasColumnType("decimal(18,2)");
            entity.Property(x => x.GiaTriPhatSinhLuyKe).HasColumnType("decimal(18,2)");

            entity.HasIndex(x => new { x.ChiTietGiaoChiTieuId, x.KyBaoCaoKPIId })
                  .IsUnique();

            entity.HasOne(x => x.ChiTietGiaoChiTieu)
                  .WithMany()
                  .HasForeignKey(x => x.ChiTietGiaoChiTieuId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.KyBaoCaoKPI)
                  .WithMany(x => x.TheoDoiThucHienKPIs)
                  .HasForeignKey(x => x.KyBaoCaoKPIId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================
        // DanhGiaKPI
        // =====================================================
        builder.Entity<DanhGiaKPI>(entity =>
        {
            entity.Property(x => x.GiaTriMucTieu).HasColumnType("decimal(18,2)");
            entity.Property(x => x.GiaTriDauKy).HasColumnType("decimal(18,2)");
            entity.Property(x => x.GiaTriCuoiKy).HasColumnType("decimal(18,2)");
            entity.Property(x => x.GiaTriCungKyNamTruoc).HasColumnType("decimal(18,2)");
            entity.Property(x => x.ChenhLechSoVoiDauKy).HasColumnType("decimal(18,2)");
            entity.Property(x => x.TyLeTangTruongSoVoiDauKy).HasColumnType("decimal(18,2)");
            entity.Property(x => x.ChenhLechSoVoiCungKyNamTruoc).HasColumnType("decimal(18,2)");
            entity.Property(x => x.TyLeTangTruongSoVoiCungKyNamTruoc).HasColumnType("decimal(18,2)");
            entity.Property(x => x.TyLeHoanThanh).HasColumnType("decimal(18,2)");

            entity.HasIndex(x => new { x.ChiTietGiaoChiTieuId, x.KyBaoCaoKPIId })
                  .IsUnique();

            entity.HasOne(x => x.ChiTietGiaoChiTieu)
                  .WithMany()
                  .HasForeignKey(x => x.ChiTietGiaoChiTieuId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.KyBaoCaoKPI)
                  .WithMany(x => x.DanhGiaKPIs)
                  .HasForeignKey(x => x.KyBaoCaoKPIId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================
        // CauHinhNguongDanhGiaKPI
        // =====================================================
        builder.Entity<CauHinhNguongDanhGiaKPI>(entity =>
        {
            entity.Property(x => x.TuTyLe).HasColumnType("decimal(18,2)");
            entity.Property(x => x.DenTyLe).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Diem).HasColumnType("decimal(18,2)");
            entity.Property(x => x.TieuChiDanhGia)
                  .HasMaxLength(50);
            entity.Property(x => x.QuyTacDanhGia)
                  .HasMaxLength(50);
            entity.Property(x => x.DieuKienThoiHan)
                  .HasMaxLength(30);

            entity.HasOne(x => x.DanhMucChiTieu)
                  .WithMany()
                  .HasForeignKey(x => x.DanhMucChiTieuId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

