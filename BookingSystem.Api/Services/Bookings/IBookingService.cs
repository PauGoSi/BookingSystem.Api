using BookingSystem.Api.DTOs.Booking;

namespace BookingSystem.Api.Services.Bookings
{
    public interface IBookingService
    {
        // Retrieves all bookings
        Task<IEnumerable<BookingDto>> GetBookingsAsync();

        // Retrieves a single booking by id
        Task<BookingDto?> GetBookingByIdAsync(int id);

        // Creates a new booking with validation result
        Task<(bool Success, string? Error, int StatusCode, BookingDto? Data)> CreateBookingAsync(CreateBookingDto dto);

        // Updates an existing booking
        Task<(bool Success, string? Error, int StatusCode)> UpdateBookingAsync(int id, UpdateBookingDto dto);
    }
}