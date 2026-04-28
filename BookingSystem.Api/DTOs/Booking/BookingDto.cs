using BookingSystem.Api.Enums;

namespace BookingSystem.Api.DTOs.Booking
{
    public class BookingDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int ResourceId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public BookingStatus Status { get; set; }
        public string? Notes { get; set; }
    }
}