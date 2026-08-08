using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;

using dotNetAssignment.Models.Enums;
using dotNetAssignment.Services.Implementations;

namespace dotNetAssignment.Tests.Services
{
    [TestFixture]
    public class JwtServiceTests
    {
        private JwtService _jwtService;

        [SetUp]
        public void Setup()
        {
            _jwtService = new JwtService();
        }

        [Test]
        public void GenerateAccessToken_ReturnsValidJwt()
        {
            var token = _jwtService.GenerateAccessToken(Guid.NewGuid(), "test@test.com", UserRole.Customer);

            Assert.That(token, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void GenerateAccessToken_ContainsCorrectUserIdClaim()
        {
            var userId = Guid.NewGuid();

            var token = _jwtService.GenerateAccessToken(userId, "test@test.com", UserRole.Customer);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            Assert.That(
                jwt.Claims.First(c => c.Type == "userid").Value,
                Is.EqualTo(userId.ToString()));
        }

        [Test]
        public void GenerateRefreshToken_ReturnsValidResponse()
        {
            var response = _jwtService.GenerateRefreshToken(Guid.NewGuid());

            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.RefreshToken, Is.Not.Null.And.Not.Empty);
            });
        }

        [Test]
        public void GenerateRefreshToken_ContainsCorrectUserIdClaim()
        {
            var userId = Guid.NewGuid();

            var response = _jwtService.GenerateRefreshToken(userId);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(response.RefreshToken);

            Assert.That(
                jwt.Claims.First(c => c.Type == "userid").Value,
                Is.EqualTo(userId.ToString()));
        }

        [Test]
        public void GenerateRefreshToken_ContainsMatchingJwtId()
        {
            var response = _jwtService.GenerateRefreshToken(Guid.NewGuid());

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(response.RefreshToken);

            Assert.That(
                jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value,
                Is.EqualTo(response.JwtId.ToString()));
        }

        [Test]
        public void ValidateRefreshToken_WithValidToken_ReturnsClaimsPrincipal()
        {
            var response = _jwtService.GenerateRefreshToken(Guid.NewGuid());

            var principal = _jwtService.ValidateRefreshToken(response.RefreshToken);

            Assert.That(principal, Is.Not.Null);
        }

        [Test]
        public void ValidateRefreshToken_WithInvalidToken_ThrowsSecurityTokenMalformedException()
        {
            Assert.Throws<SecurityTokenMalformedException>(() =>
                _jwtService.ValidateRefreshToken("invalid-token"));
        }

        [Test]
        public void GetJwtId_ReturnsCorrectJwtId()
        {
            var response = _jwtService.GenerateRefreshToken(Guid.NewGuid());
            var principal = _jwtService.ValidateRefreshToken(response.RefreshToken);
            var jwtId = _jwtService.GetJwtId(principal);

            Assert.That(jwtId, Is.EqualTo(response.JwtId));
        }

        [Test]
        public void GetUserId_ReturnsCorrectUserId()
        {
            var userId = Guid.NewGuid();

            var response = _jwtService.GenerateRefreshToken(userId);
            var principal = _jwtService.ValidateRefreshToken(response.RefreshToken);
            var extractedUserId = _jwtService.GetUserId(principal);

            Assert.That(extractedUserId, Is.EqualTo(userId));
        }
    }
}
