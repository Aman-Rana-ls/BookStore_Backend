using System.Collections.Generic;
using System.Threading.Tasks;
using ModelLayer;
using RepoLayer.Entity;

namespace RepoLayer.Interfaces
{
    public interface ICartRL
    {
        Task<Cart> AddToCart(int userId, int bookId);
        Task<IEnumerable<CartResponseModel>> GetUserCart(int userId);
        Task<Cart> GetCartItem(int userId, int bookId);
        Task<Cart> UpdateCart(int cartId, int quantity,bool IsPurchased);
        Task<bool> RemoveFromCart(int cartId);
    }
}