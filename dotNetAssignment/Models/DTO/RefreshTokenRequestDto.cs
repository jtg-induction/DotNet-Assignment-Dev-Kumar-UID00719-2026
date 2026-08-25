using System.ComponentModel.DataAnnotations;

namespace dotNetAssignment.Models.DTO
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
