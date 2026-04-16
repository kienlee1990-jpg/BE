using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KPITrackerAPI.Authorization;
using KPITrackerAPI.Data;
using KPITrackerAPI.Entities;

namespace KPITrackerAPI.Extensions;

public static class SeedAdminPermissions
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        const string adminRole = "Admin";
        var roleEntity = await roleManager.FindByNameAsync(adminRole);
        if (roleEntity == null)
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
            roleEntity = await roleManager.FindByNameAsync(adminRole);
        }

        if (roleEntity == null)
        {
            throw new Exception("Failed to ensure the admin role exists.");
        }

        const string adminEmail = "admin@example.com";
        var adminUser = await userManager.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = adminEmail,
                NormalizedEmail = adminEmail.ToUpperInvariant(),
                EmailConfirmed = true,
                FullName = "Administrator",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (!result.Succeeded)
            {
                throw new Exception("Failed to create admin user: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
        else
        {
            adminUser.FullName = "Administrator";
            adminUser.IsActive = true;
            context.Update(adminUser);
            await context.SaveChangesAsync();

            var roles = await userManager.GetRolesAsync(adminUser);
            if (!roles.Contains(adminRole))
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }

        var desiredPermissions = AppPermissions.All.ToHashSet(StringComparer.Ordinal);
        var existingPermissions = await context.Permissions.ToListAsync();

        var obsoletePermissions = existingPermissions
            .Where(permission => !desiredPermissions.Contains(permission.Name))
            .ToList();

        if (obsoletePermissions.Count > 0)
        {
            var obsoletePermissionIds = obsoletePermissions
                .Select(permission => permission.Id)
                .ToList();

            var obsoleteUserPermissions = context.UserPermissions
                .Where(userPermission => obsoletePermissionIds.Contains(userPermission.PermissionId));
            var obsoleteRolePermissions = context.RolePermissions
                .Where(rolePermission => obsoletePermissionIds.Contains(rolePermission.PermissionId));

            context.UserPermissions.RemoveRange(obsoleteUserPermissions);
            context.RolePermissions.RemoveRange(obsoleteRolePermissions);
            context.Permissions.RemoveRange(obsoletePermissions);
        }

        var existingPermissionNames = existingPermissions
            .Select(permission => permission.Name)
            .ToHashSet(StringComparer.Ordinal);

        var missingPermissions = desiredPermissions
            .Where(permissionName => !existingPermissionNames.Contains(permissionName))
            .Select(permissionName => new Permission { Name = permissionName })
            .ToList();

        if (missingPermissions.Count > 0)
        {
            context.Permissions.AddRange(missingPermissions);
        }

        if (obsoletePermissions.Count > 0 || missingPermissions.Count > 0)
        {
            await context.SaveChangesAsync();
        }

        var activePermissions = await context.Permissions.ToListAsync();
        var rolePermissions = await context.RolePermissions
            .Where(rolePermission => rolePermission.RoleId == roleEntity.Id)
            .ToListAsync();

        foreach (var permission in activePermissions)
        {
            if (!rolePermissions.Any(rolePermission => rolePermission.PermissionId == permission.Id))
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
