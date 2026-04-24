using BookingSystem.Api.DTOs.Role;
using BookingSystem.Api.Services.Roles;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        // Injects role service into controller
        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // Retrieves all roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            var roles = await _roleService.GetRolesAsync();
            return Ok(roles);
        }

        // Retrieves a single role by id
        [HttpGet("{id:int}")]
        public async Task<ActionResult<RoleDto>> GetRoleById([FromRoute] int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            return Ok(role);
        }

        // Creates a new role
        [HttpPost]
        public async Task<ActionResult<RoleDto>> CreateRole(CreateRoleDto dto)
        {
            var result = await _roleService.CreateRoleAsync(dto);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            return CreatedAtAction(
                nameof(GetRoleById),
                new { id = result.Data!.Id },
                result.Data
            );
        }

        // Updates an existing role
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateRole([FromRoute] int id, UpdateRoleDto dto)
        {
            var result = await _roleService.UpdateRoleAsync(id, dto);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            return NoContent();
        }

        // Deletes a role
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRole([FromRoute] int id)
        {
            var result = await _roleService.DeleteRoleAsync(id);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            return NoContent();
        }
    }
}