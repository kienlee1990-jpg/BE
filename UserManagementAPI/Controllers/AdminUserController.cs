using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.DTOs.Admin;
using UserManagementAPI.Interfaces;

namespace UserManagementAPI.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminUserController : ControllerBase
    {
        private readonly IAdminUserService _adminUserService;

        public AdminUserController(IAdminUserService adminUserService)
        {
            _adminUserService = adminUserService;
        }

        // ==================== USER ====================
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminUserService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _adminUserService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound("User not found");
            return Ok(user);
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto dto)
        {
            await _adminUserService.UpdateUserAsync(id, dto);
            return NoContent();
        }

        // ==================== ROLE (USER) ====================
        [HttpPost("users/{id}/roles")]
        public async Task<IActionResult> AssignRole(string id, [FromBody] RoleDto dto)
        {
            await _adminUserService.AssignRoleAsync(id, dto.RoleName);
            return NoContent();
        }

        [HttpDelete("users/{id}/roles")]
        public async Task<IActionResult> RemoveRole(string id, [FromBody] RoleDto dto)
        {
            await _adminUserService.RemoveRoleAsync(id, dto.RoleName);
            return NoContent();
        }

        // ==================== PERMISSION (USER) ====================
        [HttpPost("users/{id}/permissions")]
        public async Task<IActionResult> AssignPermission(string id, [FromBody] PermissionDto dto)
        {
            await _adminUserService.AssignPermissionAsync(id, dto.PermissionName);
            return NoContent();
        }

        [HttpDelete("users/{id}/permissions")]
        public async Task<IActionResult> RemovePermission(string id, [FromBody] PermissionDto dto)
        {
            await _adminUserService.RemovePermissionAsync(id, dto.PermissionName);
            return NoContent();
        }

        // ==================== ROLE (CRUD) ====================
        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole([FromBody] RoleDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RoleName))
                return BadRequest("RoleName is required");

            await _adminUserService.CreateRoleAsync(dto.RoleName);
            return Ok($"Role '{dto.RoleName}' created successfully");
        }

        [HttpDelete("roles/{roleName}")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return BadRequest("RoleName is required");

            await _adminUserService.DeleteRoleAsync(roleName);
            return NoContent();
        }

        // ==================== PERMISSION (CRUD) ====================
        [HttpPost("permissions")]
        public async Task<IActionResult> CreatePermission([FromBody] PermissionDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.PermissionName))
                return BadRequest("PermissionName is required");

            await _adminUserService.CreatePermissionAsync(dto.PermissionName);
            return Ok($"Permission '{dto.PermissionName}' created successfully");
        }

        [HttpDelete("permissions/{permissionName}")]
        public async Task<IActionResult> DeletePermission(string permissionName)
        {
            if (string.IsNullOrWhiteSpace(permissionName))
                return BadRequest("PermissionName is required");

            await _adminUserService.DeletePermissionAsync(permissionName);
            return NoContent();
        }

        // ==================== PERMISSION (ROLE) ====================

        /// Gán permission cho role
        /// POST: api/admin/roles/{roleId}/permissions
        [HttpPost("roles/{roleId}/permissions")]
        public async Task<IActionResult> AssignPermissionToRole(string roleId, [FromBody] PermissionDto dto)
        {
            await _adminUserService.AssignPermissionToRoleAsync(roleId, dto.PermissionName);
            return NoContent();
        }

        /// Thu hồi permission khỏi role
        /// DELETE: api/admin/roles/{roleId}/permissions
        [HttpDelete("roles/{roleId}/permissions")]
        public async Task<IActionResult> RemovePermissionFromRole(string roleId, [FromBody] PermissionDto dto)
        {
            await _adminUserService.RemovePermissionFromRoleAsync(roleId, dto.PermissionName);
            return NoContent();
        }
    }
}