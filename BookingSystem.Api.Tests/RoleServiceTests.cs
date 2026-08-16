using BookingSystem.Api.Data;
using BookingSystem.Api.Models;
using BookingSystem.Api.Services.Roles;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Api.Tests
{
    public class RoleServiceTests
    {
        [Fact]
        public async Task GetRolesAsync_ShouldReturnAdminAndUserRoles()
        {
            // Arrange
            using var context = CreateDbContext();

            context.Roles.AddRange(
                new Role
                {
                    Id = 1,
                    Name = "Admin"
                },
                new Role
                {
                    Id = 2,
                    Name = "User"
                });

            await context.SaveChangesAsync();

            var service = new RoleService(context);

            // Act
            var result = (await service.GetRolesAsync()).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, role => role.Name == "Admin");
            Assert.Contains(result, role => role.Name == "User");
        }

        [Fact]
        public async Task GetRoleByIdAsync_ShouldReturnRole_WhenRoleExists()
        {
            // Arrange
            using var context = CreateDbContext();

            context.Roles.Add(new Role
            {
                Id = 1,
                Name = "Admin"
            });

            await context.SaveChangesAsync();

            var service = new RoleService(context);

            // Act
            var result = await service.GetRoleByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result!.Id);
            Assert.Equal("Admin", result.Name);
        }

        [Fact]
        public async Task GetRoleByIdAsync_ShouldReturnNull_WhenRoleDoesNotExist()
        {
            // Arrange
            using var context = CreateDbContext();

            var service = new RoleService(context);

            // Act
            var result = await service.GetRoleByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        // Helpers
        private static AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }
    }
}