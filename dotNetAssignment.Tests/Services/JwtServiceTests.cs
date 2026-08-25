using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

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
        public void GenerateAccessToken_ReturnsTokenWithExpectedClaims()
        {
            var userId = Guid.NewGuid();
            var email = "user@example.com";
            var role = UserRole.Customer;

            var token = _jwtService.GenerateAccessToken(
                userId,
                email,
                role);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            Assert.Multiple(() =>
            {
                Assert.That(token, Is.Not.Null.And.Not.Empty);

                Assert.That(
                    jwt.Claims
                        .First(x => x.Type == ClaimTypes.NameIdentifier)
                        .Value,
                    Is.EqualTo(userId.ToString()));

                Assert.That(
                    jwt.Claims
                        .First(x => x.Type == ClaimTypes.Email)
                        .Value,
                    Is.EqualTo(email));

                Assert.That(
                    jwt.Claims
                        .First(x => x.Type == ClaimTypes.Role)
                        .Value,
                    Is.EqualTo(role.ToString()));
            });
        }

        [Test]
        public void GenerateRefreshToken_ReturnsTokenWithExpectedClaims()
        {
            var userId = Guid.NewGuid();

            var result = _jwtService.GenerateRefreshToken(userId);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(result.RefreshToken);

            var userIdClaim = jwt.Claims
                .First(x => x.Type == ClaimTypes.NameIdentifier)
                .Value;

            var jwtIdClaim = jwt.Claims
                .First(x => x.Type == JwtRegisteredClaimNames.Jti)
                .Value;

            Assert.Multiple(() =>
            {
                Assert.That(
                    result.RefreshToken,
                    Is.Not.Null.And.Not.Empty);

                Assert.That(
                    result.JwtId,
                    Is.Not.EqualTo(Guid.Empty));

                Assert.That(
                    userIdClaim,
                    Is.EqualTo(userId.ToString()));

                Assert.That(
                    jwtIdClaim,
                    Is.EqualTo(result.JwtId.ToString()));
            });
        }

        [Test]
        public void ValidateRefreshToken_WhenTokenIsValid_ReturnsPrincipal()
        {
            var userId = Guid.NewGuid();

            var refreshToken =
                _jwtService.GenerateRefreshToken(userId).RefreshToken;

            var principal =
                _jwtService.ValidateRefreshToken(refreshToken);

            Assert.Multiple(() =>
            {
                Assert.That(principal, Is.Not.Null);

                Assert.That(
                    principal.FindFirst(ClaimTypes.NameIdentifier).Value,
                    Is.EqualTo(userId.ToString()));
            });
        }

        [Test]
        public void GetJwtId_ReturnsJwtIdFromPrincipal()
        {
            var jwtId = Guid.NewGuid();

            var identity = new ClaimsIdentity(
                new[]
                {
                    new Claim(
                        JwtRegisteredClaimNames.Jti,
                        jwtId.ToString())
                });

            var principal = new ClaimsPrincipal(identity);

            var result = _jwtService.GetJwtId(principal);

            Assert.That(result, Is.EqualTo(jwtId));
        }

        [Test]
        public void GetUserId_ReturnsUserIdFromPrincipal()
        {
            var userId = Guid.NewGuid();

            var identity = new ClaimsIdentity(
                new[]
                {
                    new Claim(
                        ClaimTypes.NameIdentifier,
                        userId.ToString())
                });

            var principal = new ClaimsPrincipal(identity);

            var result = _jwtService.GetUserId(principal);

            Assert.That(result, Is.EqualTo(userId));
        }
    }
}
