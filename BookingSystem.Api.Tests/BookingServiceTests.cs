using BookingSystem.Api.Data;
using BookingSystem.Api.Enums;
using BookingSystem.Api.Models;
using BookingSystem.Api.Services.Bookings;
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
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Completed bookings cannot be modified.", result.Error);
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
        Assert.False(result.Success);
        Assert.Equal(409, result.StatusCode);
        Assert.Equal("Resource is already booked in this time range.", result.Error);
        Assert.Null(result.Data);
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
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("StartTime must be before EndTime.", result.Error);
        Assert.Null(result.Data);
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
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("StartTime must be in the future.", result.Error);
        Assert.Null(result.Data);
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
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Resource is not active.", result.Error);
        Assert.Null(result.Data);
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
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal("Booking not found.", result.Error);
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
        Assert.False(result.Success);
        Assert.Equal(403, result.StatusCode);
        Assert.Equal("You are not allowed to access this booking.", result.Error);
    }
    
    [Fact]
    public async Task CreateBookingAsync_ShouldReturn201_WhenBookingIsValid()
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
            Description = "Valid resource",
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
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Notes = "Valid booking"
        };

        // Act
        var result = await service.CreateBookingAsync(
            dto,
            currentUserId: 1,
            isAdmin: false
        );

        // Assert
        Assert.True(result.Success);
        Assert.Equal(201, result.StatusCode);
        Assert.Null(result.Error);

        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data!.ResourceId);
        Assert.Equal(BookingStatus.Active, result.Data!.Status);
    }
        
    [Fact]
    public async Task CancelBookingAsync_ShouldReturn400_WhenBookingIsAlreadyCancelled()
    {
        // Arrange
        using var context = CreateDbContext();

        var booking = new Booking
        {
            Id = 1,
            UserId = 1,
            ResourceId = 1,
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Status = BookingStatus.Cancelled,
            CreatedAt = DateTime.UtcNow
        };

        context.Bookings.Add(booking);

        await context.SaveChangesAsync();

        var service = new BookingService(context);

        // Act
        var result = await service.CancelBookingAsync(
            id: 1,
            currentUserId: 1,
            isAdmin: false
        );

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Booking is already cancelled.", result.Error);
    }
    
    [Fact]
    public async Task CancelBookingAsync_ShouldReturn400_WhenBookingIsCompleted()
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
            Status = BookingStatus.Completed,
            CreatedAt = DateTime.UtcNow
        };

        context.Bookings.Add(booking);

        await context.SaveChangesAsync();

        var service = new BookingService(context);

        // Act
        var result = await service.CancelBookingAsync(
            id: 1,
            currentUserId: 1,
            isAdmin: false
        );

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Completed bookings cannot be cancelled.", result.Error);
    }
    
    [Fact]
    public async Task CompleteBookingAsync_ShouldReturn400_WhenBookingIsCancelled()
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
            Status = BookingStatus.Cancelled,
            CreatedAt = DateTime.UtcNow
        };

        context.Bookings.Add(booking);

        await context.SaveChangesAsync();

        var service = new BookingService(context);

        // Act
        var result = await service.CompleteBookingAsync(
            id: 1,
            currentUserId: 1,
            isAdmin: false
        );

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Cancelled bookings cannot be completed.", result.Error);
    }
    
    [Fact]
    public async Task CompleteBookingAsync_ShouldReturn400_WhenEndTimeHasNotPassed()
    {
        // Arrange
        using var context = CreateDbContext();

        var booking = new Booking
        {
            Id = 1,
            UserId = 1,
            ResourceId = 1,
            StartTime = DateTime.UtcNow.AddMinutes(-30),
            EndTime = DateTime.UtcNow.AddHours(1),
            Status = BookingStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Bookings.Add(booking);

        await context.SaveChangesAsync();

        var service = new BookingService(context);

        // Act
        var result = await service.CompleteBookingAsync(
            id: 1,
            currentUserId: 1,
            isAdmin: false
        );

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Booking cannot be completed before EndTime has passed.", result.Error);
    }
    
    [Fact]
    public async Task DeleteBookingAsync_ShouldReturn403_WhenUserDeletesAnotherUsersBooking()
    {
        // Arrange
        using var context = CreateDbContext();

        var booking = new Booking
        {
            Id = 1,
            UserId = 2,
            ResourceId = 1,
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Status = BookingStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Bookings.Add(booking);

        await context.SaveChangesAsync();

        var service = new BookingService(context);

        // Act
        var result = await service.DeleteBookingAsync(
            id: 1,
            currentUserId: 1,
            isAdmin: false
        );

        // Assert
        Assert.False(result.Success);
        Assert.Equal(403, result.StatusCode);
        Assert.Equal("You are not allowed to access this booking.", result.Error);
    }
    
    [Fact]
    public async Task GetBookingsAsync_ShouldReturnOnlyCurrentUsersBookings_WhenUserIsNotAdmin()
    {
        // Arrange
        using var context = CreateDbContext();

        var booking1 = new Booking
        {
            Id = 1,
            UserId = 1,
            ResourceId = 1,
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Status = BookingStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var booking2 = new Booking
        {
            Id = 2,
            UserId = 2,
            ResourceId = 1,
            StartTime = DateTime.UtcNow.AddHours(3),
            EndTime = DateTime.UtcNow.AddHours(4),
            Status = BookingStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Bookings.AddRange(booking1, booking2);

        await context.SaveChangesAsync();

        var service = new BookingService(context);

        var query = new DTOs.Booking.BookingQueryDto(); 

        // Act
        var result = await service.GetBookingsAsync(
            query,
            currentUserId: 1,
            isAdmin: false
        );

        // Assert
        Assert.Single(result);

        var booking = result.First();

        Assert.Equal(1, booking.UserId);
        Assert.Equal(1, booking.Id);
    }

    [Fact]
    public async Task GetBookingsAsync_ShouldReturnAllBookings_WhenUserIsAdmin()
    {
        // Arrange
        using var context = CreateDbContext();

        var booking1 = new Booking
        {
            Id = 1,
            UserId = 1,
            ResourceId = 1,
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Status = BookingStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var booking2 = new Booking
        {
            Id = 2,
            UserId = 2,
            ResourceId = 1,
            StartTime = DateTime.UtcNow.AddHours(3),
            EndTime = DateTime.UtcNow.AddHours(4),
            Status = BookingStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Bookings.AddRange(booking1, booking2);

        await context.SaveChangesAsync();

        var service = new BookingService(context);

        var query = new DTOs.Booking.BookingQueryDto();

        // Act
        var result = await service.GetBookingsAsync(
            query,
            currentUserId: 1,
            isAdmin: true
        );

        // Assert
        Assert.Equal(2, result.Count());

        var bookings = result.ToList();

        Assert.Contains(bookings, b => b.UserId == 1);
        Assert.Contains(bookings, b => b.UserId == 2);
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}