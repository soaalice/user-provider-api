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

    public UserService(AppDbContext context, JwtTokenService jwtTokenService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
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

        return (userDto, null);
    }
}