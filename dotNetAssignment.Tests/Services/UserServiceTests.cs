using System;
using System.Threading.Tasks;

using Moq;
using NUnit.Framework;

using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.DTO.Address;
using dotNetAssignment.Models.Entities;
using dotNetAssignment.Repositories.User;
using dotNetAssignment.Services.Implementations;
using dotNetAssignment.Services.Interfaces;

namespace dotNetAssignment.Tests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<IPasswordService> _passwordService;
        private UserService _userService;
        private Guid _userId;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _passwordService = new Mock<IPasswordService>();

            _userService = new UserService(
                _userRepository.Object,
                _passwordService.Object);

            _userId = Guid.NewGuid();
        }

        [Test]
        public async Task UpdateUserAsync_WhenUserDoesNotExist_ReturnsFailure()
        {
            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync((Users)null);

            var request = new UpdateUserRequestDto
            {
                Name = "Updated Name",
                PhoneNumber = "9876543210"
            };

            var result = await _userService.UpdateUserAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("User not found."));
            });
        }

        [Test]
        public async Task UpdateUserAsync_WhenUserIsInactive_ReturnsFailure()
        {
            var user = new Users
            {
                Id = _userId,
                IsActive = false
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            var request = new UpdateUserRequestDto
            {
                Name = "Updated Name",
                PhoneNumber = "9876543210"
            };

            var result = await _userService.UpdateUserAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("User not found."));
            });
        }

        [Test]
        public async Task UpdateUserAsync_WhenValidRequest_UpdatesUser()
        {
            var user = new Users
            {
                Id = _userId,
                Name = "Old Name",
                PhoneNumber = "1111111111",
                IsActive = true
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            var request = new UpdateUserRequestDto
            {
                Name = "New Name",
                PhoneNumber = "2222222222"
            };

            var result = await _userService.UpdateUserAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo("User updated successfully"));
                Assert.That(user.Name, Is.EqualTo("New Name"));
                Assert.That(user.PhoneNumber, Is.EqualTo("2222222222"));
            });
        }

        [Test]
        public async Task AddAddressAsync_WhenUserDoesNotExist_ReturnsFailure()
        {
            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync((Users)null);

            var request = new AddAddressRequestDto
            {
                LineOne = "123 Street",
                Landmark = "Near Park",
                Pincode = "110001",
                City = "Delhi",
                State = "Delhi"
            };

            var result = await _userService.AddAddressAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("User not found."));
            });
        }

        [Test]
        public async Task AddAddressAsync_WhenUserIsInactive_ReturnsFailure()
        {
            var user = new Users
            {
                Id = _userId,
                IsActive = false
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            var request = new AddAddressRequestDto
            {
                LineOne = "123 Street",
                Landmark = "Near Park",
                Pincode = "110001",
                City = "Delhi",
                State = "Delhi"
            };

            var result = await _userService.AddAddressAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("User not found."));
            });
        }

        [Test]
        public async Task AddAddressAsync_WhenValidRequest_AddsAddressAndReturnsSuccess()
        {
            var user = new Users
            {
                Id = _userId,
                IsActive = true
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            var request = new AddAddressRequestDto
            {
                LineOne = "123 Street",
                Landmark = "Near Park",
                Pincode = "110001",
                City = "Delhi",
                State = "Delhi"
            };

            var result = await _userService.AddAddressAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo("Address added successfully."));
            });
        }

        [Test]
        public async Task UpdateAddressAsync_WhenUserDoesNotExist_ReturnsFailure()
        {
            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync((Users)null);

            var request = new UpdateAddressRequestDto
            {
                AddressId = Guid.NewGuid(),
                City = "Mumbai"
            };

            var result = await _userService.UpdateAddressAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("User not found."));
            });
        }

        [Test]
        public async Task UpdateAddressAsync_WhenUserIsInactive_ReturnsFailure()
        {
            var user = new Users
            {
                Id = _userId,
                IsActive = false
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            var request = new UpdateAddressRequestDto
            {
                AddressId = Guid.NewGuid(),
                City = "Mumbai"
            };

            var result = await _userService.UpdateAddressAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("User not found."));
            });
        }

        [Test]
        public async Task UpdateAddressAsync_WhenAddressDoesNotExist_ReturnsFailure()
        {
            var user = new Users
            {
                Id = _userId,
                IsActive = true
            };

            var addressId = Guid.NewGuid();

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            _userRepository
                .Setup(x => x.GetAddressByIdAsync(addressId))
                .ReturnsAsync((UserAddresses)null);

            var request = new UpdateAddressRequestDto
            {
                AddressId = addressId,
                City = "Mumbai"
            };

            var result = await _userService.UpdateAddressAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("Address not found."));
            });
        }

        [Test]
        public async Task UpdateAddressAsync_WhenValidRequest_UpdatesAddress()
        {
            var user = new Users
            {
                Id = _userId,
                IsActive = true
            };

            var addressId = Guid.NewGuid();

            var address = new UserAddresses
            {
                Id = addressId,
                UserId = _userId,
                LineOne = "Old Street",
                Landmark = "Old Landmark",
                Pincode = "110001",
                City = "Dholakpur",
                State = "Delhi"
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            _userRepository
                .Setup(x => x.GetAddressByIdAsync(addressId))
                .ReturnsAsync(address);

            var request = new UpdateAddressRequestDto
            {
                AddressId = addressId,
                LineOne = "New Street",
                Landmark = "New Landmark",
                Pincode = "400001",
                City = "Pehelwanpur",
                State = "Maharashtra"
            };

            var result = await _userService.UpdateAddressAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo("Address updated successfully."));
                Assert.That(address.LineOne, Is.EqualTo("New Street"));
                Assert.That(address.Landmark, Is.EqualTo("New Landmark"));
                Assert.That(address.Pincode, Is.EqualTo("400001"));
                Assert.That(address.City, Is.EqualTo("Pehelwanpur"));
                Assert.That(address.State, Is.EqualTo("Maharashtra"));
            });
        }

        [Test]
        public async Task ChangePasswordAsync_WhenUserDoesNotExist_ReturnsFailure()
        {
            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync((Users)null);

            var request = new ChangePasswordRequestDto
            {
                OldPassword = "oldPassword",
                NewPassword = "newPassword"
            };

            var result = await _userService.ChangePasswordAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("User not found."));
            });
        }

        [Test]
        public async Task ChangePasswordAsync_WhenUserIsInactive_ReturnsFailure()
        {
            var user = new Users
            {
                Id = _userId,
                Password = "hashedPassword",
                IsActive = false
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            var request = new ChangePasswordRequestDto
            {
                OldPassword = "oldPassword",
                NewPassword = "newPassword"
            };

            var result = await _userService.ChangePasswordAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("User not found."));
            });
        }

        [Test]
        public async Task ChangePasswordAsync_WhenOldPasswordIsWrong_ReturnsFailure()
        {
            var user = new Users
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
                    "hashedPassword",
                    "wrongPassword"))
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
                Assert.That(result.Message, Is.EqualTo("Wrong password."));
                Assert.That(user.Password, Is.EqualTo("hashedPassword"));
            });
        }

        [Test]
        public async Task ChangePasswordAsync_WhenOldPasswordIsCorrect_ChangesPassword()
        {
            var user = new Users
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
                    "oldHashedPassword",
                    "oldPassword"))
                .Returns(true);

            var request = new ChangePasswordRequestDto
            {
                OldPassword = "oldPassword",
                NewPassword = "newPassword"
            };

            var result = await _userService.ChangePasswordAsync(_userId, request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo("Password changed successfully."));
                Assert.That(user.Password, Is.EqualTo("newPassword"));
            });
        }

        [Test]
        public async Task DeactivateUserAsync_WhenUserDoesNotExist_ReturnsFailure()
        {
            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync((Users)null);

            var result = await _userService.DeactivateUserAsync(_userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("User not found."));
            });
        }

        [Test]
        public async Task DeactivateUserAsync_WhenUserIsInactive_ReturnsFailure()
        {
            var user = new Users
            {
                Id = _userId,
                IsActive = false
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            var result = await _userService.DeactivateUserAsync(_userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo("User not found."));
            });
        }

        [Test]
        public async Task DeactivateUserAsync_WhenUserIsActive_DeactivatesUser()
        {
            var user = new Users
            {
                Id = _userId,
                IsActive = true
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            var result = await _userService.DeactivateUserAsync(_userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo("User deactivated successfully."));
                Assert.That(user.IsActive, Is.False);
            });
        }
    }
}