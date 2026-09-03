using dotNetAssignment.Constants;
using dotNetAssignment.Filters;
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
using System.Web.Security;

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
        [ActiveUserFilter]
        [HttpPost]
        [Route("{restaurantId}")]
        public async Task<IHttpActionResult> PlaceOrder([FromUri] Guid restaurantId, [FromBody] OrderRequestDto request)
        {
            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);
            var response = await _orderService.PlaceOrder(restaurantId, request, userId);
            
            if (!response.Success)
            {
                if (response.Message == ExceptionMessages.InsufficientBalance || response.Message==ExceptionMessages.InvalidOrderItems)
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
        [ActiveUserFilter]
        [HttpGet]
        [Route("details/{orderId:guid}")]
        public async Task<IHttpActionResult> GetOrderDetails(Guid orderId)
        {
            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);

            var response = await _orderService.OrderDetails(orderId, userId);
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
        [ActiveUserFilter]
        [HttpPost]
        [Route("cancel")]
        public async Task<IHttpActionResult> CancelOrder(CancelOrderRequestDto request)
        {
            var userId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);

            var response = await _orderService.CancelOrder(request, userId);
            if (!response.Success)
            {
                if(response.Message == ExceptionMessages.OrderCannotBeCancelled || response.Message == ExceptionMessages.OrderAlreadyCancelled)
                {
                    return Content(HttpStatusCode.BadRequest, response);
                }
                return Content(HttpStatusCode.NotFound, response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Updates the status of a specific order
        /// </summary>
        /// <param name="request">Request contains id of the order to update and the new order status to which it has to be updated.</param>
        /// <returns>Returns failure or success response of the operation</returns>
        [Authorize(Roles = "Owner,Admin")]
        [ActiveUserFilter]
        [HttpPost]
        [Route("update")]
        public async Task<IHttpActionResult> UpdateOrderStatus(UpdateOrderStatusDto request)
        {
            var ownerId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);
            var response = await _orderService.UpdateOrderStatusAsync(request, ownerId);

            if (!response.Success)
            {
                if(response.Message == ExceptionMessages.OrderDoesNotExist)
                {
                    return Content(HttpStatusCode.NotFound, response);
                }
                if (response.Message == ExceptionMessages.OrderStatusCanNotBeSame || response.Message == ExceptionMessages.OrderStatusCanNotBeUpdated)
                {
                    return Content(HttpStatusCode.BadRequest, response);
                }
            }

            return Ok(response);
        }

        /// <summary>
        /// Fetches details of all the orders from the restaurants the owner owns.
        /// </summary>
        /// <param name="request">Request contains pagination, sorting, filtering and searching parameters.</param>
        /// <returns>Returns failure or success response of the operation</returns>
        [Authorize(Roles = "Owner,Admin")]
        [ActiveUserFilter]
        [HttpGet]
        [Route("get")]
        public async Task<IHttpActionResult> GetOrders([FromUri] DashboardOrderListRequestDto request)
        {
            request = request ?? new DashboardOrderListRequestDto();
            var ownerId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);
            var response = await _orderService.GetDashboardOrdersAsync(request, ownerId);
            return Ok(response);
        }
    }
}
