using dotNetAssignment.Constants;
using dotNetAssignment.Models.Entities;
using dotNetAssignment.Repositories.RestaurantRepo;
using dotNetAssignment.Services.Implementations;

using Moq;
using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dotNetAssignment.Tests.Services
{
    [TestFixture]
    public class RestaurantServiceTests
    {
        private Mock<IRestaurantRepository> _restaurantRepository;
        private RestaurantService _restaurantService;

        [SetUp]
        public void Setup()
        {
            _restaurantRepository = new Mock<IRestaurantRepository>();

            _restaurantService = new RestaurantService(
                _restaurantRepository.Object);
        }


        [Test]
        public async Task GetAllRestaurantsListAsync_WhenPageIsValid_ReturnsRestaurants()
        {
            var restaurants = new List<Restaurant>
            {
                new Restaurant
                {
                    Id = Guid.NewGuid(),
                    Name = "Burger Hub",
                    AddressLineOne = "24 MG Road",
                    Landmark = "Near Metro",
                    City = "Bengaluru",
                    State = "Karnataka",
                    Pincode = "560001",
                    Rating = 4.5m
                },
                new Restaurant
                {
                    Id = Guid.NewGuid(),
                    Name = "Pizza Palace",
                    AddressLineOne = "12 Main Road",
                    Landmark = "Near Mall",
                    City = "Delhi",
                    State = "Delhi",
                    Pincode = "110001",
                    Rating = 4.2m
                }
            };

            _restaurantRepository
                .Setup(x => x.GetAllRestaurantsAsync(1, 2))
                .ReturnsAsync(restaurants);

            _restaurantRepository
                .Setup(x => x.GetRestaurantCountAsync())
                .ReturnsAsync(5);

            var result =
                await _restaurantService.GetAllRestaurantsListAsync(1, 2);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Data.Restaurants.Count, Is.EqualTo(2));
                Assert.That(result.Data.Page, Is.EqualTo(1));
                Assert.That(result.Data.PageSize, Is.EqualTo(2));
                Assert.That(result.Data.TotalCount, Is.EqualTo(5));
                Assert.That(result.Data.TotalPages, Is.EqualTo(3));

                Assert.That(
                    result.Data.Restaurants[0].Name,
                    Is.EqualTo("Burger Hub"));

                Assert.That(
                    result.Data.Restaurants[0].City,
                    Is.EqualTo("Bengaluru"));

                Assert.That(
                    result.Data.Restaurants[0].Rating,
                    Is.EqualTo(4.5m));
            });
        }


        [Test]
        public async Task GetAllRestaurantsListAsync_WhenPageIsOutOfRange_ReturnsPageNotFound()
        {
            _restaurantRepository
                .Setup(x => x.GetAllRestaurantsAsync(4, 2))
                .ReturnsAsync(new List<Restaurant>());

            _restaurantRepository
                .Setup(x => x.GetRestaurantCountAsync())
                .ReturnsAsync(5);

            var result =
                await _restaurantService.GetAllRestaurantsListAsync(4, 2);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(
                    result.Message,
                    Is.EqualTo(ExceptionMessages.PageNotFound));
            });
        }


        [Test]
        public async Task GetMenuListAsync_WhenRestaurantDoesNotExist_ReturnsFailure()
        {
            var restaurantId = Guid.NewGuid();

            _restaurantRepository
                .Setup(x => x.RestaurantExistsAsync(restaurantId))
                .ReturnsAsync(false);

            var result =
                await _restaurantService.GetMenuListAsync(
                    restaurantId,
                    1,
                    2);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ExceptionMessages.RestaurantDoesntExists));
            });

        }


        [Test]
        public async Task GetMenuListAsync_WhenPageIsOutOfRange_ReturnsPageNotFound()
        {
            var restaurantId = Guid.NewGuid();

            _restaurantRepository
                .Setup(x => x.RestaurantExistsAsync(restaurantId))
                .ReturnsAsync(true);

            _restaurantRepository
                .Setup(x => x.GetRestaurantMenuAsync(
                    restaurantId, 4, 2))
                .ReturnsAsync(new List<Menu>());

            _restaurantRepository
                .Setup(x => x.GetRestaurantMenuCountAsync(restaurantId))
                .ReturnsAsync(5);

            var result = await _restaurantService.GetMenuListAsync(restaurantId, 4, 2);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(
                    result.Message,
                    Is.EqualTo(ExceptionMessages.PageNotFound));
            });
        }


        [Test]
        public async Task GetMenuListAsync_WhenPageIsValid_ReturnsMenuItems()
        {
            var restaurantId = Guid.NewGuid();

            var menuItems = new List<Menu>
            {
                new Menu
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId,
                    DishName = "Burger",
                    Price = 150,
                    Rating = 4.5m
                },
                new Menu
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurantId,
                    DishName = "Pizza",
                    Price = 250,
                    Rating = 4.2m
                }
            };

            _restaurantRepository
                .Setup(x => x.RestaurantExistsAsync(restaurantId))
                .ReturnsAsync(true);

            _restaurantRepository
                .Setup(x => x.GetRestaurantMenuAsync(restaurantId, 1, 2))
                .ReturnsAsync(menuItems);

            _restaurantRepository
                .Setup(x => x.GetRestaurantMenuCountAsync(restaurantId))
                .ReturnsAsync(5);

            var result = await _restaurantService.GetMenuListAsync(restaurantId, 1, 2);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Data.menu.Count, Is.EqualTo(2));
                Assert.That(result.Data.Page, Is.EqualTo(1));
                Assert.That(result.Data.PageSize, Is.EqualTo(2));
                Assert.That(result.Data.TotalCount, Is.EqualTo(5));
                Assert.That(result.Data.TotalPages, Is.EqualTo(3));
                Assert.That(result.Data.menu[0].DishName, Is.EqualTo("Burger"));
                Assert.That(result.Data.menu[0].Price, Is.EqualTo(150));
                Assert.That(result.Data.menu[0].Rating, Is.EqualTo(4.5m));
            });
        }
    }
}
