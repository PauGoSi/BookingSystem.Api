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
}