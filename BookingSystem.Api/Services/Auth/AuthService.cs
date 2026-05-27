using BookingSystem.Api.Data;
using BookingSystem.Api.DTOs.Auth;
using BookingSystem.Api.DTOs.User;
using BookingSystem.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookingSystem.Api.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        // Injects database context and password hasher
        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
        }

        // Registers a new user with hashed password
        public async Task<(bool Success, string? Error, int StatusCode, UserDto? Data)> RegisterAsync(RegisterDto dto)
        {
            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");

            if (userRole == null)
            {
                return (false, "User role not found.", 500, null);
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                return (false, "Email is required.", 400, null);
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                return (false, "Password is required.", 400, null);
            }

            var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (emailExists)
            {
                return (false, "Email is already in use.", 409, null);
            }

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = string.Empty,
                RoleId = userRole.Id
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
                RoleId = user.RoleId
            };

            return (true, null, 201, result);
        }

        // Authenticates a user by validating email and password
        public async Task<(bool Success, string? Error, int StatusCode, string? Token)> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
            {
                return (false, "Invalid email or password.", 401, null);
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return (false, "Invalid email or password.", 401, null);
            }

            // JWT token generation will be added in the next step
            var token = GenerateJwtToken(user);

            return (true, null, 200, token);
        }

        // Generates a JWT token for an authenticated user
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(jwtSettings["ExpiresInMinutes"]!)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}