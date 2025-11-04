using UserService.Application.DTOs;

namespace UserService.Application.Services;

public interface IUserService
{
    Task<ApiResponse<UserDto>> GetUserByIdAsync(int id);
    Task<ApiResponse<IEnumerable<UserDto>>> GetAllUsersAsync();
    Task<ApiResponse<UserDto>> UpdateUserAsync(int id, UpdateUserDto dto);
    Task<ApiResponse<bool>> DeleteUserAsync(int id);
    Task<ApiResponse<bool>> AssignRoleAsync(AssignRoleDto dto);
    Task<ApiResponse<bool>> RemoveRoleAsync(int userId, int roleId);
}
