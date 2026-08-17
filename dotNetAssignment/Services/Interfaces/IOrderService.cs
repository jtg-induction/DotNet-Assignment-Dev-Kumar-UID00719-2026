using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using dotNetAssignment.Models.DTO;

namespace dotNetAssignment.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ApiResponseDto<PlaceOrderResponseDto>> PlaceOrder(OrderRequestDto request, Guid userId);

        Task<ApiResponseDto<OrderDetailsResponseDto>> OrderDetails(OrderDetailsRequestDto request, Guid userId);

        Task<ApiResponseDto<string>> CancelOrder(CancelOrderRequestDto request, Guid userId);
    }
}
