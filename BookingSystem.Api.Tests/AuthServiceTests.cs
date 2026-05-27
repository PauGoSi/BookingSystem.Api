using BookingSystem.Api.Data;
using BookingSystem.Api.Models;
using BookingSystem.Api.Services.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BookingSystem.Api.Tests
{
    public class AuthServiceTests
    {
        // RegisterAsync tests
        [Fact]
        public async Task RegisterAsync_ShouldAssignUserRole_ToNewUsers()
        {
            // Arrange
            using var context = CreateDbContext();
            var configuration = CreateConfiguration();

            var role = new Role
            {
                Id = 1,
                Name = "User"
            };

            context.Roles.Add(role);
            await context.SaveChangesAsync();

            var service = new AuthService(context, configuration);

            var dto = new DTOs.Auth.RegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Password = "Password123!"
            };

            // Act
            var result = await service.RegisterAsync(dto);

            // Assert
            Assert.True(result.Success);

            var savedUser = await context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            Assert.NotNull(savedUser);

            Assert.Equal(1, savedUser!.RoleId);
            Assert.Equal("User", savedUser.Role.Name);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturn201_WhenUserIsValid()
        {
            // Arrange
            using var context = CreateDbContext();
            var configuration = CreateConfiguration();
            var role = CreateRole();

            context.Roles.Add(role);
            await context.SaveChangesAsync();

            var service = new AuthService(context, configuration);

            var dto = new DTOs.Auth.RegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Password = "Password123!"
            };

            // Act
            var result = await service.RegisterAsync(dto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(201, result.StatusCode);
            Assert.Null(result.Error);
            Assert.NotNull(result.Data);

            Assert.Equal("Test", result.Data!.FirstName);
            Assert.Equal("User", result.Data.LastName);
            Assert.Equal("test@example.com", result.Data.Email);

            var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");

            Assert.NotNull(savedUser);
            Assert.NotEqual("Password123!", savedUser!.PasswordHash);
            Assert.False(string.IsNullOrWhiteSpace(savedUser.PasswordHash));
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturn409_WhenEmailAlreadyExists()
        {
            // Arrange
            using var context = CreateDbContext();
            var configuration = CreateConfiguration();
            var role = CreateRole();
            var existingUser = CreateUser(email: "test@example.com");

            context.Roles.Add(role);
            context.Users.Add(existingUser);

            await context.SaveChangesAsync();

            var service = new AuthService(context, configuration);

            var dto = new DTOs.Auth.RegisterDto
            {
                FirstName = "New",
                LastName = "User",
                Email = "test@example.com",
                Password = "Password123!"
            };

            // Act
            var result = await service.RegisterAsync(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(409, result.StatusCode);
            Assert.Equal("Email is already in use.", result.Error);
            Assert.Null(result.Data);
        }

        // LoginAsync tests
        [Fact]
        public async Task LoginAsync_ShouldReturn401_WhenPasswordIsInvalid()
        {
            // Arrange
            using var context = CreateDbContext();
            var configuration = CreateConfiguration();
            var role = CreateRole();

            context.Roles.Add(role);
            await context.SaveChangesAsync();

            var service = new AuthService(context, configuration);

            var registerDto = new DTOs.Auth.RegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Password = "CorrectPassword123!"
            };

            await service.RegisterAsync(registerDto);

            var loginDto = new DTOs.Auth.LoginDto
            {
                Email = "test@example.com",
                Password = "WrongPassword123!"
            };

            // Act
            var result = await service.LoginAsync(loginDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(401, result.StatusCode);
            Assert.Equal("Invalid email or password.", result.Error);
            Assert.Null(result.Token);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            using var context = CreateDbContext();
            var configuration = CreateConfiguration();
            var role = CreateRole();

            context.Roles.Add(role);
            await context.SaveChangesAsync();

            var service = new AuthService(context, configuration);

            const string password = "CorrectPassword123!";

            var registerDto = new DTOs.Auth.RegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Password = password
            };

            var registerResult = await service.RegisterAsync(registerDto);

            Assert.True(registerResult.Success, registerResult.Error);

            var loginDto = new DTOs.Auth.LoginDto
            {
                Email = "test@example.com",
                Password = password
            };

            // Act
            var result = await service.LoginAsync(loginDto);

            // Assert
            Assert.True(result.Success, result.Error);
            Assert.Equal(200, result.StatusCode);
            Assert.Null(result.Error);

            Assert.NotNull(result.Token);
            Assert.False(string.IsNullOrWhiteSpace(result.Token));
        }

        [Fact]
        public async Task LoginAsync_ShouldReturn401_WhenEmailDoesNotExist()
        {
            // Arrange
            using var context = CreateDbContext();
            var configuration = CreateConfiguration();

            var service = new AuthService(context, configuration);

            var loginDto = new DTOs.Auth.LoginDto
            {
                Email = "unknown@example.com",
                Password = "Password123!"
            };

            // Act
            var result = await service.LoginAsync(loginDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(401, result.StatusCode);
            Assert.Equal("Invalid email or password.", result.Error);
            Assert.Null(result.Token);
        }

        // Helpers
        private static Role CreateRole()
        {
            return new Role
            {
                Id = 1,
                Name = "User"
            };
        }

        private static User CreateUser(
            string email = "john@example.com",
            string passwordHash = "hashed-password",
            int id = 1,
            int roleId = 1)
        {
            return new User
            {
                Id = id,
                FirstName = "John",
                LastName = "Doe",
                Email = email,
                PasswordHash = passwordHash,
                RoleId = roleId,
                CreatedAt = DateTime.UtcNow
            };
        }

        private static AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        private static IConfiguration CreateConfiguration()
        {
            var settings = new Dictionary<string, string?>
            {
                { "Jwt:Key", "THIS_IS_A_TEMPORARY_DEVELOPMENT_KEY_CHANGE_LATER" },
                { "Jwt:Issuer", "BookingSystem.Api" },
                { "Jwt:Audience", "BookingSystem.Client" },
                { "Jwt:ExpiresInMinutes", "60" }
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();
        }
    }
}