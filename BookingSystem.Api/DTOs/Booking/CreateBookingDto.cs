using System.ComponentModel.DataAnnotations;

namespace BookingSystem.Api.DTOs.Booking
{
    public class CreateBookingDto
    {
        [Range(1, int.MaxValue)]
        public int ResourceId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        [StringLength(2000)]
        public string? Notes { get; set; }
    }
}