namespace BookingSystem.Api.DTOs.Resource
{
    public class CreateResourceDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Location { get; set; }

        public int Capacity { get; set; }
        public bool IsActive { get; set; }
    }
}