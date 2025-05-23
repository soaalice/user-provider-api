using Microsoft.EntityFrameworkCore;
using UserProviderApi.Models;
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

    public async Task<(User? user, string? error)> RegisterAsync(string username, string email, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Username == username))
            return (null, "Username already exists");

        if (await _context.Users.AnyAsync(u => u.Email == email))
            return (null, "Email already exists");

        var user = new User
        {
            Username = username,
            Email = email,
            Password = PasswordHelper.HashPassword(password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return (user, null);
    }

    public async Task<(User? user, string token, string? error)> LoginAsync(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        
        if (user == null)
            return (null, string.Empty, "Invalid credentials");

        if (!PasswordHelper.VerifyPassword(password, user.Password))
            return (null, string.Empty, "Invalid credentials");

        var token = _jwtTokenService.GenerateToken(user);
        return (user, token, null);
    }
}