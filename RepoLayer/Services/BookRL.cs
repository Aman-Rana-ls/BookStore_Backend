using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModelLayer;
using RepoLayer.Context;
using RepoLayer.Interfaces;
using RepoLayer.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoLayer.Services
{
    public class BookRL : IBookRL
    {
        private readonly AppDbContext _context;
        private readonly ILogger<BookRL> _logger;

        public BookRL(AppDbContext context, ILogger<BookRL> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            _logger.LogInformation("Fetching all books");
            return await _context.Books
                .Include(b => b.AdminUser)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Book> GetBookByIdAsync(int bookId)
        {
            _logger.LogInformation("Fetching book with ID {BookId}", bookId);
            return await _context.Books
                .Include(b => b.AdminUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BookId == bookId);
        }

        public async Task<Book> AddBookAsync(BookInputModel book, int Id)
        {
            _logger.LogInformation("Adding new book: {BookName} by {Author}", book.BookName, book.Author);
            var newBook = new Book
            {
                BookName = book.BookName,
                Author = book.Author,
                Description = book.Description,
                DiscountPrice = book.DiscountPrice,
                Price = book.Price,
                Quantity = book.Quantity,
                BookImage = book.BookImage,
                AdminUserId = Id,
                CreatedAt = System.DateTime.UtcNow
            };

            await _context.Books.AddAsync(newBook);
            await _context.SaveChangesAsync();
            return newBook;
        }

        public async Task<Book> UpdateBookAsync(int bookId, BookInputModel book)
        {
            _logger.LogInformation("Updating book with ID {BookId}", bookId);
            var existingBook = await _context.Books.FindAsync(bookId);
            if (existingBook == null) return null;

            existingBook.BookName = book.BookName;
            existingBook.Author = book.Author;
            existingBook.Description = book.Description;
            existingBook.DiscountPrice = book.DiscountPrice;
            existingBook.Price = book.Price;
            existingBook.Quantity = book.Quantity;
            existingBook.BookImage = book.BookImage;
            existingBook.UpdatedAt = System.DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingBook;
        }

        public async Task<bool> DeleteBookAsync(int bookId)
        {
            _logger.LogInformation("Deleting book with ID {BookId}", bookId);
            var book = await _context.Books.FindAsync(bookId);
            if (book == null) return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}