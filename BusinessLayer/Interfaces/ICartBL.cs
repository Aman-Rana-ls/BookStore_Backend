using System.Collections.Generic;
using System.Threading.Tasks;
using ModelLayer;
using RepoLayer.Entity;

namespace BusinessLayer.Interfaces
{
    public interface ICartBL
    {
        Task<Cart> AddToCart(int userId, int bookId);
        Task<IEnumerable<CartResponseModel>> GetUserCart(int userId);
        Task<Cart> UpdateCartItem(int userId, int bookId, int quantity,bool IsPurchased);
        Task<bool> RemoveFromCart(int userId, int bookId);
    }
}