using BookingSystem.Api.Data;
using BookingSystem.Api.Enums;
using BookingSystem.Api.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Api.Tests
{
    public class DatabaseConstraintTests
    {
        [Fact]
        public async Task SaveChangesAsync_ShouldThrow_WhenResourceCapacityIsZero()
        {
            // Arrange
            await using var connection = CreateOpenConnection();
            await using var context = CreateDbContext(connection);

            var resource = new Resource
            {
                Name = "Meeting Room",
                Description = "Test room",
                Location = "Test Building",
                Capacity = 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Resources.Add(resource);

            // Act
            var action = async () => await context.SaveChangesAsync();

            // Assert
            await Assert.ThrowsAsync<DbUpdateException>(action);
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldThrow_WhenBookingEndTimeEqualsStartTime()
        {
            // Arrange
            await using var connection = CreateOpenConnection();
            await using var context = CreateDbContext(connection);

            var role = CreateRole();
            var user = CreateUser(role);
            var resource = CreateResource();

            context.Roles.Add(role);
            context.Users.Add(user);
            context.Resources.Add(resource);

            await context.SaveChangesAsync();

            var startTime = DateTime.UtcNow.AddHours(1);

            var booking = new Booking
            {
                UserId = user.Id,
                ResourceId = resource.Id,
                StartTime = startTime,
                EndTime = startTime,
                Status = BookingStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            context.Bookings.Add(booking);

            // Act
            var action = async () => await context.SaveChangesAsync();

            // Assert
            await Assert.ThrowsAsync<DbUpdateException>(action);
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldThrow_WhenDeletingUserWithBookings()
        {
            // Arrange
            await using var connection = CreateOpenConnection();
            await using var context = CreateDbContext(connection);

            var role = CreateRole();
            var user = CreateUser(role);
            var resource = CreateResource();

            context.Roles.Add(role);
            context.Users.Add(user);
            context.Resources.Add(resource);

            await context.SaveChangesAsync();

            var booking = CreateBooking(user.Id, resource.Id);

            context.Bookings.Add(booking);
            await context.SaveChangesAsync();

            var userId = user.Id;

            // Clear EF Core's ChangeTracker so it no longer tracks the relationship
            // between the user and the booking.
            // This ensures that the delete is sent to the database and that the
            // database foreign key constraint itself is what rejects the deletion.
            context.ChangeTracker.Clear();

            var userToDelete = await context.Users.FindAsync(userId);

            Assert.NotNull(userToDelete);

            context.Users.Remove(userToDelete!);

            // Act
            var action = async () => await context.SaveChangesAsync();

            // Assert
            await Assert.ThrowsAsync<DbUpdateException>(action);
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldThrow_WhenDeletingResourceWithBookings()
        {
            // Arrange
            await using var connection = CreateOpenConnection();
            await using var context = CreateDbContext(connection);

            var role = CreateRole();
            var user = CreateUser(role);
            var resource = CreateResource();

            context.Roles.Add(role);
            context.Users.Add(user);
            context.Resources.Add(resource);

            await context.SaveChangesAsync();

            var booking = CreateBooking(user.Id, resource.Id);

            context.Bookings.Add(booking);
            await context.SaveChangesAsync();

            var resourceId = resource.Id;

            // Clear EF Core's ChangeTracker so it no longer tracks the relationship
            // between the resource and the booking.
            // This allows the DELETE to reach SQLite, where the foreign key
            // constraint should reject deletion of a referenced resource.
            context.ChangeTracker.Clear();

            var resourceToDelete = await context.Resources.FindAsync(resourceId);

            Assert.NotNull(resourceToDelete);

            context.Resources.Remove(resourceToDelete!);

            // Act
            var action = async () => await context.SaveChangesAsync();

            // Assert
            await Assert.ThrowsAsync<DbUpdateException>(action);
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldThrow_WhenDeletingRoleWithUsers()
        {
            // Arrange
            await using var connection = CreateOpenConnection();
            await using var context = CreateDbContext(connection);

            var role = CreateRole();
            var user = CreateUser(role);

            context.Roles.Add(role);
            context.Users.Add(user);

            await context.SaveChangesAsync();

            var roleId = role.Id;

            // Clear EF Core's ChangeTracker so it no longer tracks the relationship
            // between the role and the user.
            // Without this, EF Core may detect the required relationship itself
            // before SaveChangesAsync reaches the database.
            // Clearing it ensures that this test specifically verifies the
            // database-level foreign key constraint.
            context.ChangeTracker.Clear();

            var roleToDelete = await context.Roles.FindAsync(roleId);

            Assert.NotNull(roleToDelete);

            context.Roles.Remove(roleToDelete!);

            // Act
            var action = async () => await context.SaveChangesAsync();

            // Assert
            await Assert.ThrowsAsync<DbUpdateException>(action);
        }

        // Helpers
        private static SqliteConnection CreateOpenConnection()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            return connection;
        }

        private static AppDbContext CreateDbContext(SqliteConnection connection)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new AppDbContext(options);

            context.Database.EnsureCreated();

            return context;
        }

        private static Role CreateRole()
        {
            return new Role
            {
                Name = "User"
            };
        }

        private static User CreateUser(Role role)
        {
            const string email = "john@example.com";

            return new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
                PasswordHash = "hashed-password",
                Role = role,
                CreatedAt = DateTime.UtcNow
            };
        }

        private static Resource CreateResource()
        {
            return new Resource
            {
                Name = "Meeting Room",
                Description = "Test room",
                Location = "Test Building",
                Capacity = 10,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        private static Booking CreateBooking(int userId, int resourceId)
        {
            return new Booking
            {
                UserId = userId,
                ResourceId = resourceId,
                StartTime = DateTime.UtcNow.AddHours(1),
                EndTime = DateTime.UtcNow.AddHours(2),
                Status = BookingStatus.Active,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}