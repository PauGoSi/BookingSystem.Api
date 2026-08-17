using BookingSystem.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace BookingSystem.Api.DTOs.User
{
    public class CreateUserDto
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

        [Required]
        [MinLength(8)]
        [StringLength(100)]
        public string Password { get; set; } = null!;

        public string Role { get; set; } = null!;
    }
}