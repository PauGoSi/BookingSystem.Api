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

        // Retrieves paginated bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings([FromQuery] BookingQueryDto query)
        {
            var bookings = await _bookingService.GetBookingsAsync(query);
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

        // Updates an existing booking
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBooking(int id, UpdateBookingDto dto)
        {
            var result = await _bookingService.UpdateBookingAsync(id, dto);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            return NoContent();
        }

        // Deletes a booking
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var result = await _bookingService.DeleteBookingAsync(id);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            return NoContent();
        }
    }
}