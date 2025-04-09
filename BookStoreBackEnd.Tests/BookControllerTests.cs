using BookStore.Controllers;
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
    public class BookControllerTests
    {
        private Mock<IBookBL> _mockBookBL;
        private Mock<ILogger<BookController>> _mockLogger;
        private BookController _controller;

        [SetUp]
        public void Setup()
        {
            _mockBookBL = new Mock<IBookBL>();
            _mockLogger = new Mock<ILogger<BookController>>();
            _controller = new BookController(_mockBookBL.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetAllBooks_ReturnsOkWithBooks()
        {
            var books = new List<Book> { new Book { BookId = 1, BookName = "Sample Book" } };
            _mockBookBL.Setup(bl => bl.GetAllBooksAsync()).ReturnsAsync(books);

            var actionResult = await _controller.GetAllBooks();

            Assert.That(actionResult.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = actionResult.Result as OkObjectResult;
            var response = okResult.Value as ResponseModel<IEnumerable<Book>>;

            Assert.That(response.Success, Is.True);
            Assert.That(response.Data, Is.EqualTo(books));
        }

        [Test]
        public async Task GetBookById_ReturnsOk_WhenBookExists()
        {
            var book = new Book { BookId = 1, BookName = "Book 1" };
            _mockBookBL.Setup(bl => bl.GetBookByIdAsync(1)).ReturnsAsync(book);

            var actionResult = await _controller.GetBookById(1);

            Assert.That(actionResult.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = actionResult.Result as OkObjectResult;
            var response = okResult.Value as ResponseModel<Book>;

            Assert.That(response.Success, Is.True);
            Assert.That(response.Data, Is.EqualTo(book));
        }

        [Test]
        public async Task GetBookById_ReturnsNotFound_WhenBookDoesNotExist()
        {
            _mockBookBL.Setup(bl => bl.GetBookByIdAsync(1)).ReturnsAsync((Book)null);

            var actionResult = await _controller.GetBookById(1);

            Assert.That(actionResult.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = actionResult.Result as NotFoundObjectResult;
            var response = notFoundResult.Value as ResponseModel<string>;

            Assert.That(response.Success, Is.False);
            Assert.That(response.Message, Is.EqualTo("Book not found"));
        }

        [Test]
        public async Task AddBook_ReturnsCreated_WhenSuccessful()
        {
            var input = new BookInputModel
            {
                BookName = "New Book",
                Author = "John",
                DiscountPrice = 100,
                Price = 200,
                Quantity = 10
            };

            var addedBook = new Book { BookId = 1, BookName = input.BookName };

            _mockBookBL.Setup(bl => bl.AddBookAsync(input, 123)).ReturnsAsync(addedBook);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "123"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var actionResult = await _controller.AddBook(input);

            Assert.That(actionResult, Is.InstanceOf<CreatedAtActionResult>());
            var createdResult = actionResult as CreatedAtActionResult;
            var response = createdResult.Value as ResponseModel<Book>;

            Assert.That(response.Success, Is.True);
            Assert.That(response.Data, Is.EqualTo(addedBook));
        }

        [Test]
        public async Task AddBook_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            var input = new BookInputModel
            {
                BookName = "Book",
                Author = "Author",
                DiscountPrice = 50,
                Price = 100,
                Quantity = 5
            };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _controller.AddBook(input);

            Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
            var unauthorizedResult = result as UnauthorizedObjectResult;
            var response = unauthorizedResult.Value as ResponseModel<string>;

            Assert.That(response.Success, Is.False);
            Assert.That(response.Message, Is.EqualTo("Unauthorized"));
        }

        [Test]
        public async Task DeleteBook_ReturnsOk_WhenSuccessful()
        {
            _mockBookBL.Setup(bl => bl.DeleteBookAsync(1)).ReturnsAsync(true);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "123"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await _controller.DeleteBook(1);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var response = okResult.Value as ResponseModel<string>;

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success, Is.True);
            Assert.That(response.Message, Is.EqualTo("Book deleted successfully"));
        }

        [Test]
        public async Task DeleteBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            _mockBookBL.Setup(bl => bl.DeleteBookAsync(1)).ReturnsAsync(false);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "123"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await _controller.DeleteBook(1);

            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;
            var response = notFoundResult.Value as ResponseModel<string>;

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success, Is.False);
            Assert.That(response.Message, Is.EqualTo("Book not found"));
        }
    }
}
