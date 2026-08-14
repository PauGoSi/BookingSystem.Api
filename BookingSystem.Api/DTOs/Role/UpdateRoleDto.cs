using System.ComponentModel.DataAnnotations;

namespace BookingSystem.Api.DTOs.Role
{
    public class UpdateRoleDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;
    }
}