using NUnit.Framework;

using dotNetAssignment.Services.Implementations;

namespace dotNetAssignment.Tests.Services
{
    [TestFixture]
    public class PasswordServiceTests
    {
        private PasswordService _passwordService;

        [SetUp]
        public void Setup()
        {
            _passwordService = new PasswordService();
        }

        [Test]
        public void HashPassword_Should_ReturnHashedPassword()
        {
            var password = "Admin@123";

            var hash = _passwordService.HashPassword(password);

            Assert.Multiple(() =>
            {
                Assert.That(hash, Is.Not.Null.And.Not.Empty);
                Assert.That(hash, Is.Not.EqualTo(password));
            });
        }

        [Test]
        public void VerifyPassword_Should_ReturnTrue_WhenPasswordMatches()
        {
            var password = "Admin@123";
            var hash = _passwordService.HashPassword(password);

            var result = _passwordService.VerifyPassword(password, hash);

            Assert.That(result, Is.True);
        }

        [Test]
        public void VerifyPassword_Should_ReturnFalse_WhenPasswordDoesNotMatch()
        {
            var password = "Admin@123";
            var hash = _passwordService.HashPassword(password);

            var result = _passwordService.VerifyPassword("WrongPassword", hash);

            Assert.That(result, Is.False);
        }

        [Test]
        public void HashPassword_Should_ReturnDifferentHashes_ForSamePassword()
        {
            var password = "Admin@123";

            var hash1 = _passwordService.HashPassword(password);
            var hash2 = _passwordService.HashPassword(password);

            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }
    }
}
