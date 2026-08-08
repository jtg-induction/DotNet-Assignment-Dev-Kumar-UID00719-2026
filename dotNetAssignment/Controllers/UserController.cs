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

        [Authorize]
        [HttpPut]
        [Route("")]
        public async Task<IHttpActionResult> UpdateUser(UpdateUserRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst("userid").Value);
            var response = await _userService.UpdateUserAsync(userId, request);

            if (!response.Success)
            {
                return Content(HttpStatusCode.BadRequest, response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        [Route("address")]
        public async Task<IHttpActionResult> AddAddress(AddAddressRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst("userid").Value);
            var response = await _userService.AddAddressAsync(userId, request);

            if (!response.Success)
            {
                return Content(HttpStatusCode.BadRequest, response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        [Route("address")]
        public async Task<IHttpActionResult> UpdateAddress(UpdateAddressRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst("userid").Value);
            var response = await _userService.UpdateAddressAsync(userId, request);

            if (!response.Success)
            {
                return Content(HttpStatusCode.BadRequest, response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        [Route("address")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst("userid").Value);
            var response = await _userService.ChangePasswordAsync(userId, request);

            if (!response.Success)
            {
                return Content(HttpStatusCode.BadRequest, response);
            }

            return Ok(response);
        }
    }
}