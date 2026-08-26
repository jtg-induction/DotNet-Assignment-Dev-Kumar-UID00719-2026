using dotNetAssignment.Constants;
using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.Entities;
using dotNetAssignment.Models.Enums;
using dotNetAssignment.Repositories.RestaurantRepo;
using dotNetAssignment.Repositories.UserRepo;
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
        private Mock<IUserRepository> _userRepository;
        private RestaurantService _restaurantService;

        [SetUp]
        public void Setup()
        {
            _restaurantRepository = new Mock<IRestaurantRepository>();
            _userRepository = new Mock<IUserRepository>();

            _restaurantService = new RestaurantService(
                _restaurantRepository.Object,
                _userRepository.Object);
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
                Assert.That(result.Data.Menu.Count, Is.EqualTo(2));
                Assert.That(result.Data.Page, Is.EqualTo(1));
                Assert.That(result.Data.PageSize, Is.EqualTo(2));
                Assert.That(result.Data.TotalCount, Is.EqualTo(5));
                Assert.That(result.Data.TotalPages, Is.EqualTo(3));
                Assert.That(result.Data.Menu[0].DishName, Is.EqualTo("Burger"));
                Assert.That(result.Data.Menu[0].Price, Is.EqualTo(150));
                Assert.That(result.Data.Menu[0].Rating, Is.EqualTo(4.5m));
            });
        }


        [Test]
        public async Task GetAllRestaurantsListAsync_WhenNoRestaurantsExist_ReturnsEmptyList()
        {
            _restaurantRepository
                .Setup(x => x.GetAllRestaurantsAsync(1, 10))
                .ReturnsAsync(new List<Restaurant>());

            _restaurantRepository
                .Setup(x => x.GetRestaurantCountAsync())
                .ReturnsAsync(0);

            var result =
                await _restaurantService.GetAllRestaurantsListAsync(1, 10);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Data.Restaurants, Is.Empty);
                Assert.That(result.Data.TotalCount, Is.EqualTo(0));
                Assert.That(result.Data.TotalPages, Is.EqualTo(0));
            });
        }


        [Test]
        public async Task GetMenuListAsync_WhenRestaurantHasNoMenuItems_ReturnsEmptyList()
        {
            var restaurantId = Guid.NewGuid();

            _restaurantRepository
                .Setup(x => x.RestaurantExistsAsync(restaurantId))
                .ReturnsAsync(true);

            _restaurantRepository
                .Setup(x => x.GetRestaurantMenuAsync(
                    restaurantId, 1, 10))
                .ReturnsAsync(new List<Menu>());

            _restaurantRepository
                .Setup(x => x.GetRestaurantMenuCountAsync(restaurantId))
                .ReturnsAsync(0);

            var result =
                await _restaurantService.GetMenuListAsync(
                    restaurantId, 1, 10);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Data.Menu, Is.Empty);
                Assert.That(result.Data.TotalCount, Is.EqualTo(0));
                Assert.That(result.Data.TotalPages, Is.EqualTo(0));
            });
        }


        [Test]
        public async Task CreateRestaurantAsync_WhenActiveCustomerExists_CreatesRestaurantAndPromotesOwner()
        {
            var ownerId = Guid.NewGuid();

            var owner = new User
            {
                Id = ownerId,
                IsActive = true,
                Role = UserRole.Customer
            };

            var request = new CreateRestaurantRequestDto
            {
                OwnerId = ownerId,
                Name = "Burger Hub",
                AddressLineOne = "24 MG Road",
                Landmark = "Near Metro",
                City = "Bengaluru",
                State = "Karnataka",
                Pincode = "560001"
            };

            Restaurant capturedRestaurant = null;
            RestaurantOwner capturedRestaurantOwner = null;

            _userRepository
                .Setup(x => x.GetUserByIdAsync(ownerId))
                .ReturnsAsync(owner);

            _restaurantRepository
                .Setup(x => x.AddRestaurantAsync(It.IsAny<Restaurant>()))
                .Callback<Restaurant>(restaurant =>
                    capturedRestaurant = restaurant)
                .Returns(Task.CompletedTask);

            _restaurantRepository
                .Setup(x => x.AddRestaurantOwnerAsync(It.IsAny<RestaurantOwner>()))
                .Callback<RestaurantOwner>(restaurantOwner =>
                    capturedRestaurantOwner = restaurantOwner)
                .Returns(Task.CompletedTask);

            var result =
                await _restaurantService.CreateRestaurantAsync(request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(SuccessMessages.RestaurantCreated));

                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data.RestaurantId, Is.EqualTo(capturedRestaurant.Id));

                Assert.That(owner.Role, Is.EqualTo(UserRole.Owner));

                Assert.That(capturedRestaurant, Is.Not.Null);
                Assert.That(capturedRestaurant.Name, Is.EqualTo(request.Name));
                Assert.That(capturedRestaurant.AddressLineOne, Is.EqualTo(request.AddressLineOne));
                Assert.That(capturedRestaurant.City, Is.EqualTo(request.City));
                Assert.That(capturedRestaurant.State, Is.EqualTo(request.State));
                Assert.That(capturedRestaurant.Pincode,Is.EqualTo(request.Pincode));
                Assert.That(capturedRestaurant.Rating, Is.EqualTo(0));

                Assert.That(capturedRestaurantOwner, Is.Not.Null);
                Assert.That(capturedRestaurantOwner.UserId,Is.EqualTo(ownerId));
                Assert.That(capturedRestaurantOwner.RestaurantId,Is.EqualTo(capturedRestaurant.Id));
            });

            _restaurantRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }


        [Test]
        public async Task CreateRestaurantAsync_WhenOwnerDoesNotExist_ReturnsUserNotFound()
        {
            var ownerId = Guid.NewGuid();

            var request = new CreateRestaurantRequestDto
            {
                OwnerId = ownerId,
                Name = "Burger Hub"
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(ownerId))
                .ReturnsAsync((User)null);

            var result =
                await _restaurantService.CreateRestaurantAsync(request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message, Is.EqualTo(ExceptionMessages.UserNotFound));
            });

            _restaurantRepository.Verify(
                x => x.AddRestaurantAsync(It.IsAny<Restaurant>()),
                Times.Never);

            _restaurantRepository.Verify(
                x => x.AddRestaurantOwnerAsync(It.IsAny<RestaurantOwner>()),
                Times.Never);

            _restaurantRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }


        [Test]
        public async Task CreateRestaurantAsync_WhenOwnerIsInactive_ReturnsUserNotFound()
        {
            var ownerId = Guid.NewGuid();

            var owner = new User
            {
                Id = ownerId,
                IsActive = false,
                Role = UserRole.Customer
            };

            var request = new CreateRestaurantRequestDto
            {
                OwnerId = ownerId,
                Name = "Burger Hub"
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(ownerId))
                .ReturnsAsync(owner);

            var result = await _restaurantService.CreateRestaurantAsync(request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message,Is.EqualTo(ExceptionMessages.UserNotFound));
            });

            _restaurantRepository.Verify(
                x => x.AddRestaurantAsync(It.IsAny<Restaurant>()),
                Times.Never);

            _restaurantRepository.Verify(
                x => x.AddRestaurantOwnerAsync(It.IsAny<RestaurantOwner>()),
                Times.Never);

            _restaurantRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }


        [Test]
        public async Task OnboardNewRestaurantOwnerAsync_WhenUserAndRestaurantExist_OnboardsOwner()
        {
            var ownerId = Guid.NewGuid();
            var restaurantId = Guid.NewGuid();

            var owner = new User
            {
                Id = ownerId,
                IsActive = true
            };

            var restaurant = new Restaurant
            {
                Id = restaurantId,
                Name = "Burger Hub"
            };

            var request = new OnboardNewRestaurantOwnerDto
            {
                OwnerId = ownerId,
                RestaurantId = restaurantId
            };

            RestaurantOwner capturedRestaurantOwner = null;

            _userRepository
                .Setup(x => x.GetUserByIdAsync(ownerId))
                .ReturnsAsync(owner);

            _restaurantRepository
                .Setup(x => x.GetRestaurantByIdAsync(restaurantId))
                .ReturnsAsync(restaurant);

            _restaurantRepository
                .Setup(x => x.AddRestaurantOwnerAsync(It.IsAny<RestaurantOwner>()))
                .Callback<RestaurantOwner>(restaurantOwner =>
                    capturedRestaurantOwner = restaurantOwner)
                .Returns(Task.CompletedTask);

            var result = await _restaurantService.OnboardNewRestaurantOwnerAsync(request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Message, Is.EqualTo(SuccessMessages.RestaurantOwnerOnboarded));

                Assert.That(capturedRestaurantOwner, Is.Not.Null);
                Assert.That(capturedRestaurantOwner.UserId, Is.EqualTo(ownerId));
                Assert.That(capturedRestaurantOwner.RestaurantId, Is.EqualTo(restaurantId));
            });

            _restaurantRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }


        [Test]
        public async Task OnboardNewRestaurantOwnerAsync_WhenUserDoesNotExist_ReturnsUserNotFound()
        {
            var ownerId = Guid.NewGuid();
            var restaurantId = Guid.NewGuid();

            var request = new OnboardNewRestaurantOwnerDto
            {
                OwnerId = ownerId,
                RestaurantId = restaurantId
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(ownerId))
                .ReturnsAsync((User)null);

            var result =
                await _restaurantService.OnboardNewRestaurantOwnerAsync(request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(
                    result.Message,
                    Is.EqualTo(ExceptionMessages.UserNotFound));
            });

            _restaurantRepository.Verify(
                x => x.GetRestaurantByIdAsync(It.IsAny<Guid>()),
                Times.Never);

            _restaurantRepository.Verify(
                x => x.AddRestaurantOwnerAsync(It.IsAny<RestaurantOwner>()),
                Times.Never);

            _restaurantRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }


        [Test]
        public async Task OnboardNewRestaurantOwnerAsync_WhenUserIsInactive_ReturnsUserNotFound()
        {
            var ownerId = Guid.NewGuid();
            var restaurantId = Guid.NewGuid();

            var owner = new User
            {
                Id = ownerId,
                IsActive = false
            };

            var request = new OnboardNewRestaurantOwnerDto
            {
                OwnerId = ownerId,
                RestaurantId = restaurantId
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(ownerId))
                .ReturnsAsync(owner);

            var result =
                await _restaurantService.OnboardNewRestaurantOwnerAsync(request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Message,Is.EqualTo(ExceptionMessages.UserNotFound));
            });

            _restaurantRepository.Verify(
                x => x.GetRestaurantByIdAsync(It.IsAny<Guid>()),
                Times.Never);

            _restaurantRepository.Verify(
                x => x.AddRestaurantOwnerAsync(It.IsAny<RestaurantOwner>()),
                Times.Never);

            _restaurantRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }


        [Test]
        public async Task OnboardNewRestaurantOwnerAsync_WhenRestaurantDoesNotExist_ReturnsRestaurantNotFound()
        {
            var ownerId = Guid.NewGuid();
            var restaurantId = Guid.NewGuid();

            var owner = new User
            {
                Id = ownerId,
                IsActive = true
            };

            var request = new OnboardNewRestaurantOwnerDto
            {
                OwnerId = ownerId,
                RestaurantId = restaurantId
            };

            _userRepository
                .Setup(x => x.GetUserByIdAsync(ownerId))
                .ReturnsAsync(owner);

            _restaurantRepository
                .Setup(x => x.GetRestaurantByIdAsync(restaurantId))
                .ReturnsAsync((Restaurant)null);

            var result =
                await _restaurantService.OnboardNewRestaurantOwnerAsync(request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(
                    result.Message,
                    Is.EqualTo(ExceptionMessages.RestaurantDoesntExists));
            });

            _restaurantRepository.Verify(
                x => x.AddRestaurantOwnerAsync(It.IsAny<RestaurantOwner>()),
                Times.Never);

            _restaurantRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }
    }
}
