using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserProviderApi.Models;
using UserProviderApi.Models.Dto;
using UserProviderApi.Services;

namespace UserProviderApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    public class RegisterRequest
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class LoginRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<UserDto?>>> Register([FromBody] RegisterRequest request)
    {
        var (user, error) = await _userService.RegisterAsync(request.Username, request.Email, request.Password);

        if (error != null)
        {
            return BadRequest(new ApiResponse<UserDto?>
            {
                Status = "error",
                Message = error,
                Datas = default
            });
        }

        return Ok(new ApiResponse<UserDto?>
        {
            Status = "success",
            Message = "Registration successful",
            Datas = user
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<UserDto?>>> Login([FromBody] LoginRequest request)
    {
        var (user, error) = await _userService.LoginAsync(request.Username, request.Password);

        if (error != null)
        {
            return BadRequest(new ApiResponse<UserDto?>
            {
                Status = "error",
                Message = error,
                Datas = default
            });
        }

        return Ok(new ApiResponse<UserDto?>
        {
            Status = "success",
            Message = "Login successful",
            Datas = user
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public ActionResult<ApiResponse<object?>> Logout()
    {
        return Ok(new ApiResponse<object?>
        {
            Status = "success",
            Message = "Logout successful",
            Datas = default
        });
    }
}