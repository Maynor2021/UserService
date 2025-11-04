using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs;
using UserService.Application.Services;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllUsers()
    {
        var response = await _userService.GetAllUsersAsync();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var response = await _userService.GetUserByIdAsync(id);
        
        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _userService.UpdateUserAsync(id, dto);
        
        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var response = await _userService.DeleteUserAsync(id);
        
        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpPost("assign-role")]
    [AllowAnonymous]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _userService.AssignRoleAsync(dto);
        
        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpDelete("{userId}/roles/{roleId}")]
    [AllowAnonymous]
    public async Task<IActionResult> RemoveRole(int userId, int roleId)
    {
        var response = await _userService.RemoveRoleAsync(userId, roleId);
        
        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }
}
