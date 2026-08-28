using dotNetAssignment.Constants;
using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.DTO.Address;
using dotNetAssignment.Models.Entities;
using dotNetAssignment.Models.Enums;
using dotNetAssignment.Repositories.OrderRepository;
using dotNetAssignment.Repositories.RestaurantRepo;
using dotNetAssignment.Repositories.UserRepo;
using dotNetAssignment.Services.Implementations;
using dotNetAssignment.Services.Interfaces;

using Moq;
using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace dotNetAssignment.Tests.Services
{
    [TestFixture]
    public class OrderServiceTests
    {
        private Mock<IRestaurantRepository> _restaurantRepository;
        private Mock<IOrderRepository> _orderRepository;
        private Mock<IUserRepository> _userRepository;

        private OrderService _orderService;
        private Guid _userId;

        [SetUp]
        public void Setup()
        {
            _restaurantRepository = new Mock<IRestaurantRepository>();
            _orderRepository = new Mock<IOrderRepository>();
            _userRepository = new Mock<IUserRepository>();
            _orderService = new OrderService(
                _restaurantRepository.Object,
                _orderRepository.Object,
                _userRepository.Object);

            _userId = Guid.NewGuid();
        }



        [Test]
        public async Task PlaceOrder_WhenRestaurantDoesNotExist_ReturnsFailure()
        {
            var user = new User
            {
                Id = _userId,
                IsActive = true
            };

            var restaurantId = Guid.NewGuid();

            _userRepository
                .Setup(x => x.GetUserForUpdateAsync(_userId))
                .ReturnsAsync(user);

            _restaurantRepository
                .Setup(x => x.GetRestaurantByIdAsync(restaurantId))
                .ReturnsAsync((Restaurant)null);

            var request = new OrderRequestDto
            {
                RestaurantId = restaurantId,
                AddressId = Guid.NewGuid(),
                OrderItems = new List<OrderItemsRequestDto>()
            };

            var result = await _orderService.PlaceOrder(request, _userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(
                    result.Message,
                    Is.EqualTo(ExceptionMessages.RestaurantDoesntExists));
            });
        }

        [Test]
        public async Task PlaceOrder_WhenAddressDoesNotExist_ReturnsFailure()
        {
            var user = new User
            {
                Id = _userId,
                IsActive = true
            };

            var restaurantId = Guid.NewGuid();
            var addressId = Guid.NewGuid();

            _userRepository
                .Setup(x => x.GetUserForUpdateAsync(_userId))
                .ReturnsAsync(user);

            _restaurantRepository
                .Setup(x => x.GetRestaurantByIdAsync(restaurantId))
                .ReturnsAsync(new Restaurant
                {
                    Id = restaurantId
                });

            _userRepository
                .Setup(x => x.GetAddressByIdAsync(addressId))
                .ReturnsAsync((UserAddress)null);

            var request = new OrderRequestDto
            {
                RestaurantId = restaurantId,
                AddressId = addressId,
                OrderItems = new List<OrderItemsRequestDto>()
            };

            var result = await _orderService.PlaceOrder(request, _userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(
                    result.Message,
                    Is.EqualTo(ExceptionMessages.AddressNotFound));
            });
        }

        [Test]
        public async Task PlaceOrder_WhenAddressBelongsToAnotherUser_ReturnsFailure()
        {
            var user = new User
            {
                Id = _userId,
                IsActive = true
            };

            var restaurantId = Guid.NewGuid();
            var addressId = Guid.NewGuid();

            var address = new UserAddress
            {
                Id = addressId,
                UserId = Guid.NewGuid()
            };

            _userRepository
                .Setup(x => x.GetUserForUpdateAsync(_userId))
                .ReturnsAsync(user);

            _restaurantRepository
                .Setup(x => x.GetRestaurantByIdAsync(restaurantId))
                .ReturnsAsync(new Restaurant
                {
                    Id = restaurantId
                });

            _userRepository
                .Setup(x => x.GetAddressByIdAsync(addressId))
                .ReturnsAsync(address);

            var request = new OrderRequestDto
            {
                RestaurantId = restaurantId,
                AddressId = addressId,
                OrderItems = new List<OrderItemsRequestDto>()
            };

            var result = await _orderService.PlaceOrder(request, _userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(
                    result.Message,
                    Is.EqualTo(ExceptionMessages.AddressNotFound));
            });
        }

        [Test]
        public async Task PlaceOrder_WhenMenuItemDoesNotExist_ReturnsFailure()
        {
            var user = new User
            {
                Id = _userId,
                IsActive = true,
                Balance = 1000m
            };

            var restaurantId = Guid.NewGuid();
            var addressId = Guid.NewGuid();
            var menuId = Guid.NewGuid();

            var address = new UserAddress
            {
                Id = addressId,
                UserId = _userId,
                LineOne = "123 Main Street",
                Landmark = "Near Mall",
                Pincode = "110001",
                City = "Delhi",
                State = "Delhi"
            };

            _userRepository
                .Setup(x => x.GetUserForUpdateAsync(_userId))
                .ReturnsAsync(user);

            _restaurantRepository
                .Setup(x => x.GetRestaurantByIdAsync(restaurantId))
                .ReturnsAsync(new Restaurant
                {
                    Id = restaurantId
                });

            _userRepository
                .Setup(x => x.GetAddressByIdAsync(addressId))
                .ReturnsAsync(address);

            _orderRepository
                .Setup(x => x.GetAllMenuItemsByOrderIdAsync(
                    It.Is<List<Guid>>(
                        ids => ids.Count == 1 && ids[0] == menuId)
                    )).ReturnsAsync(new List<Menu>());


            var request = new OrderRequestDto
            {
                RestaurantId = restaurantId,
                AddressId = addressId,
                OrderItems = new List<OrderItemsRequestDto>
                {
                    new OrderItemsRequestDto
                    {
                        Id = menuId,
                        Quantity = 1
                    }
                }
            };

            var result = await _orderService.PlaceOrder(request, _userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(
                    result.Message,
                    Is.EqualTo(ExceptionMessages.MenuItemDoesntExists));
            });
        }

        [Test]
        public async Task PlaceOrder_WhenMenuItemBelongsToAnotherRestaurant_ReturnsFailure()
        {
            var user = new User
            {
                Id = _userId,
                IsActive = true,
                Balance = 1000m
            };

            var restaurantId = Guid.NewGuid();
            var anotherRestaurantId = Guid.NewGuid();
            var addressId = Guid.NewGuid();
            var menuId = Guid.NewGuid();

            var address = new UserAddress
            {
                Id = addressId,
                UserId = _userId,
                LineOne = "123 Main Street",
                Landmark = "Near Mall",
                Pincode = "110001",
                City = "Delhi",
                State = "Delhi"
            };

            var menuItem = new Menu
            {
                Id = menuId,
                RestaurantId = anotherRestaurantId,
                Price = 100m,
                QuantityAvailable = 10
            };

            _userRepository
                .Setup(x => x.GetUserForUpdateAsync(_userId))
                .ReturnsAsync(user);

            _restaurantRepository
                .Setup(x => x.GetRestaurantByIdAsync(restaurantId))
                .ReturnsAsync(new Restaurant
                {
                    Id = restaurantId
                });

            _userRepository
                .Setup(x => x.GetAddressByIdAsync(addressId))
                .ReturnsAsync(address);

            _orderRepository
                .Setup(x => x.GetAllMenuItemsByOrderIdAsync(
                    It.Is<List<Guid>>(
                        ids => ids.Count == 1 && ids[0] == menuId)
                    )).ReturnsAsync(new List<Menu>
                    {
                        menuItem
                    });

            var request = new OrderRequestDto
            {
                RestaurantId = restaurantId,
                AddressId = addressId,
                OrderItems = new List<OrderItemsRequestDto>
                {
                    new OrderItemsRequestDto
                    {
                        Id = menuId,
                        Quantity = 1
                    }
                }
            };

            var result = await _orderService.PlaceOrder(request, _userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(
                    result.Message,
                    Is.EqualTo(ExceptionMessages.MenuItemDoesntExists));
            });
        }

        [Test]
        public async Task PlaceOrder_WhenQuantityExceedsStock_ReturnsFailure()
        {
            var user = new User
            {
                Id = _userId,
                IsActive = true,
                Balance = 1000m
            };

            var restaurantId = Guid.NewGuid();
            var addressId = Guid.NewGuid();
            var menuId = Guid.NewGuid();

            var address = new UserAddress
            {
                Id = addressId,
                UserId = _userId
            };

            var menuItem = new Menu
            {
                Id = menuId,
                RestaurantId = restaurantId,
                Price = 100m,
                QuantityAvailable = 2
            };

            _userRepository
                .Setup(x => x.GetUserForUpdateAsync(_userId))
                .ReturnsAsync(user);

            _restaurantRepository
                .Setup(x => x.GetRestaurantByIdAsync(restaurantId))
                .ReturnsAsync(new Restaurant
                {
                    Id = restaurantId
                });

            _userRepository
                .Setup(x => x.GetAddressByIdAsync(addressId))
                .ReturnsAsync(address);

            _orderRepository
                .Setup(x => x.GetAllMenuItemsByOrderIdAsync(
                    It.Is<List<Guid>>(
                        ids => ids.Count == 1 && ids[0] == menuId)
                    )).ReturnsAsync(new List<Menu>
                    {
                        menuItem
                    });

            var request = new OrderRequestDto
            {
                RestaurantId = restaurantId,
                AddressId = addressId,
                OrderItems = new List<OrderItemsRequestDto>
                {
                    new OrderItemsRequestDto
                    {
                        Id = menuId,
                        Quantity = 3
                    }
                }
            };

            var result = await _orderService.PlaceOrder(request, _userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(
                    result.Message,
                    Is.EqualTo(ExceptionMessages.InsufficientStock));
            });
        }

        [Test]
        public async Task PlaceOrder_WhenBalanceIsInsufficient_ReturnsFailure()
        {
            var user = new User
            {
                Id = _userId,
                IsActive = true,
                Balance = 50m
            };

            var restaurantId = Guid.NewGuid();
            var addressId = Guid.NewGuid();
            var menuId = Guid.NewGuid();

            var address = new UserAddress
            {
                Id = addressId,
                UserId = _userId
            };

            var menuItem = new Menu
            {
                Id = menuId,
                RestaurantId = restaurantId,
                Price = 100m,
                QuantityAvailable = 10
            };

            _userRepository
                .Setup(x => x.GetUserForUpdateAsync(_userId))
                .ReturnsAsync(user);

            _restaurantRepository
                .Setup(x => x.GetRestaurantByIdAsync(restaurantId))
                .ReturnsAsync(new Restaurant
                {
                    Id = restaurantId
                });

            _userRepository
                .Setup(x => x.GetAddressByIdAsync(addressId))
                .ReturnsAsync(address);

            _orderRepository
                .Setup(x => x.GetAllMenuItemsByOrderIdAsync(
                    It.Is<List<Guid>>(
                        ids => ids.Count == 1 && ids[0] == menuId)
                    )).ReturnsAsync(new List<Menu>
                    {
                        menuItem
                    });

            var request = new OrderRequestDto
            {
                RestaurantId = restaurantId,
                AddressId = addressId,
                OrderItems = new List<OrderItemsRequestDto>
                {
                    new OrderItemsRequestDto
                    {
                        Id = menuId,
                        Quantity = 1
                    }
                }
            };

            var result = await _orderService.PlaceOrder(request, _userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(
                    result.Message,
                    Is.EqualTo(ExceptionMessages.InsufficientBalance));
            });
        }



        [Test]
        public async Task OrderDetails_WhenOrderDoesNotExist_ReturnsFailure()
        {
            var orderId = Guid.NewGuid();

            _orderRepository
                .Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync((Order)null);

            var result = await _orderService.OrderDetails(orderId, _userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(
                    result.Message,
                    Is.EqualTo(ExceptionMessages.OrderDoesntExists));
            });
        }

        [Test]
        public async Task OrderDetails_WhenOrderBelongsToAnotherUser_ReturnsFailure()
        {

            var orderId = Guid.NewGuid();

            var order = new Order
            {
                Id = orderId,
                UserId = Guid.NewGuid()
            };

            _orderRepository
                .Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync(order);

            var result = await _orderService.OrderDetails(orderId, _userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(
                    result.Message,
                    Is.EqualTo(ExceptionMessages.OrderDoesntExists));
            });
        }


        [Test]
        public async Task OrderDetails_WhenOrderExists_ReturnsOrderDetails()
        {
            var orderId = Guid.NewGuid();
            var restaurantId = Guid.NewGuid();
            var menuId1 = Guid.NewGuid();
            var menuId2 = Guid.NewGuid();

            var order = new Order
            {
                Id = orderId,
                UserId = _userId,
                Status = OrderStatus.Placed,

                Restaurant = new Restaurant
                {
                    Id = restaurantId,
                    Name = "Burger Hub"
                },

                AddressLineOne = "24 MG Road",
                Landmark = "Near Metro",
                Pincode = "560001",
                City = "Bengaluru",
                State = "Karnataka"
            };

            var orderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId,
                    MenuId = menuId1,
                    Quantity = 2,
                    Price = 150m,

                    Menu = new Menu
                    {
                        Id = menuId1,
                        DishName = "Burger"
                    }
                },

                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId,
                    MenuId = menuId2,
                    Quantity = 1,
                    Price = 250m,

                    Menu = new Menu
                    {
                        Id = menuId2,
                        DishName = "Pizza"
                    }
                }
            };

            _orderRepository
                .Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync(order);

            _orderRepository
                .Setup(x => x.GetOrderItemsByOrderIdAsync(orderId))
                .ReturnsAsync(orderItems);

            var result = await _orderService.OrderDetails(orderId, _userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message,Is.EqualTo(SuccessMessages.OrderDetailsFetched));

                Assert.That(result.Data.OrderStatus,Is.EqualTo(OrderStatus.Placed.ToString()));
                Assert.That(result.Data.RestaurantName,Is.EqualTo("Burger Hub"));

                Assert.That(result.Data.DeliveryAddress.AddressLineOne,Is.EqualTo("24 MG Road"));
                Assert.That(result.Data.DeliveryAddress.Landmark,Is.EqualTo("Near Metro"));
                Assert.That(result.Data.DeliveryAddress.Pincode,Is.EqualTo("560001"));
                Assert.That(result.Data.DeliveryAddress.City,Is.EqualTo("Bengaluru"));
                Assert.That(result.Data.DeliveryAddress.State,Is.EqualTo("Karnataka"));

                Assert.That(result.Data.OrderItems.Count,Is.EqualTo(2));
                Assert.That(result.Data.OrderItems[0].DishName,Is.EqualTo("Burger"));
                Assert.That(result.Data.OrderItems[0].Quantity,Is.EqualTo(2));
                Assert.That(result.Data.OrderItems[0].Price,Is.EqualTo(150m));
                Assert.That(result.Data.OrderItems[1].DishName,Is.EqualTo("Pizza"));
                Assert.That(result.Data.OrderItems[1].Quantity,Is.EqualTo(1));
                Assert.That(result.Data.OrderItems[1].Price,Is.EqualTo(250m));

                Assert.That(result.Data.TotalAmount,Is.EqualTo(550m));
            });
        }


        [Test]
        public async Task CancelOrder_WhenOrderDoesNotExist_ReturnsFailure()
        {
            var orderId = Guid.NewGuid();
            var request = new CancelOrderRequestDto
            {
                OrderId = orderId
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(new User
                {
                    Id = _userId,
                    Balance = 1000m,
                    IsActive = true
                });

            _orderRepository
                .Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync((Order)null);

            var result = await _orderService.CancelOrder(request, _userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(
                    result.Message,
                    Is.EqualTo(ExceptionMessages.OrderDoesntExists));
            });
        }


        [Test]
        public async Task CancelOrder_WhenOrderBelongsToAnotherUser_ReturnsFailure()
        {
            var orderId = Guid.NewGuid();
            var request = new CancelOrderRequestDto
            {
                OrderId = orderId
            };

            var user = new User
            {
                Id = _userId,
                Balance = 1000m,
                IsActive = true
            };

            var order = new Order
            {
                Id = orderId,
                UserId = Guid.NewGuid(),
                Status = OrderStatus.Placed
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            _orderRepository
                .Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync(order);

            var result = await _orderService.CancelOrder(request, _userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ExceptionMessages.OrderDoesntExists));
            });
        }

        [Test]
        public async Task CancelOrder_WhenOrderIsNotPlaced_ReturnsFailure()
        {
            var orderId = Guid.NewGuid();
            var request = new CancelOrderRequestDto
            {
                OrderId = orderId
            };

            var user = new User
            {
                Id = _userId,
                Balance = 1000m,
                IsActive = true
            };

            var order = new Order
            {
                Id = orderId,
                UserId = _userId,
                Status = OrderStatus.Cancelled
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            _orderRepository
                .Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync(order);

            var result = await _orderService.CancelOrder(request, _userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ExceptionMessages.OrderCannotBeCancelled));
            });
        }

        [Test]
        public async Task UpdateOrderStatusAsync_WhenOrderDoesNotExist_ReturnsFailure()
        {
            var orderId = Guid.NewGuid();
            var request = new UpdateOrderStatusDto
            {
                OrderId = orderId,
                Status = OrderStatus.Accepted
            };

            _orderRepository
                .Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync((Order)null);

            var result = await _orderService.UpdateOrderStatusAsync( request, _userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ExceptionMessages.OrderDoesNotExist));
            });
        }


        [Test]
        public async Task UpdateOrderStatusAsync_WhenOwnerDoesNotOwnRestaurant_ReturnsFailure()
        {
            var orderId = Guid.NewGuid();
            var restaurantId = Guid.NewGuid();

            var request = new UpdateOrderStatusDto
            {
                OrderId = orderId,
                Status = OrderStatus.Accepted
            };

            var order = new Order
            {
                Id = orderId,
                RestaurantId = restaurantId,
                Status = OrderStatus.Placed
            };

            _orderRepository
                .Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync(order);

            _restaurantRepository
                .Setup(x => x.IsRestaurantOwnerAsync(
                    restaurantId,
                    _userId))
                .ReturnsAsync(false);

            var result = await _orderService.UpdateOrderStatusAsync( request, _userId);

            Assert.Multiple(() =>
            {
                Assert.That( result.Success, Is.False);
                Assert.That(result.Message,Is.EqualTo(ExceptionMessages.YouCantPerformThisAction));
            });
        }

        [Test]
        public async Task UpdateOrderStatusAsync_WhenStatusIsSame_ReturnsFailure()
        {
            var orderId = Guid.NewGuid();
            var restaurantId = Guid.NewGuid();

            var request = new UpdateOrderStatusDto
            {
                OrderId = orderId,
                Status = OrderStatus.Placed
            };

            var order = new Order
            {
                Id = orderId,
                RestaurantId = restaurantId,
                Status = OrderStatus.Placed
            };

            _orderRepository
                .Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync(order);

            _restaurantRepository
                .Setup(x => x.IsRestaurantOwnerAsync(
                    restaurantId,
                    _userId))
                .ReturnsAsync(true);

            var result = await _orderService.UpdateOrderStatusAsync( request, _userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message,Is.EqualTo(ExceptionMessages.OrderStatusCanNotBeSame));
            });
        }

        [Test]
        public async Task UpdateOrderStatusAsync_WhenTransitionIsValid_ReturnsSuccess()
        {
            var orderId = Guid.NewGuid();
            var restaurantId = Guid.NewGuid();

            var request = new UpdateOrderStatusDto
            {
                OrderId = orderId,
                Status = OrderStatus.Accepted
            };

            var order = new Order
            {
                Id = orderId,
                RestaurantId = restaurantId,
                Status = OrderStatus.Placed
            };

            _orderRepository
                .Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync(order);

            _restaurantRepository
                .Setup(x => x.IsRestaurantOwnerAsync( restaurantId, _userId))
                .ReturnsAsync(true);

            var result = await _orderService.UpdateOrderStatusAsync( request, _userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(SuccessMessages.OrderStatusUpdated));
                Assert.That(order.Status, Is.EqualTo(OrderStatus.Accepted));
            });
        }


        [Test]
        public async Task GetDashboardOrdersAsync_ReturnsDashboardOrders()
        {
            var ownerId = Guid.NewGuid();
            var restaurantId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var request = new DashboardOrderListRequestDto
            {
                Page = 1,
                PageSize = 10
            };

            var order = new Order
            {
                Id = orderId,
                RestaurantId = restaurantId,
                UserId = userId,
                Status = OrderStatus.Placed,
                PlacedAt = DateTime.UtcNow,
                Restaurant = new Restaurant
                {
                    Id = restaurantId,
                    Name = "Burger Hub"
                },
                User = new User
                {
                    Id = userId,
                    Name = "John"
                }
            };

            _restaurantRepository
                .Setup(x => x.GetAllRestaurantIdsByOwnerIdAsync(ownerId))
                .ReturnsAsync(new List<Guid>
                {
                    restaurantId
                });

            _orderRepository
                .Setup(x => x.GetDashboardOrdersCountAsync(
                    request,
                    It.Is<List<Guid>>(ids =>
                        ids.Count == 1 &&
                        ids[0] == restaurantId)))
                .ReturnsAsync(1);

            _orderRepository
                .Setup(x => x.GetOrdersForDashboardAsync(
                    request,
                    It.Is<List<Guid>>(ids =>
                        ids.Count == 1 &&
                        ids[0] == restaurantId)))
                .ReturnsAsync(new List<Order>
                {
                    order
                });

            var result = await _orderService.GetDashboardOrdersAsync(request, ownerId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(SuccessMessages.OrdersFetched));
                Assert.That(result.Data.Orders.Count, Is.EqualTo(1));
                Assert.That(result.Data.Orders[0].OrderId, Is.EqualTo(orderId));
                Assert.That(result.Data.Orders[0].RestaurantName, Is.EqualTo("Burger Hub"));
                Assert.That(result.Data.Orders[0].CustomerName, Is.EqualTo("John"));
                Assert.That(result.Data.Orders[0].OrderStatus, Is.EqualTo(OrderStatus.Placed));
                Assert.That(result.Data.TotalCount, Is.EqualTo(1));
                Assert.That(result.Data.Page, Is.EqualTo(1));
                Assert.That(result.Data.PageSize, Is.EqualTo(10));
                Assert.That(result.Data.TotalPages, Is.EqualTo(1));
            });
        }
    }
}
