using System.ComponentModel.DataAnnotations;

using dotNetAssignment.Models.Enums;

namespace dotNetAssignment.Models.DTO.SignUp
{
    public class SignupRequestDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        public UserRole Role { get; set; }
    }
}
