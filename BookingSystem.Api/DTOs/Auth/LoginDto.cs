using System.ComponentModel.DataAnnotations;

namespace BookingSystem.Api.DTOs.Auth
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        [StringLength(254)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = null!;
    }
}