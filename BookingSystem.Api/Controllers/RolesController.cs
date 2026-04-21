using BookingSystem.Api.Data;
using BookingSystem.Api.DTOs.Role;
using BookingSystem.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RolesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            var roles = await _context.Roles
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .ToListAsync();

            return Ok(roles);
        }

        [HttpPost]
        public async Task<ActionResult<RoleDto>> CreateRole(CreateRoleDto dto)
        {
            var role = new Role
            {
                Name = dto.Name
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            var result = new RoleDto
            {
                Id = role.Id,
                Name = role.Name
            };

            return CreatedAtAction(nameof(GetRoles), new { id = result.Id }, result);
        }
    }
}