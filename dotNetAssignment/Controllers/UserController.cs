using dotNetAssignment.Constants;
using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.DTO.Address;
using dotNetAssignment.Models.DTO.SignUp;
using dotNetAssignment.Services.Interfaces;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace dotNetAssignment.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Updates the details of a user
        /// </summary>
        /// <param name="request">Request cntains information to be updated</param>
        /// <returns>Returns failure or success response of the operation</returns>
        [Authorize]
        [HttpPatch]
        [Route("")]
        public async Task<IHttpActionResult> UpdateUser(UpdateUserRequestDto request)
        {
            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);
            var response = await _userService.UpdateUserAsync(userId, request);

            if (!response.Success)
            {
                return Content(HttpStatusCode.NotFound, response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Adds a new address for a user
        /// </summary>
        /// <param name="request">Request contains all the details of the Address</param>
        /// <returns>Returns failure or success response of the operation</returns>
        [Authorize]
        [HttpPost]
        [Route("address")]
        public async Task<IHttpActionResult> AddAddress(AddAddressRequestDto request)
        {
            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);
            var response = await _userService.AddAddressAsync(userId, request);

            if (!response.Success)
            {
                return Content(HttpStatusCode.NotFound, response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Updates an existing address of the user
        /// </summary>
        /// <param name="request">Request contains updated details fo the address</param>
        /// <returns>Returns failure or success response of the operation</returns>
        [Authorize]
        [HttpPatch]
        [Route("address")]
        public async Task<IHttpActionResult> UpdateAddress(UpdateAddressRequestDto request)
        {
            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);
            var response = await _userService.UpdateAddressAsync(userId, request);

            if (!response.Success)
            {
                return Content(HttpStatusCode.NotFound, response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Changes password of a user
        /// </summary>
        /// <param name="request">Request contains old and new password</param>
        /// <returns>Returns failure or success response of the operation</returns>
        [Authorize]
        [HttpPatch]
        [Route("password")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordRequestDto request)
        {
            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);
            var response = await _userService.ChangePasswordAsync(userId, request);

            if (!response.Success)
            {
                if (response.Message == ExceptionMessages.UserNotFound)
                {
                    return Content(HttpStatusCode.NotFound, response);
                }

                if (response.Message == ExceptionMessages.WrongPassword)
                {
                    return Content(HttpStatusCode.Unauthorized, response);
                }
            }

            return Ok(response);
        }

        /// <summary>
        /// Deactivates an existing user
        /// </summary>
        /// <returns>Returns failure or success response of the operation</returns>
        [Authorize]
        [HttpPatch]
        [Route("deactivate")]
        public async Task<IHttpActionResult> DeactivateUSer()
        {
            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);
            var response = await _userService.DeactivateUserAsync(userId);

            if (!response.Success)
            {
                return Content(HttpStatusCode.NotFound, response);
            }

            return Ok(response);
        }
    }
}
