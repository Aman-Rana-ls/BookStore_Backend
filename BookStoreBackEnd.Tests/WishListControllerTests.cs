using BookStoreBackEnd.Controllers;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelLayer;
using Moq;
using NUnit.Framework;
using RepoLayer.Entity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStoreBackEnd.Tests
{
    [TestFixture]
    public class WishListControllerTests
    {
        private Mock<IWishBL> _mockWishBL;
        private Mock<ILogger<WishListController>> _mockLogger;
        private WishListController _controller;

        [SetUp]
        public void Setup()
        {
            _mockWishBL = new Mock<IWishBL>();
            _mockLogger = new Mock<ILogger<WishListController>>();
            _controller = new WishListController(_mockWishBL.Object, _mockLogger.Object);
        }

        private void SetUserContext(int userId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task GetWishlistItems_ReturnsOk_WithItems()
        {
            SetUserContext(1);
            var mockItems = new List<WishListResponceModel>
            {
                new WishListResponceModel { Title = "Book1", AuthorName = "Author1" }
            };
            _mockWishBL.Setup(bl => bl.GetWishlistItems(1)).ReturnsAsync(mockItems);

            var result = await _controller.GetWishlistItems();
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var okResult = result as OkObjectResult;
            var response = okResult.Value as ResponseModel<List<WishListResponceModel>>;
            Assert.That(response.Success, Is.True);
            Assert.That(response.Data, Is.EqualTo(mockItems));
        }

        [Test]
        public async Task AddToWishlist_ReturnsOk_WhenBookAdded()
        {
            SetUserContext(1);
            _mockWishBL.Setup(bl => bl.BookExists(10)).ReturnsAsync(true);
            _mockWishBL.Setup(bl => bl.IsInWishlist(1, 10)).ReturnsAsync(false);
            _mockWishBL.Setup(bl => bl.AddToWishlist(1, 10)).ReturnsAsync(true);

            var result = await _controller.AddToWishlist(10);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var response = (result as OkObjectResult).Value as ResponseModel<string>;
            Assert.That(response.Success, Is.True);
            Assert.That(response.Message, Is.EqualTo("Book added to wishlist"));
        }

        [Test]
        public async Task AddToWishlist_ReturnsNotFound_WhenBookNotExists()
        {
            SetUserContext(1);
            _mockWishBL.Setup(bl => bl.BookExists(99)).ReturnsAsync(false);

            var result = await _controller.AddToWishlist(99);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var response = (result as NotFoundObjectResult).Value as ResponseModel<string>;
            Assert.That(response.Success, Is.False);
            Assert.That(response.Message, Is.EqualTo("Book not found"));
        }

        [Test]
        public async Task AddToWishlist_ReturnsBadRequest_WhenBookAlreadyInWishlist()
        {
            SetUserContext(1);
            _mockWishBL.Setup(bl => bl.BookExists(10)).ReturnsAsync(true);
            _mockWishBL.Setup(bl => bl.IsInWishlist(1, 10)).ReturnsAsync(true);

            var result = await _controller.AddToWishlist(10);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var response = (result as BadRequestObjectResult).Value as ResponseModel<string>;
            Assert.That(response.Success, Is.False);
            Assert.That(response.Message, Is.EqualTo("Book already in wishlist"));
        }

        [Test]
        public async Task RemoveFromWishlist_ReturnsOk_WhenBookRemoved()
        {
            SetUserContext(1);
            _mockWishBL.Setup(bl => bl.RemoveFromWishlist(1, 5)).ReturnsAsync(true);

            var result = await _controller.RemoveFromWishlist(5);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var response = (result as OkObjectResult).Value as ResponseModel<string>;
            Assert.That(response.Success, Is.True);
            Assert.That(response.Message, Is.EqualTo("Book removed from wishlist"));
        }

        [Test]
        public async Task RemoveFromWishlist_ReturnsNotFound_WhenBookNotInWishlist()
        {
            SetUserContext(1);
            _mockWishBL.Setup(bl => bl.RemoveFromWishlist(1, 99)).ReturnsAsync(false);

            var result = await _controller.RemoveFromWishlist(99);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var response = (result as NotFoundObjectResult).Value as ResponseModel<string>;
            Assert.That(response.Success, Is.False);
            Assert.That(response.Message, Is.EqualTo("Book not found in wishlist"));
        }

        [Test]
        public async Task MoveToCart_ReturnsOk_WhenBookMoved()
        {
            SetUserContext(1);
            _mockWishBL.Setup(bl => bl.IsInWishlist(1, 3)).ReturnsAsync(true);
            _mockWishBL.Setup(bl => bl.MoveToCart(1, 3)).ReturnsAsync(true);

            var result = await _controller.MoveToCart(3);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var response = (result as OkObjectResult).Value as ResponseModel<string>;
            Assert.That(response.Success, Is.True);
            Assert.That(response.Message, Is.EqualTo("Book moved to cart"));
        }

        [Test]
        public async Task MoveToCart_ReturnsNotFound_WhenBookNotInWishlist()
        {
            SetUserContext(1);
            _mockWishBL.Setup(bl => bl.IsInWishlist(1, 3)).ReturnsAsync(false);

            var result = await _controller.MoveToCart(3);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var response = (result as NotFoundObjectResult).Value as ResponseModel<string>;
            Assert.That(response.Success, Is.False);
            Assert.That(response.Message, Is.EqualTo("Book not found in wishlist"));
        }
    }
}
