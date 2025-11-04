using UserService.Application.DTOs;

namespace UserService.Application.Services;

public interface IAuthService
{
    Task<ApiResponse<LoginResponseDto>> RegisterAsync(RegisterRequestDto request);
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    string GenerateJwtToken(int userId, string email, List<string> roles);
}
