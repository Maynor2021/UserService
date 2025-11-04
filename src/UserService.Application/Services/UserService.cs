using UserService.Application.DTOs;
using UserService.Domain.Interfaces;

namespace UserService.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<ApiResponse<UserDto>> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
        {
            return new ApiResponse<UserDto>
            {
                Success = false,
                Message = "Usuario no encontrado"
            };
        }

        return new ApiResponse<UserDto>
        {
            Success = true,
            Message = "Usuario obtenido exitosamente",
            Data = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                CreatedAt = user.CreatedAt,
                IsEnabled = user.IsEnabled,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            }
        };
    }

    public async Task<ApiResponse<IEnumerable<UserDto>>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        
        var userDtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            FullName = u.FullName,
            CreatedAt = u.CreatedAt,
            IsEnabled = u.IsEnabled,
            Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
        });

        return new ApiResponse<IEnumerable<UserDto>>
        {
            Success = true,
            Message = "Usuarios obtenidos exitosamente",
            Data = userDtos
        };
    }

    public async Task<ApiResponse<UserDto>> UpdateUserAsync(int id, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
        {
            return new ApiResponse<UserDto>
            {
                Success = false,
                Message = "Usuario no encontrado"
            };
        }

        if (!string.IsNullOrEmpty(dto.FullName))
            user.FullName = dto.FullName;
        
        if (!string.IsNullOrEmpty(dto.Email))
            user.Email = dto.Email;

        var updatedUser = await _userRepository.UpdateAsync(user);

        return new ApiResponse<UserDto>
        {
            Success = true,
            Message = "Usuario actualizado exitosamente",
            Data = new UserDto
            {
                Id = updatedUser.Id,
                Username = updatedUser.Username,
                Email = updatedUser.Email,
                FullName = updatedUser.FullName,
                CreatedAt = updatedUser.CreatedAt,
                IsEnabled = updatedUser.IsEnabled,
                Roles = updatedUser.UserRoles.Select(ur => ur.Role.Name).ToList()
            }
        };
    }

    public async Task<ApiResponse<bool>> DeleteUserAsync(int id)
    {
        var result = await _userRepository.DeleteAsync(id);
        
        if (!result)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Usuario no encontrado"
            };
        }

        return new ApiResponse<bool>
        {
            Success = true,
            Message = "Usuario eliminado exitosamente",
            Data = true
        };
    }

    public async Task<ApiResponse<bool>> AssignRoleAsync(AssignRoleDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.UserId);
        if (user == null)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Usuario no encontrado"
            };
        }

        var role = await _roleRepository.GetByIdAsync(dto.RoleId);
        if (role == null)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Rol no encontrado"
            };
        }

        var result = await _roleRepository.AssignRoleToUserAsync(dto.UserId, dto.RoleId);
        
        if (!result)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "El usuario ya tiene este rol asignado"
            };
        }

        return new ApiResponse<bool>
        {
            Success = true,
            Message = "Rol asignado exitosamente",
            Data = true
        };
    }

    public async Task<ApiResponse<bool>> RemoveRoleAsync(int userId, int roleId)
    {
        var result = await _roleRepository.RemoveRoleFromUserAsync(userId, roleId);
        
        if (!result)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "El usuario no tiene este rol asignado"
            };
        }

        return new ApiResponse<bool>
        {
            Success = true,
            Message = "Rol removido exitosamente",
            Data = true
        };
    }
}
