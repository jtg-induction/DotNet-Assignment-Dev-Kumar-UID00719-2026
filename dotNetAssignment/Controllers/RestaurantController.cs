using dotNetAssignment.Filters;
using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.Enums;
using dotNetAssignment.Services.Interfaces;
using dotNetAssignment.Constants;
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
        public async Task<IHttpActionResult> GetAllRestaurants(int page = 1, int pageSize = 10)
        {
            var response = await _restaurantService.GetAllRestaurantsListAsync(page, pageSize);

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
        [Route("menu/{restaurantId:guid}")]
        public async Task<IHttpActionResult> GetMenuItems(Guid restaurantId, int page = 1, int pageSize = 10)
        {
            var response = await _restaurantService.GetMenuListAsync(restaurantId, page, pageSize);

            if (!response.Success)
            {
                return Content(HttpStatusCode.NotFound, response);
            }

            return Ok(response);
        }


        /// <summary>
        /// Creates a new restaurant. This endpoint is restricted to users with the "Admin" role.
        /// </summary>
        /// <param name="request">The request object containing the details of the restaurant to be created.</param>
        /// <returns>Returns failure or success response of the operation</returns>
        [ActiveUserFilter]
        [Authorize(Roles = Admin)]
        [HttpPost]
        [Route("create")]
        public async Task<IHttpActionResult> CreateRestaurant(CreateRestaurantRequestDto request)
        {
            var response = await _restaurantService.CreateRestaurantAsync(request);

            if (!response.Success)
            {
                if(response.Message == ExceptionMessages.UserNotFound)
                {
                    return Content(HttpStatusCode.NotFound, response);
                }
            }

            return Ok(response);
        }


        /// <summary>
        /// Onboards a new restaurant owner by associating them with an existing restaurant.
        /// </summary>
        /// <param name="request">The request object containing the details of the restaurant owner to be onboarded.</param>
        /// <returns>Returns failure or success response of the operation</returns>
        [ActiveUserFilter]
        [Authorize(Roles = Admin)]
        [HttpPost]
        [Route("onboard")]
        public async Task<IHttpActionResult> OnboardNewRestaurantOwner(OnboardNewRestaurantOwnerDto request)
        {
            var response = await _restaurantService.OnboardNewRestaurantOwnerAsync(request);

            if (!response.Success)
            {
                if (response.Message == ExceptionMessages.UserNotFound || response.Message == ExceptionMessages.RestaurantDoesntExists)
                {
                    return Content(HttpStatusCode.NotFound, response);
                }
            }

            return Ok(response);
        }
    }
}
