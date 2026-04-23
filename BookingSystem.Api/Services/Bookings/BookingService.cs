using BookingSystem.Api.Data;
using BookingSystem.Api.DTOs.Booking;
using BookingSystem.Api.Models;
using BookingSystem.Api.Services.Bookings;
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

        // Creates a new booking and returns it as DTO
        public async Task<BookingDto> CreateBookingAsync(CreateBookingDto dto)
        {
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

            return new BookingDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                ResourceId = booking.ResourceId,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Notes = booking.Notes
            };
        }
    }
}