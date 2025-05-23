using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserProviderApi.Models
{
    [Table("Users")]
    public class User
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("username")]
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public required string Username { get; set; }

        [Column("password")]
        [Required]
        public required string Password { get; set; }

        [Column("email")]
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
