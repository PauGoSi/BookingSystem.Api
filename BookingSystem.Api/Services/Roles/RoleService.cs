using BookingSystem.Api.Data;
using BookingSystem.Api.DTOs.Role;
using BookingSystem.Api.Models;
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

        // Creates a new role with validation and returns result
        public async Task<(bool Success, string? Error, int StatusCode, RoleDto? Data)> CreateRoleAsync(CreateRoleDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return (false, "Role name is required.", 400, null);
            }

            var roleExists = await _context.Roles.AnyAsync(r => r.Name == dto.Name);
            if (roleExists)
            {
                return (false, "Role name is already in use.", 409, null);
            }

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

            return (true, null, 201, result);
        }

        // Updates an existing role with validation
        public async Task<(bool Success, string? Error, int StatusCode)> UpdateRoleAsync(int id, UpdateRoleDto dto)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return (false, "Role not found.", 404);
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return (false, "Role name is required.", 400);
            }

            var roleExists = await _context.Roles.AnyAsync(r =>
                r.Id != id &&
                r.Name == dto.Name);

            if (roleExists)
            {
                return (false, "Role name is already in use.", 409);
            }

            role.Name = dto.Name;

            await _context.SaveChangesAsync();

            return (true, null, 204);
        }

        // Deletes a role by id
        public async Task<(bool Success, string? Error, int StatusCode)> DeleteRoleAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return (false, "Role not found.", 404);
            }

            var hasUsers = await _context.Users.AnyAsync(u => u.RoleId == id);

            if (hasUsers)
            {
                return (false, "Role cannot be deleted because it is assigned to users.", 409);
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return (true, null, 204);
        }
    }
}