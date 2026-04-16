using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KPITrackerAPI.Data;
using KPITrackerAPI.DTOs.Admin;
using KPITrackerAPI.Entities;
using KPITrackerAPI.Interfaces;
using KPITrackerAPI.Middlewares;

namespace KPITrackerAPI.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminUserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // ================= USER =================
        public async Task<IEnumerable<object>> GetAllUsersAsync()
        {
            var users = await _userManager.Users
                .Include(u => u.UserPermissions)
                .ThenInclude(up => up.Permission)
                .ToListAsync();

            var resultList = new List<object>();

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);

                resultList.Add(new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.FullName,
                    u.Address,
                    u.PhoneNumber,
                    u.IsActive,
                    Roles = roles,
                    Permissions = u.UserPermissions
                        .Where(up => up.IsGranted)
                        .Select(up => up.Permission.Name)
                        .ToList()
                });
            }

            return resultList;
        }

        public async Task<object?> GetUserByIdAsync(string id)
        {
            var user = await _userManager.Users
                .Include(u => u.UserPermissions)
                .ThenInclude(up => up.Permission)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.FullName,
                user.Address,
                user.PhoneNumber,
                user.IsActive,
                Roles = roles,
                Permissions = user.UserPermissions
                    .Where(up => up.IsGranted)
                    .Select(up => up.Permission.Name)
                    .ToList()
            };
        }

        public async Task UpdateUserAsync(string id, UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) throw new Exception("User not found");

            if (dto.FullName != null) user.FullName = dto.FullName;
            if (dto.Address != null) user.Address = dto.Address;
            if (dto.PhoneNumber != null) user.PhoneNumber = dto.PhoneNumber;
            if (dto.IsActive.HasValue) user.IsActive = dto.IsActive.Value;

            await _userManager.UpdateAsync(user);
        }

        // ================= ROLE =================
        public async Task AssignRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new KeyNotFoundException("User not found");

            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) throw new KeyNotFoundException("Role does not exist");

            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Contains(roleName))
                throw new InvalidOperationException("User already has this role");

            // 1?? Gán role
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            // 2?? Ð?ng b? permission t? role
            var rolePermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == role.Id)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            var userPermissions = await _context.UserPermissions
                .Where(up => up.UserId == userId)
                .Select(up => up.PermissionId)
                .ToListAsync();

            var newPermissions = rolePermissions.Except(userPermissions);

            foreach (var permId in newPermissions)
            {
                _context.UserPermissions.Add(new UserPermission
                {
                    UserId = userId,
                    PermissionId = permId,
                    IsGranted = true
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new KeyNotFoundException("User not found");

            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) throw new KeyNotFoundException("Role not found");

            var isInRole = await _userManager.IsInRoleAsync(user, roleName);
            if (!isInRole)
                throw new InvalidOperationException("User does not have this role");

            // 1?? Xóa role kh?i user
            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            // 2?? L?y t?t c? permission t? role v?a xóa
            var rolePermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == role.Id)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            // 3?? L?y t?t c? các permission user v?n có t? các role còn l?i
            var remainingRoleIds = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            var remainingRolePermissions = await _context.RolePermissions
                .Where(rp => remainingRoleIds.Contains(rp.RoleId))
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            // 4?? Xóa permission mà user không còn role nào c?p
            var permissionsToRemove = rolePermissions.Except(remainingRolePermissions).ToList();

            var userPermissionsToRemove = await _context.UserPermissions
                .Where(up => up.UserId == userId && permissionsToRemove.Contains(up.PermissionId))
                .ToListAsync();

            _context.UserPermissions.RemoveRange(userPermissionsToRemove);

            await _context.SaveChangesAsync();
        }

        public async Task CreateRoleAsync(string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
                throw new Exception("Role already exists");

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task DeleteRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                throw new Exception("Role not found");

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // ================= PERMISSION =================
        public async Task AssignPermissionAsync(string userId, string permissionName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            var permission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name == permissionName);
            if (permission == null) throw new Exception("Permission not found");

            var userPermission = await _context.UserPermissions
                .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permission.Id);

            if (userPermission != null)
                userPermission.IsGranted = true;
            else
                _context.UserPermissions.Add(new UserPermission
                {
                    UserId = userId,
                    PermissionId = permission.Id,
                    IsGranted = true
                });

            await _context.SaveChangesAsync();
        }

        public async Task RemovePermissionAsync(string userId, string permissionName)
        {
            var permission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name == permissionName);
            if (permission == null)
                throw new NotFoundException("Permission not found");

            var userPermission = await _context.UserPermissions
                .FirstOrDefaultAsync(up =>
                    up.UserId == userId &&
                    up.PermissionId == permission.Id);

            if (userPermission == null)
            {
                _context.UserPermissions.Add(new UserPermission
                {
                    UserId = userId,
                    PermissionId = permission.Id,
                    IsGranted = false
                });
            }
            else
            {
                userPermission.IsGranted = false;
            }

            await _context.SaveChangesAsync();
        }

        public async Task CreatePermissionAsync(string permissionName)
        {
            var exists = await _context.Permissions
                .AnyAsync(p => p.Name == permissionName);
            if (exists)
                throw new Exception("Permission already exists");

            _context.Permissions.Add(new Permission { Name = permissionName });
            await _context.SaveChangesAsync();
        }

        public async Task DeletePermissionAsync(string permissionName)
        {
            var permission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name == permissionName);
            if (permission == null) throw new Exception("Permission not found");

            var userPermissions = _context.UserPermissions
                .Where(up => up.PermissionId == permission.Id);
            var rolePermissions = _context.RolePermissions
                .Where(rp => rp.PermissionId == permission.Id);

            _context.UserPermissions.RemoveRange(userPermissions);
            _context.RolePermissions.RemoveRange(rolePermissions);
            _context.Permissions.Remove(permission);

            await _context.SaveChangesAsync();
        }

        // ================= PERMISSION CHO ROLE =================

        /// Gán permission cho role
        public async Task AssignPermissionToRoleAsync(string roleId, string permissionName)
        {
            // 1?? L?y role
            var role = await _roleManager.FindByIdAsync(roleId)
                ?? throw new KeyNotFoundException("Role not found");

            // 2?? L?y permission
            var permission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name == permissionName)
                ?? throw new KeyNotFoundException("Permission not found");

            // 3?? Ki?m tra RolePermissions dã t?n t?i chua
            var rolePermission = await _context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permission.Id);

            if (rolePermission == null)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permission.Id
                });
            }

            // 4?? Ð?ng b? permission cho t?t c? user dang có role này
            var usersInRole = await _context.UserRoles
                .Where(ur => ur.RoleId == roleId)
                .Select(ur => ur.UserId)
                .ToListAsync();

            foreach (var userId in usersInRole)
            {
                var exists = await _context.UserPermissions
                    .AnyAsync(up => up.UserId == userId && up.PermissionId == permission.Id);

                if (!exists)
                {
                    _context.UserPermissions.Add(new UserPermission
                    {
                        UserId = userId,
                        PermissionId = permission.Id,
                        IsGranted = true
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        /// Thu h?i permission kh?i role
        public async Task RemovePermissionFromRoleAsync(string roleId, string permissionName)
        {
            // 1?? L?y role
            var role = await _roleManager.FindByIdAsync(roleId)
                ?? throw new KeyNotFoundException("Role not found");

            // 2?? L?y permission
            var permission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name == permissionName)
                ?? throw new KeyNotFoundException("Permission not found");

            // 3?? Ki?m tra RolePermissions t?n t?i
            var rolePermission = await _context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permission.Id);

            if (rolePermission != null)
            {
                _context.RolePermissions.Remove(rolePermission);
            }

            // 4?? Ð?ng b? UserPermissions: thu h?i permission t? user ch? còn role này c?p
            var usersInRole = await _context.UserRoles
                .Where(ur => ur.RoleId == roleId)
                .Select(ur => ur.UserId)
                .ToListAsync();

            foreach (var userId in usersInRole)
            {
                // Ki?m tra user còn role nào khác c?p permission này không
                var stillHasPermissionFromOtherRoles = await _context.UserRoles
                    .Where(ur => ur.UserId == userId && ur.RoleId != roleId)
                    .Join(_context.RolePermissions,
                          ur => ur.RoleId,
                          rp => rp.RoleId,
                          (ur, rp) => rp.PermissionId)
                    .AnyAsync(pid => pid == permission.Id);

                if (!stillHasPermissionFromOtherRoles)
                {
                    var userPermission = await _context.UserPermissions
                        .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permission.Id);

                    if (userPermission != null)
                    {
                        _context.UserPermissions.Remove(userPermission);
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
