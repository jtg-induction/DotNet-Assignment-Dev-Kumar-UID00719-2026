using System;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;

using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.DTO.Login;
using dotNetAssignment.Models.DTO.SignUp;
using dotNetAssignment.Models.Entities;
using dotNetAssignment.Models.Enums;
using dotNetAssignment.Repositories.Jwt;
using dotNetAssignment.Repositories.UserRepo;
using dotNetAssignment.Services.Implementations;
using dotNetAssignment.Services.Interfaces;
using dotNetAssignment.Constants;

namespace dotNetAssignment.Tests.Services
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        private Mock<IPasswordService> _passwordService;
        private Mock<IUserRepository> _userRepository;
        private Mock<IJwtService> _jwtService;
        private Mock<IJwtRepository> _jwtRepository;

        private AuthenticationService _authenticationService;

        [SetUp]
        public void Setup()
        {
            _passwordService = new Mock<IPasswordService>();
            _userRepository = new Mock<IUserRepository>();
            _jwtService = new Mock<IJwtService>();
            _jwtRepository = new Mock<IJwtRepository>();

            _authenticationService = new AuthenticationService(
                _passwordService.Object,
                _userRepository.Object,
                _jwtService.Object,
                _jwtRepository.Object);
        }

        [Test]
        public async Task SignupAsync_EmailAlreadyExists_ReturnsFailure()
        {
            var request = new SignupRequestDto
            {
                Name = "Dev",
                Email = "dev@test.com",
                Password = "Password123",
                PhoneNumber = "9999999999",
                Role = UserRole.Customer
            };

            _userRepository
                .Setup(x => x.EmailExistsAsync(request.Email))
                .ReturnsAsync(true);

            var result = await _authenticationService.SignupAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo(ExceptionMessages.EmailAlreadyExists));
        }

        [Test]
        public async Task SignupAsync_ValidRequest_ReturnsAuthenticationResponse()
        {
            var request = new SignupRequestDto
            {
                Name = "Dev",
                Email = "dev@test.com",
                Password = "Password123",
                PhoneNumber = "9999999999",
                Role = UserRole.Customer
            };

            var refreshResponse = new RefreshTokenResponseDto
            {
                JwtId = Guid.NewGuid(),
                RefreshToken = "refresh-token"
            };

            _userRepository
                .Setup(x => x.EmailExistsAsync(request.Email))
                .ReturnsAsync(false);

            _passwordService
                .Setup(x => x.HashPassword(request.Password))
                .Returns("hashed-password");

            _jwtService
                .Setup(x => x.GenerateAccessToken(It.IsAny<Guid>(), request.Email, request.Role))
                .Returns("access-token");

            _jwtService
                .Setup(x => x.GenerateRefreshToken(It.IsAny<Guid>()))
                .Returns(refreshResponse);

            var result = await _authenticationService.SignupAsync(request);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo(SuccessMessages.UserCreated));
            Assert.That(result.Data.AccessToken, Is.EqualTo("access-token"));
            Assert.That(result.Data.RefreshToken, Is.EqualTo("refresh-token"));
        }

        [Test]
        public async Task SignupAsync_ValidRequest_CreatesCorrectUser()
        {
            var request = new SignupRequestDto
            {
                Name = "Dev",
                Email = "dev@test.com",
                Password = "Password123",
                PhoneNumber = "9999999999",
                Role = UserRole.Customer
            };

            var refreshResponse = new RefreshTokenResponseDto
            {
                JwtId = Guid.NewGuid(),
                RefreshToken = "refresh-token"
            };

            User capturedUser = null;

            _userRepository
                .Setup(x => x.EmailExistsAsync(request.Email))
                .ReturnsAsync(false);

            _passwordService
                .Setup(x => x.HashPassword(request.Password))
                .Returns("hashed-password");

            _userRepository
                .Setup(x => x.AddUser(It.IsAny<User>()))
                .Callback<User>(user => capturedUser = user);

            _jwtService
                .Setup(x => x.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("access-token");

            _jwtService
                .Setup(x => x.GenerateRefreshToken(It.IsAny<Guid>()))
                .Returns(refreshResponse);

            await _authenticationService.SignupAsync(request);

            Assert.That(capturedUser, Is.Not.Null);
            Assert.That(capturedUser.Name, Is.EqualTo(request.Name));
            Assert.That(capturedUser.Email, Is.EqualTo(request.Email));
            Assert.That(capturedUser.PhoneNumber, Is.EqualTo(request.PhoneNumber));
            Assert.That(capturedUser.Role, Is.EqualTo(request.Role));
            Assert.That(capturedUser.Password, Is.EqualTo("hashed-password"));
            Assert.That(capturedUser.IsActive, Is.True);
            Assert.That(capturedUser.Balance, Is.EqualTo(1000m));
        }

        [Test]
        public async Task LoginAsync_UserDoesNotExist_ReturnsFailure()
        {
            var request = new LoginRequestDto
            {
                Email = "test@test.com",
                Password = "Password123"
            };

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(request.Email))
                .ReturnsAsync((User)null);

            var result = await _authenticationService.LoginAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo(ExceptionMessages.InvalidEmailOrPassword));
        }

        [Test]
        public async Task LoginAsync_InvalidPassword_ReturnsFailure()
        {
            var request = new LoginRequestDto
            {
                Email = "test@test.com",
                Password = "Password123"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Password = "hashed-password",
                Role = UserRole.Customer
            };

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _passwordService
                .Setup(x => x.VerifyPassword(request.Password, user.Password))
                .Returns(false);

            var result = await _authenticationService.LoginAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo(ExceptionMessages.InvalidEmailOrPassword));
        }

        [Test]
        public async Task LoginAsync_ValidCredentials_ReturnsTokens()
        {
            var request = new LoginRequestDto
            {
                Email = "test@test.com",
                Password = "Password123"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Password = "hashed-password",
                Role = UserRole.Customer
            };

            var refreshResponse = new RefreshTokenResponseDto
            {
                JwtId = Guid.NewGuid(),
                RefreshToken = "refresh-token"
            };

            _userRepository
                .Setup(x => x.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _passwordService
                .Setup(x => x.VerifyPassword(request.Password, user.Password))
                .Returns(true);

            _jwtService
                .Setup(x => x.GenerateAccessToken(user.Id, user.Email, user.Role))
                .Returns("access-token");

            _jwtService
                .Setup(x => x.GenerateRefreshToken(user.Id))
                .Returns(refreshResponse);

            var result = await _authenticationService.LoginAsync(request);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo(SuccessMessages.UserLoggedIn));
            Assert.That(result.Data.AccessToken, Is.EqualTo("access-token"));
            Assert.That(result.Data.RefreshToken, Is.EqualTo("refresh-token"));
        }

        [Test]
        public async Task LogoutAsync_InvalidRefreshToken_ReturnsFailure()
        {
            var request = new RefreshTokenRequestDto
            {
                RefreshToken = "invalid-token"
            };

            _jwtService
                .Setup(x => x.ValidateRefreshToken(request.RefreshToken))
                .Throws<SecurityTokenException>();

            var result = await _authenticationService.LogoutAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo(ExceptionMessages.InvalidRefreshToken));
        }

        [Test]
        public async Task LogoutAsync_JwtIdDoesNotExist_ReturnsFailure()
        {
            var request = new RefreshTokenRequestDto
            {
                RefreshToken = "refresh-token"
            };

            var principal = new ClaimsPrincipal();
            var jwtId = Guid.NewGuid();

            _jwtService
                .Setup(x => x.ValidateRefreshToken(request.RefreshToken))
                .Returns(principal);

            _jwtService
                .Setup(x => x.GetJwtId(principal))
                .Returns(jwtId);

            _jwtRepository
                .Setup(x => x.JwtIdExistsAsync(jwtId))
                .ReturnsAsync(false);

            var result = await _authenticationService.LogoutAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo(ExceptionMessages.InvalidRefreshToken));
        }

        [Test]
        public async Task LogoutAsync_ValidRefreshToken_RemovesJwtId()
        {
            var request = new RefreshTokenRequestDto
            {
                RefreshToken = "refresh-token"
            };

            var principal = new ClaimsPrincipal();
            var jwtId = Guid.NewGuid();

            _jwtService
                .Setup(x => x.ValidateRefreshToken(request.RefreshToken))
                .Returns(principal);

            _jwtService
                .Setup(x => x.GetJwtId(principal))
                .Returns(jwtId);

            _jwtRepository
                .Setup(x => x.JwtIdExistsAsync(jwtId))
                .ReturnsAsync(true);

            var result = await _authenticationService.LogoutAsync(request);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo(SuccessMessages.UserLoggedOut));
        }

        [Test]
        public async Task TokenRefreshAsync_InvalidRefreshToken_ReturnsFailure()
        {
            var request = new RefreshTokenRequestDto
            {
                RefreshToken = "invalid-token"
            };

            _jwtService
                .Setup(x => x.ValidateRefreshToken(request.RefreshToken))
                .Throws<SecurityTokenException>();

            var result = await _authenticationService.TokenRefreshAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo(ExceptionMessages.InvalidRefreshToken));
        }

        [Test]
        public async Task TokenRefreshAsync_JwtIdDoesNotExist_ReturnsFailure()
        {
            var request = new RefreshTokenRequestDto
            {
                RefreshToken = "refresh-token"
            };

            var principal = new ClaimsPrincipal();
            var jwtId = Guid.NewGuid();

            _jwtService
                .Setup(x => x.ValidateRefreshToken(request.RefreshToken))
                .Returns(principal);

            _jwtService
                .Setup(x => x.GetJwtId(principal))
                .Returns(jwtId);

            _jwtRepository
                .Setup(x => x.JwtIdExistsAsync(jwtId))
                .ReturnsAsync(false);

            var result = await _authenticationService.TokenRefreshAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo(ExceptionMessages.InvalidRefreshToken));
        }

        [Test]
        public async Task TokenRefreshAsync_UserNotFound_RemovesJwtIdAndReturnsFailure()
        {
            var request = new RefreshTokenRequestDto
            {
                RefreshToken = "refresh-token"
            };

            var principal = new ClaimsPrincipal();
            var jwtId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _jwtService
                .Setup(x => x.ValidateRefreshToken(request.RefreshToken))
                .Returns(principal);

            _jwtService
                .Setup(x => x.GetJwtId(principal))
                .Returns(jwtId);

            _jwtService
                .Setup(x => x.GetUserId(principal))
                .Returns(userId);

            _jwtRepository
                .Setup(x => x.JwtIdExistsAsync(jwtId))
                .ReturnsAsync(true);

            _userRepository
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync((User)null);

            var result = await _authenticationService.TokenRefreshAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo(ExceptionMessages.UserNotFound));
        }

        [Test]
        public async Task TokenRefreshAsync_ValidRequest_ReturnsNewAccessToken()
        {
            var request = new RefreshTokenRequestDto
            {
                RefreshToken = "refresh-token"
            };

            var principal = new ClaimsPrincipal();
            var jwtId = Guid.NewGuid();

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@test.com",
                Role = UserRole.Customer,
                IsActive = true
            };

            _jwtService
                .Setup(x => x.ValidateRefreshToken(request.RefreshToken))
                .Returns(principal);

            _jwtService
                .Setup(x => x.GetJwtId(principal))
                .Returns(jwtId);

            _jwtService
                .Setup(x => x.GetUserId(principal))
                .Returns(user.Id);

            _jwtRepository
                .Setup(x => x.JwtIdExistsAsync(jwtId))
                .ReturnsAsync(true);

            _userRepository
                .Setup(x => x.GetUserByIdAsync(user.Id))
                .ReturnsAsync(user);

            _jwtService
                .Setup(x => x.GenerateAccessToken(user.Id, user.Email, user.Role))
                .Returns("new-access-token");

            var result = await _authenticationService.TokenRefreshAsync(request);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo(SuccessMessages.TokenRefreshed));
            Assert.That(result.Data.AccessToken, Is.EqualTo("new-access-token"));
            Assert.That(result.Data.RefreshToken, Is.EqualTo(request.RefreshToken));
        }

        [Test]
        public async Task TokenRefreshAsync_UserInactive_RemovesJwtIdAndReturnsFailure()
        {
            var request = new RefreshTokenRequestDto
            {
                RefreshToken = "refresh-token"
            };

            var principal = new ClaimsPrincipal();
            var jwtId = Guid.NewGuid();

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@test.com",
                Role = UserRole.Customer,
                IsActive = false
            };

            _jwtService
                .Setup(x => x.ValidateRefreshToken(request.RefreshToken))
                .Returns(principal);

            _jwtService
                .Setup(x => x.GetJwtId(principal))
                .Returns(jwtId);

            _jwtService
                .Setup(x => x.GetUserId(principal))
                .Returns(user.Id);

            _jwtRepository
                .Setup(x => x.JwtIdExistsAsync(jwtId))
                .ReturnsAsync(true);

            _userRepository
                .Setup(x => x.GetUserByIdAsync(user.Id))
                .ReturnsAsync(user);

            var result = await _authenticationService.TokenRefreshAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo(ExceptionMessages.UserNotFound));
        }
    }
}
