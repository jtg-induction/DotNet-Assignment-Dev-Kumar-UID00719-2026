using dotNetAssignment.Constants;
using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.Entities;
using dotNetAssignment.Repositories.OrderRepository;
using dotNetAssignment.Repositories.RestaurantRepo;
using dotNetAssignment.Repositories.UserRepo;
using dotNetAssignment.Services.Interfaces;
using dotNetAssignment.Models.Enums;
using dotNetAssignment.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace dotNetAssignment.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;

        public OrderService(
            IRestaurantRepository restaurantRepository,
            IOrderRepository orderRepository,
            IUserRepository userRepository)
        {
            _restaurantRepository = restaurantRepository;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
        }
        public async Task<ApiResponseDto<string>> PlaceOrder(OrderRequestDto request, Guid userId)
        {
            var restaurant = await _restaurantRepository.GetRestaurantByIdAsync(request.RestaurantId);
            if (restaurant == null)
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = ExceptionMessages.RestaurantDoesntExists
                };
            }

            var user = await _userRepository.GetUserByIdAsync(userId);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RestaurantId = request.RestaurantId,
                Status = OrderStatus.Placed,
                PlacedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                AddressLineOne = restaurant.AddressLineOne,
                Landmark = restaurant.Landmark,
                Pincode = restaurant.Pincode,
                City = restaurant.City,
                State = restaurant.State,
            };

            foreach (OrderItemsRequestDto item in request.orderItems)
            {
                var menuitem = await _orderRepository.GetMenuItemByIdAsync(item.Id);
                if(menuitem == null)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = ExceptionMessages.MenuItemDoesntExists
                    };
                }

                var price = menuitem.Price * item.Quantity;

                if(user.Balance < price)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = ExceptionMessages.InsufficientBalance
                    };
                }

                var OrderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    MenuId = menuitem.Id,
                    Quantity = item.Quantity,
                    Price = menuitem.Price
                };

                user.Balance -= price;

                _orderRepository.AddOrderItem(OrderItem);
            }

            _orderRepository.AddOrder(order);
            await _orderRepository.SaveChangesAsync();

            return new ApiResponseDto<string>
            {
                Success = true,
                Message = SuccessMessages.OrderPlaced
            };

        }
    }
}