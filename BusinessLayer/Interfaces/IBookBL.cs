using RepoLayer.Entity;
using ModelLayer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IBookBL
    {
        Task<List<Book>> GetAllBooksAsync();
        Task<Book> GetBookByIdAsync(int bookId);
        Task<Book> AddBookAsync(BookInputModel book, int  Id);
        Task<Book> UpdateBookAsync(int bookId, BookInputModel book);
        Task<bool> DeleteBookAsync(int bookId);
    }
}