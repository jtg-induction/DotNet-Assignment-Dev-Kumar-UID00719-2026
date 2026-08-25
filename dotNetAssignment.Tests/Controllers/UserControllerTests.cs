using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http.Results;

using Moq;
using NUnit.Framework;

using dotNetAssignment.Constants;
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
                    new Claim(
                        ClaimTypes.NameIdentifier,
                        _userId.ToString())
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
                Message = SuccessMessages.UserUpdated
            };

            _userService
                .Setup(x => x.UpdateUserAsync(_userId, request))
                .ReturnsAsync(response);

            var result = await _controller.UpdateUser(request);

            var okResult =
                result as OkNegotiatedContentResult<ApiResponseDto<string>>;

            Assert.That(
                result,
                Is.InstanceOf<OkNegotiatedContentResult<ApiResponseDto<string>>>());

            Assert.That(okResult.Content.Success, Is.True);
            Assert.That(
                okResult.Content.Message,
                Is.EqualTo(SuccessMessages.UserUpdated));
        }

        [Test]
        public async Task UpdateUser_WhenServiceReturnsFailure_ReturnsNotFound()
        {
            var request = new UpdateUserRequestDto();

            var response = new ApiResponseDto<string>
            {
                Success = false,
                Message = ExceptionMessages.UserNotFound
            };

            _userService
                .Setup(x => x.UpdateUserAsync(_userId, request))
                .ReturnsAsync(response);

            var result = await _controller.UpdateUser(request);

            var notFound =
                result as NegotiatedContentResult<ApiResponseDto<string>>;

            Assert.That(
                result,
                Is.InstanceOf<NegotiatedContentResult<ApiResponseDto<string>>>());

            Assert.That(
                notFound.StatusCode,
                Is.EqualTo(HttpStatusCode.NotFound));

            Assert.That(notFound.Content.Success, Is.False);
            Assert.That(
                notFound.Content.Message,
                Is.EqualTo(ExceptionMessages.UserNotFound));
        }

        [Test]
        public async Task AddAddress_WhenServiceReturnsSuccess_ReturnsOk()
        {
            var request = new AddAddressRequestDto();

            var response = new ApiResponseDto<string>
            {
                Success = true,
                Message = SuccessMessages.AddressAdded
            };

            _userService
                .Setup(x => x.AddAddressAsync(_userId, request))
                .ReturnsAsync(response);

            var result = await _controller.AddAddress(request);

            var okResult =
                result as OkNegotiatedContentResult<ApiResponseDto<string>>;

            Assert.That(
                result,
                Is.InstanceOf<OkNegotiatedContentResult<ApiResponseDto<string>>>());

            Assert.That(okResult.Content.Success, Is.True);
            Assert.That(
                okResult.Content.Message,
                Is.EqualTo(SuccessMessages.AddressAdded));
        }

        [Test]
        public async Task AddAddress_WhenServiceReturnsFailure_ReturnsNotFound()
        {
            var request = new AddAddressRequestDto();

            var response = new ApiResponseDto<string>
            {
                Success = false,
                Message = ExceptionMessages.UserNotFound
            };

            _userService
                .Setup(x => x.AddAddressAsync(_userId, request))
                .ReturnsAsync(response);

            var result = await _controller.AddAddress(request);

            var notFound =
                result as NegotiatedContentResult<ApiResponseDto<string>>;

            Assert.That(
                result,
                Is.InstanceOf<NegotiatedContentResult<ApiResponseDto<string>>>());

            Assert.That(
                notFound.StatusCode,
                Is.EqualTo(HttpStatusCode.NotFound));

            Assert.That(notFound.Content.Success, Is.False);
            Assert.That(
                notFound.Content.Message,
                Is.EqualTo(ExceptionMessages.UserNotFound));
        }

        [Test]
        public async Task UpdateAddress_WhenServiceReturnsSuccess_ReturnsOk()
        {
            var request = new UpdateAddressRequestDto();

            var response = new ApiResponseDto<string>
            {
                Success = true,
                Message = SuccessMessages.AddressUpdated
            };

            _userService
                .Setup(x => x.UpdateAddressAsync(_userId, request))
                .ReturnsAsync(response);

            var result = await _controller.UpdateAddress(request);

            var okResult =
                result as OkNegotiatedContentResult<ApiResponseDto<string>>;

            Assert.That(
                result,
                Is.InstanceOf<OkNegotiatedContentResult<ApiResponseDto<string>>>());

            Assert.That(okResult.Content.Success, Is.True);
            Assert.That(
                okResult.Content.Message,
                Is.EqualTo(SuccessMessages.AddressUpdated));
        }

        [Test]
        public async Task UpdateAddress_WhenServiceReturnsFailure_ReturnsNotFound()
        {
            var request = new UpdateAddressRequestDto();

            var response = new ApiResponseDto<string>
            {
                Success = false,
                Message = ExceptionMessages.AddressNotFound
            };

            _userService
                .Setup(x => x.UpdateAddressAsync(_userId, request))
                .ReturnsAsync(response);

            var result = await _controller.UpdateAddress(request);

            var notFound =
                result as NegotiatedContentResult<ApiResponseDto<string>>;

            Assert.That(
                result,
                Is.InstanceOf<NegotiatedContentResult<ApiResponseDto<string>>>());

            Assert.That(
                notFound.StatusCode,
                Is.EqualTo(HttpStatusCode.NotFound));

            Assert.That(notFound.Content.Success, Is.False);
            Assert.That(
                notFound.Content.Message,
                Is.EqualTo(ExceptionMessages.AddressNotFound));
        }

        [Test]
        public async Task ChangePassword_WhenServiceReturnsSuccess_ReturnsOk()
        {
            var request = new ChangePasswordRequestDto();

            var response = new ApiResponseDto<string>
            {
                Success = true,
                Message = SuccessMessages.PasswordChanged
            };

            _userService
                .Setup(x => x.ChangePasswordAsync(_userId, request))
                .ReturnsAsync(response);

            var result = await _controller.ChangePassword(request);

            var okResult =
                result as OkNegotiatedContentResult<ApiResponseDto<string>>;

            Assert.That(
                result,
                Is.InstanceOf<OkNegotiatedContentResult<ApiResponseDto<string>>>());

            Assert.That(okResult.Content.Success, Is.True);
            Assert.That(
                okResult.Content.Message,
                Is.EqualTo(SuccessMessages.PasswordChanged));
        }

        [Test]
        public async Task ChangePassword_WhenUserDoesNotExist_ReturnsNotFound()
        {
            var request = new ChangePasswordRequestDto();

            var response = new ApiResponseDto<string>
            {
                Success = false,
                Message = ExceptionMessages.UserNotFound
            };

            _userService
                .Setup(x => x.ChangePasswordAsync(_userId, request))
                .ReturnsAsync(response);

            var result = await _controller.ChangePassword(request);

            var notFound =
                result as NegotiatedContentResult<ApiResponseDto<string>>;

            Assert.That(
                result,
                Is.InstanceOf<NegotiatedContentResult<ApiResponseDto<string>>>());

            Assert.That(
                notFound.StatusCode,
                Is.EqualTo(HttpStatusCode.NotFound));

            Assert.That(notFound.Content.Success, Is.False);
            Assert.That(
                notFound.Content.Message,
                Is.EqualTo(ExceptionMessages.UserNotFound));
        }

        [Test]
        public async Task ChangePassword_WhenPasswordIsWrong_ReturnsUnauthorized()
        {
            var request = new ChangePasswordRequestDto();

            var response = new ApiResponseDto<string>
            {
                Success = false,
                Message = ExceptionMessages.WrongPassword
            };

            _userService
                .Setup(x => x.ChangePasswordAsync(_userId, request))
                .ReturnsAsync(response);

            var result = await _controller.ChangePassword(request);

            var unauthorized =
                result as NegotiatedContentResult<ApiResponseDto<string>>;

            Assert.That(
                result,
                Is.InstanceOf<NegotiatedContentResult<ApiResponseDto<string>>>());

            Assert.That(
                unauthorized.StatusCode,
                Is.EqualTo(HttpStatusCode.Unauthorized));

            Assert.That(unauthorized.Content.Success, Is.False);
            Assert.That(
                unauthorized.Content.Message,
                Is.EqualTo(ExceptionMessages.WrongPassword));
        }

        [Test]
        public async Task DeactivateUser_WhenServiceReturnsSuccess_ReturnsOk()
        {
            var request = new DeactivateAccountRequestDto();
            var response = new ApiResponseDto<string>
            {
                Success = true,
                Message = SuccessMessages.UserDeactivated
            };

            _userService
                .Setup(x => x.DeactivateUserAsync(_userId, request))
                .ReturnsAsync(response);

            var result = await _controller.DeactivateUser(request);

            var okResult =
                result as OkNegotiatedContentResult<ApiResponseDto<string>>;

            Assert.That(
                result,
                Is.InstanceOf<OkNegotiatedContentResult<ApiResponseDto<string>>>());

            Assert.That(okResult.Content.Success, Is.True);
            Assert.That(
                okResult.Content.Message,
                Is.EqualTo(SuccessMessages.UserDeactivated));
        }

        [Test]
        public async Task DeactivateUser_WhenServiceReturnsFailure_ReturnsNotFound()
        {
            var request = new DeactivateAccountRequestDto();
            var response = new ApiResponseDto<string>
            {
                Success = false,
                Message = ExceptionMessages.UserNotFound
            };

            _userService
                .Setup(x => x.DeactivateUserAsync(_userId, request))
                .ReturnsAsync(response);

            var result = await _controller.DeactivateUser(request);

            var notFound =
                result as NegotiatedContentResult<ApiResponseDto<string>>;

            Assert.That(
                result,
                Is.InstanceOf<NegotiatedContentResult<ApiResponseDto<string>>>());

            Assert.That(
                notFound.StatusCode,
                Is.EqualTo(HttpStatusCode.NotFound));

            Assert.That(notFound.Content.Success, Is.False);
            Assert.That(
                notFound.Content.Message,
                Is.EqualTo(ExceptionMessages.UserNotFound));
        }
    }
}
