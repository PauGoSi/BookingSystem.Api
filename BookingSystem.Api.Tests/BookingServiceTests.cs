using BookingSystem.Api.Data;
using BookingSystem.Api.Enums;
using BookingSystem.Api.Models;
using BookingSystem.Api.Services.Bookings;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Api.Tests;

public class BookingServiceTests
{
    [Fact]
    public async Task UpdateBookingAsync_ShouldReturn400_WhenBookingIsCompleted()
    {
        // Arrange
        using var context = CreateDbContext();

        var booking = new Booking
        {
            Id = 1,
            UserId = 1,
            ResourceId = 1,
            StartTime = DateTime.UtcNow.AddHours(-2),
            EndTime = DateTime.UtcNow.AddHours(-1),
            Status = BookingStatus.Completed
        };

        context.Bookings.Add(booking);

        await context.SaveChangesAsync();

        var service = new BookingService(context);

        // Act
        var result = await service.UpdateBookingAsync(
            1,
            new DTOs.Booking.UpdateBookingDto
            {
                ResourceId = 1,
                StartTime = DateTime.UtcNow.AddHours(1),
                EndTime = DateTime.UtcNow.AddHours(2),
                Notes = "Updated booking"
            },
            1,
            false
        );

        // Assert
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Error.Should().Be("Completed bookings cannot be modified.");
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldReturn409_WhenBookingOverlapsExistingBooking()
    {
        // Arrange
        using var context = CreateDbContext();

        var user = new User
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            PasswordHash = "hashed-password",
            RoleId = 1,
            CreatedAt = DateTime.UtcNow
        };

        var resource = new Resource
        {
            Id = 1,
            Name = "Meeting Room",
            Description = "Test room",
            Location = "Test Building or Street",
            Capacity = 4,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var existingBooking = new Booking
        {
            Id = 1,
            UserId = 1,
            ResourceId = 1,
            StartTime = DateTime.UtcNow.AddHours(2),
            EndTime = DateTime.UtcNow.AddHours(4),
            Status = BookingStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        context.Resources.Add(resource);
        context.Bookings.Add(existingBooking);

        await context.SaveChangesAsync();

        var service = new BookingService(context);

        var dto = new DTOs.Booking.CreateBookingDto
        {
            ResourceId = 1,
            StartTime = DateTime.UtcNow.AddHours(3),
            EndTime = DateTime.UtcNow.AddHours(5),
            Notes = "Overlapping booking"
        };

        // Act
        var result = await service.CreateBookingAsync(
            dto,
            currentUserId: 1,
            isAdmin: false
        );

        // Assert
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(409);
        result.Error.Should().Be("Resource is already booked in this time range.");
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldReturn400_WhenStartTimeIsAfterEndTime()
    {
        // Arrange
        using var context = CreateDbContext();

        var user = new User
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            PasswordHash = "hashed-password",
            RoleId = 1,
            CreatedAt = DateTime.UtcNow
        };

        var resource = new Resource
        {
            Id = 1,
            Name = "Meeting Room",
            Description = "Test room",
            Location = "Test Building",
            Capacity = 4,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        context.Resources.Add(resource);

        await context.SaveChangesAsync();

        var service = new BookingService(context);

        var dto = new DTOs.Booking.CreateBookingDto
        {
            ResourceId = 1,
            StartTime = DateTime.UtcNow.AddHours(5),
            EndTime = DateTime.UtcNow.AddHours(2),
            Notes = "Invalid booking"
        };

        // Act
        var result = await service.CreateBookingAsync(
            dto,
            currentUserId: 1,
            isAdmin: false
        );

        // Assert
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Error.Should().Be("StartTime must be before EndTime.");
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldReturn400_WhenStartTimeIsInThePast()
    {
        // Arrange
        using var context = CreateDbContext();

        var user = new User
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            PasswordHash = "hashed-password",
            RoleId = 1,
            CreatedAt = DateTime.UtcNow
        };

        var resource = new Resource
        {
            Id = 1,
            Name = "Meeting Room",
            Description = "Test room",
            Location = "Test Building",
            Capacity = 4,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        context.Resources.Add(resource);

        await context.SaveChangesAsync();

        var service = new BookingService(context);

        var dto = new DTOs.Booking.CreateBookingDto
        {
            ResourceId = 1,
            StartTime = DateTime.UtcNow.AddHours(-1),
            EndTime = DateTime.UtcNow.AddHours(1),
            Notes = "Past booking"
        };

        // Act
        var result = await service.CreateBookingAsync(
            dto,
            currentUserId: 1,
            isAdmin: false
        );

        // Assert
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Error.Should().Be("StartTime must be in the future.");
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldReturn400_WhenResourceIsInactive()
    {
        // Arrange
        using var context = CreateDbContext();

        var user = new User
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            PasswordHash = "hashed-password",
            RoleId = 1,
            CreatedAt = DateTime.UtcNow
        };

        var inactiveResource = new Resource
        {
            Id = 1,
            Name = "Inactive Room",
            Description = "Inactive resource",
            Location = "Test Building",
            Capacity = 4,
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        context.Resources.Add(inactiveResource);

        await context.SaveChangesAsync();

        var service = new BookingService(context);

        var dto = new DTOs.Booking.CreateBookingDto
        {
            ResourceId = 1,
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Notes = "Inactive resource booking"
        };

        // Act
        var result = await service.CreateBookingAsync(
            dto,
            currentUserId: 1,
            isAdmin: false
        );

        // Assert
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Error.Should().Be("Resource is not active.");
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task UpdateBookingAsync_ShouldReturn404_WhenBookingDoesNotExist()
    {
        // Arrange
        using var context = CreateDbContext();

        var service = new BookingService(context);

        var dto = new DTOs.Booking.UpdateBookingDto
        {
            ResourceId = 1,
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Notes = "Updated booking"
        };

        // Act
        var result = await service.UpdateBookingAsync(
            id: 999,
            dto: dto,
            currentUserId: 1,
            isAdmin: true
        );

        // Assert
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.Error.Should().Be("Booking not found.");
    }

    [Fact]
    public async Task UpdateBookingAsync_ShouldReturn403_WhenUserUpdatesAnotherUsersBooking()
    {
        // Arrange
        using var context = CreateDbContext();

        var booking = new Booking
        {
            Id = 1,
            UserId = 2,
            ResourceId = 1,
            StartTime = DateTime.UtcNow.AddHours(2),
            EndTime = DateTime.UtcNow.AddHours(3),
            Status = BookingStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Bookings.Add(booking);

        await context.SaveChangesAsync();

        var service = new BookingService(context);

        var dto = new DTOs.Booking.UpdateBookingDto
        {
            ResourceId = 1,
            StartTime = DateTime.UtcNow.AddHours(4),
            EndTime = DateTime.UtcNow.AddHours(5),
            Notes = "Trying to update another user's booking"
        };

        // Act
        var result = await service.UpdateBookingAsync(
            id: 1,
            dto: dto,
            currentUserId: 1,
            isAdmin: false
        );

        // Assert
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(403);
        result.Error.Should().Be("You are not allowed to access this booking.");
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}