using BookingSystem.Api.DTOs.Role;

namespace BookingSystem.Api.Services.Roles
{
    public interface IRoleService
    {
        // Retrieves all roles
        Task<IEnumerable<RoleDto>> GetRolesAsync();

        // Retrieves a single role by id
        Task<RoleDto?> GetRoleByIdAsync(int id);
    }
}