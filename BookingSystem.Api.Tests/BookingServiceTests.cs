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
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);

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
            true
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
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);

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
}