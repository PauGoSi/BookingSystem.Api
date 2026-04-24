using BookingSystem.Api.Data;
using BookingSystem.Api.DTOs.User;
using BookingSystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Api.Services.Users
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        // Injects database context for user data access
        public UserService(AppDbContext context)
        {
            _context = context;
        }

        // Retrieves all users as DTOs
        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            return await _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    RoleId = u.RoleId
                })
                .ToListAsync();
        }

        // Retrieves a single user by id
        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    RoleId = u.RoleId
                })
                .FirstOrDefaultAsync();
        }

        // Creates a new user with validation and returns result
        public async Task<(bool Success, string? Error, int StatusCode, UserDto? Data)> CreateUserAsync(CreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName))
            {
                return (false, "First name is required.", 400, null);
            }

            if (string.IsNullOrWhiteSpace(dto.LastName))
            {
                return (false, "Last name is required.", 400, null);
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                return (false, "Email is required.", 400, null);
            }

            var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (emailExists)
            {
                return (false, "Email is already in use.", 409, null);
            }

            var roleExists = await _context.Roles.AnyAsync(r => r.Id == dto.RoleId);
            if (!roleExists)
            {
                return (false, "Role not found.", 404, null);
            }

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = dto.PasswordHash,
                RoleId = dto.RoleId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                RoleId = user.RoleId
            };

            return (true, null, 201, result);
        }

        // Updates an existing user with validation
        public async Task<(bool Success, string? Error, int StatusCode)> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return (false, "User not found.", 404);
            }

            if (string.IsNullOrWhiteSpace(dto.FirstName))
            {
                return (false, "First name is required.", 400);
            }

            if (string.IsNullOrWhiteSpace(dto.LastName))
            {
                return (false, "Last name is required.", 400);
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                return (false, "Email is required.", 400);
            }

            var emailExists = await _context.Users.AnyAsync(u =>
                u.Id != id &&
                u.Email == dto.Email);

            if (emailExists)
            {
                return (false, "Email is already in use.", 409);
            }

            var roleExists = await _context.Roles.AnyAsync(r => r.Id == dto.RoleId);
            if (!roleExists)
            {
                return (false, "Role not found.", 404);
            }

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.RoleId = dto.RoleId;

            await _context.SaveChangesAsync();

            return (true, null, 204);
        }

        // Deletes a user by id
        public async Task<(bool Success, string? Error, int StatusCode)> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return (false, "User not found.", 404);
            }

            var hasBookings = await _context.Bookings.AnyAsync(b => b.UserId == id);

            if (hasBookings)
            {
                return (false, "User cannot be deleted because they have bookings.", 409);
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return (true, null, 204);
        }
    }
}