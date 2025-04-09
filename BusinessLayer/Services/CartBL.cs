using BusinessLayer.Interfaces;
using Microsoft.Extensions.Logging;
using ModelLayer;
using RepoLayer.Entity;
using RepoLayer.Interfaces;

namespace BusinessLayer.Services
{
    public class CartBL : ICartBL
    {
        private readonly ICartRL _cartRL;
        private readonly ILogger<CartBL> _logger;

        public CartBL(ICartRL cartRL, ILogger<CartBL> logger)
        {
            _cartRL = cartRL;
            _logger = logger;
        }

        public async Task<Cart> AddToCart(int userId, int bookId)
        {
            _logger.LogInformation("Adding book {BookId} to cart for user {UserId}", bookId, userId);
            var result = await _cartRL.AddToCart(userId, bookId);
            _logger.LogInformation("Book {BookId} added to cart for user {UserId}", bookId, userId);
            return result;
        }

        public async Task<IEnumerable<CartResponseModel>> GetUserCart(int userId)
        {
            _logger.LogInformation("Fetching cart items for user {UserId}", userId);
            var cartItems = await _cartRL.GetUserCart(userId);
            _logger.LogInformation("Fetched {Count} cart items for user {UserId}", cartItems?.Count() ?? 0, userId);
            return cartItems;
        }

        public async Task<Cart> UpdateCartItem(int userId, int bookId, int quantity, bool IsPurchased)
        {
            _logger.LogInformation("Updating cart item {BookId} for user {UserId} with quantity {Quantity} and purchased status {IsPurchased}", bookId, userId, quantity, IsPurchased);
            var cartItem = await _cartRL.GetCartItem(userId, bookId);
            if (cartItem == null)
            {
                _logger.LogWarning("Cart item {BookId} not found for user {UserId}", bookId, userId);
                return null;
            }

            var updatedCart = await _cartRL.UpdateCart(cartItem.CartId, quantity, IsPurchased);
            _logger.LogInformation("Cart item {BookId} updated successfully for user {UserId}", bookId, userId);
            return updatedCart;
        }

        public async Task<bool> RemoveFromCart(int userId, int bookId)
        {
            _logger.LogInformation("Removing book {BookId} from cart for user {UserId}", bookId, userId);
            var cartItem = await _cartRL.GetCartItem(userId, bookId);
            if (cartItem == null)
            {
                _logger.LogWarning("Cart item {BookId} not found for user {UserId}", bookId, userId);
                return false;
            }

            var result = await _cartRL.RemoveFromCart(cartItem.CartId);
            _logger.LogInformation("Book {BookId} removed from cart for user {UserId}", bookId, userId);
            return result;
        }
    }
}
