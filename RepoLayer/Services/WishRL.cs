using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepoLayer.Context;
using RepoLayer.Entity;
using RepoLayer.Interfaces;

namespace RepoLayer.Services
{
    public class WishRL : IWishRL
    {
        private readonly AppDbContext _context;
        private readonly ILogger<WishRL> _logger;

        public WishRL(AppDbContext context, ILogger<WishRL> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task<bool> BookExists(int bookId)
        {
            _logger.LogInformation("Checking if book with ID {BookId} exists", bookId);
            return _context.Books.AnyAsync(b => b.BookId == bookId);
        }

        public Task<bool> IsInWishlist(int userId, int bookId)
        {
            _logger.LogInformation("Checking if book {BookId} is already in wishlist for user {UserId}", bookId, userId);
            return _context.Wishlists.AnyAsync(w => w.UserId == userId && w.BookId == bookId);
        }

        public async Task<bool> AddToWishlist(int userId, int bookId)
        {
            try
            {
                _logger.LogInformation("Adding book {BookId} to wishlist for user {UserId}", bookId, userId);
                await _context.Wishlists.AddAsync(new Wishlist { UserId = userId, BookId = bookId });
                var result = await _context.SaveChangesAsync() > 0;
                _logger.LogInformation("Add to wishlist result: {Result}", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding to wishlist for user {UserId}, book {BookId}", userId, bookId);
                throw;
            }
        }

        public async Task<List<Wishlist>> GetWishlistItems(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching wishlist items for user {UserId}", userId);
                var items = await _context.Wishlists
                    .Where(w => w.UserId == userId)
                    .Include(w => w.Book)
                    .ToListAsync();
                _logger.LogInformation("Wishlist item count: {Count}", items.Count);
                return items;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching wishlist for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> RemoveFromWishlist(int userId, int bookId)
        {
            try
            {
                _logger.LogInformation("Attempting to remove book {BookId} from wishlist for user {UserId}", bookId, userId);
                var item = await _context.Wishlists.FirstOrDefaultAsync(w => w.UserId == userId && w.BookId == bookId);
                if (item == null)
                {
                    _logger.LogWarning("Wishlist item not found for user {UserId}, book {BookId}", userId, bookId);
                    return false;
                }

                _context.Wishlists.Remove(item);
                var result = await _context.SaveChangesAsync() > 0;
                _logger.LogInformation("Remove from wishlist result: {Result}", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing book {BookId} from wishlist for user {UserId}", bookId, userId);
                throw;
            }
        }

        public async Task<bool> MoveToCart(int userId, int bookId)
        {
            _logger.LogInformation("Moving book {BookId} from wishlist to cart for user {UserId}", bookId, userId);
            return await RemoveFromWishlist(userId, bookId);
        }
    }
}
