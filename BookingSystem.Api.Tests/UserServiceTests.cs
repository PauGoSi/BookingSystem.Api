using BookingSystem.Api.Data;
using BookingSystem.Api.Models;
using BookingSystem.Api.Services.Users;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Api.Tests
{
    public class UserServiceTests
    {
        // CreateUserAsync tests
        [Fact]
        public async Task CreateUserAsync_ShouldReturn409_WhenEmailAlreadyExists()
        {
            // Arrange
            using var context = CreateDbContext();
            var role = CreateRole();
            var existingUser = CreateUser(email: "test@example.com");

            context.Roles.Add(role);
            context.Users.Add(existingUser);

            await context.SaveChangesAsync();

            var service = new UserService(context);

            var dto = new DTOs.User.CreateUserDto
            {
                FirstName = "New",
                LastName = "User",
                Email = "test@example.com",
                Password = "Password123!",
                RoleId = 1
            };

            // Act
            var result = await service.CreateUserAsync(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(409, result.StatusCode);
            Assert.Equal("Email is already in use.", result.Error);
            Assert.Null(result.Data);
            Assert.Equal(1, await context.Users.CountAsync());
        }

        [Fact]
        public async Task CreateUserAsync_ShouldReturn201_WhenUserIsValid()
        {
            // Arrange
            using var context = CreateDbContext();
            var role = CreateRole();

            context.Roles.Add(role);

            await context.SaveChangesAsync();

            var service = new UserService(context);

            var dto = new DTOs.User.CreateUserDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "Password123!",
                RoleId = 1
            };

            // Act
            var result = await service.CreateUserAsync(dto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(201, result.StatusCode);
            Assert.NotNull(result.Data);

            Assert.Equal("John", result.Data.FirstName);
            Assert.Equal("Doe", result.Data.LastName);
            Assert.Equal("john@example.com", result.Data.Email);
            Assert.Equal(1, result.Data.RoleId);

            var createdUser = await context.Users.FirstAsync();

            Assert.NotEqual("Password123!", createdUser.PasswordHash);
            Assert.False(string.IsNullOrWhiteSpace(createdUser.PasswordHash));
        }

        // UpdateUserAsync tests
        [Fact]
        public async Task UpdateUserAsync_ShouldReturn404_WhenUserDoesNotExist()
        {
            // Arrange
            using var context = CreateDbContext();

            var service = new UserService(context);

            var dto = new DTOs.User.UpdateUserDto
            {
                FirstName = "Updated",
                LastName = "User",
                Email = "updated@example.com",
                RoleId = 1
            };

            // Act
            var result = await service.UpdateUserAsync(
                id: 999,
                dto: dto
            );

            // Assert
            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("User not found.", result.Error);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldReturn409_WhenEmailAlreadyExists()
        {
            // Arrange
            using var context = CreateDbContext();
            var role = CreateRole();
            var user1 = CreateUser(id: 1, email: "john@example.com", passwordHash: "hashed-password");
            var user2 = CreateUser(id: 2, email: "jane@example.com", passwordHash: "hashed -password");

            context.Roles.Add(role);

            context.Users.AddRange(user1, user2);

            await context.SaveChangesAsync();

            var service = new UserService(context);

            var dto = new DTOs.User.UpdateUserDto
            {
                FirstName = "Updated",
                LastName = "User",
                Email = "john@example.com",
                RoleId = 1
            };

            // Act
            var result = await service.UpdateUserAsync(
                id: 2,
                dto: dto
            );

            // Assert
            Assert.False(result.Success);
            Assert.Equal(409, result.StatusCode);
            Assert.Equal("Email is already in use.", result.Error);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldNormalizeEmail_WhenEmailIsUpdated()
        {
            // Arrange
            using var context = CreateDbContext();
            var role = CreateRole();
            var user = CreateUser(email: "old@example.com");

            context.Roles.Add(role);
            context.Users.Add(user);

            await context.SaveChangesAsync();

            var service = new UserService(context);

            var dto = new DTOs.User.UpdateUserDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "  New.Email@Example.COM  ",
                RoleId = 1
            };

            // Act
            var result = await service.UpdateUserAsync(
                id: 1,
                dto: dto
            );

            // Assert
            Assert.True(result.Success);
            Assert.Equal(204, result.StatusCode);

            var updatedUser = await context.Users.FindAsync(1);

            Assert.NotNull(updatedUser);
            Assert.Equal("New.Email@Example.COM", updatedUser!.Email);
            Assert.Equal("NEW.EMAIL@EXAMPLE.COM", updatedUser.NormalizedEmail);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldReturn409_WhenNormalizedEmailAlreadyExists()
        {
            // Arrange
            using var context = CreateDbContext();
            var role = CreateRole();

            var user1 = CreateUser(
                id: 1,
                email: "john@example.com");

            var user2 = CreateUser(
                id: 2,
                email: "jane@example.com");

            context.Roles.Add(role);
            context.Users.AddRange(user1, user2);

            await context.SaveChangesAsync();

            var service = new UserService(context);

            var dto = new DTOs.User.UpdateUserDto
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "  JOHN@EXAMPLE.COM  ",
                RoleId = 1
            };

            // Act
            var result = await service.UpdateUserAsync(
                id: 2,
                dto: dto
            );

            // Assert
            Assert.False(result.Success);
            Assert.Equal(409, result.StatusCode);
            Assert.Equal("Email is already in use.", result.Error);

            var unchangedUser = await context.Users.FindAsync(2);

            Assert.NotNull(unchangedUser);
            Assert.Equal("jane@example.com", unchangedUser!.Email);
            Assert.Equal("JANE@EXAMPLE.COM", unchangedUser.NormalizedEmail);
        }

        // DeleteUserAsync tests
        [Fact]
        public async Task DeleteUserAsync_ShouldReturn404_WhenUserDoesNotExist()
        {
            // Arrange
            using var context = CreateDbContext();

            var service = new UserService(context);

            // Act
            var result = await service.DeleteUserAsync(999);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("User not found.", result.Error);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldReturn409_WhenUserHasBookings()
        {
            // Arrange
            using var context = CreateDbContext();
            var role = CreateRole();
            var user = CreateUser();

            var resource = new Resource
            {
                Id = 1,
                Name = "Meeting Room",
                Description = "Room",
                Location = "Office",
                Capacity = 10,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var booking = new Booking
            {
                Id = 1,
                UserId = 1,
                ResourceId = 1,
                StartTime = DateTime.UtcNow.AddHours(1),
                EndTime = DateTime.UtcNow.AddHours(2),
                CreatedAt = DateTime.UtcNow
            };

            context.Roles.Add(role);
            context.Users.Add(user);
            context.Resources.Add(resource);
            context.Bookings.Add(booking);

            await context.SaveChangesAsync();

            var service = new UserService(context);

            // Act
            var result = await service.DeleteUserAsync(1);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(409, result.StatusCode);
            Assert.Equal(
                "User cannot be deleted because they have bookings.",
                result.Error);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldReturn204_WhenUserHasNoBookings()
        {
            // Arrange
            using var context = CreateDbContext();
            var role = CreateRole();
            var user = CreateUser();

            context.Roles.Add(role);
            context.Users.Add(user);

            await context.SaveChangesAsync();

            var service = new UserService(context);

            // Act
            var result = await service.DeleteUserAsync(1);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(204, result.StatusCode);
            Assert.Null(result.Error);
            Assert.Equal(0, await context.Users.CountAsync());
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
            var normalizedEmail = email.Trim().ToUpperInvariant();

            return new User
            {
                Id = id,
                FirstName = "John",
                LastName = "Doe",
                Email = email.Trim(),
                NormalizedEmail = normalizedEmail,
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
    }
}