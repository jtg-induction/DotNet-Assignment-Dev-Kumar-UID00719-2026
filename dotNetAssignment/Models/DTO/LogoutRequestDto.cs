using System.ComponentModel.DataAnnotations;

namespace dotNetAssignment.Models.DTO
{
    public class LogoutRequestDto
    {
        /// <summary>
        /// The refresh token associated with the user trying to logout.
        /// </summary>
        [Required]
        public string RefreshToken { get; set; }
    }
}
