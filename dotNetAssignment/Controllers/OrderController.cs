using dotNetAssignment.Models.DTO;
using dotNetAssignment.Services.Implementations;
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
    [RoutePrefix("api/order")]
    public class OrderController : ApiController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize]
        [HttpPut]
        [Route("")]
        public async Task<IHttpActionResult> PlaceOrder(OrderRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst("userid").Value);

            var response = await _orderService.PlaceOrder(request, userId);
            if (!response.Success)
            {
                return Content(HttpStatusCode.BadRequest, response);
            }

            return Ok(response);
        }
    }
}