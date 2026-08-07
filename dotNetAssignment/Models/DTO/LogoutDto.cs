using System.ComponentModel.DataAnnotations;

namespace dotNetAssignment.Models.DTO
{
    public class LogoutRequestDto
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}