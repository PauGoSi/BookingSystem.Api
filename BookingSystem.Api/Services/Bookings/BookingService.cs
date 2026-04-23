using BookingSystem.Api.Data;
using BookingSystem.Api.DTOs.Booking;
using BookingSystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Api.Services.Bookings
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _context;

        // Injects database context for data access
        public BookingService(AppDbContext context)
        {
            _context = context;
        }

        // Retrieves all bookings as DTOs
        public async Task<IEnumerable<BookingDto>> GetBookingsAsync()
        {
            return await _context.Bookings
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    ResourceId = b.ResourceId,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Notes = b.Notes
                })
                .ToListAsync();
        }

        // Retrieves a single booking by id
        public async Task<BookingDto?> GetBookingByIdAsync(int id)
        {
            return await _context.Bookings
                .Where(b => b.Id == id)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    ResourceId = b.ResourceId,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Notes = b.Notes
                })
                .FirstOrDefaultAsync();
        }

        // Creates a new booking with validation and returns result
        public async Task<(bool Success, string? Error, int StatusCode, BookingDto? Data)> CreateBookingAsync(CreateBookingDto dto)
        {
            // Validates that start time is earlier than end time
            if (dto.StartTime >= dto.EndTime)
            {
                return (false, "StartTime must be before EndTime.", 400, null);
            }

            // Checks that the user exists
            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
            if (!userExists)
            {
                return (false, "User not found.", 404, null);
            }

            // Checks that the resource exists
            var resource = await _context.Resources.FirstOrDefaultAsync(r => r.Id == dto.ResourceId);
            if (resource == null)
            {
                return (false, "Resource not found.", 404, null);
            }

            // Checks that the resource is active
            if (!resource.IsActive)
            {
                return (false, "Resource is not active.", 400, null);
            }

            // Checks for overlapping bookings on the same resource
            var hasOverlap = await _context.Bookings.AnyAsync(b =>
                b.ResourceId == dto.ResourceId &&
                dto.StartTime < b.EndTime &&
                dto.EndTime > b.StartTime);

            if (hasOverlap)
            {
                return (false, "Resource is already booked in this time range.", 409, null);
            }

            var booking = new Booking
            {
                UserId = dto.UserId,
                ResourceId = dto.ResourceId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Status = "Active",
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var result = new BookingDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                ResourceId = booking.ResourceId,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Notes = booking.Notes
            };

            return (true, null, 201, result);
        }
    }
}