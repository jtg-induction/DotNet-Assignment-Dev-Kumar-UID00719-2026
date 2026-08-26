using System.ComponentModel.DataAnnotations;

namespace dotNetAssignment.Models.DTO
{
    public class RefreshTokenRequestDto
    {
        /// <summary>
        /// The refresh token that the user wants to use to obtain a new access token.
        /// </summary>
        [Required]
        public string RefreshToken { get; set; }
    }
}
