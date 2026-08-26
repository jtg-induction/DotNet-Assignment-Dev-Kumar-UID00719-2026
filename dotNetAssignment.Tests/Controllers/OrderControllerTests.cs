using dotNetAssignment.Constants;
using dotNetAssignment.Controllers;
using dotNetAssignment.Models.DTO;
using dotNetAssignment.Services.Interfaces;

using Moq;
using NUnit.Framework;

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace dotNetAssignment.Tests.Controllers
{
    [TestFixture]
    public class OrderControllerTests
    {
        private Mock<IOrderService> _orderService;
        private OrderController _orderController;
        private Guid _userId;

        [SetUp]
        public void Setup()
        {
            _orderService = new Mock<IOrderService>();

            _orderController = new OrderController(
                _orderService.Object);

            _userId = Guid.NewGuid();

            var identity = new ClaimsIdentity(
                new[]
                {
                    new Claim(
                        ClaimTypes.NameIdentifier,
                        _userId.ToString())
                });

            _orderController.User =
                new ClaimsPrincipal(identity);
        }

        [Test]
        public async Task PlaceOrder_WhenSuccessful_ReturnsOk()
        {
            var request = new OrderRequestDto();
            var response =
                new ApiResponseDto<PlaceOrderResponseDto>
                {
                    Success = true
                };

            _orderService
                .Setup(x => x.PlaceOrder(request, _userId))
                .ReturnsAsync(response);

            var result = await _orderController.PlaceOrder(request);

            Assert.That(result, Is.TypeOf<OkNegotiatedContentResult<ApiResponseDto<PlaceOrderResponseDto>>>());
            
            _orderService.Verify(x => x.PlaceOrder(request, _userId), Times.Once);
        }


        [Test]
        public async Task PlaceOrder_WhenBalanceIsInsufficient_ReturnsBadRequest()
        {
            var request = new OrderRequestDto();
            var response =
                new ApiResponseDto<PlaceOrderResponseDto>
                {
                    Success = false,
                    Message = ExceptionMessages.InsufficientBalance
                };

            _orderService
                .Setup(x => x.PlaceOrder(request, _userId))
                .ReturnsAsync(response);

            var result = await _orderController.PlaceOrder(request);

            var badRequest =
                result as NegotiatedContentResult<
                    ApiResponseDto<PlaceOrderResponseDto>>;

            Assert.Multiple(() =>
            {
                Assert.That(badRequest, Is.Not.Null);
                Assert.That(
                    badRequest.StatusCode,
                    Is.EqualTo(
                        System.Net.HttpStatusCode.BadRequest));
                Assert.That(
                    badRequest.Content.Message,
                    Is.EqualTo(
                        ExceptionMessages.InsufficientBalance));
            });

            _orderService.Verify(x => x.PlaceOrder(request, _userId), Times.Once);
        }


        [Test]
        public async Task PlaceOrder_WhenStockIsInsufficient_ReturnsBadRequest()
        {
            var request = new OrderRequestDto();
            var response =
                new ApiResponseDto<PlaceOrderResponseDto>
                {
                    Success = false,
                    Message = ExceptionMessages.InsufficientStock
                };

            _orderService
                .Setup(x => x.PlaceOrder(request, _userId))
                .ReturnsAsync(response);

            var result = await _orderController.PlaceOrder(request);

            var badRequest =
                result as NegotiatedContentResult<
                    ApiResponseDto<PlaceOrderResponseDto>>;

            Assert.Multiple(() =>
            {
                Assert.That(badRequest, Is.Not.Null);
                Assert.That(
                    badRequest.StatusCode,
                    Is.EqualTo(
                        System.Net.HttpStatusCode.BadRequest));
                Assert.That(
                    badRequest.Content.Message,
                    Is.EqualTo(
                        ExceptionMessages.InsufficientStock));
            });

            _orderService.Verify(x => x.PlaceOrder(request, _userId), Times.Once);
        }


        [Test]
        public async Task PlaceOrder_WhenOtherFailureOccurs_ReturnsNotFound()
        {
            var request = new OrderRequestDto();
            var response =
                new ApiResponseDto<PlaceOrderResponseDto>
                {
                    Success = false,
                    Message = ExceptionMessages.RestaurantDoesntExists
                };

            _orderService
                .Setup(x => x.PlaceOrder(request, _userId))
                .ReturnsAsync(response);

            var result = await _orderController.PlaceOrder(request);

            var notFound = result as NegotiatedContentResult<
                ApiResponseDto<PlaceOrderResponseDto>>;

            Assert.Multiple(() =>
            {
                Assert.That(notFound, Is.Not.Null);
                Assert.That(
                    notFound.StatusCode,
                    Is.EqualTo(
                        System.Net.HttpStatusCode.NotFound));
                Assert.That(
                    notFound.Content.Message,
                    Is.EqualTo(
                        ExceptionMessages.RestaurantDoesntExists));
            });

            _orderService.Verify(x => x.PlaceOrder(request, _userId), Times.Once);
        }


        [Test]
        public async Task GetOrderDetails_WhenSuccessful_ReturnsOk()
        {
            var orderId = Guid.NewGuid();
            var response =
                new ApiResponseDto<OrderDetailsResponseDto>
                {
                    Success = true
                };

            _orderService
                .Setup(x => x.OrderDetails(orderId, _userId))
                .ReturnsAsync(response);

            var result = await _orderController.GetOrderDetails(orderId);

            Assert.That(result, Is.TypeOf<OkNegotiatedContentResult<ApiResponseDto<OrderDetailsResponseDto>>>());

            _orderService.Verify(
                x => x.OrderDetails(orderId, _userId),
                Times.Once);
        }


        [Test]
        public async Task GetOrderDetails_WhenOrderDoesNotExist_ReturnsNotFound()
        {
            var orderId = Guid.NewGuid();
            var response =
                new ApiResponseDto<OrderDetailsResponseDto>
                {
                    Success = false,
                    Message = ExceptionMessages.OrderDoesntExists
                };

            _orderService
                .Setup(x => x.OrderDetails(orderId, _userId))
                .ReturnsAsync(response);

            var result = await _orderController.GetOrderDetails(orderId);
            var notFound = result as NegotiatedContentResult<ApiResponseDto<OrderDetailsResponseDto>>;

            Assert.Multiple(() =>
            {
                Assert.That(notFound, Is.Not.Null);
                Assert.That(notFound.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
                Assert.That(notFound.Content.Message, Is.EqualTo(ExceptionMessages.OrderDoesntExists));
            });

            _orderService.Verify(
                x => x.OrderDetails(orderId, _userId),
                Times.Once);
        }


        [Test]
        public async Task CancelOrder_WhenSuccessful_ReturnsOk()
        {
            var orderId = Guid.NewGuid();

            var response =
                new ApiResponseDto<string>
                {
                    Success = true
                };

            _orderService
                .Setup(x => x.CancelOrder(orderId, _userId))
                .ReturnsAsync(response);

            var result = await _orderController.CancelOrder(orderId);

            Assert.That(result, Is.TypeOf<OkNegotiatedContentResult<ApiResponseDto<string>>>());

            _orderService.Verify(
                x => x.CancelOrder(orderId, _userId),
                Times.Once);
        }


        [Test]
        public async Task CancelOrder_WhenOrderCannotBeCancelled_ReturnsBadRequest()
        {
            var orderId = Guid.NewGuid();

            var response =
                new ApiResponseDto<string>
                {
                    Success = false,
                    Message = ExceptionMessages.OrderCannotBeCancelled
                };

            _orderService
                .Setup(x => x.CancelOrder(orderId, _userId))
                .ReturnsAsync(response);

            var result = await _orderController.CancelOrder(orderId);

            var badRequest = result as NegotiatedContentResult<ApiResponseDto<string>>;

            Assert.Multiple(() =>
            {
                Assert.That(badRequest, Is.Not.Null);
                Assert.That(badRequest.StatusCode, Is.EqualTo(
                        System.Net.HttpStatusCode.BadRequest));
                Assert.That(badRequest.Content.Message, Is.EqualTo(
                    ExceptionMessages.OrderCannotBeCancelled));
            });

            _orderService.Verify(
                x => x.CancelOrder(orderId, _userId),
                Times.Once);
        }


        [Test]
        public async Task CancelOrder_WhenOtherFailureOccurs_ReturnsNotFound()
        {
            var orderId = Guid.NewGuid();

            var response =
                new ApiResponseDto<string>
                {
                    Success = false,
                    Message = ExceptionMessages.OrderDoesntExists
                };

            _orderService
                .Setup(x => x.CancelOrder(orderId, _userId))
                .ReturnsAsync(response);

            var result =await _orderController.CancelOrder(orderId);

            var notFound = result as NegotiatedContentResult<ApiResponseDto<string>>;

            Assert.Multiple(() =>
            {
                Assert.That(notFound, Is.Not.Null);
                Assert.That(notFound.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
                Assert.That(notFound.Content.Message, Is.EqualTo(ExceptionMessages.OrderDoesntExists));
            });

            _orderService.Verify(
                x => x.CancelOrder(orderId, _userId),
                Times.Once);
        }
    }
}
