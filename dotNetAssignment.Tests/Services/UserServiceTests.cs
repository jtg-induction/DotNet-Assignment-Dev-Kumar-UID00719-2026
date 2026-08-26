using dotNetAssignment.Constants;
using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.DTO.Address;
using dotNetAssignment.Models.DTO.SignUp;
using dotNetAssignment.Models.Entities;
using dotNetAssignment.Repositories.Jwt;
using dotNetAssignment.Repositories.UserRepo;
using dotNetAssignment.Services.Implementations;
using dotNetAssignment.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dotNetAssignment.Tests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<IPasswordService> _passwordService;
        private Mock<IJwtService> _jwtService;
        private Mock<IJwtRepository> _jwtRepository;
        private UserService _userService;
        private Guid _userId;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _passwordService = new Mock<IPasswordService>();
            _jwtService = new Mock<IJwtService>();
            _jwtRepository = new Mock<IJwtRepository>();

            _userService = new UserService(
                _userRepository.Object,
                _passwordService.Object,
                _jwtService.Object,
                _jwtRepository.Object);

            _userId = Guid.NewGuid();
        }


        [Test]
        public async Task UpdateUserAsync_WhenUserExists_UpdatesUser()
        {
            var user = new User
            {
                Id = _userId,
                Name = "Old Name",
                PhoneNumber = "9999999999",
                IsActive = true
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            var request = new UpdateUserRequestDto
            {
                Name = "New Name",
                PhoneNumber = "8888888888"
            };

            var result = await _userService.UpdateUserAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(SuccessMessages.UserUpdated));
                Assert.That(user.Name, Is.EqualTo("New Name"));
                Assert.That(user.PhoneNumber, Is.EqualTo("8888888888"));
            });

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }


        [Test]
        public async Task UpdateUserAsync_WhenPhoneNumberIsSame_ReturnsFailure()
        {
            var user = new User
            {
                Id = _userId,
                Name = "Dev",
                PhoneNumber = "9999999999",
                IsActive = true
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            var request = new UpdateUserRequestDto
            {
                PhoneNumber = "9999999999"
            };

            var result =
                await _userService.UpdateUserAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(
                    result.Message,
                    Is.EqualTo(ExceptionMessages.SamePhoneNumber));
            });

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }


        [Test]
        public async Task UpdateUserAsync_WhenNothingIsProvided_ReturnsFailure()
        {
            var user = new User
            {
                Id = _userId,
                Name = "Dev",
                PhoneNumber = "9999999999",
                IsActive = true
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            var request = new UpdateUserRequestDto();

            var result =
                await _userService.UpdateUserAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(
                    result.Message,
                    Is.EqualTo(ExceptionMessages.UserNotUpdated));
            });

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }


        [Test]
        public async Task AddAddressAsync_WhenUserExists_AddsAddress()
        {
            var user = new User
            {
                Id = _userId,
                IsActive = true
            };

            var request = new AddAddressRequestDto
            {
                LineOne = "123 Main Street",
                Landmark = "Near Mall",
                Pincode = "110001",
                City = "Delhi",
                State = "Delhi"
            };

            var result = await _userService.AddAddressAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(SuccessMessages.AddressAdded));
            });

            _userRepository.Verify(
                x => x.AddAddress(It.Is<UserAddress>(address =>
                    address.UserId == _userId &&
                    address.LineOne == request.LineOne &&
                    address.Landmark == request.Landmark &&
                    address.Pincode == request.Pincode &&
                    address.City == request.City &&
                    address.State == request.State)),
                Times.Once);

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }



        [Test]
        public async Task UpdateAddressAsync_WhenAddressDoesNotExist_ReturnsFailure()
        {
            var user = new User
            {
                Id = _userId,
                IsActive = true
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            var addressId = Guid.NewGuid();

            _userRepository
                .Setup(x => x.GetAddressByIdAsync(addressId))
                .ReturnsAsync((UserAddress)null);

            var request = new UpdateAddressRequestDto
            {
                AddressId = addressId,
                City = "Mumbai"
            };

            var result = await _userService.UpdateAddressAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ExceptionMessages.AddressNotFound));
            });
        }


        [Test]
        public async Task UpdateAddressAsync_WhenAddressBelongsToAnotherUser_ReturnsFailure()
        {
            var addressId = Guid.NewGuid();

            var address = new UserAddress
            {
                Id = addressId,
                UserId = Guid.NewGuid()
            };

            _userRepository
                .Setup(x => x.GetAddressByIdAsync(addressId))
                .ReturnsAsync(address);

            var request = new UpdateAddressRequestDto
            {
                AddressId = addressId,
                City = "Mumbai"
            };

            var result =
                await _userService.UpdateAddressAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(
                    result.Message,
                    Is.EqualTo(ExceptionMessages.AddressNotFound));
            });

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }


        [Test]
        public async Task UpdateAddressAsync_WhenNoFieldsAreProvided_ReturnsFailure()
        {
            var address = new UserAddress
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                City = "Delhi"
            };

            _userRepository
                .Setup(x => x.GetAddressByIdAsync(address.Id))
                .ReturnsAsync(address);

            var request = new UpdateAddressRequestDto
            {
                AddressId = address.Id
            };

            var result =
                await _userService.UpdateAddressAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(
                    result.Message,
                    Is.EqualTo(ExceptionMessages.AddressNotUpdated));
            });

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }


        [Test]
        public async Task UpdateAddressAsync_WhenAddressExists_UpdatesAddress()
        {
            var user = new User
            {
                Id = _userId,
                IsActive = true
            };

            var address = new UserAddress
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                LineOne = "Old Street",
                City = "Delhi",
                State = "Delhi",
                Pincode = "110001"
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            _userRepository
                .Setup(x => x.GetAddressByIdAsync(address.Id))
                .ReturnsAsync(address);

            var request = new UpdateAddressRequestDto
            {
                AddressId = address.Id,
                LineOne = "New Street",
                City = "Mumbai",
                State = "Maharashtra",
                Pincode = "400001"
            };

            var result = await _userService.UpdateAddressAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(SuccessMessages.AddressUpdated));
                Assert.That(address.LineOne, Is.EqualTo("New Street"));
                Assert.That(address.City, Is.EqualTo("Mumbai"));
                Assert.That(address.State, Is.EqualTo("Maharashtra"));
                Assert.That(address.Pincode, Is.EqualTo("400001"));
            });

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }


        [Test]
        public async Task ChangePasswordAsync_WhenOldPasswordIsWrong_ReturnsFailure()
        {
            var user = new User
            {
                Id = _userId,
                Password = "hashedPassword",
                IsActive = true
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            _passwordService
                .Setup(x => x.VerifyPassword(
                    "wrongPassword",
                    "hashedPassword"))
                .Returns(false);

            var request = new ChangePasswordRequestDto
            {
                OldPassword = "wrongPassword",
                NewPassword = "newPassword"
            };

            var result = await _userService.ChangePasswordAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ExceptionMessages.WrongPassword));
                Assert.That(user.Password, Is.EqualTo("hashedPassword"));
            });
        }

        [Test]
        public async Task ChangePasswordAsync_WhenNewPasswordIsSameAsOld_ReturnsFailure()
        {
            var user = new User
            {
                Id = _userId,
                Password = "hashedPassword",
                IsActive = true
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            _passwordService
                .Setup(x => x.VerifyPassword(
                    "oldPassword",
                    "hashedPassword"))
                .Returns(true);

            var request = new ChangePasswordRequestDto
            {
                OldPassword = "oldPassword",
                NewPassword = "oldPassword"
            };

            var result = await _userService.ChangePasswordAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ExceptionMessages.SamePassword));
                Assert.That(user.Password, Is.EqualTo("hashedPassword"));
            });
        }

        [Test]
        public async Task ChangePasswordAsync_WhenPasswordIsValid_ChangesPassword()
        {
            var user = new User
            {
                Id = _userId,
                Password = "oldHashedPassword",
                IsActive = true
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            _passwordService
                .Setup(x => x.VerifyPassword(
                    "oldPassword",
                    "oldHashedPassword"))
                .Returns(true);

            _passwordService
                .Setup(x => x.VerifyPassword(
                    "newPassword",
                    "oldHashedPassword"))
                .Returns(false);

            _passwordService
                .Setup(x => x.HashPassword("newPassword"))
                .Returns("newHashedPassword");

            var request = new ChangePasswordRequestDto
            {
                OldPassword = "oldPassword",
                NewPassword = "newPassword"
            };

            var result = await _userService.ChangePasswordAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(SuccessMessages.PasswordChanged));
                Assert.That(user.Password, Is.EqualTo("newHashedPassword"));
            });

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }


        [Test]
        public async Task DeactivateUserAsync_WhenUserExists_DeactivatesUser()
        {
            var request = new DeactivateAccountRequestDto();
            var user = new User
            {
                Id = _userId,
                IsActive = true
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            var principal = new ClaimsPrincipal();
            var jwtId = Guid.NewGuid();

            _jwtService
                .Setup(x => x.ValidateRefreshToken(request.RefreshToken))
                .Returns(principal);

            _jwtService
                .Setup(x => x.GetUserId(principal))
                .Returns(_userId);

            _jwtService
                .Setup(x => x.GetJwtId(principal))
                .Returns(jwtId);

            var result = await _userService.DeactivateUserAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(SuccessMessages.UserDeactivated));
                Assert.That(user.IsActive, Is.False);
            });

            _jwtRepository.Verify(
                x => x.RemoveJwtIdAsync(jwtId),
                Times.Once);

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);

            _jwtRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Test]
        public async Task DeactivateUserAsync_WhenRefreshTokenIsInvalid_ReturnsFailure()
        {
            var request = new DeactivateAccountRequestDto
            {
                RefreshToken = "invalid-token"
            };

            var user = new User
            {
                Id = _userId,
                IsActive = true
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            _jwtService
                .Setup(x => x.ValidateRefreshToken(request.RefreshToken))
                .Throws<SecurityTokenException>();

            var result =
                await _userService.DeactivateUserAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(
                    result.Message,
                    Is.EqualTo(ExceptionMessages.InvalidRefreshToken));

                Assert.That(user.IsActive, Is.True);
            });

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);

            _jwtRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }


        [Test]
        public async Task DeactivateUserAsync_WhenTokenBelongsToAnotherUser_ReturnsFailure()
        {
            var request = new DeactivateAccountRequestDto
            {
                RefreshToken = "valid-token"
            };

            var user = new User
            {
                Id = _userId,
                IsActive = true
            };

            var principal = new ClaimsPrincipal();

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            _jwtService
                .Setup(x => x.ValidateRefreshToken(request.RefreshToken))
                .Returns(principal);

            _jwtService
                .Setup(x => x.GetUserId(principal))
                .Returns(Guid.NewGuid());

            var result =
                await _userService.DeactivateUserAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(
                    result.Message,
                    Is.EqualTo(ExceptionMessages.InvalidRefreshToken));

                Assert.That(user.IsActive, Is.True);
            });

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);

            _jwtRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }
    }
}
