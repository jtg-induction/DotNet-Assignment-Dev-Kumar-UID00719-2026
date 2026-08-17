using dotNetAssignment.Constants;
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

        /// <summary>
        /// Places an order for a user.
        /// The user must be authenticated to place an order.
        /// The request contains the restaurant ID, address ID, and a list of order items.
        /// The method returns a response indicating the success or failure of the order placement.
        /// </summary>
        /// <param name="request">The request containing the order details.</param>
        /// <returns>Returns failure or success response of the operation</returns>
        [Authorize]
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> PlaceOrder(OrderRequestDto request)
        {
            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);
            var response = await _orderService.PlaceOrder(request, userId);
            
            if (!response.Success)
            {
                if (response.Message == ExceptionMessages.InsufficientBalance || response.Message == ExceptionMessages.InsufficientStock)
                {
                    return Content(HttpStatusCode.BadRequest, response);
                }
                else
                {
                    return Content(HttpStatusCode.NotFound, response);
                }
            }
            return Ok(response);
        }

        /// <summary>
        /// Retrieves the details of a specific order for the authenticated user.
        /// </summary>
        /// <param name="request">The request containing the order id</param>
        /// <returns>Returns failure or success response of the operation</returns>
        [Authorize]
        [HttpGet]
        [Route("details")]
        public async Task<IHttpActionResult> GetOrderDetails(OrderDetailsRequestDto request)
        {
            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);

            var response = await _orderService.OrderDetails(request, userId);
            if (!response.Success)
            {
                return Content(HttpStatusCode.NotFound, response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Cancels an order for the authenticated user.
        /// </summary>
        /// <param name="request">The request containing order id of the order to be cancelled.</param>
        /// <returns>Returns failure or success response of the operation</returns>
        [Authorize]
        [HttpPost]
        [Route("cancel")]
        public async Task<IHttpActionResult> CancelOrder(CancelOrderRequestDto request)
        {
            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);

            var response = await _orderService.CancelOrder(request, userId);
            if (!response.Success)
            {
                if(response.Message == ExceptionMessages.OrderCannotBeCancelled)
                {
                    return Content(HttpStatusCode.BadRequest, response);
                }
                return Content(HttpStatusCode.NotFound, response);
            }

            return Ok(response);
        }
    }
}
