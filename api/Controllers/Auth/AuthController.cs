using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Data;
using Api.DTOs.Auth;
using Api.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Api.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly IConfiguration _config;

        public AuthController(DBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already exists.");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new UserResponseDto { Id = user.Id, Username = user.Username, Email = user.Email });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                return Unauthorized("Invalid credentials.");

            var token = GenerateToken(user);
            return Ok(new { token });
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users
                .Include(u => u.UserPermissions).ThenInclude(up => up.Permission)
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound();
            return Ok(new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Permissions = user.UserPermissions.Select(up => up.Permission.Name).Distinct().ToList()
            });
        }

        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var permissions = _context.UserPermissions
                .Where(up => up.UserId == user.Id)
                .Select(up => up.Permission.Name)
                .Distinct()
                .ToList();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };
            claims.AddRange(permissions.Select(p => new Claim("permission", p)));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
