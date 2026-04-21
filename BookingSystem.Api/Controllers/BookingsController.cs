using BookingSystem.Api.Data;
using BookingSystem.Api.DTOs.Booking;
using BookingSystem.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings()
        {
            var bookings = await _context.Bookings
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

            return Ok(bookings);
        }

        [HttpPost]
        public async Task<ActionResult<BookingDto>> CreateBooking(CreateBookingDto dto)
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

            var result = new BookingDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                ResourceId = booking.ResourceId,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Notes = booking.Notes
            };

            return CreatedAtAction(nameof(GetBookings), new { id = result.Id }, result);
        }
    }
}