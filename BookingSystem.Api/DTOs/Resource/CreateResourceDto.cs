using System.ComponentModel.DataAnnotations;

namespace BookingSystem.Api.DTOs.Resource
{
    public class CreateResourceDto
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = null!;

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }

        public bool IsActive { get; set; }
    }
}