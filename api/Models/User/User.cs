using System.ComponentModel.DataAnnotations;
using Api.Models.Auth;

namespace Api.Models.User
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public DateOnly? DateOfBirth { get; set; }

        public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    }
}
