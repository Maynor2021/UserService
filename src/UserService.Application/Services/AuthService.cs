using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UserService.Application.DTOs;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using BCrypt.Net;

namespace UserService.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUserRepository userRepository, 
        IRoleRepository roleRepository,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _configuration = configuration;
    }

    public async Task<ApiResponse<LoginResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        try
        {
            // Validar si el usuario ya existe
            var exists = await _userRepository.ExistsAsync(request.Email, request.Username);
            if (exists)
            {
                return new ApiResponse<LoginResponseDto>
                {
                    Success = false,
                    Message = "El email o nombre de usuario ya está registrado"
                };
            }

            // Crear el usuario
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FullName = request.FullName,
                CreatedAt = DateTime.UtcNow,
                IsEnabled = true
            };

            Console.WriteLine($"📝 Creando usuario: {user.Username} - {user.Email}");
            var createdUser = await _userRepository.CreateAsync(user);
            Console.WriteLine($"✅ Usuario creado con ID: {createdUser.Id}");

            // Asignar rol por defecto (Estudiante)
            Console.WriteLine($"🔐 Asignando rol Estudiante (ID: 3) al usuario {createdUser.Id}");
            var roleAssigned = await _roleRepository.AssignRoleToUserAsync(createdUser.Id, 3); // ID 3 = Estudiante
            Console.WriteLine($"✅ Rol asignado: {roleAssigned}");

            // Obtener usuario con roles
            var userWithRoles = await _userRepository.GetByIdAsync(createdUser.Id);
            if (userWithRoles == null)
            {
                Console.WriteLine($"❌ ERROR: No se pudo obtener el usuario recién creado con ID: {createdUser.Id}");
                return new ApiResponse<LoginResponseDto>
                {
                    Success = false,
                    Message = "Error al recuperar el usuario después de crearlo"
                };
            }
            
            var roles = userWithRoles.UserRoles.Select(ur => ur.Role.Name).ToList();
            Console.WriteLine($"✅ Usuario obtenido con {roles.Count} roles: {string.Join(", ", roles)}");

            // Generar token
            var token = GenerateJwtToken(createdUser.Id, createdUser.Email, roles);

            return new ApiResponse<LoginResponseDto>
            {
                Success = true,
                Message = "Usuario registrado exitosamente",
                Data = new LoginResponseDto
                {
                    Token = token,
                    User = MapToUserDto(userWithRoles)
                }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR al registrar usuario: {ex.Message}");
            Console.WriteLine($"❌ StackTrace: {ex.StackTrace}");
            return new ApiResponse<LoginResponseDto>
            {
                Success = false,
                Message = $"Error al registrar usuario: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        // Buscar usuario por email o username
        User? user = null;
        
        if (request.EmailOrUsername.Contains("@"))
        {
            user = await _userRepository.GetByEmailAsync(request.EmailOrUsername);
        }
        else
        {
            user = await _userRepository.GetByUsernameAsync(request.EmailOrUsername);
        }

        if (user == null)
        {
            return new ApiResponse<LoginResponseDto>
            {
                Success = false,
                Message = "Credenciales inválidas"
            };
        }

        // Verificar contraseña
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return new ApiResponse<LoginResponseDto>
            {
                Success = false,
                Message = "Credenciales inválidas"
            };
        }

        // Verificar si el usuario está habilitado
        if (!user.IsEnabled)
        {
            return new ApiResponse<LoginResponseDto>
            {
                Success = false,
                Message = "Usuario deshabilitado"
            };
        }

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var token = GenerateJwtToken(user.Id, user.Email, roles);

        return new ApiResponse<LoginResponseDto>
        {
            Success = true,
            Message = "Login exitoso",
            Data = new LoginResponseDto
            {
                Token = token,
                User = MapToUserDto(user)
            }
        };
    }

    public string GenerateJwtToken(int userId, string email, List<string> roles)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "DefaultSecretKeyForDevelopment123456789"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Agregar roles como claims
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "UserServiceAPI",
            audience: _configuration["Jwt:Audience"] ?? "UserServiceClient",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            CreatedAt = user.CreatedAt,
            IsEnabled = user.IsEnabled,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        };
    }
}
