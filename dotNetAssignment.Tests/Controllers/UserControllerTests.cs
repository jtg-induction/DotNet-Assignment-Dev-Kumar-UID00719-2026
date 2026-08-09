using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Net;
using System.Web.Http.Results;

using Moq;
using NUnit.Framework;

using dotNetAssignment.Controllers;
using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.DTO.Address;
using dotNetAssignment.Models.DTO.SignUp;
using dotNetAssignment.Services.Interfaces;

namespace dotNetAssignment.Tests.Controllers
{
    [TestFixture]
    public class UserControllerTests
    {
        private Mock<IUserService> _userService;
        private UserController _controller;
        private Guid _userId;

        [SetUp]
        public void Setup()
        {
            _userService = new Mock<IUserService>();
            _controller = new UserController(_userService.Object);

            _userId = Guid.NewGuid();

            var identity = new ClaimsIdentity(
                new[]
                {
                    new Claim("userid", _userId.ToString())
                },
                "TestAuthentication");

            _controller.User = new ClaimsPrincipal(identity);
        }

        [Test]
        public async Task UpdateUser_WhenServiceReturnsSuccess_ReturnsOk()
        {
            var request = new UpdateUserRequestDto();

            var response = new ApiResponseDto<string>
            {
                Success = true,
                Message = "User updated successfully."
            };

            _userService
                .Setup(x => x.UpdateUserAsync(_userId, request))
                .ReturnsAsync(response);

            var result = await _controller.UpdateUser(request);

            var okResult =
                result as OkNegotiatedContentResult<ApiResponseDto<string>>;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<OkNegotiatedContentResult<ApiResponseDto<string>>>());
                Assert.That(okResult.Content.Success, Is.True);
                Assert.That(okResult.Content.Message, Is.EqualTo("User updated successfully."));
            });
        }

        [Test]
        public async Task UpdateUser_WhenServiceReturnsFailure_ReturnsBadRequest()
        {
            var request = new UpdateUserRequestDto();

            var response = new ApiResponseDto<string>
            {
                Success = false,
                Message = "User not found."
            };

            _userService
                .Setup(x => x.UpdateUserAsync(_userId, request))
                .ReturnsAsync(response);

            var result = await _controller.UpdateUser(request);

            var badRequest =
                result as NegotiatedContentResult<ApiResponseDto<string>>;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<NegotiatedContentResult<ApiResponseDto<string>>>());
                Assert.That(badRequest.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(badRequest.Content.Success, Is.False);
                Assert.That(badRequest.Content.Message, Is.EqualTo("User not found."));
            });
        }

        [Test]
        public async Task UpdateUser_WithInvalidModelState_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Name", "Invalid name");

            var result = await _controller.UpdateUser(new UpdateUserRequestDto());

            Assert.That(result, Is.InstanceOf<InvalidModelStateResult>());
        }

        [Test]
        public async Task AddAddress_WhenServiceReturnsSuccess_ReturnsOk()
        {
            var request = new AddAddressRequestDto();

            var response = new ApiResponseDto<string>
            {
                Success = true,
                Message = "Address added successfully."
            };

            _userService
                .Setup(x => x.AddAddressAsync(_userId, request))
                .ReturnsAsync(response);

            var result = await _controller.AddAddress(request);

            var okResult =
                result as OkNegotiatedContentResult<ApiResponseDto<string>>;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<OkNegotiatedContentResult<ApiResponseDto<string>>>());
                Assert.That(okResult.Content.Success, Is.True);
                Assert.That(okResult.Content.Message, Is.EqualTo("Address added successfully."));
            });
        }

        [Test]
        public async Task AddAddress_WhenServiceReturnsFailure_ReturnsBadRequest()
        {
            var request = new AddAddressRequestDto();

            var response = new ApiResponseDto<string>
            {
                Success = false,
                Message = "Address already exists."
            };

            _userService
                .Setup(x => x.AddAddressAsync(_userId, request))
                .ReturnsAsync(response);

            var result = await _controller.AddAddress(request);

            var badRequest =
                result as NegotiatedContentResult<ApiResponseDto<string>>;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<NegotiatedContentResult<ApiResponseDto<string>>>());
                Assert.That(badRequest.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(badRequest.Content.Success, Is.False);
                Assert.That(badRequest.Content.Message, Is.EqualTo("Address already exists."));
            });
        }

        [Test]
        public async Task AddAddress_WithInvalidModelState_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("LineOne", "Required");

            var result = await _controller.AddAddress(new AddAddressRequestDto());

            Assert.That(result, Is.InstanceOf<InvalidModelStateResult>());
        }

        [Test]
        public async Task UpdateAddress_WhenServiceReturnsSuccess_ReturnsOk()
        {
            var request = new UpdateAddressRequestDto();

            var response = new ApiResponseDto<string>
            {
                Success = true,
                Message = "Address updated successfully."
            };

            _userService
                .Setup(x => x.UpdateAddressAsync(_userId, request))
                .ReturnsAsync(response);

            var result = await _controller.UpdateAddress(request);

            var okResult =
                result as OkNegotiatedContentResult<ApiResponseDto<string>>;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<OkNegotiatedContentResult<ApiResponseDto<string>>>());
                Assert.That(okResult.Content.Success, Is.True);
                Assert.That(okResult.Content.Message, Is.EqualTo("Address updated successfully."));
            });
        }

        [Test]
        public async Task UpdateAddress_WhenServiceReturnsFailure_ReturnsBadRequest()
        {
            var request = new UpdateAddressRequestDto();

            var response = new ApiResponseDto<string>
            {
                Success = false,
                Message = "Address not found."
            };

            _userService
                .Setup(x => x.UpdateAddressAsync(_userId, request))
                .ReturnsAsync(response);

            var result = await _controller.UpdateAddress(request);

            var badRequest =
                result as NegotiatedContentResult<ApiResponseDto<string>>;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<NegotiatedContentResult<ApiResponseDto<string>>>());
                Assert.That(badRequest.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(badRequest.Content.Success, Is.False);
                Assert.That(badRequest.Content.Message, Is.EqualTo("Address not found."));
            });
        }

        [Test]
        public async Task UpdateAddress_WithInvalidModelState_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("AddressId", "Required");

            var result = await _controller.UpdateAddress(new UpdateAddressRequestDto());

            Assert.That(result, Is.InstanceOf<InvalidModelStateResult>());
        }

        [Test]
        public async Task ChangePassword_WhenServiceReturnsSuccess_ReturnsOk()
        {
            var request = new ChangePasswordRequestDto();

            var response = new ApiResponseDto<string>
            {
                Success = true,
                Message = "Password changed successfully."
            };

            _userService
                .Setup(x => x.ChangePasswordAsync(_userId, request))
                .ReturnsAsync(response);

            var result = await _controller.ChangePassword(request);

            var okResult =
                result as OkNegotiatedContentResult<ApiResponseDto<string>>;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<OkNegotiatedContentResult<ApiResponseDto<string>>>());
                Assert.That(okResult.Content.Success, Is.True);
                Assert.That(okResult.Content.Message, Is.EqualTo("Password changed successfully."));
            });
        }

        [Test]
        public async Task ChangePassword_WhenServiceReturnsFailure_ReturnsBadRequest()
        {
            var request = new ChangePasswordRequestDto();

            var response = new ApiResponseDto<string>
            {
                Success = false,
                Message = "Current password is incorrect."
            };

            _userService
                .Setup(x => x.ChangePasswordAsync(_userId, request))
                .ReturnsAsync(response);

            var result = await _controller.ChangePassword(request);

            var badRequest =
                result as NegotiatedContentResult<ApiResponseDto<string>>;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<NegotiatedContentResult<ApiResponseDto<string>>>());
                Assert.That(badRequest.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(badRequest.Content.Success, Is.False);
                Assert.That(badRequest.Content.Message, Is.EqualTo("Current password is incorrect."));
            });
        }

        [Test]
        public async Task ChangePassword_WithInvalidModelState_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("CurrentPassword", "Required");

            var result = await _controller.ChangePassword(new ChangePasswordRequestDto());

            Assert.That(result, Is.InstanceOf<InvalidModelStateResult>());
        }

        [Test]
        public async Task DeactivateUser_WhenServiceReturnsSuccess_ReturnsOk()
        {
            var response = new ApiResponseDto<string>
            {
                Success = true,
                Message = "User deactivated successfully."
            };

            _userService
                .Setup(x => x.DeactivateUserAsync(_userId))
                .ReturnsAsync(response);

            var result = await _controller.DeactivateUSer();

            var okResult =
                result as OkNegotiatedContentResult<ApiResponseDto<string>>;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<OkNegotiatedContentResult<ApiResponseDto<string>>>());
                Assert.That(okResult.Content.Success, Is.True);
                Assert.That(okResult.Content.Message, Is.EqualTo("User deactivated successfully."));
            });
        }

        [Test]
        public async Task DeactivateUser_WhenServiceReturnsFailure_ReturnsBadRequest()
        {
            var response = new ApiResponseDto<string>
            {
                Success = false,
                Message = "User not found."
            };

            _userService
                .Setup(x => x.DeactivateUserAsync(_userId))
                .ReturnsAsync(response);

            var result = await _controller.DeactivateUSer();

            var badRequest =
                result as NegotiatedContentResult<ApiResponseDto<string>>;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<NegotiatedContentResult<ApiResponseDto<string>>>());
                Assert.That(badRequest.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(badRequest.Content.Success, Is.False);
                Assert.That(badRequest.Content.Message, Is.EqualTo("User not found."));
            });
        }
    }
}