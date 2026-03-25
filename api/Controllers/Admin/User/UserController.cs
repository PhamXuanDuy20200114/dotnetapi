using Api.Data;
using Api.DTOs.Admin;
using Api.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers.Admin.User
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly DBContext _context;
        public UserController(DBContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.UserPermissions).ThenInclude(up => up.Permission)
                .ToListAsync();
            return Ok(users.Select(u => new AdminUserResponseDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                DateOfBirth = u.DateOfBirth,
                Permissions = u.UserPermissions.Select(up => up.Permission.Name).ToList()
            }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserPermissions).ThenInclude(up => up.Permission)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();
            return Ok(new AdminUserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Permissions = user.UserPermissions.Select(up => up.Permission.Name).ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(AdminCreateUserDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already exists.");

            var user = new Models.User.User
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                PhoneNumber = dto.PhoneNumber,
                DateOfBirth = dto.DateOfBirth
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            foreach (var permId in dto.PermissionIds)
                _context.UserPermissions.Add(new UserPermission { UserId = user.Id, PermissionId = permId });
            await _context.SaveChangesAsync();

            return Ok(new AdminUserResponseDto { Id = user.Id, Username = user.Username, Email = user.Email });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, AdminUpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Username = dto.Username;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            user.DateOfBirth = dto.DateOfBirth;

            _context.UserPermissions.RemoveRange(_context.UserPermissions.Where(up => up.UserId == id));
            foreach (var permId in dto.PermissionIds)
                _context.UserPermissions.Add(new UserPermission { UserId = id, PermissionId = permId });

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
