using System;
using System.Threading.Tasks;

using dotNetAssignment.Constants;
using dotNetAssignment.Models.Enums;
using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.Entities;
using dotNetAssignment.Repositories.OrderRepository;
using dotNetAssignment.Repositories.RestaurantRepo;
using dotNetAssignment.Repositories.UserRepo;
using dotNetAssignment.Services.Interfaces;
using dotNetAssignment.Models.DTO.Address;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Places an order for a user at a specific restaurant with the provided order details.
        /// </summary>
        /// <param name="request">The order request details.</param>
        /// <param name="userId">The ID of the user placing the order.</param>
        /// <returns>The result of the order placement operation.</returns>
        public async Task<ApiResponseDto<PlaceOrderResponseDto>> PlaceOrder(OrderRequestDto request, Guid userId)
        {
            using (var transaction = _orderRepository.BeginTransaction())
            {
                try
                {
                    var user = await _userRepository.GetUserForUpdateAsync(userId);
                    var restaurant = await _restaurantRepository.GetRestaurantByIdAsync(request.RestaurantId);

                    if (restaurant == null)
                    {
                        return new ApiResponseDto<PlaceOrderResponseDto>
                        {
                            Success = false,
                            Message = ExceptionMessages.RestaurantDoesntExists
                        };
                    }

                    var address = await _userRepository.GetAddressByIdAsync(request.AddressId);
                    if (address == null || address.UserId != userId)
                    {
                        return new ApiResponseDto<PlaceOrderResponseDto>
                        {
                            Success = false,
                            Message = ExceptionMessages.AddressNotFound
                        };
                    }

                    var order = new Order
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        RestaurantId = request.RestaurantId,
                        Status = OrderStatus.Placed,
                        PlacedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        AddressLineOne = address.LineOne,
                        Landmark = address.Landmark,
                        Pincode = address.Pincode,
                        City = address.City,
                        State = address.State,
                    };

                    var orderItemIds = request.OrderItems.Select(oi => oi.Id).ToList();
                    var menuItems = await _orderRepository.GetAllMenuItemsByOrderIdAsync(orderItemIds);

                    var totalPrice = 0m;
                    foreach (OrderItemsRequestDto item in request.OrderItems)
                    {
                        var menuItem = menuItems.Find(mi => mi.Id == item.Id);
                        if (menuItem == null || menuItem.RestaurantId != request.RestaurantId)
                        {
                            return new ApiResponseDto<PlaceOrderResponseDto>
                            {
                                Success = false,
                                Message = ExceptionMessages.MenuItemDoesntExists
                            };
                        }


                        if (item.Quantity > menuItem.QuantityAvailable)
                        {
                            return new ApiResponseDto<PlaceOrderResponseDto>
                            {
                                Success = false,
                                Message = ExceptionMessages.InsufficientStock
                            };
                        }

                        totalPrice += menuItem.Price * item.Quantity;
                    }

                    if(totalPrice > user.Balance)
                    {
                        return new ApiResponseDto<PlaceOrderResponseDto>
                        {
                            Success = false,
                            Message = ExceptionMessages.InsufficientBalance
                        };
                    }

                    foreach (OrderItemsRequestDto item in request.OrderItems)
                    {
                        var menuItem = menuItems.Find(mi => mi.Id == item.Id);

                        var price = menuItem.Price * item.Quantity;

                        var OrderItem = new OrderItem
                        {
                            Id = Guid.NewGuid(),
                            OrderId = order.Id,
                            MenuId = menuItem.Id,
                            Quantity = item.Quantity,
                            Price = menuItem.Price
                        };

                        user.Balance -= price;
                        menuItem.QuantityAvailable -= item.Quantity;
                        _orderRepository.AddOrderItem(OrderItem);
                    }

                    _orderRepository.AddOrder(order);
                    await _orderRepository.SaveChangesAsync();
                    await _userRepository.SaveChangesAsync();

                    transaction.Commit();

                    return new ApiResponseDto<PlaceOrderResponseDto>
                    {
                        Success = true,
                        Message = SuccessMessages.OrderPlaced,
                        Data = new PlaceOrderResponseDto
                        {
                            OrderId = order.Id
                        }
                    };
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Retrieves the details of a specific order for a user.
        /// </summary>
        /// <param name="request">The request containing the order ID.</param>
        /// <param name="userId">The ID of the user requesting the order details.</param>
        /// <returns>The result of the order details retrieval operation.</returns>
        public async Task<ApiResponseDto<OrderDetailsResponseDto>> OrderDetails(Guid orderId, Guid userId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null || order.UserId != userId)
            {
                return new ApiResponseDto<OrderDetailsResponseDto>
                {
                    Success = false,
                    Message = ExceptionMessages.OrderDoesntExists
                };
            }

            var orderItems = await _orderRepository.GetOrderItemsByOrderIdAsync(order.Id);
            var response = new OrderDetailsResponseDto
            {
                OrderStatus = order.Status.ToString(),
                RestaurantName = order.Restaurant.Name,
                DeliveryAddress = new OrderAddressResponseDto
                {
                    AddressLineOne = order.AddressLineOne,
                    Landmark = order.Landmark,
                    Pincode = order.Pincode,
                    City = order.City,
                    State = order.State
                },
                OrderItems = new List<OrderItemDetailsDto>(),
                TotalAmount = 0
            };

            foreach (var item in orderItems)
            {
                response.OrderItems.Add(new OrderItemDetailsDto
                {
                    DishName = item.Menu.DishName,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
                response.TotalAmount += item.Price * item.Quantity;
            }

            return new ApiResponseDto<OrderDetailsResponseDto>
            {
                Success = true,
                Message = SuccessMessages.OrderDetailsFetched,
                Data = response
            };
        }

        /// <summary>
        /// Cancels a specific order for a user, updating the order status and refunding the user's balance if applicable.
        /// </summary>
        /// <param name="request">The request containing the order ID.</param>
        /// <param name="userId">The ID of the user requesting to cancel the order.</param>
        /// <returns>The result of the order cancellation operation.</returns>
        public async Task<ApiResponseDto<string>> CancelOrder(Guid orderId, Guid userId)
        {
            using (var transaction = _orderRepository.BeginTransaction())
            {
                try
                {
                    var user = await _userRepository.GetUserByIdAsync(userId);

                    var order = await _orderRepository.GetOrderByIdAsync(orderId);
                    if (order == null || order.UserId != userId)
                    {
                        return new ApiResponseDto<string>
                        {
                            Success = false,
                            Message = ExceptionMessages.OrderDoesntExists
                        };
                    }

                    if (order.Status != OrderStatus.Placed)
                    {
                        return new ApiResponseDto<string>
                        {
                            Success = false,
                            Message = ExceptionMessages.OrderCannotBeCancelled
                        };
                    }

                    order.Status = OrderStatus.Cancelled;
                    order.UpdatedAt = DateTime.UtcNow;
                    var orderItems = await _orderRepository.GetOrderItemsByOrderIdAsync(order.Id);

                    foreach (var item in orderItems)
                    {
                        var menuitem = await _orderRepository.GetMenuItemByIdAsync(item.MenuId);
                        menuitem.QuantityAvailable += item.Quantity;
                        user.Balance += item.Price * item.Quantity;
                    }

                    await _orderRepository.SaveChangesAsync();
                    await _userRepository.SaveChangesAsync();
                    transaction.Commit();

                    return new ApiResponseDto<string>
                    {
                        Success = true,
                        Message = SuccessMessages.OrderCancelled
                    };
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
