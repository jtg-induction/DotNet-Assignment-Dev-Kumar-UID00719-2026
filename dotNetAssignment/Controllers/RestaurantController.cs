using dotNetAssignment.Models.DTO;
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

        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        [Authorize]
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAllRestaurants ()
        { 
            var response = await _restaurantService.GetAllRestaurantsListAsync();

            if (!response.Success)
            {
                return Content(HttpStatusCode.BadRequest, response);
            }

            return Ok(response);
        }


        [Authorize]
        [HttpGet]
        [Route("menu")]
        public async Task<IHttpActionResult> GetMenuItems(MenuRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _restaurantService.GetMenuListAsync(request.RestaurantId);

            if (!response.Success)
            {
                return Content(HttpStatusCode.BadRequest, response);
            }

            return Ok(response);
        }
    }
}