using dotNetAssignment.Data;
using dotNetAssignment.Models.Entities;
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
    }
}
