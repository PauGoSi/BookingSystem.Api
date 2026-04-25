namespace BookingSystem.Api.DTOs.Booking
{
    public class BookingQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}