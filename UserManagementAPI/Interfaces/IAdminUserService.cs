using UserManagementAPI.DTOs.Admin;

namespace UserManagementAPI.Interfaces
{
    public interface IAdminUserService
    {
        Task<IEnumerable<object>> GetAllUsersAsync();
        Task<object?> GetUserByIdAsync(string id);
        Task UpdateUserAsync(string id, UpdateUserDto dto);
        Task AssignRoleAsync(string userId, string roleName);
        Task RemoveRoleAsync(string userId, string roleName);
        Task AssignPermissionAsync(string userId, string permissionName);
        Task RemovePermissionAsync(string userId, string permissionName);
        Task AssignPermissionToRoleAsync(string roleId, string permissionName);
        Task RemovePermissionFromRoleAsync(string roleId, string permissionName);
        // ROLE
        Task CreateRoleAsync(string roleName);
        Task DeleteRoleAsync(string roleName);

        // PERMISSION
        Task CreatePermissionAsync(string permissionName);
        Task DeletePermissionAsync(string permissionName);
    }
}