using System;
using System.Security.Claims;

using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.Enums;

namespace dotNetAssignment.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(Guid userId, string email, UserRole role);
        RefreshTokenResponseDto GenerateRefreshToken(Guid userId);
        ClaimsPrincipal ValidateRefreshToken(string refreshToken);
        Guid GetJwtId(ClaimsPrincipal principal);
        Guid GetUserId(ClaimsPrincipal principal);
    }
}