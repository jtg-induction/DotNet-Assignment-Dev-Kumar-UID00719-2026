using System;

namespace dotNetAssignment.Models.DTO
{
    public class RefreshTokenResponseDto
    {
        /// <summary>
        /// The new token issued after generating a new refresh token.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// The unique identifier (JWT ID) associated with the new refresh token.
        /// </summary>
        public Guid JwtId { get; set; }
    }
}
