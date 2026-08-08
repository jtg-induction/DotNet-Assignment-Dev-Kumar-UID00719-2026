using System.ComponentModel.DataAnnotations;

namespace dotNetAssignment.Models.DTO.Login
{
    public class LoginRequestDto
    {
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; }
    }
}
