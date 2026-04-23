using BookingSystem.Api.DTOs.Booking;
using BookingSystem.Api.Services.Bookings;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        // Service for handling booking business logic
        private readonly IBookingService _bookingService;

        // Injects booking service into controller
        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // Retrieves all bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings()
        {
            var bookings = await _bookingService.GetBookingsAsync();
            return Ok(bookings);
        }

        // Retrieves a single booking by id
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookingDto>> GetBookingById([FromRoute] int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            return Ok(booking);
        }

        // Creates a new booking
        [HttpPost]
        public async Task<ActionResult<BookingDto>> CreateBooking(CreateBookingDto dto)
        {
            var result = await _bookingService.CreateBookingAsync(dto);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            return CreatedAtAction(
                nameof(GetBookingById),
                new { id = result.Data!.Id },
                result.Data
            );
        }
    }
}