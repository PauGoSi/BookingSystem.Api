using BookingSystem.Api.Data;
using BookingSystem.Api.DTOs.Role;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Api.Services.Roles
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _context;

        // Injects database context for role data access
        public RoleService(AppDbContext context)
        {
            _context = context;
        }

        // Retrieves all roles as DTOs
        public async Task<IEnumerable<RoleDto>> GetRolesAsync()
        {
            return await _context.Roles
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .ToListAsync();
        }

        // Retrieves a single role by id
        public async Task<RoleDto?> GetRoleByIdAsync(int id)
        {
            return await _context.Roles
                .Where(r => r.Id == id)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .FirstOrDefaultAsync();
        }
    }
}