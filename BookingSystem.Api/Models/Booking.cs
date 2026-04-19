namespace BookingSystem.Api.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int ResourceId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string Status { get; set; } = null!;
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation
        public User User { get; set; } = null!;
        public Resource Resource { get; set; } = null!;
    }
}