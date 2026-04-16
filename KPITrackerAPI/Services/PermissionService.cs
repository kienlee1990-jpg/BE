using KPITrackerAPI.Data;
using KPITrackerAPI.Entities;
using KPITrackerAPI.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KPITrackerAPI.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public PermissionService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<List<string>> GetPermissionsAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            // 1. Permission t? role
            var rolePermissions = await _context.RolePermissions
                .Where(rp => rp.Role != null
                          && rp.Permission != null
                          && roles.Contains(rp.Role.Name))
                .Select(rp => rp.Permission.Name)
                .ToListAsync();

            // 2. Permission override t? user
            var userPermissions = await _context.UserPermissions
                .Where(up => up.UserId == user.Id && up.Permission != null)
                .Select(up => new
                {
                    up.Permission.Name,
                    up.IsGranted
                })
                .ToListAsync();

            var granted = userPermissions
                .Where(p => p.IsGranted)
                .Select(p => p.Name);

            var denied = userPermissions
                .Where(p => !p.IsGranted)
                .Select(p => p.Name);

            // 3. Merge
            return rolePermissions
                .Union(granted)
                .Except(denied)
                .Distinct()
                .ToList();
        }
    }
}
