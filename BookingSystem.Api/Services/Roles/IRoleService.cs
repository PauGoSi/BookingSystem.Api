using BookingSystem.Api.DTOs.Role;

namespace BookingSystem.Api.Services.Roles
{
    public interface IRoleService
    {
        // Retrieves all roles
        Task<IEnumerable<RoleDto>> GetRolesAsync();

        // Retrieves a single role by id
        Task<RoleDto?> GetRoleByIdAsync(int id);

        // Creates a new role
        Task<(bool Success, string? Error, int StatusCode, RoleDto? Data)> CreateRoleAsync(CreateRoleDto dto);

        // Updates an existing role
        Task<(bool Success, string? Error, int StatusCode)> UpdateRoleAsync(int id, UpdateRoleDto dto);

        // Deletes a role by id
        Task<(bool Success, string? Error, int StatusCode)> DeleteRoleAsync(int id);
    }
}