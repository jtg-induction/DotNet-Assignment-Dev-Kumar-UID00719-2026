using System;

namespace dotNetAssignment.Models.DTO
{
    public class RefreshTokenResponseDto
    {
        public string RefreshToken { get; set; }
        public Guid JwtId { get; set; }
    }
}