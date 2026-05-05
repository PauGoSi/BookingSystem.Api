using BookingSystem.Api.DTOs.Booking;
using BookingSystem.Api.Services.Bookings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BookingSystem.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/bookings")]
    public class BookingsController : ControllerBase
    {
        // Service for handling booking business logic
        private readonly IBookingService _bookingService;

        // Injects booking service into controller
        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // Gets the current authenticated user id from JWT claims
        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }

        // Checks whether the current authenticated user is an admin
        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }

        // Retrieves paginated bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings([FromQuery] BookingQueryDto query)
        {
            var bookings = await _bookingService.GetBookingsAsync(query, GetCurrentUserId(), IsAdmin());
            return Ok(bookings);
        }

        // Cancels a booking
        [HttpPatch("{id:int}/cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var result = await _bookingService.CancelBookingAsync(id, GetCurrentUserId(), IsAdmin());

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            return NoContent();
        }

        // Completes a booking
        [HttpPatch("{id:int}/complete")]
        public async Task<IActionResult> CompleteBooking([FromRoute] int id)
        {
            var result = await _bookingService.CompleteBookingAsync(id, GetCurrentUserId(), IsAdmin());

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            return NoContent();
        }

        // Retrieves a single booking by id
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookingDto>> GetBookingById([FromRoute] int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id, GetCurrentUserId(), IsAdmin());

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
            var result = await _bookingService.CreateBookingAsync(dto, GetCurrentUserId(), IsAdmin());

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
            var result = await _bookingService.UpdateBookingAsync(id, dto, GetCurrentUserId(), IsAdmin());

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
            var result = await _bookingService.DeleteBookingAsync(id, GetCurrentUserId(), IsAdmin());

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            return NoContent();
        }
    }
}