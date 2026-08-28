using dotNetAssignment.Data;
using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.Entities;
using dotNetAssignment.Models.Enums;
using dotNetAssignment.Repositories.OrderRepository;
using dotNetAssignment.Tests.Helpers;

using Moq;
using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace dotNetAssignment.Tests.Repositories.OrderRepo
{
    [TestFixture]
    public class OrderRepositoryTests
    {
        private Mock<RestaurantDbContext> _context;
        private Mock<DbSet<Menu>> _menus;
        private Mock<DbSet<Order>> _orders;
        private Mock<DbSet<OrderItem>> _orderItems;
        private OrderRepository _repository;

        [SetUp]
        public void Setup()
        {
            _context = new Mock<RestaurantDbContext>();
            _menus = new Mock<DbSet<Menu>>();
            _orders = new Mock<DbSet<Order>>();
            _orderItems = new Mock<DbSet<OrderItem>>();

            _context.Setup(x => x.Menus).Returns(_menus.Object);
            _context.Setup(x => x.Orders).Returns(_orders.Object);
            _context.Setup(x => x.OrderItems).Returns(_orderItems.Object);

            _repository = new OrderRepository(_context.Object);
        }

        private Mock<DbSet<T>> CreateAsyncDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IDbAsyncEnumerable<T>>()
                .Setup(x => x.GetAsyncEnumerator())
                .Returns(() => new TestDbAsyncEnumerator<T>(data.GetEnumerator()));

            mockSet.As<IQueryable<T>>()
                .Setup(x => x.Provider)
                .Returns(() => new TestDbAsyncQueryProvider<T>(data.Provider));

            mockSet.As<IQueryable<T>>()
                .Setup(x => x.Expression)
                .Returns(data.Expression);

            mockSet.As<IQueryable<T>>()
                .Setup(x => x.ElementType)
                .Returns(data.ElementType);

            mockSet.As<IQueryable<T>>()
                .Setup(x => x.GetEnumerator())
                .Returns(() => data.GetEnumerator());

            mockSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockSet.Object);
            mockSet.Setup(x => x.AsNoTracking()).Returns(mockSet.Object);

            return mockSet;
        }

        [Test]
        public async Task MenuItemExistsAsync_WhenMenuItemExists_ReturnsTrue()
        {
            var menuId = Guid.NewGuid();

            var data = new List<Menu>
            {
                new Menu { Id = menuId }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);

            _context.Setup(x => x.Menus).Returns(mockSet.Object);

            var result = await _repository.MenuItemExistsAsync(menuId);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task MenuItemExistsAsync_WhenMenuItemDoesNotExist_ReturnsFalse()
        {
            var searchedMenuId = Guid.NewGuid();
            var data = new List<Menu>
            {
                new Menu { Id = Guid.NewGuid() }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);

            _context.Setup(x => x.Menus).Returns(mockSet.Object);

            var result = await _repository.MenuItemExistsAsync(searchedMenuId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetAllMenuItemsByOrderIdAsync_WhenMenuItemIdsAreNull_ReturnsEmptyList()
        {
            var result =
                await _repository.GetAllMenuItemsByOrderIdAsync(null);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }


        [Test]
        public async Task GetAllMenuItemsByOrderIdAsync_WhenMenuItemIdsAreEmpty_ReturnsEmptyList()
        {
            var result =
                await _repository.GetAllMenuItemsByOrderIdAsync(
                    new List<Guid>());

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }


        [Test]
        public void AddOrder_AddsOrder()
        {
            var order = new Order
            {
                Id = Guid.NewGuid()
            };

            _repository.AddOrder(order);

            _orders.Verify(x => x.Add(order), Times.Once);
        }

        [Test]
        public void AddOrderItem_AddsOrderItem()
        {
            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid()
            };

            _repository.AddOrderItem(orderItem);

            _orderItems.Verify(x => x.Add(orderItem), Times.Once);
        }

        [Test]
        public async Task SaveChangesAsync_CallsContextSaveChanges()
        {
            _context.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            await _repository.SaveChangesAsync();

            _context.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetOrderByIdAsync_WhenOrderExists_ReturnsOrder()
        {
            var orderId = Guid.NewGuid();

            var data = new List<Order>
            {
                new Order
                {
                    Id = orderId
                }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);
            _context.Setup(x => x.Orders).Returns(mockSet.Object);

            var result = await _repository.GetOrderByIdAsync(orderId);

            Assert.That(result, Is.EqualTo(data.First()));
        }


        [Test]
        public async Task GetOrderItemsByOrderIdAsync_ReturnsItemsForSpecifiedOrder()
        {
            var orderId = Guid.NewGuid();
            var anotherOrderId = Guid.NewGuid();

            var item1 = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = orderId
            };

            var item2 = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = orderId
            };

            var otherItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = anotherOrderId
            };

            var data = new List<OrderItem>
            {
                item1,
                item2,
                otherItem
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);
            _context.Setup(x => x.OrderItems).Returns(mockSet.Object);

            var result = await _repository.GetOrderItemsByOrderIdAsync(orderId);

            Assert.Multiple(() =>
            {
                Assert.That(result.Count, Is.EqualTo(2));
                Assert.That(result, Does.Contain(item1));
                Assert.That(result, Does.Contain(item2));
                Assert.That(result, Does.Not.Contain(otherItem));
            });
        }


        [Test]
        public async Task GetOrdersForDashboardAsync_ReturnsOrdersFromOwnerRestaurants()
        {
            var restaurantId1 = Guid.NewGuid();
            var restaurantId2 = Guid.NewGuid();
            var otherRestaurantId = Guid.NewGuid();

            var data = new List<Order>
            {
                new Order
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId1,
                    Status = OrderStatus.Placed,
                    PlacedAt = DateTime.UtcNow
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId2,
                    Status = OrderStatus.Accepted,
                    PlacedAt = DateTime.UtcNow
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = otherRestaurantId,
                    Status = OrderStatus.Dispatched,
                    PlacedAt = DateTime.UtcNow
                }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);
            _context.Setup(x => x.Orders).Returns(mockSet.Object);

            var request = new DashboardOrderListRequestDto
            {
                Page = 1,
                PageSize = 10,
                SortBy = SortOrdersFields.PlacedAt,
                SortOrder = "desc"
            };

            var result = await _repository.GetOrdersForDashboardAsync(
                request,
                new List<Guid> { restaurantId1, restaurantId2 });

            Assert.Multiple(() =>
            {
                Assert.That(result.Count, Is.EqualTo(2));
                Assert.That(result.All(x =>
                    x.RestaurantId == restaurantId1 ||
                    x.RestaurantId == restaurantId2), Is.True);
            });
        }


        [Test]
        public async Task GetOrdersForDashboardAsync_FiltersByStatus()
        {
            var restaurantId = Guid.NewGuid();

            var data = new List<Order>
            {
                new Order
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId,
                    Status = OrderStatus.Placed,
                    PlacedAt = DateTime.UtcNow
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId,
                    Status = OrderStatus.Accepted,
                    PlacedAt = DateTime.UtcNow
                }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);
            _context.Setup(x => x.Orders).Returns(mockSet.Object);

            var request = new DashboardOrderListRequestDto
            {
                Page = 1,
                PageSize = 10,
                SortBy = SortOrdersFields.PlacedAt,
                SortOrder = "desc"
            };

            request.Filter.Status = OrderStatus.Accepted;

            var result = await _repository.GetOrdersForDashboardAsync(
                request,
                new List<Guid> { restaurantId });

            Assert.Multiple(() =>
            {
                Assert.That(result.Count, Is.EqualTo(1));
                Assert.That(result[0].Status, Is.EqualTo(OrderStatus.Accepted));
            });
        }


        [Test]
        public async Task GetOrdersForDashboardAsync_FiltersByPlacedAtDate()
        {
            var restaurantId = Guid.NewGuid();

            var targetDate = new DateTime(2026, 8, 26);
            var nextDate = targetDate.AddDays(1);

            var data = new List<Order>
            {
                new Order
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId,
                    Status = OrderStatus.Placed,
                    PlacedAt = targetDate.AddHours(5)
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId,
                    Status = OrderStatus.Accepted,
                    PlacedAt = targetDate.AddHours(10)
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId,
                    Status = OrderStatus.Dispatched,
                    PlacedAt = nextDate.AddHours(2)
                }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);
            _context.Setup(x => x.Orders).Returns(mockSet.Object);

            var request = new DashboardOrderListRequestDto
            {
                Page = 1,
                PageSize = 10,
                SortBy = SortOrdersFields.PlacedAt,
                SortOrder = "desc"
            };

            request.Filter.PlacedAt = targetDate.AddHours(15);

            var result = await _repository.GetOrdersForDashboardAsync(
                request,
                new List<Guid> { restaurantId });

            Assert.Multiple(() =>
            {
                Assert.That(result.Count, Is.EqualTo(2));
                Assert.That(result.All(x =>
                    x.PlacedAt >= targetDate &&
                    x.PlacedAt < nextDate), Is.True);
            });
        }


        [Test]
        public async Task GetOrdersForDashboardAsync_FiltersBySearchOrderId()
        {
            var restaurantId = Guid.NewGuid();
            var searchOrderId = Guid.NewGuid();

            var data = new List<Order>
            {
                new Order
                {
                    Id = searchOrderId,
                    RestaurantId = restaurantId,
                    Status = OrderStatus.Placed,
                    PlacedAt = DateTime.UtcNow
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId,
                    Status = OrderStatus.Accepted,
                    PlacedAt = DateTime.UtcNow
                }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);
            _context.Setup(x => x.Orders).Returns(mockSet.Object);

            var request = new DashboardOrderListRequestDto
            {
                Page = 1,
                PageSize = 10,
                SearchOrderId = searchOrderId,
                SortBy = SortOrdersFields.PlacedAt,
                SortOrder = "desc"
            };

            var result = await _repository.GetOrdersForDashboardAsync(
                request,
                new List<Guid> { restaurantId });

            Assert.Multiple(() =>
            {
                Assert.That(result.Count, Is.EqualTo(1));
                Assert.That(result[0].Id, Is.EqualTo(searchOrderId));
            });
        }


        [Test]
        public async Task GetOrdersForDashboardAsync_SortsByStatusAscending()
        {
            var restaurantId = Guid.NewGuid();

            var dispatched = new Order
            {
                Id = Guid.NewGuid(),
                RestaurantId = restaurantId,
                Status = OrderStatus.Dispatched,
                PlacedAt = DateTime.UtcNow
            };

            var accepted = new Order
            {
                Id = Guid.NewGuid(),
                RestaurantId = restaurantId,
                Status = OrderStatus.Accepted,
                PlacedAt = DateTime.UtcNow
            };

            var placed = new Order
            {
                Id = Guid.NewGuid(),
                RestaurantId = restaurantId,
                Status = OrderStatus.Placed,
                PlacedAt = DateTime.UtcNow
            };

            var delivered = new Order
            {
                Id = Guid.NewGuid(),
                RestaurantId = restaurantId,
                Status = OrderStatus.Delivered,
                PlacedAt = DateTime.UtcNow
            };

            var data = new List<Order>
            {
                delivered,
                placed,
                accepted,
                dispatched
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);
            _context.Setup(x => x.Orders).Returns(mockSet.Object);

            var request = new DashboardOrderListRequestDto
            {
                Page = 1,
                PageSize = 10,
                SortBy = SortOrdersFields.Status,
                SortOrder = "asc"
            };

            var result = await _repository.GetOrdersForDashboardAsync(
                request,
                new List<Guid> { restaurantId });

            Assert.That(
                result.Select(x => x.Status).ToList(),
                Is.EqualTo(new[]
                {
                OrderStatus.Dispatched,
                OrderStatus.Accepted,
                OrderStatus.Placed,
                OrderStatus.Delivered
                }));
        }


        [Test]
        public async Task GetOrdersForDashboardAsync_SortsByStatusDescending()
        {
            var restaurantId = Guid.NewGuid();

            var dispatched = new Order
            {
                Id = Guid.NewGuid(),
                RestaurantId = restaurantId,
                Status = OrderStatus.Dispatched,
                PlacedAt = DateTime.UtcNow
            };

            var accepted = new Order
            {
                Id = Guid.NewGuid(),
                RestaurantId = restaurantId,
                Status = OrderStatus.Accepted,
                PlacedAt = DateTime.UtcNow
            };

            var placed = new Order
            {
                Id = Guid.NewGuid(),
                RestaurantId = restaurantId,
                Status = OrderStatus.Placed,
                PlacedAt = DateTime.UtcNow
            };

            var delivered = new Order
            {
                Id = Guid.NewGuid(),
                RestaurantId = restaurantId,
                Status = OrderStatus.Delivered,
                PlacedAt = DateTime.UtcNow
            };

            var data = new List<Order>
            {
                dispatched,
                accepted,
                placed,
                delivered
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);
            _context.Setup(x => x.Orders).Returns(mockSet.Object);

            var request = new DashboardOrderListRequestDto
            {
                Page = 1,
                PageSize = 10,
                SortBy = SortOrdersFields.Status,
                SortOrder = "desc"
            };

            var result = await _repository.GetOrdersForDashboardAsync(
                request,
                new List<Guid> { restaurantId });

            Assert.That(result.Select(x => x.Status).ToList(), Is.EqualTo(new[]
                {
                OrderStatus.Delivered,
                OrderStatus.Placed,
                OrderStatus.Accepted,
                OrderStatus.Dispatched
                }));
        }


        [Test]
        public async Task GetOrdersForDashboardAsync_SortsByPlacedAtAscending()
        {
            var restaurantId = Guid.NewGuid();

            var older = new Order
            {
                Id = Guid.NewGuid(),
                RestaurantId = restaurantId,
                PlacedAt = DateTime.UtcNow.AddHours(-3)
            };

            var middle = new Order
            {
                Id = Guid.NewGuid(),
                RestaurantId = restaurantId,
                PlacedAt = DateTime.UtcNow.AddHours(-2)
            };

            var newer = new Order
            {
                Id = Guid.NewGuid(),
                RestaurantId = restaurantId,
                PlacedAt = DateTime.UtcNow.AddHours(-1)
            };

            var data = new List<Order>
            {
                newer,
                older,
                middle
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);
            _context.Setup(x => x.Orders).Returns(mockSet.Object);

            var request = new DashboardOrderListRequestDto
            {
                Page = 1,
                PageSize = 10,
                SortBy = SortOrdersFields.PlacedAt,
                SortOrder = "asc"
            };

            var result = await _repository.GetOrdersForDashboardAsync(
                request,
                new List<Guid> { restaurantId });

            Assert.That(result.Select(x => x.Id).ToList(),
                Is.EqualTo(new[]
                {older.Id, middle.Id, newer.Id}));
        }


        [Test]
        public async Task GetOrdersForDashboardAsync_SortsByPlacedAtDescending()
        {
            var restaurantId = Guid.NewGuid();

            var older = new Order
            {
                Id = Guid.NewGuid(),
                RestaurantId = restaurantId,
                PlacedAt = DateTime.UtcNow.AddHours(-3)
            };

            var middle = new Order
            {
                Id = Guid.NewGuid(),
                RestaurantId = restaurantId,
                PlacedAt = DateTime.UtcNow.AddHours(-2)
            };

            var newer = new Order
            {
                Id = Guid.NewGuid(),
                RestaurantId = restaurantId,
                PlacedAt = DateTime.UtcNow.AddHours(-1)
            };

            var data = new List<Order>
            {
                older,
                newer,
                middle
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);
            _context.Setup(x => x.Orders).Returns(mockSet.Object);

            var request = new DashboardOrderListRequestDto
            {
                Page = 1,
                PageSize = 10,
                SortBy = SortOrdersFields.PlacedAt,
                SortOrder = "desc"
            };

            var result = await _repository.GetOrdersForDashboardAsync(
                request,
                new List<Guid> { restaurantId });

            Assert.That(
                result.Select(x => x.Id).ToList(),
                Is.EqualTo(new[]
                { newer.Id, middle.Id, older.Id }));
        }

        [Test]
        public async Task GetDashboardOrdersCountAsync_ReturnsCountForOwnerRestaurants()
        {
            var restaurantId1 = Guid.NewGuid();
            var restaurantId2 = Guid.NewGuid();
            var otherRestaurantId = Guid.NewGuid();

            var data = new List<Order>
            {
                new Order
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId1,
                    PlacedAt = DateTime.UtcNow
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId2,
                    PlacedAt = DateTime.UtcNow
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = otherRestaurantId,
                    PlacedAt = DateTime.UtcNow
                }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);
            _context.Setup(x => x.Orders).Returns(mockSet.Object);

            var request = new DashboardOrderListRequestDto
            {
                Page = 1,
                PageSize = 10
            };

            var result = await _repository.GetDashboardOrdersCountAsync(
                request,
                new List<Guid> { restaurantId1, restaurantId2 });

            Assert.That(result, Is.EqualTo(2));
        }


        [Test]
        public async Task GetDashboardOrdersCountAsync_FiltersByStatus()
        {
            var restaurantId = Guid.NewGuid();

            var data = new List<Order>
            {
                new Order
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId,
                    Status = OrderStatus.Placed,
                    PlacedAt = DateTime.UtcNow
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId,
                    Status = OrderStatus.Accepted,
                    PlacedAt = DateTime.UtcNow
                }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);
            _context.Setup(x => x.Orders).Returns(mockSet.Object);

            var request = new DashboardOrderListRequestDto
            {
                Page = 1,
                PageSize = 10
            };

            request.Filter.Status = OrderStatus.Accepted;

            var result = await _repository.GetDashboardOrdersCountAsync(
                request,
                new List<Guid> { restaurantId });

            Assert.That(result, Is.EqualTo(1));
        }


        [Test]
        public async Task GetDashboardOrdersCountAsync_FiltersByPlacedAtDate()
        {
            var restaurantId = Guid.NewGuid();

            var targetDate = new DateTime(2026, 8, 26);
            var nextDate = targetDate.AddDays(1);

            var data = new List<Order>
            {
                new Order
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId,
                    PlacedAt = targetDate.AddHours(5)
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId,
                    PlacedAt = targetDate.AddHours(10)
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId,
                    PlacedAt = nextDate.AddHours(2)
                }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);
            _context.Setup(x => x.Orders).Returns(mockSet.Object);

            var request = new DashboardOrderListRequestDto
            {
                Page = 1,
                PageSize = 10
            };

            request.Filter.PlacedAt = targetDate;

            var result = await _repository.GetDashboardOrdersCountAsync(
                request,
                new List<Guid> { restaurantId });

            Assert.That(result, Is.EqualTo(2));
        }


        [Test]
        public async Task GetDashboardOrdersCountAsync_FiltersBySearchOrderId()
        {
            var restaurantId = Guid.NewGuid();
            var searchOrderId = Guid.NewGuid();

            var data = new List<Order>
            {
                new Order
                {
                    Id = searchOrderId,
                    RestaurantId = restaurantId,
                    PlacedAt = DateTime.UtcNow
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId,
                    PlacedAt = DateTime.UtcNow
                }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);
            _context.Setup(x => x.Orders).Returns(mockSet.Object);

            var request = new DashboardOrderListRequestDto
            {
                Page = 1,
                PageSize = 10,
                SearchOrderId = searchOrderId
            };

            var result = await _repository.GetDashboardOrdersCountAsync(
                request,
                new List<Guid> { restaurantId });

            Assert.That(result, Is.EqualTo(1));
        }
    }
}
