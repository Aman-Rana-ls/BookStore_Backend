using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Interfaces;
using ModelLayer;
using RepoLayer.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace BookStore.Controllers
{
    /// <summary>
    /// Controller for managing books in the bookstore.
    /// </summary>
    [Route("book")]
    [ApiController]
    [Authorize]
    public class BookController : ControllerBase
    {
        private readonly IBookBL _bookBL;
        private readonly ILogger<BookController> _logger;

        public BookController(IBookBL bookBL, ILogger<BookController> logger)
        {
            _bookBL = bookBL;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all books.
        /// </summary>
        /// <returns>A list of all books.</returns>
        [HttpGet]
        public async Task<ActionResult<ResponseModel<IEnumerable<Book>>>> GetAllBooks()
        {
            try
            {
                _logger.LogInformation("Fetching all books");
                var books = await _bookBL.GetAllBooksAsync();

                return Ok(new ResponseModel<IEnumerable<Book>>
                {
                    Success = true,
                    Message = "Books retrieved successfully",
                    Data = books
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all books");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = "An error occurred while fetching books",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Retrieves a specific book by ID.
        /// </summary>
        /// <param name="id">Book ID</param>
        /// <returns>The book with the given ID if found.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseModel<Book>>> GetBookById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching book with ID: {BookId}", id);
                var book = await _bookBL.GetBookByIdAsync(id);

                if (book != null)
                {
                    _logger.LogInformation("Book found with ID: {BookId}", id);
                    return Ok(new ResponseModel<Book>
                    {
                        Success = true,
                        Message = "Book retrieved successfully",
                        Data = book
                    });
                }

                _logger.LogWarning("Book not found with ID: {BookId}", id);
                return NotFound(new ResponseModel<string>
                {
                    Success = false,
                    Message = "Book not found",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching book with ID: {BookId}", id);
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = "An error occurred while fetching the book",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Adds a new book. Only accessible by Admin.
        /// </summary>
        /// <param name="book">The book data to be added</param>
        /// <returns>The added book</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddBook([FromBody] BookInputModel book)
        {
            try
            {
                _logger.LogInformation("Attempting to add new book");

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    _logger.LogWarning("Unauthorized attempt to add book");
                    return Unauthorized(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Unauthorized",
                        Data = null
                    });
                }

                var newBook = await _bookBL.AddBookAsync(book, userId);
                _logger.LogInformation("Book added successfully with ID: {BookId}", newBook.BookId);

                return CreatedAtAction(
                    nameof(GetBookById),
                    new { id = newBook.BookId },
                    new ResponseModel<Book>
                    {
                        Success = true,
                        Message = "Book added successfully",
                        Data = newBook
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new book");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = "An error occurred while adding the book",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Deletes a book by ID. Only accessible by Admin.
        /// </summary>
        /// <param name="id">ID of the book to delete</param>
        /// <returns>Status of the delete operation</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseModel<string>>> DeleteBook(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete book with ID: {BookId}", id);
                bool isDeleted = await _bookBL.DeleteBookAsync(id);

                if (isDeleted)
                {
                    _logger.LogInformation("Book deleted successfully with ID: {BookId}", id);
                    return Ok(new ResponseModel<string>
                    {
                        Success = true,
                        Message = "Book deleted successfully",
                        Data = null
                    });
                }

                _logger.LogWarning("Book not found for deletion with ID: {BookId}", id);
                return NotFound(new ResponseModel<string>
                {
                    Success = false,
                    Message = "Book not found",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting book with ID: {BookId}", id);
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = "An error occurred while deleting the book",
                    Data = null
                });
            }
        }
    }
}
