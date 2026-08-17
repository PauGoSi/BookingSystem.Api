using BookingSystem.Api.Data;
using BookingSystem.Api.DTOs.User;
using BookingSystem.Api.Enums;
using BookingSystem.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Api.Services.Users
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        // Injects database context for user data access and password hasher
        public UserService(AppDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
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
                    Role = u.Role.Name == "Admin"
                        ? SystemRole.Admin
                        : SystemRole.User
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
                    Role = u.Role.Name == "Admin"
                        ? SystemRole.Admin
                        : SystemRole.User
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

            var normalizedEmail = NormalizeEmail(dto.Email);

            var emailExists = await _context.Users
                .AnyAsync(u => u.NormalizedEmail == normalizedEmail);

            if (emailExists)
            {
                return (false, "Email is already in use.", 409, null);
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                return (false, "Password is required.", 400, null);
            }

            if (string.IsNullOrWhiteSpace(dto.Role))
            {
                return (false, "Role is required.", 400, null);
            }

            var roleName = dto.Role.Trim();

            if (roleName != "Admin" && roleName != "User")
            {
                return (false, "Role must be either 'Admin' or 'User'.", 400, null);
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName);

            if (role == null)
            {
                return (false, "Role not found.", 500, null);
            }

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email.Trim(),
                NormalizedEmail = normalizedEmail,
                PasswordHash = string.Empty,
                RoleId = role.Id,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = Enum.Parse<SystemRole>(roleName)
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

            var normalizedEmail = NormalizeEmail(dto.Email);

            var emailExists = await _context.Users.AnyAsync(u =>
                u.Id != id &&
                u.NormalizedEmail == normalizedEmail);

            if (emailExists)
            {
                return (false, "Email is already in use.", 409);
            }

            if (string.IsNullOrWhiteSpace(dto.Role))
            {
                return (false, "Role is required.", 400);
            }

            var roleName = dto.Role.Trim();

            if (roleName != "Admin" && roleName != "User")
            {
                return (false, "Role must be either 'Admin' or 'User'.", 400);
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName);

            if (role == null)
            {
                return (false, "Role not found.", 500);
            }

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email.Trim();
            user.NormalizedEmail = normalizedEmail;
            user.RoleId = role.Id;

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

        private static string NormalizeEmail(string email)
        {
            return email.Trim().ToUpperInvariant();
        }
    }
}