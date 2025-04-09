
    using RepoLayer.Entity;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ModelLayer;
    using RepoLayer.Entity;

    namespace RepoLayer.Interfaces
    {
        public interface IBookRL
        {
            Task<List<Book>> GetAllBooksAsync();
            Task<Book> GetBookByIdAsync(int bookId);
            Task<Book> AddBookAsync(BookInputModel book, int Id);
            Task<Book> UpdateBookAsync(int bookId, BookInputModel book);
            Task<bool> DeleteBookAsync(int bookId);
        }
    }