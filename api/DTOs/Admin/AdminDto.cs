namespace Api.DTOs.Admin
{
    public class AdminUserResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public List<string> Permissions { get; set; } = new();
    }

    public class AdminCreateUserDto
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public List<int> PermissionIds { get; set; } = new();
    }

    public class AdminUpdateUserDto
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public List<int> PermissionIds { get; set; } = new();
    }

    public class PermissionResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Group { get; set; } = null!;
    }

    public class PermissionGroupDto
    {
        public string Group { get; set; } = null!;
        public List<PermissionResponseDto> Permissions { get; set; } = new();
    }
}
