using dotNetAssignment.Data;
using dotNetAssignment.Models.Entities;
using dotNetAssignment.Repositories.RestaurantRepo;
using dotNetAssignment.Tests.Helpers;

using Moq;
using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace dotNetAssignment.Tests.Repositories.RestaurantRepo
{
    [TestFixture]
    public class RestaurantRepositoryTests
    {
        private Mock<RestaurantDbContext> _context;
        private Mock<DbSet<Restaurant>> _restaurants;
        private Mock<DbSet<Menu>> _menus;
        private RestaurantRepository _repository;

        [SetUp]
        public void Setup()
        {
            _context = new Mock<RestaurantDbContext>();
            _restaurants = new Mock<DbSet<Restaurant>>();
            _menus = new Mock<DbSet<Menu>>();

            _context
                .Setup(x => x.Restaurants)
                .Returns(_restaurants.Object);

            _context
                .Setup(x => x.Menus)
                .Returns(_menus.Object);

            _repository = new RestaurantRepository(_context.Object);
        }

        private Mock<DbSet<T>> CreateAsyncDbSet<T>(IQueryable<T> data) where T : class
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
        public async Task GetAllRestaurantsAsync_WhenRestaurantsExist_ReturnsRestaurants()
        {
            var restaurant1 = new Restaurant
            {
                Id = Guid.NewGuid(),
                Name = "Burger Hub"
            };

            var restaurant2 = new Restaurant
            {
                Id = Guid.NewGuid(),
                Name = "Pizza Palace"
            };

            var data = new List<Restaurant>
            {
                restaurant1,
                restaurant2
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);

            _context
                .Setup(x => x.Restaurants)
                .Returns(mockSet.Object);

            var result =
                await _repository.GetAllRestaurantsAsync(1, 10);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result, Does.Contain(restaurant1));
            Assert.That(result, Does.Contain(restaurant2));
        }


        [Test]
        public async Task GetAllRestaurantsAsync_AppliesPaginationAndOrdering()
        {
            var data = new List<Restaurant>
            {
                new Restaurant
                {
                    Id = Guid.NewGuid(),
                    Name = "Burger Hub"
                },
                new Restaurant
                {
                    Id = Guid.NewGuid(),
                    Name = "Dominos"
                },
                new Restaurant
                {
                    Id = Guid.NewGuid(),
                    Name = "Pizza Palace"
                },
                new Restaurant
                {
                    Id = Guid.NewGuid(),
                    Name = "Taco Town"
                }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);

            _context
                .Setup(x => x.Restaurants)
                .Returns(mockSet.Object);

            var result =
                await _repository.GetAllRestaurantsAsync(2, 2);

            Assert.Multiple(() =>
            {
                Assert.That(result.Count, Is.EqualTo(2));
                Assert.That(result[0].Name, Is.EqualTo("Pizza Palace"));
                Assert.That(result[1].Name, Is.EqualTo("Taco Town"));
            });
        }


        [Test]
        public async Task GetAllRestaurantsAsync_WhenPageIsOutOfRange_ReturnsEmptyList()
        {
            var data = new List<Restaurant>
            {
                new Restaurant
                {
                    Id = Guid.NewGuid(),
                    Name = "Burger Hub"
                }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);

            _context
                .Setup(x => x.Restaurants)
                .Returns(mockSet.Object);

            var result =
                await _repository.GetAllRestaurantsAsync(5, 10);

            Assert.That(result, Is.Empty);
        }


        [Test]
        public async Task GetRestaurantCountAsync_ReturnsCorrectCount()
        {
            var data = new List<Restaurant>
            {
                new Restaurant
                {
                    Id = Guid.NewGuid(),
                    Name = "Burger Hub"
                },
                new Restaurant
                {
                    Id = Guid.NewGuid(),
                    Name = "Pizza Palace"
                },
                new Restaurant
                {
                    Id = Guid.NewGuid(),
                    Name = "Taco Town"
                }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);

            _context
                .Setup(x => x.Restaurants)
                .Returns(mockSet.Object);

            var result =
                await _repository.GetRestaurantCountAsync();

            Assert.That(result, Is.EqualTo(3));
        }


        [Test]
        public async Task RestaurantExistsAsync_WhenRestaurantExists_ReturnsTrue()
        {
            var restaurantId = Guid.NewGuid();

            var data = new List<Restaurant>
            {
                new Restaurant
                {
                    Id = restaurantId,
                    Name = "Burger Hub"
                }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);

            _context
                .Setup(x => x.Restaurants)
                .Returns(mockSet.Object);

            var result =
                await _repository.RestaurantExistsAsync(restaurantId);

            Assert.That(result, Is.True);
        }


        [Test]
        public async Task RestaurantExistsAsync_WhenRestaurantDoesNotExist_ReturnsFalse()
        {
            var existingRestaurantId = Guid.NewGuid();
            var searchedRestaurantId = Guid.NewGuid();

            var data = new List<Restaurant>
            {
                new Restaurant
                {
                    Id = existingRestaurantId,
                    Name = "Burger Hub"
                }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);

            _context
                .Setup(x => x.Restaurants)
                .Returns(mockSet.Object);

            var result =
                await _repository.RestaurantExistsAsync(
                    searchedRestaurantId);

            Assert.That(result, Is.False);
        }


        [Test]
        public async Task GetRestaurantMenuAsync_ReturnsMenuItemsForRestaurant()
        {
            var restaurantId = Guid.NewGuid();
            var anotherRestaurantId = Guid.NewGuid();

            var menu1 = new Menu
            {
                Id = Guid.NewGuid(),
                RestaurantId = restaurantId,
                DishName = "Burger"
            };

            var menu2 = new Menu
            {
                Id = Guid.NewGuid(),
                RestaurantId = restaurantId,
                DishName = "Pizza"
            };

            var menuFromAnotherRestaurant = new Menu
            {
                Id = Guid.NewGuid(),
                RestaurantId = anotherRestaurantId,
                DishName = "Pasta"
            };

            var data = new List<Menu>
            {
                menu1,
                menu2,
                menuFromAnotherRestaurant
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);

            _context
                .Setup(x => x.Menus)
                .Returns(mockSet.Object);

            var result =
                await _repository.GetRestaurantMenuAsync(
                    restaurantId,
                    1,
                    10);

            Assert.Multiple(() =>
            {
                Assert.That(result.Count, Is.EqualTo(2));
                Assert.That(result, Does.Contain(menu1));
                Assert.That(result, Does.Contain(menu2));
                Assert.That(result, Does.Not.Contain(menuFromAnotherRestaurant));
            });
        }


        [Test]
        public async Task GetRestaurantMenuAsync_AppliesPaginationAndOrdering()
        {
            var restaurantId = Guid.NewGuid();

            var data = new List<Menu>
            {
                new Menu
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId,
                    DishName = "Burger"
                },
                new Menu
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId,
                    DishName = "Fries"
                },
                new Menu
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId,
                    DishName = "Pizza"
                },
                new Menu
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId,
                    DishName = "Sandwich"
                }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);

            _context
                .Setup(x => x.Menus)
                .Returns(mockSet.Object);

            var result = await _repository.GetRestaurantMenuAsync(restaurantId, 2, 2);

            Assert.Multiple(() =>
            {
                Assert.That(result.Count, Is.EqualTo(2));
                Assert.That(result[0].DishName, Is.EqualTo("Pizza"));
                Assert.That(result[1].DishName, Is.EqualTo("Sandwich"));
            });
        }


        [Test]
        public async Task GetRestaurantMenuAsync_WhenPageIsOutOfRange_ReturnsEmptyList()
        {
            var restaurantId = Guid.NewGuid();

            var data = new List<Menu>
            {
                new Menu
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId,
                    DishName = "Burger"
                }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);

            _context
                .Setup(x => x.Menus)
                .Returns(mockSet.Object);

            var result = await _repository.GetRestaurantMenuAsync(restaurantId, 5, 10);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetRestaurantMenuCountAsync_ReturnsCorrectCount()
        {
            var restaurantId = Guid.NewGuid();
            var anotherRestaurantId = Guid.NewGuid();

            var data = new List<Menu>
            {
                new Menu
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId,
                    DishName = "Burger"
                },
                new Menu
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId,
                    DishName = "Pizza"
                },
                new Menu
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = anotherRestaurantId,
                    DishName = "Pasta"
                }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);

            _context
                .Setup(x => x.Menus)
                .Returns(mockSet.Object);

            var result = await _repository.GetRestaurantMenuCountAsync(restaurantId);

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public async Task GetRestaurantByIdAsync_WhenRestaurantExists_ReturnsRestaurant()
        {
            var restaurantId = Guid.NewGuid();

            var restaurant = new Restaurant
            {
                Id = restaurantId,
                Name = "Burger Hub"
            };

            var data = new List<Restaurant>
            {
                restaurant
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);

            _context
                .Setup(x => x.Restaurants)
                .Returns(mockSet.Object);

            var result = await _repository.GetRestaurantByIdAsync(restaurantId);

            Assert.That(result, Is.EqualTo(restaurant));
        }


        [Test]
        public async Task GetRestaurantByIdAsync_WhenRestaurantDoesNotExist_ReturnsNull()
        {
            var searchedRestaurantId = Guid.NewGuid();

            var data = new List<Restaurant>
            {
                new Restaurant
                {
                    Id = Guid.NewGuid(),
                    Name = "Burger Hub"
                }
            }.AsQueryable();

            var mockSet = CreateAsyncDbSet(data);

            _context
                .Setup(x => x.Restaurants)
                .Returns(mockSet.Object);

            var result = await _repository.GetRestaurantByIdAsync(searchedRestaurantId);

            Assert.That(result, Is.Null);
        }
    }
}

