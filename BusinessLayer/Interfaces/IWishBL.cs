using System.Collections.Generic;
using System.Threading.Tasks;
using ModelLayer;

namespace BusinessLayer.Interfaces
{
    public interface IWishBL
    {
        Task<List<WishListResponceModel>> GetWishlistItems(int userId);
        Task<bool> AddToWishlist(int userId, int bookId);
        Task<bool> RemoveFromWishlist(int userId, int bookId);
        Task<bool> MoveToCart(int userId, int bookId);
        Task<bool> BookExists(int bookId);
        Task<bool> IsInWishlist(int userId, int bookId);
    }
}