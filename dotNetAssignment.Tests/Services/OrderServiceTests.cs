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
        public async Task PlaceOrder_WhenUserDoesNotExist_ReturnsFailure()
        {
            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync((User)null);

            var request = new OrderRequestDto
            {
                RestaurantId = Guid.NewGuid(),
                AddressId = Guid.NewGuid(),
                orderItems = new List<OrderItemsRequestDto>()
            };

            var result = await _orderService.PlaceOrder(request, _userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(
                    result.Message,
                    Is.EqualTo(ExceptionMessages.UserNotFound));
            });
        }

        [Test]
        public async Task PlaceOrder_WhenUserIsInactive_ReturnsFailure()
        {
            var user = new User
            {
                Id = _userId,
                IsActive = false
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            var request = new OrderRequestDto
            {
                RestaurantId = Guid.NewGuid(),
                AddressId = Guid.NewGuid(),
                orderItems = new List<OrderItemsRequestDto>()
            };

            var result = await _orderService.PlaceOrder(request, _userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(
                    result.Message,
                    Is.EqualTo(ExceptionMessages.UserNotFound));
            });
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
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            _restaurantRepository
                .Setup(x => x.GetRestaurantByIdAsync(restaurantId))
                .ReturnsAsync((Restaurant)null);

            var request = new OrderRequestDto
            {
                RestaurantId = restaurantId,
                AddressId = Guid.NewGuid(),
                orderItems = new List<OrderItemsRequestDto>()
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
                .Setup(x => x.GetUserByIdAsync(_userId))
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
                orderItems = new List<OrderItemsRequestDto>()
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
                .Setup(x => x.GetUserByIdAsync(_userId))
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
                orderItems = new List<OrderItemsRequestDto>()
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
                .Setup(x => x.GetUserByIdAsync(_userId))
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
                .Setup(x => x.GetMenuItemByIdAsync(menuId))
                .ReturnsAsync((Menu)null);

            var request = new OrderRequestDto
            {
                RestaurantId = restaurantId,
                AddressId = addressId,
                orderItems = new List<OrderItemsRequestDto>
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
                .Setup(x => x.GetUserByIdAsync(_userId))
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
                .Setup(x => x.GetMenuItemByIdAsync(menuId))
                .ReturnsAsync(menuItem);

            var request = new OrderRequestDto
            {
                RestaurantId = restaurantId,
                AddressId = addressId,
                orderItems = new List<OrderItemsRequestDto>
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
                .Setup(x => x.GetUserByIdAsync(_userId))
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
                .Setup(x => x.GetMenuItemByIdAsync(menuId))
                .ReturnsAsync(menuItem);

            var request = new OrderRequestDto
            {
                RestaurantId = restaurantId,
                AddressId = addressId,
                orderItems = new List<OrderItemsRequestDto>
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
                .Setup(x => x.GetUserByIdAsync(_userId))
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
                .Setup(x => x.GetMenuItemByIdAsync(menuId))
                .ReturnsAsync(menuItem);

            var request = new OrderRequestDto
            {
                RestaurantId = restaurantId,
                AddressId = addressId,
                orderItems = new List<OrderItemsRequestDto>
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
        public async Task OrderDetails_WhenUserDoesNotExist_ReturnsFailure()
        {
            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync((User)null);

            var request = new OrderDetailsRequestDto
            {
                OrderId = Guid.NewGuid()
            };

            var result = await _orderService.OrderDetails(request, _userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(
                    result.Message,
                    Is.EqualTo(ExceptionMessages.UserNotFound));
            });
        }

        [Test]
        public async Task OrderDetails_WhenUserIsInactive_ReturnsFailure()
        {
            var user = new User
            {
                Id = _userId,
                IsActive = false
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            var request = new OrderDetailsRequestDto
            {
                OrderId = Guid.NewGuid()
            };

            var result = await _orderService.OrderDetails(request, _userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(
                    result.Message,
                    Is.EqualTo(ExceptionMessages.UserNotFound));
            });
        }

        [Test]
        public async Task OrderDetails_WhenOrderDoesNotExist_ReturnsFailure()
        {
            var user = new User
            {
                Id = _userId,
                IsActive = true
            };

            var orderId = Guid.NewGuid();

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            _orderRepository
                .Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync((Order)null);

            var request = new OrderDetailsRequestDto
            {
                OrderId = orderId
            };

            var result = await _orderService.OrderDetails(request, _userId);

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
            var user = new User
            {
                Id = _userId,
                IsActive = true
            };

            var orderId = Guid.NewGuid();

            var order = new Order
            {
                Id = orderId,
                UserId = Guid.NewGuid()
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(_userId))
                .ReturnsAsync(user);

            _orderRepository
                .Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync(order);

            var request = new OrderDetailsRequestDto
            {
                OrderId = orderId
            };

            var result = await _orderService.OrderDetails(request, _userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(
                    result.Message,
                    Is.EqualTo(ExceptionMessages.OrderDoesntExists));
            });
        }
    }
}
