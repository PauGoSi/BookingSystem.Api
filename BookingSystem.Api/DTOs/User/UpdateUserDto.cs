using BookingSystem.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace BookingSystem.Api.DTOs.User
{
    public class UpdateUserDto
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(254)]
        public string Email { get; set; } = null!;

        public SystemRole Role { get; set; }
    }
}