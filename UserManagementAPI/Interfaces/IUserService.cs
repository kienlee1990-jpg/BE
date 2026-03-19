using UserManagementAPI.DTOs.User;
using UserManagementAPI.Responses;

namespace UserManagementAPI.Interfaces;

public interface IUserService
{
    Task<ApiResponse<List<UserDto>>> GetAllUsersAsync();
    Task<ApiResponse<string>> UpdateUserAsync(string id, string currentUserId, bool isAdmin, UpdateUserDto dto);
    Task<UserDto?> GetUserByIdAsync(string id);
}