using Api.Data;
using Api.DTOs.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers.Admin.Permission
{
    [ApiController]
    [Authorize]
    public class PermissionController : ControllerBase
    {
        private readonly DBContext _context;
        public PermissionController(DBContext context) => _context = context;

        [HttpGet("api/admin/permissions")]
        public async Task<IActionResult> GetPermissions()
        {
            var groups = await _context.Permissions
                .GroupBy(p => p.Group)
                .Select(g => new PermissionGroupDto
                {
                    Group = g.Key,
                    Permissions = g
                    .OrderBy(p => p.Id)
                    .Select(p => new PermissionResponseDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Group = p.Group
                    }).ToList()
                })
                .ToListAsync();
            return Ok(groups);
        }
    }
}
