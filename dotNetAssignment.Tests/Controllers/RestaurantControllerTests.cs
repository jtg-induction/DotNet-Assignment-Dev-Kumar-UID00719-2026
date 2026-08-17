using dotNetAssignment.Constants;
using dotNetAssignment.Controllers;
using dotNetAssignment.Models.DTO;
using dotNetAssignment.Services.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace dotNetAssignment.Tests.Controllers
{
    [TestFixture]
    public class RestaurantControllerTests
    {
        private Mock<IRestaurantService> _restaurantService;
        private RestaurantController _restaurantController;

        private Guid _restaurantId;

        [SetUp]
        public void Setup()
        {
            _restaurantService = new Mock<IRestaurantService>();

            _restaurantController = new RestaurantController(
                _restaurantService.Object);

            _restaurantId = Guid.NewGuid();
        }


        [Test]
        public async Task GetAllRestaurants_WhenServiceReturnsSuccess_ReturnsOk()
        {
            var request = new PaginationRequestDto
            {
                Page = 1,
                PageSize = 10
            };

            var response = new ApiResponseDto<RestaurantListResponseDto>
            {
                Success = true,
                Data = new RestaurantListResponseDto()
            };

            _restaurantService
                .Setup(x => x.GetAllRestaurantsListAsync(
                    request.Page,
                    request.PageSize))
                .ReturnsAsync(response);

            var result = await _restaurantController.GetAllRestaurants(request);

            Assert.That(result, Is.TypeOf<OkNegotiatedContentResult<
                ApiResponseDto<RestaurantListResponseDto>>>());
        }


        [Test]
        public async Task GetAllRestaurants_WhenServiceReturnsFailure_ReturnsNotFound()
        {
            var request = new PaginationRequestDto
            {
                Page = 5,
                PageSize = 10
            };

            var response = new ApiResponseDto<RestaurantListResponseDto>
            {
                Success = false,
                Message = ExceptionMessages.RestaurantDoesntExists
            };

            _restaurantService
                .Setup(x => x.GetAllRestaurantsListAsync(
                    request.Page,
                    request.PageSize))
                .ReturnsAsync(response);

            var result = await _restaurantController.GetAllRestaurants(request);

            var notFoundResult =
                result as NegotiatedContentResult<
                    ApiResponseDto<RestaurantListResponseDto>>;

            Assert.Multiple(() =>
            {
                Assert.That(notFoundResult, Is.Not.Null);
                Assert.That(
                    notFoundResult.StatusCode,
                    Is.EqualTo(HttpStatusCode.NotFound));

                Assert.That(
                    notFoundResult.Content.Success,
                    Is.False);

                Assert.That(
                    notFoundResult.Content.Message,
                    Is.EqualTo(ExceptionMessages.RestaurantDoesntExists));
            });
        }


        [Test]
        public async Task GetMenuItems_WhenServiceReturnsSuccess_ReturnsOk()
        {
            var request = new MenuRequestDto
            {
                RestaurantId = _restaurantId,
                Page = 1,
                PageSize = 10
            };

            var response = new ApiResponseDto<MenuListResponseDto>
            {
                Success = true,
                Data = new MenuListResponseDto()
            };

            _restaurantService
                .Setup(x => x.GetMenuListAsync(
                    request.RestaurantId,
                    request.Page,
                    request.PageSize))
                .ReturnsAsync(response);

            var result = await _restaurantController.GetMenuItems(request);

            Assert.That(result, Is.TypeOf<OkNegotiatedContentResult<
                ApiResponseDto<MenuListResponseDto>>>());

        }


        [Test]
        public async Task GetMenuItems_WhenServiceReturnsFailure_ReturnsNotFound()
        {
            var request = new MenuRequestDto
            {
                RestaurantId = _restaurantId,
                Page = 3,
                PageSize = 10
            };

            var response = new ApiResponseDto<MenuListResponseDto>
            {
                Success = false,
                Message = ExceptionMessages.RestaurantDoesntExists
            };

            _restaurantService
                .Setup(x => x.GetMenuListAsync(
                    request.RestaurantId,
                    request.Page,
                    request.PageSize))
                .ReturnsAsync(response);

            var result = await _restaurantController.GetMenuItems(request);

            var notFoundResult =
                result as NegotiatedContentResult<
                    ApiResponseDto<MenuListResponseDto>>;

            Assert.Multiple(() =>
            {
                Assert.That(notFoundResult, Is.Not.Null);
                Assert.That(
                    notFoundResult.StatusCode,
                    Is.EqualTo(HttpStatusCode.NotFound));

                Assert.That(
                    notFoundResult.Content.Success,
                    Is.False);

                Assert.That(
                    notFoundResult.Content.Message,
                    Is.EqualTo(ExceptionMessages.RestaurantDoesntExists));
            });
        }
    }
}
