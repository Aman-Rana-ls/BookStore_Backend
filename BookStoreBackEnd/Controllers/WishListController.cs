using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Security.Claims;
using BusinessLayer.Interfaces;
using ModelLayer;
using System;
using System.Collections.Generic;
using RepoLayer.Entity;

namespace BookStoreBackEnd.Controllers
{
    [Route("wishlist")]
    [ApiController]
    public class WishListController : ControllerBase
    {
        private readonly IWishBL _wishBL;
        private readonly ILogger<WishListController> _logger;

        public WishListController(IWishBL wishBL, ILogger<WishListController> logger)
        {
            _wishBL = wishBL;
            _logger = logger;
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                _logger.LogWarning("Invalid user ID in token");
                throw new UnauthorizedAccessException("Invalid user ID in token");
            }
            return userId;
        }

        /// <summary>
        /// Retrieves the wishlist items for the logged-in user.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetWishlistItems()
        {
            try
            {
                int userId = GetUserIdFromToken();
                var items = await _wishBL.GetWishlistItems(userId);
                return Ok(new ResponseModel<List<WishListResponceModel>>
                {
                    Success = true,
                    Message = "Wishlist retrieved",
                    Data = items
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving wishlist");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Adds a book to the wishlist for the logged-in user.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddToWishlist([FromQuery] int bookId)
        {
            try
            {
                int userId = GetUserIdFromToken();
                if (!await _wishBL.BookExists(bookId))
                {
                    _logger.LogWarning("Book not found, ID: {BookId}", bookId);
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Book not found",
                        Data = null
                    });
                }

                if (await _wishBL.IsInWishlist(userId, bookId))
                {
                    _logger.LogWarning("Book already in wishlist, User ID: {UserId}, Book ID: {BookId}", userId, bookId);
                    return BadRequest(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Book already in wishlist",
                        Data = null
                    });
                }

                bool added = await _wishBL.AddToWishlist(userId, bookId);
                return added
                    ? Ok(new ResponseModel<string> { Success = true, Message = "Book added to wishlist", Data = null })
                    : BadRequest(new ResponseModel<string> { Success = false, Message = "Failed to add book", Data = null });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding book to wishlist");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Removes a book from the wishlist of the logged-in user.
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> RemoveFromWishlist([FromQuery] int bookId)
        {
            try
            {
                int userId = GetUserIdFromToken();
                bool removed = await _wishBL.RemoveFromWishlist(userId, bookId);
                return removed
                    ? Ok(new ResponseModel<string> { Success = true, Message = "Book removed from wishlist", Data = null })
                    : NotFound(new ResponseModel<string> { Success = false, Message = "Book not found in wishlist", Data = null });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing book from wishlist");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Moves a book from the wishlist to the cart.
        /// </summary>
        [HttpPost("move-to-cart")]
        public async Task<IActionResult> MoveToCart([FromQuery] int bookId)
        {
            try
            {
                int userId = GetUserIdFromToken();
                if (!await _wishBL.IsInWishlist(userId, bookId))
                {
                    _logger.LogWarning("Book not found in wishlist, User ID: {UserId}, Book ID: {BookId}", userId, bookId);
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Book not found in wishlist",
                        Data = null
                    });
                }

                bool moved = await _wishBL.MoveToCart(userId, bookId);
                return moved
                    ? Ok(new ResponseModel<string> { Success = true, Message = "Book moved to cart", Data = null })
                    : BadRequest(new ResponseModel<string> { Success = false, Message = "Failed to move book to cart", Data = null });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error moving book to cart");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}