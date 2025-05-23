using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserProviderApi.Models;
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
    public async Task<ActionResult<ApiResponse<User?>>> Register([FromBody] RegisterRequest request)
    {
        var (user, error) = await _userService.RegisterAsync(request.Username, request.Email, request.Password);

        if (error != null)
        {
            return BadRequest(new ApiResponse<User?>
            {
                Status = "error",
                Message = error,
                Datas = default
            });
        }

        return Ok(new ApiResponse<User?>
        {
            Status = "success",
            Message = "Registration successful",
            Datas = user
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<object?>>> Login([FromBody] LoginRequest request)
    {
        var (user, token, error) = await _userService.LoginAsync(request.Username, request.Password);

        if (error != null)
        {
            return BadRequest(new ApiResponse<object?>
            {
                Status = "error",
                Message = error,
                Datas = default
            });
        }

        return Ok(new ApiResponse<object?>
        {
            Status = "success",
            Message = "Login successful",
            Datas = new { user, token }
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