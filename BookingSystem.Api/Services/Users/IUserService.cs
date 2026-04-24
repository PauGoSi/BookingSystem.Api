using BookingSystem.Api.DTOs.User;

namespace BookingSystem.Api.Services.Users
{
    public interface IUserService
    {
        // Retrieves all users
        Task<IEnumerable<UserDto>> GetUsersAsync();

        // Retrieves a single user by id
        Task<UserDto?> GetUserByIdAsync(int id);

        // Creates a new user
        Task<(bool Success, string? Error, int StatusCode, UserDto? Data)> CreateUserAsync(CreateUserDto dto);

        // Updates an existing user
        Task<(bool Success, string? Error, int StatusCode)> UpdateUserAsync(int id, UpdateUserDto dto);

        // Deletes a user by id
        Task<(bool Success, string? Error, int StatusCode)> DeleteUserAsync(int id);
    }
}