using BookingSystem.Api.DTOs.Booking;

namespace BookingSystem.Api.Services.Bookings
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDto>> GetBookingsAsync(BookingQueryDto query, int currentUserId, bool isAdmin);

        Task<BookingDto?> GetBookingByIdAsync(int id, int currentUserId, bool isAdmin);

        Task<(bool Success, string? Error, int StatusCode, BookingDto? Data)> CreateBookingAsync(CreateBookingDto dto, int currentUserId, bool isAdmin);

        Task<(bool Success, string? Error, int StatusCode)> UpdateBookingAsync(int id, UpdateBookingDto dto, int currentUserId, bool isAdmin);

        Task<(bool Success, string? Error, int StatusCode)> DeleteBookingAsync(int id, int currentUserId, bool isAdmin);

        Task<(bool Success, string? Error, int StatusCode)> CancelBookingAsync(int id, int currentUserId, bool isAdmin);

        Task<(bool Success, string? Error, int StatusCode)> CompleteBookingAsync(int id, int currentUserId, bool isAdmin);
    }
}