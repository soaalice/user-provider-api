namespace UserProviderApi.Models.Dto;

public class UserActivityDto
{
    public int Id { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? IpAddress { get; set; }
    public string? DeviceInfo { get; set; }
}
