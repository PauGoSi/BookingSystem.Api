using BookingSystem.Api.DTOs.Auth;
using BookingSystem.Api.DTOs.User;

namespace BookingSystem.Api.Services.Auth
{
    public interface IAuthService
    {
        // Registers a new user
        Task<(bool Success, string? Error, int StatusCode, UserDto? Data)> RegisterAsync(RegisterDto dto);

        // Authenticates a user and returns JWT token
        Task<(bool Success, string? Error, int StatusCode, string? Token)> LoginAsync(LoginDto dto);
    }
}