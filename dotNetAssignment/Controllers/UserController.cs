using dotNetAssignment.Constants;
using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.DTO.Address;
using dotNetAssignment.Models.DTO.SignUp;
using dotNetAssignment.Services.Interfaces;
using dotNetAssignment.Filters;
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
        /// Updates the user information for the authenticated user.
        /// </summary>
        /// <param name="request">Request contains the updated user information</param>
        /// <returns>An API response indicating the success or failure of the operation.</returns>
        [Authorize]
        [ActiveUserFilter]
        [HttpPatch]
        [Route("")]
        public async Task<IHttpActionResult> UpdateUser(UpdateUserRequestDto request)
        {
            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);
            var response = await _userService.UpdateUserAsync(userId, request);

            if (!response.Success)
            {
                if(response.Message == ExceptionMessages.SamePhoneNumber || response.Message == ExceptionMessages.SameName || response.Message == ExceptionMessages.UserNotUpdated || response.Message == ExceptionMessages.PhoneNumberAlreadyExists)
                {
                    return Content(HttpStatusCode.BadRequest, response);
                }
            }

            return Ok(response);
        }

        /// <summary>
        /// Adds a new address for the authenticated user.
        /// </summary>
        /// <param name="request">Request contains the new address information</param>
        /// <returns>An API response indicating the success or failure of the operation.</returns>
        [Authorize]
        [ActiveUserFilter]
        [HttpPost]
        [Route("address")]
        public async Task<IHttpActionResult> AddAddress(AddAddressRequestDto request)
        {
            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);
            var response = await _userService.AddAddressAsync(userId, request);

            return Ok(response);
        }

        /// <summary>
        /// Updates an existing address for the authenticated user.
        /// </summary>
        /// <param name="request">Request contains the updated address information</param>
        /// <returns>An API response indicating the success or failure of the operation.</returns>
        [Authorize]
        [ActiveUserFilter]
        [HttpPatch]
        [Route("address")]
        public async Task<IHttpActionResult> UpdateAddress(UpdateAddressRequestDto request)
        {
            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);
            var response = await _userService.UpdateAddressAsync(userId, request);

            if (!response.Success)
            {
                if(response.Message == ExceptionMessages.AddressNotFound)
                {
                    return Content(HttpStatusCode.NotFound, response);
                }
                if (response.Message == ExceptionMessages.AddressNotUpdated)
                {
                    return Content(HttpStatusCode.BadRequest, response);
                }
            }

            return Ok(response);
        }

        /// <summary>
        /// Changes the password for the authenticated user.
        /// </summary>
        /// <param name="request">Request contains the new password information</param>
        /// <returns>An API response indicating the success or failure of the operation.</returns>
        [Authorize]
        [ActiveUserFilter]
        [HttpPatch]
        [Route("password")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordRequestDto request)
        {
            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);
            var response = await _userService.ChangePasswordAsync(userId, request);

            if (!response.Success)
            {
                if (response.Message == ExceptionMessages.WrongPassword)
                {
                    return Content(HttpStatusCode.BadRequest, response);
                }
                if (response.Message == ExceptionMessages.SamePassword)
                {
                    return Content(HttpStatusCode.BadRequest, response);
                }
            }

            return Ok(response);
        }

        /// <summary>
        /// Deactivates the account of the authenticated user.
        /// </summary>
        /// <param name="request">Request contains the deactivation information</param>
        /// <returns>An API response indicating the success or failure of the operation.</returns>
        [Authorize]
        [ActiveUserFilter]
        [HttpPatch]
        [Route("deactivate")]
        public async Task<IHttpActionResult> DeactivateUser(DeactivateAccountRequestDto request)
        {
            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);
            var response = await _userService.DeactivateUserAsync(userId, request);

            if (!response.Success)
            {
                if(response.Message == ExceptionMessages.InvalidRefreshToken)
                {
                    return Content(HttpStatusCode.Unauthorized, response);
                }
            }

            return Ok(response);
        }
    }
}
