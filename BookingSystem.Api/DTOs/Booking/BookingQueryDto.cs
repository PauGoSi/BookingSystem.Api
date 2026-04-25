namespace BookingSystem.Api.DTOs.Booking
{
    public class BookingQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public int? UserId { get; set; }
        public int? ResourceId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}