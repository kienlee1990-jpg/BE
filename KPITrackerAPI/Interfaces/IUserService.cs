using KPITrackerAPI.DTOs.Admin;
using KPITrackerAPI.DTOs.User;
using KPITrackerAPI.Responses;

namespace KPITrackerAPI.Interfaces;

public interface IUserService
{
    Task<ApiResponse<List<UserDto>>> GetAllUsersAsync();
    Task<ApiResponse<string>> UpdateUserAsync(string id, string currentUserId, bool isAdmin, UpdateUserDto dto);
    Task<UserDto?> GetUserByIdAsync(string id);
}
