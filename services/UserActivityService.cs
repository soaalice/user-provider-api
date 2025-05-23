using Microsoft.AspNetCore.Http;
using UserProviderApi.Models;

namespace UserProviderApi.Services;

public class UserActivityService
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserActivityService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogActivityAsync(
        int userId,
        string activityType,
        string? oldValue = null,
        string? newValue = null)
    {
        var context = _httpContextAccessor.HttpContext;
        var activity = new UserActivity
        {
            UserId = userId,
            ActivityType = activityType,
            OldValue = oldValue,
            NewValue = newValue,
            IpAddress = context?.Connection.RemoteIpAddress?.ToString(),
            DeviceInfo = context?.Request.Headers.UserAgent.ToString(),
            CreatedAt = DateTime.UtcNow
        };

        _context.UserActivities.Add(activity);
        await _context.SaveChangesAsync();
    }
}
