using Microsoft.EntityFrameworkCore;
using UserProviderApi.Exceptions;
using UserProviderApi.Models;
using UserProviderApi.Models.Dto;
using UserProviderApi.Utils;

namespace UserProviderApi.Services;

public class UserService
{
    private readonly AppDbContext _context;
    private readonly JwtTokenService _jwtTokenService;
    private readonly UserActivityService _activityService;

    public UserService(AppDbContext context, JwtTokenService jwtTokenService, UserActivityService activityService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _activityService = activityService;
    }

    public async Task<(UserDto? user, string? error)> RegisterAsync(string username, string email, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Username == username))
            throw new ApiException("Username already exists", 400);

        if (await _context.Users.AnyAsync(u => u.Email == email))
            throw new ApiException("Email already exists", 400);

        var user = new User
        {
            Username = username,
            Email = email,
            Password = PasswordHelper.HashPassword(password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        await _activityService.LogActivityAsync(user.Id, "register");

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };

        return (userDto, null);
    }

    public async Task<(UserDto? user, string? error)> LoginAsync(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        
        if (user == null)
            throw new ApiException("Invalid credentials", 401);

        if (!PasswordHelper.VerifyPassword(password, user.Password))
            throw new ApiException("Invalid credentials", 401);

        var token = _jwtTokenService.GenerateToken(user);

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            Token = token
        };

        await _activityService.LogActivityAsync(user.Id, "login");

        return (userDto, null);
    }

    public async Task<(UserDto? user, string? error)> UpdateAsync(int userId, string? username, string? email, string? password)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new ApiException("User not found", 404);

        if (username != null && username != user.Username)
        {
            if (await _context.Users.AnyAsync(u => u.Username == username && u.Id != userId))
                throw new ApiException("Username already exists", 400);
            await _activityService.LogActivityAsync(userId, "update_username", user.Username, username);
            user.Username = username;
        }

        if (email != null && email != user.Email)
        {
            if (await _context.Users.AnyAsync(u => u.Email == email && u.Id != userId))
                throw new ApiException("Email already exists", 400);
            await _activityService.LogActivityAsync(userId, "update_email", user.Email, email);
            user.Email = email;
        }

        if (password != null)
        {
            user.Password = PasswordHelper.HashPassword(password);
            await _activityService.LogActivityAsync(userId, "update_password");
        }

        await _context.SaveChangesAsync();

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };

        return (userDto, null);
    }

    public async Task<string?> DeleteAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new ApiException("User not found", 404);

        await _activityService.LogActivityAsync(userId, "delete");
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return null;
    }
}