using BookingSystem.Api.DTOs.User;
using BookingSystem.Api.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        // Injects user service into controller
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // Retrieves all users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        // Retrieves a single user by id
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDto>> GetUserById([FromRoute] int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // Creates a new user
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto dto)
        {
            var result = await _userService.CreateUserAsync(dto);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            return CreatedAtAction(
                nameof(GetUserById),
                new { id = result.Data!.Id },
                result.Data
            );
        }

        // Updates an existing user
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser([FromRoute] int id, UpdateUserDto dto)
        {
            var result = await _userService.UpdateUserAsync(id, dto);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            return NoContent();
        }

        // Deletes a user
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var result = await _userService.DeleteUserAsync(id);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            return NoContent();
        }
    }
}