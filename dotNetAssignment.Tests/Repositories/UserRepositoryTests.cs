using dotNetAssignment.Data;
using dotNetAssignment.Models.Entities;
using dotNetAssignment.Repositories.UserRepo;
using dotNetAssignment.Tests.Helpers;

using Moq;
using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace dotNetAssignment.Tests.Repositories.UserRepo
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private Mock<RestaurantDbContext> _context;
        private Mock<DbSet<User>> _users;
        private Mock<DbSet<UserAddress>> _userAddresses;
        private UserRepository _repository;

        [SetUp]
        public void Setup()
        {
            _context = new Mock<RestaurantDbContext>();
            _users = new Mock<DbSet<User>>();
            _userAddresses = new Mock<DbSet<UserAddress>>();

            _context
                .Setup(x => x.Users)
                .Returns(_users.Object);

            _context
                .Setup(x => x.UserAddresses)
                .Returns(_userAddresses.Object);

            _repository = new UserRepository(_context.Object);
        }

        private Mock<DbSet<T>> CreateAsyncDbSet<T>(
            IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();

            mockSet
                .As<IDbAsyncEnumerable<T>>()
                .Setup(x => x.GetAsyncEnumerator())
                .Returns(() =>
                    new TestDbAsyncEnumerator<T>(
                        data.GetEnumerator()));

            mockSet
                .As<IQueryable<T>>()
                .Setup(x => x.Provider)
                .Returns(() =>
                    new TestDbAsyncQueryProvider<T>(
                        data.Provider));

            mockSet
                .As<IQueryable<T>>()
                .Setup(x => x.Expression)
                .Returns(data.Expression);

            mockSet
                .As<IQueryable<T>>()
                .Setup(x => x.ElementType)
                .Returns(data.ElementType);

            mockSet
                .As<IQueryable<T>>()
                .Setup(x => x.GetEnumerator())
                .Returns(() =>
                    data.GetEnumerator());

            return mockSet;
        }

        [Test]
        public async Task EmailExistsAsync_WhenEmailExists_ReturnsTrue()
        {
            var email = "user@example.com";

            var data = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Email = email
                }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);

            _context
                .Setup(x => x.Users)
                .Returns(mockSet.Object);

            var result = await _repository.EmailExistsAsync(email);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task EmailExistsAsync_WhenEmailDoesNotExist_ReturnsFalse()
        {
            var data = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Email = "existing@example.com"
                }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);

            _context
                .Setup(x => x.Users)
                .Returns(mockSet.Object);

            var result =
                await _repository.EmailExistsAsync(
                    "user@example.com");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task PhoneNumberExistsAsync_WhenPhoneNumberDoesNotExist_ReturnsFalse()
        {
            var data = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    PhoneNumber = "9876543210"
                }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);

            _context
                .Setup(x => x.Users)
                .Returns(mockSet.Object);

            var result =
                await _repository.PhoneNumberExistsAsync("9999999999");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task PhoneNumberExistsAsync_WhenPhoneNumberExists_ReturnsTrue()
        {
            var phoneNumber = "9876543210";

            var data = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    PhoneNumber = phoneNumber
                }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);

            _context
                .Setup(x => x.Users)
                .Returns(mockSet.Object);

            var result =
                await _repository.PhoneNumberExistsAsync(phoneNumber);

            Assert.That(result, Is.True);
        }

        [Test]
        public void AddUser_AddsUser()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "user@example.com"
            };

            _repository.AddUser(user);

            _users.Verify(
                x => x.Add(user),
                Times.Once);
        }

        [Test]
        public void AddAddress_AddsAddress()
        {
            var address = new UserAddress();

            _repository.AddAddress(address);

            _userAddresses.Verify(
                x => x.Add(address),
                Times.Once);
        }

        [Test]
        public async Task GetUserByIdAsync_WhenUserExists_ReturnsUser()
        {
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                Email = "user@example.com"
            };

            var data = new List<User>
            {
                user
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);

            _context
                .Setup(x => x.Users)
                .Returns(mockSet.Object);

            var result =
                await _repository.GetUserByIdAsync(userId);

            Assert.That(result, Is.EqualTo(user));
        }

        [Test]
        public async Task GetUserByIdAsync_WhenUserDoesNotExist_ReturnsNull()
        {
            var existingUserId = Guid.NewGuid();
            var searchedUserId = Guid.NewGuid();

            var data = new List<User>
            {
                new User
                {
                    Id = existingUserId,
                    Email = "user@example.com"
                }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);

            _context
                .Setup(x => x.Users)
                .Returns(mockSet.Object);

            var result =
                await _repository.GetUserByIdAsync(searchedUserId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetAddressByIdAsync_WhenAddressExists_ReturnsAddress()
        {
            var addressId = Guid.NewGuid();

            var address = new UserAddress
            {
                Id = addressId
            };

            var data = new List<UserAddress>
            {
                address
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);

            _context
                .Setup(x => x.UserAddresses)
                .Returns(mockSet.Object);

            var result =
                await _repository.GetAddressByIdAsync(addressId);

            Assert.That(result, Is.EqualTo(address));
        }

        [Test]
        public async Task GetAddressByIdAsync_WhenAddressDoesNotExist_ReturnsNull()
        {
            var searchedAddressId = Guid.NewGuid();

            var data = new List<UserAddress>
            {
                new UserAddress
                {
                    Id = Guid.NewGuid()
                }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);

            _context
                .Setup(x => x.UserAddresses)
                .Returns(mockSet.Object);

            var result =
                await _repository.GetAddressByIdAsync(
                    searchedAddressId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task SaveChangesAsync_CallsContextSaveChanges()
        {
            _context
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            await _repository.SaveChangesAsync();

            _context.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }
    }
}
