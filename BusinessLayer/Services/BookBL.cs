using ModelLayer;
using RepoLayer.Entity;
using RepoLayer.Interfaces;
using BusinessLayer.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class BookBL : IBookBL
    {
        private readonly IBookRL _bookRL;
        private readonly ILogger<BookBL> _logger;

        public BookBL(IBookRL bookRL, ILogger<BookBL> logger)
        {
            _bookRL = bookRL;
            _logger = logger;
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all books.");
                var books = await _bookRL.GetAllBooksAsync();
                _logger.LogInformation("Successfully fetched {Count} books.", books.Count);
                return books;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error occurred while fetching books.");
                throw;
            }
        }

        public async Task<Book> GetBookByIdAsync(int bookId)
        {
            try
            {
                _logger.LogInformation("Fetching book with ID: {BookId}", bookId);
                var book = await _bookRL.GetBookByIdAsync(bookId);
                if (book == null)
                {
                    _logger.LogWarning("Book with ID: {BookId} not found.", bookId);
                }
                return book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching book with ID: {BookId}", bookId);
                throw;
            }
        }

        public async Task<Book> AddBookAsync(BookInputModel book, int Id)
        {
            try
            {
                _logger.LogInformation("Adding a new book with Title: {Title}, Author: {Author}", book.BookName, book.Author);
                var addedBook = await _bookRL.AddBookAsync(book, Id);
                _logger.LogInformation("Successfully added book with ID: {BookId}", addedBook.BookId);
                return addedBook;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new book.");
                throw;
            }
        }

        public async Task<Book> UpdateBookAsync(int bookId, BookInputModel book)
        {
            try
            {
                _logger.LogInformation("Updating book with ID: {BookId}", bookId);
                var updatedBook = await _bookRL.UpdateBookAsync(bookId, book);
                if (updatedBook == null)
                {
                    _logger.LogWarning("Failed to update book with ID: {BookId}. It may not exist.", bookId);
                }
                else
                {
                    _logger.LogInformation("Successfully updated book with ID: {BookId}", bookId);
                }
                return updatedBook;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating book with ID: {BookId}", bookId);
                throw;
            }
        }

        public async Task<bool> DeleteBookAsync(int bookId)
        {
            try
            {
                _logger.LogInformation("Deleting book with ID: {BookId}", bookId);
                var result = await _bookRL.DeleteBookAsync(bookId);
                if (result)
                {
                    _logger.LogInformation("Successfully deleted book with ID: {BookId}", bookId);
                }
                else
                {
                    _logger.LogWarning("Failed to delete book with ID: {BookId}. It may not exist.", bookId);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting book with ID: {BookId}", bookId);
                throw;
            }
        }
    }
}
