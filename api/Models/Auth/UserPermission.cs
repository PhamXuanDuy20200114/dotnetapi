namespace Api.Models.Auth
{
    public class UserPermission
    {
        public int UserId { get; set; }
        public int PermissionId { get; set; }

        public global::Api.Models.User.User User { get; set; } = null!;
        public Permission Permission { get; set; } = null!;
    }
}
