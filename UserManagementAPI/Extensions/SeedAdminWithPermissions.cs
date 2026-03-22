using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Entities;
using UserManagementAPI.Data;

namespace UserManagementAPI.Data;

public static class SeedAdminUpdatePermissions
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        // ===== 1. Seed Role Admin =====
        const string adminRole = "Admin";
        var roleEntity = await roleManager.FindByNameAsync(adminRole);
        if (roleEntity == null)
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
            roleEntity = await roleManager.FindByNameAsync(adminRole);
        }

        // ===== 2. Seed or Update Admin User =====
        const string adminEmail = "admin@example.com";
        var adminUser = await userManager.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = adminEmail,
                NormalizedEmail = adminEmail.ToUpper(),
                EmailConfirmed = true,
                FullName = "Administrator",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (!result.Succeeded)
                throw new Exception("Failed to create admin user: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));

            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
        else
        {
            // Update thông tin admin nếu muốn (không update password mặc định)
            adminUser.FullName = "Administrator";
            adminUser.IsActive = true;
            context.Update(adminUser);
            await context.SaveChangesAsync();

            // Gán role nếu chưa có
            var roles = await userManager.GetRolesAsync(adminUser);
            if (!roles.Contains(adminRole))
                await userManager.AddToRoleAsync(adminUser, adminRole);
        }

        // ===== 3. Seed Permissions =====
        if (!context.Permissions.Any())
        {
            var permissions = new[]
            {
                "CreateMedicalRecord",
                "DeleteMedicalRecord",
                "ViewPatient",
                "ProcessPayment",
                "ManageUsers",
            }.Select(p => new Permission { Name = p }).ToList();

            context.Permissions.AddRange(permissions);
            await context.SaveChangesAsync();
        }

        // ===== 4. Gán tất cả permission cho Admin role (update nếu có permission mới) =====
        var rolePermissions = context.RolePermissions.ToList(); // load sẵn
        foreach (var permission in context.Permissions)
        {
            if (!rolePermissions.Any(rp => rp.RoleId == roleEntity.Id && rp.PermissionId == permission.Id))
            {
                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = roleEntity.Id,
                    PermissionId = permission.Id
                });
            }
        }

        await context.SaveChangesAsync();
    }
}