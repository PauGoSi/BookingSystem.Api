namespace BookingSystem.Api.Models
{
    public class Resource
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Location { get; set; } = null!;

        public int Capacity { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}