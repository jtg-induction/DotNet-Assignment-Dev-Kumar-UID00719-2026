using dotNetAssignment.Models.DTO.Login;
using dotNetAssignment.Models.DTO.SignUp;
using dotNetAssignment.Services.Interfaces;
using dotNetAssignment.Models.DTO;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net;

namespace dotNetAssignment.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Handles user signup requests.
        /// </summary>
        /// <param name="signupRequestDto">The signup request DTO.</param>
        /// <returns>
        /// An API response indicating the success or failure of the signup operation.
        /// </returns>
        [HttpPost]
        [Route("signup")]
        public async Task<IHttpActionResult> Signup(SignupRequestDto signupRequestDto)
        {
            var response = await _authenticationService.SignupAsync(signupRequestDto);

            if (!response.Success)
            {
                return Content(HttpStatusCode.BadRequest, response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Handles user login requests.
        /// </summary>
        /// <param name="loginRequestDto">The login request DTO.</param>
        /// <returns>
        /// An API response containing the authentication tokens on success
        /// or an error message when login fails
        /// </returns>
        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login(LoginRequestDto loginRequestDto)
        {
            var response = await _authenticationService.LoginAsync(loginRequestDto);

            if (!response.Success)
            {
                return Content(HttpStatusCode.Unauthorized, response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Handles token refresh requests.
        /// </summary>
        /// <param name="refreshTokenDto">The refresh token request DTO.</param>
        /// <returns>
        /// An API response containing the refreshed authentication tokens on success
        /// or an error message when token refresh fails
        /// </returns>
        [HttpPost]
        [Route("refresh")]
        public async Task<IHttpActionResult> Refresh(RefreshTokenRequestDto refreshTokenDto)
        {
            var response = await _authenticationService.TokenRefreshAsync(refreshTokenDto);

            if (!response.Success)
            {
                return Content(HttpStatusCode.Unauthorized, response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Handles user logout requests.
        /// </summary>
        /// <param name="refreshTokenDto">The refresh token request DTO.</param>
        /// <returns>
        /// An API response indicating the success or failure of the logout operation.
        /// </returns>
        [Authorize]
        [HttpPost]
        [Route("logout")]
        public async Task<IHttpActionResult> Logout(LogoutRequestDto request) 
        {
            var response = await _authenticationService.LogoutAsync(request);

            if (!response.Success) 
            {
                return Content(HttpStatusCode.Unauthorized, response);
            }

            return Ok(response);
        }
    }
}
