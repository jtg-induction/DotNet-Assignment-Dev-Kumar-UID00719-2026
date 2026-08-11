using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Results;

using Moq;
using NUnit.Framework;

using dotNetAssignment.Controllers;
using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.DTO.Login;
using dotNetAssignment.Models.DTO.SignUp;
using dotNetAssignment.Services.Interfaces;
using Moq;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace dotNetAssignment.Tests.Controllers
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IAuthenticationService> _authenticationService;

        private AuthController _controller;

        [SetUp]
        public void Setup()
        {
            _authenticationService = new Mock<IAuthenticationService>();
            _controller = new AuthController(_authenticationService.Object);
        }

        [Test]
        public async Task Signup_WhenServiceReturnsSuccess_ReturnsOk()
        {
            var request = new SignupRequestDto();
            var response = new ApiResponseDto<AuthenticationResponseDto>
            {
                Success = true,
                Message = "User registered successfully",
                Data = new AuthenticationResponseDto
                {
                    AccessToken = "access-token",
                    RefreshToken = "refresh-token"
                }
            };

            _authenticationService
                .Setup(x => x.SignupAsync(request))
                .ReturnsAsync(response);

            var result = await _controller.Signup(request);
            var okResult = result as OkNegotiatedContentResult<ApiResponseDto<AuthenticationResponseDto>>;

            Assert.That(result, Is.InstanceOf<OkNegotiatedContentResult<ApiResponseDto<AuthenticationResponseDto>>>());
            Assert.That(okResult.Content.Success, Is.True);
            Assert.That(okResult.Content.Message, Is.EqualTo("User registered successfully"));
        }

        [Test]
        public async Task Signup_WhenServiceReturnsFailure_ReturnsBadRequest()
        {
            var request = new SignupRequestDto();
            var response = new ApiResponseDto<AuthenticationResponseDto>
            {
                Success = false,
                Message = "Email already exists"
            };

            _authenticationService
                .Setup(x => x.SignupAsync(request))
                .ReturnsAsync(response);

            var result = await _controller.Signup(request);
            var badRequest = result as NegotiatedContentResult<ApiResponseDto<AuthenticationResponseDto>>;

            Assert.That(result, Is.InstanceOf<NegotiatedContentResult<ApiResponseDto<AuthenticationResponseDto>>>());
            Assert.That(badRequest.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(badRequest.Content.Success, Is.False);
        }

        [Test]
        public async Task Signup_WithInvalidModelState_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Email", "Required");

            var result = await _controller.Signup(new SignupRequestDto());

            Assert.That(result, Is.InstanceOf<InvalidModelStateResult>());
        }

        [Test]
        public async Task Login_WhenServiceReturnsSuccess_ReturnsOk()
        {
            var request = new LoginRequestDto();

            var response = new ApiResponseDto<AuthenticationResponseDto>
            {
                Success = true,
                Message = "Login successful",
                Data = new AuthenticationResponseDto
                {
                    AccessToken = "access-token",
                    RefreshToken = "refresh-token"
                }
            };

            _authenticationService
                .Setup(x => x.LoginAsync(request))
                .ReturnsAsync(response);

            var result = await _controller.Login(request);

            var okResult = (OkNegotiatedContentResult<ApiResponseDto<AuthenticationResponseDto>>)result;

            Assert.Multiple(() =>
            {
                Assert.That(okResult.Content.Success, Is.True);
                Assert.That(okResult.Content.Message, Is.EqualTo("Login successful"));
                Assert.That(okResult.Content.Data.AccessToken, Is.EqualTo("access-token"));
                Assert.That(okResult.Content.Data.RefreshToken, Is.EqualTo("refresh-token"));
            });

        }

        [Test]
        public async Task Login_WhenServiceReturnsFailure_ReturnsBadRequest()
        {
            var request = new LoginRequestDto();
            var response = new ApiResponseDto<AuthenticationResponseDto>
            {
                Success = false,
                Message = "Invalid email or password"
            };

            _authenticationService
                .Setup(x => x.LoginAsync(request))
                .ReturnsAsync(response);

            var result = await _controller.Login(request);
            var badRequest =(NegotiatedContentResult<ApiResponseDto<AuthenticationResponseDto>>)result;

            Assert.Multiple(() =>
            {
                Assert.That(badRequest.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(badRequest.Content.Success, Is.False);
                Assert.That(badRequest.Content.Message, Is.EqualTo("Invalid email or password"));
            });

        }

        [Test]
        public async Task Login_WithInvalidModelState_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Email", "Required");

            var result = await _controller.Login(new LoginRequestDto());

            Assert.That(result, Is.InstanceOf<InvalidModelStateResult>());
        }

        [Test]
        public async Task Refresh_WhenServiceReturnsSuccess_ReturnsOk()
        {
            var request = new RefreshTokenRequestDto();

            var response = new ApiResponseDto<AuthenticationResponseDto>
            {
                Success = true,
                Message = "Token refreshed successfully.",
                Data = new AuthenticationResponseDto
                {
                    AccessToken = "new-access-token",
                    RefreshToken = "refresh-token"
                }
            };

            _authenticationService
                .Setup(x => x.TokenRefreshAsync(request))
                .ReturnsAsync(response);

            var result = await _controller.Refresh(request);

            var okResult = (OkNegotiatedContentResult<ApiResponseDto<AuthenticationResponseDto>>)result;

            Assert.Multiple(() =>
            {
                Assert.That(okResult.Content.Success, Is.True);
                Assert.That(okResult.Content.Message, Is.EqualTo("Token refreshed successfully."));
                Assert.That(okResult.Content.Data.AccessToken, Is.EqualTo("new-access-token"));
                Assert.That(okResult.Content.Data.RefreshToken, Is.EqualTo("refresh-token"));
            });
        }

        [Test]
        public async Task Refresh_WhenServiceReturnsFailure_ReturnsBadRequest()
        {
            var request = new RefreshTokenRequestDto();
            var response = new ApiResponseDto<AuthenticationResponseDto>
            {
                Success = false,
                Message = "Invalid refresh token."
            };

            _authenticationService
                .Setup(x => x.TokenRefreshAsync(request))
                .ReturnsAsync(response);

            var result = await _controller.Refresh(request);
            var badRequest = (NegotiatedContentResult<ApiResponseDto<AuthenticationResponseDto>>)result;

            Assert.Multiple(() =>
            {
                Assert.That(badRequest.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(badRequest.Content.Success, Is.False);
                Assert.That(badRequest.Content.Message, Is.EqualTo("Invalid refresh token."));
            });
        }

        [Test]
        public async Task Logout_WhenServiceReturnsSuccess_ReturnsOk()
        {
            var request = new RefreshTokenRequestDto();
            var response = new ApiResponseDto<string>
            {
                Success = true,
                Message = "Logged out successfully."
            };

            _authenticationService
                .Setup(x => x.LogoutAsync(request))
                .ReturnsAsync(response);

            var result = await _controller.Logout(request);
            var okResult = (OkNegotiatedContentResult<ApiResponseDto<string>>)result;

            Assert.Multiple(() =>
            {
                Assert.That(okResult.Content.Success, Is.True);
                Assert.That(okResult.Content.Message, Is.EqualTo("Logged out successfully."));
            });
        }

        [Test]
        public async Task Logout_WhenServiceReturnsFailure_ReturnsBadRequest()
        {
            var request = new RefreshTokenRequestDto();
            var response = new ApiResponseDto<string>
            {
                Success = false,
                Message = "Invalid refresh token."
            };

            _authenticationService
                .Setup(x => x.LogoutAsync(request))
                .ReturnsAsync(response);

            var result = await _controller.Logout(request);
            var badRequest = (NegotiatedContentResult<ApiResponseDto<string>>)result;

            Assert.Multiple(() =>
            {
                Assert.That(badRequest.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(badRequest.Content.Success, Is.False);
                Assert.That(badRequest.Content.Message, Is.EqualTo("Invalid refresh token."));
            });
        }
    }
}
