using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using BusinessLayer.Interfaces;
using ModelLayer;
using RepoLayer.Interfaces;
using RepoLayer.Entity;

namespace BusinessLayer.Services
{
    public class WishBL : IWishBL
    {
        private readonly IWishRL _wishRL;
        private readonly ILogger<WishBL> _logger;

        public WishBL(IWishRL wishRL, ILogger<WishBL> logger)
        {
            _wishRL = wishRL;
            _logger = logger;
        }

        public async Task<List<WishListResponceModel>> GetWishlistItems(int userId)
        {
            _logger.LogInformation($"Fetching wishlist items for user {userId}");
            var items = await _wishRL.GetWishlistItems(userId);
            return items.Select(w => new WishListResponceModel
            {
                Title = w.Book.BookName,
                AuthorName = w.Book.Author,
                Image = w.Book.BookImage,
                Price = w.Book.Price,
                DiscountPrice = w.Book.DiscountPrice
            }).ToList();
        }

        public Task<bool> AddToWishlist(int userId, int bookId)
        {
            _logger.LogInformation($"Adding book {bookId} to wishlist for user {userId}");
            return _wishRL.AddToWishlist(userId, bookId);
        }

        public Task<bool> RemoveFromWishlist(int userId, int bookId)
        {
            _logger.LogInformation($"Removing book {bookId} from wishlist for user {userId}");
            return _wishRL.RemoveFromWishlist(userId, bookId);
        }

        public Task<bool> MoveToCart(int userId, int bookId)
        {
            _logger.LogInformation($"Moving book {bookId} from wishlist to cart for user {userId}");
            return _wishRL.MoveToCart(userId, bookId);
        }

        public Task<bool> BookExists(int bookId)
        {
            _logger.LogInformation($"Checking if book {bookId} exists");
            return _wishRL.BookExists(bookId);
        }

        public Task<bool> IsInWishlist(int userId, int bookId)
        {
            _logger.LogInformation($"Checking if book {bookId} is in wishlist for user {userId}");
            return _wishRL.IsInWishlist(userId, bookId);
        }
    }
}
