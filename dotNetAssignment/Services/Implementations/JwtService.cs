using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.Enums;
using dotNetAssignment.Services.Interfaces;

namespace dotNetAssignment.Services.Implementations
{
    public class JwtService : IJwtService
    {
        public string GenerateAccessToken(Guid userId, string email, UserRole role)
        {
            var key = ConfigurationManager.AppSettings["JwtKey"];
            var issuer = ConfigurationManager.AppSettings["JwtIssuer"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("userid", userId.ToString()),
                new Claim("email", email),
                new Claim("role", role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer,
                issuer,
                claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshTokenResponseDto GenerateRefreshToken(Guid userId)
        {
            var key = ConfigurationManager.AppSettings["JwtKey"];
            var issuer = ConfigurationManager.AppSettings["JwtIssuer"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);

            var jwtId = Guid.NewGuid();

            var claims = new List<Claim>
            {
                new Claim("userid", userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, jwtId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer,
                issuer,
                claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials);

            return new RefreshTokenResponseDto
            {
                RefreshToken = new JwtSecurityTokenHandler().WriteToken(token),
                JwtId = jwtId
            };
        }

        public ClaimsPrincipal ValidateRefreshToken(string refreshToken)
        {
            var key = ConfigurationManager.AppSettings["JwtKey"];
            var issuer = ConfigurationManager.AppSettings["JwtIssuer"];

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = issuer,
                ValidAudience = issuer,

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(key)),
                    ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken validatedToken;

            return tokenHandler.ValidateToken(
                refreshToken,
                tokenValidationParameters,
                out validatedToken);
        }

        public Guid GetJwtId(ClaimsPrincipal principal)
        {
            return Guid.Parse(
                principal.FindFirst(JwtRegisteredClaimNames.Jti).Value);
        }

        public Guid GetUserId(ClaimsPrincipal principal)
        {
            return Guid.Parse(principal.FindFirst("userid").Value);
        }
    }
}
