using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Entities;

namespace UserManagementAPI.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // ================= DbSet =================
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    // 🔥 RBAC
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
        // RolePermission (Many-to-Many: Role - Permission)
        // =====================================================
        builder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });

            entity.HasOne(rp => rp.Role)
                  .WithMany() // nếu sau này có navigation thì thêm vào
                  .HasForeignKey(rp => rp.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(rp => rp.Permission)
                  .WithMany()
                  .HasForeignKey(rp => rp.PermissionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // =====================================================
        // UserPermission (🔥 FIX QUAN TRỌNG)
        // =====================================================
        builder.Entity<UserPermission>(entity =>
        {
            entity.HasKey(up => new { up.UserId, up.PermissionId });

            entity.Property(up => up.IsGranted)
                  .IsRequired();

            // 🔥 FIX: map đúng navigation
            entity.HasOne(up => up.User)
                  .WithMany(u => u.UserPermissions)
                  .HasForeignKey(up => up.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(up => up.Permission)
                  .WithMany()
                  .HasForeignKey(up => up.PermissionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}