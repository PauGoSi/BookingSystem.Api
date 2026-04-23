using BookingSystem.Api.DTOs.Booking;

namespace BookingSystem.Api.Services.Bookings
{
    public interface IBookingService
    {
        // Retrieves all bookings
        Task<IEnumerable<BookingDto>> GetBookingsAsync();

        // Retrieves a single booking by id
        Task<BookingDto?> GetBookingByIdAsync(int id);

        // Creates a new booking
        Task<BookingDto> CreateBookingAsync(CreateBookingDto dto);
    }
}