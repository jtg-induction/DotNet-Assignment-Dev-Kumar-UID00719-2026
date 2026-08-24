using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.Enums;
using dotNetAssignment.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace dotNetAssignment.Controllers
{
    [RoutePrefix("api/restaurant")]


    public class RestaurantController : ApiController
    {
        private readonly IRestaurantService _restaurantService;
        private const string Admin = "Admin";
        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        /// <summary>
        /// Fetches a list of all restaurants available
        /// </summary>
        /// <param name="request">Request contains page number and number of items per page</param>
        /// <returns>Returns failure or success response of the operation</returns>
        [Authorize]
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAllRestaurants (PaginationRequestDto request)
        { 
            var response = await _restaurantService.GetAllRestaurantsListAsync(request.Page, request.PageSize);

            if (!response.Success)
            {
                return Content(HttpStatusCode.NotFound, response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Fetches all menu items for a specific restaurant
        /// </summary>
        /// <param name="request">
        /// Request contains the restaurant id of the restaurant of which to fetch the menu items
        /// Request contains page number and number of items per page
        /// </param>
        /// <returns>Returns failure or success response of the operation</returns>
        [Authorize]
        [HttpGet]
        [Route("menu")]
        public async Task<IHttpActionResult> GetMenuItems(MenuRequestDto request)
        {
            var response = await _restaurantService.GetMenuListAsync(request.RestaurantId, request.Page, request.PageSize);

            if (!response.Success)
            {
                return Content(HttpStatusCode.NotFound, response);
            }

            return Ok(response);
        }

        [Authorize(Roles = Admin)]
        [HttpPost]
        [Route("create")]
        public async Task<IHttpActionResult> CreateRestaurant(CreateRestaurantRequestDto request)
        {
            var response = await _restaurantService.CreateRestaurantAsync(request);

            if (!response.Success)
            {
                return Content(HttpStatusCode.NotFound, response);
            }

            return Ok(response);
        }
    }
}
