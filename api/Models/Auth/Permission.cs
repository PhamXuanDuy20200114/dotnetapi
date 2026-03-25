using System.ComponentModel.DataAnnotations;

namespace Api.Models.Auth
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Group { get; set; } = null!;

        public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    }
}
