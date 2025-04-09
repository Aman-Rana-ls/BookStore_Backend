using System.Collections.Generic;
using System.Threading.Tasks;
using ModelLayer;
using RepoLayer.Entity;

namespace RepoLayer.Interfaces
{
    public interface IWishRL
    {
        Task<List<Wishlist>> GetWishlistItems(int userId);
        Task<bool> AddToWishlist(int userId, int bookId);
        Task<bool> RemoveFromWishlist(int userId, int bookId);
        Task<bool> MoveToCart(int userId, int bookId);
        Task<bool> BookExists(int bookId);
        Task<bool> IsInWishlist(int userId, int bookId);
    }
}