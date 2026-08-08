using System.Threading.Tasks;
using System.Web.Http;
using System.Net;

using dotNetAssignment.Models.DTO.Login;
using dotNetAssignment.Models.DTO.SignUp;
using dotNetAssignment.Services.Interfaces;
using dotNetAssignment.Models.DTO;

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

        [HttpPost]
        [Route("signup")]
        public async Task<IHttpActionResult> Signup(SignupRequestDto signupRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authenticationService.SignupAsync(signupRequestDto);

            if (!response.Success)
            {
                return Content(HttpStatusCode.BadRequest, response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login(LoginRequestDto loginRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authenticationService.LoginAsync(loginRequestDto);

            if (!response.Success)
            {
                return Content(HttpStatusCode.BadRequest, response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IHttpActionResult> Refresh(RefreshTokenDto refreshTokenDto)
        {
            var response = await _authenticationService.TokenRefreshAsync(refreshTokenDto);

            if (!response.Success)
            {
                return Content(HttpStatusCode.BadRequest, response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IHttpActionResult> Logout(RefreshTokenDto refreshTokenDto) 
        {
            var response = await _authenticationService.LogoutAsync(refreshTokenDto);

            if (!response.Success)
            {
                return Content(HttpStatusCode.BadRequest, response);
            }

            return Ok(response);
        }
    }
}
