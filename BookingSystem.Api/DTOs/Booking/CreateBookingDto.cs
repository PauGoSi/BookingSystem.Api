namespace BookingSystem.Api.DTOs.Booking
{
    public class CreateBookingDto
    {
        public int UserId { get; set; }
        public int ResourceId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string? Notes { get; set; }
    }
}