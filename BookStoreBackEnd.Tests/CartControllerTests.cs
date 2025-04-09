using BookStoreBackEnd.Controllers;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ModelLayer;
using NUnit.Framework;
using RepoLayer.Entity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStoreBackEnd.Tests
{
    [TestFixture]
    public class CartControllerTests
    {
        private Mock<ICartBL> _mockCartBL;
        private Mock<ILogger<CartController>> _mockLogger;
        private CartController _controller;
        private int _userId = 1;

        [SetUp]
        public void Setup()
        {
            _mockCartBL = new Mock<ICartBL>();
            _mockLogger = new Mock<ILogger<CartController>>();
            _controller = new CartController(_mockCartBL.Object, _mockLogger.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _userId.ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task AddToCart_ReturnsOkResult_WhenSuccessful()
        {

            int bookId = 10;
            var cart = new Cart { BookId = bookId, UserId = _userId };

            _mockCartBL.Setup(bl => bl.AddToCart(_userId, bookId)).ReturnsAsync(cart);


            var result = await _controller.AddToCart(bookId) as OkObjectResult;

 
            Assert.That(result, Is.Not.Null);
            var response = result.Value as ResponseModel<Cart>;
            Assert.That(response.Success, Is.True);
            Assert.That(response.Message, Is.EqualTo("Book added to cart"));
            Assert.That(response.Data.BookId, Is.EqualTo(bookId));
        }

        [Test]
        public async Task GetUserCart_ReturnsOkResult()
        {

            var cartItems = new List<CartResponseModel>
            {
                new CartResponseModel
                {
                    Title = "Sample Book",
                    AuthorName = "Author",
                    Image = "img.jpg",
                    Quantity = 2,
                    Price = 100,
                    DiscountPrice = 80,
                    TotalPrice = 160
                }
            };

            _mockCartBL.Setup(bl => bl.GetUserCart(_userId)).ReturnsAsync(cartItems);


            var result = await _controller.GetUserCart() as OkObjectResult;


            Assert.That(result, Is.Not.Null);
            var response = result.Value as ResponseModel<IEnumerable<CartResponseModel>>;
            Assert.That(response.Success, Is.True);
            Assert.That(response.Message, Is.EqualTo("Cart retrieved"));
            Assert.That(response.Data, Is.Not.Empty);
        }

        [Test]
        public async Task UpdateCartItem_ReturnsOk_WhenSuccessful()
        {

            var model = new UpdateCartItemModel
            {
                BookId = 101,
                Quantity = 2,
                IsPurchased = false
            };

            var cart = new Cart
            {
                BookId = model.BookId,
                UserId = _userId,
                Quantity = model.Quantity,
                IsPurchased = model.IsPurchased
            };

            _mockCartBL.Setup(bl => bl.UpdateCartItem(_userId, model.BookId, model.Quantity, model.IsPurchased)).ReturnsAsync(cart);


            var result = await _controller.UpdateCartItem(model) as OkObjectResult;


            Assert.That(result, Is.Not.Null);
            var response = result.Value as ResponseModel<Cart>;
            Assert.That(response.Success, Is.True);
            Assert.That(response.Message, Is.EqualTo("Cart updated"));
            Assert.That(response.Data.BookId, Is.EqualTo(model.BookId));
        }

        [Test]
        public async Task RemoveFromCart_ReturnsOk_WhenSuccessful()
        {

            int bookId = 101;
            _mockCartBL.Setup(bl => bl.RemoveFromCart(_userId, bookId)).ReturnsAsync(true);


            var result = await _controller.RemoveFromCart(bookId) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var response = result.Value as ResponseModel<string>;
            Assert.That(response.Success, Is.True);
            Assert.That(response.Message, Is.EqualTo("Item removed from cart"));
        }

        [Test]
        public async Task RemoveFromCart_ReturnsNotFound_WhenItemDoesNotExist()
        {

            int bookId = 999;
            _mockCartBL.Setup(bl => bl.RemoveFromCart(_userId, bookId)).ReturnsAsync(false);

            var result = await _controller.RemoveFromCart(bookId) as NotFoundObjectResult;

            Assert.That(result, Is.Not.Null);
            var response = result.Value as ResponseModel<string>;
            Assert.That(response.Success, Is.False);
            Assert.That(response.Message, Is.EqualTo("Cart item not found"));
        }
    }
}
