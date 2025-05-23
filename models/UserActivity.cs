using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserProviderApi.Models
{
    [Table("UserActivity")]
    public class UserActivity
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        [Required]
        public int UserId { get; set; }

        [Column("activity_type")]
        [Required]
        [StringLength(50)]
        public string ActivityType { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("old_value")]
        public string? OldValue { get; set; }

        [Column("new_value")]
        public string? NewValue { get; set; }

        [Column("ip_address")]
        [StringLength(45)]
        public string? IpAddress { get; set; }

        [Column("device_info")]
        public string? DeviceInfo { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}

