using BookingSystem.Api.DTOs.Auth;
using BookingSystem.Api.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        // Injects auth service into controller
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // Registers a new user
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            return Created(string.Empty, result.Data);
        }

        // Authenticates a user
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            return Ok(new { token = result.Token });
        }
    }
}