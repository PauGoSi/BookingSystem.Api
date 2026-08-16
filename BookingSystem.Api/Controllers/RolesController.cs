using BookingSystem.Api.DTOs.Role;
using BookingSystem.Api.Services.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/roles")]
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
    }
}