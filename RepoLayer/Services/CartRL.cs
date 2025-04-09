using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModelLayer;
using RepoLayer.Context;
using RepoLayer.Entity;
using RepoLayer.Interfaces;

namespace RepoLayer.Services
{
    public class CartRL : ICartRL
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CartRL> _logger;

        public CartRL(AppDbContext context, ILogger<CartRL> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Cart> AddToCart(int userId, int bookId)
        {
            try
            {
                _logger.LogInformation("Adding bookId {BookId} to cart for userId {UserId}", bookId, userId);

                var existingItem = await _context.Carts
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId);

                if (existingItem != null)
                {
                    if (existingItem.IsPurchased)
                    {
                        throw new Exception("Cannot modify purchased items in cart");
                    }

                    existingItem.Quantity++;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Increased quantity for existing cart item");
                    return existingItem;
                }

                var newCartItem = new Cart
                {
                    UserId = userId,
                    BookId = bookId,
                    Quantity = 1,
                    IsPurchased = false
                };

                _context.Carts.Add(newCartItem);
                await _context.SaveChangesAsync();
                _logger.LogInformation("New item added to cart");
                return newCartItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart for userId {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<CartResponseModel>> GetUserCart(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching cart for userId {UserId}", userId);

                return await _context.Carts
                    .Where(c => c.UserId == userId && !c.IsPurchased)
                    .Include(c => c.Book)
                    .Select(c => new CartResponseModel
                    {
                        Title = c.Book.BookName,
                        AuthorName = c.Book.Author,
                        Image = c.Book.BookImage,
                        Quantity = c.Quantity,
                        Price = c.Book.Price,
                        DiscountPrice = c.Book.DiscountPrice,
                        TotalPrice = c.Quantity * c.Book.DiscountPrice,
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching cart for userId {UserId}", userId);
                throw;
            }
        }

        public async Task<Cart> GetCartItem(int userId, int bookId)
        {
            try
            {
                _logger.LogInformation("Fetching cart item for userId {UserId}, bookId {BookId}", userId, bookId);
                return await _context.Carts
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching cart item for userId {UserId}, bookId {BookId}", userId, bookId);
                throw;
            }
        }

        public async Task<Cart> UpdateCart(int cartId, int quantity, bool isPurchased)
        {
            try
            {
                _logger.LogInformation("Updating cart item {CartId} with quantity {Quantity} and isPurchased={IsPurchased}", cartId, quantity, isPurchased);

                var cartItem = await _context.Carts.FindAsync(cartId);
                if (cartItem == null)
                {
                    _logger.LogWarning("Cart item {CartId} not found", cartId);
                    return null;
                }

                if (quantity <= 0)
                {
                    _context.Carts.Remove(cartItem);
                    _logger.LogInformation("Removed cart item {CartId} due to zero quantity", cartId);
                }
                else
                {
                    cartItem.Quantity = quantity;
                    cartItem.IsPurchased = isPurchased;
                    _logger.LogInformation("Updated cart item {CartId}", cartId);
                }

                await _context.SaveChangesAsync();
                return cartItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item {CartId}", cartId);
                throw;
            }
        }

        public async Task<bool> RemoveFromCart(int cartId)
        {
            try
            {
                _logger.LogInformation("Attempting to remove cart item {CartId}", cartId);

                var cartItem = await _context.Carts.FindAsync(cartId);
                if (cartItem == null)
                {
                    _logger.LogWarning("Cart item {CartId} not found", cartId);
                    return false;
                }

                if (cartItem.IsPurchased)
                {
                    throw new Exception("Cannot remove purchased items from cart");
                }

                _context.Carts.Remove(cartItem);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully removed cart item {CartId}", cartId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cart item {CartId}", cartId);
                throw;
            }
        }
    }
}
