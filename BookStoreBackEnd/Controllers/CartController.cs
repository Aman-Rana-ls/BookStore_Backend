using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using RepoLayer.Entity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BookStoreBackEnd.Controllers
{
    /// <summary>
    /// Controller for managing shopping cart operations
    /// </summary>
    [Authorize]
    [Route("cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartBL _cartBL;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartBL cartBL, ILogger<CartController> logger)
        {
            _cartBL = cartBL;
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
        /// Adds a book to the user's shopping cart
        /// </summary>
        /// <param name="bookId">ID of the book to add</param>
        /// <returns>Response indicating success or failure</returns>
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromQuery] int bookId)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var result = await _cartBL.AddToCart(userId, bookId);

                return Ok(new ResponseModel<Cart>
                {
                    Success = true,
                    Message = "Book added to cart",
                    Data = result
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex.Message);
                return Unauthorized(new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding book to cart");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Retrieves all items in the user's shopping cart
        /// </summary>
        /// <returns>List of cart items</returns>
        [HttpGet]
        public async Task<IActionResult> GetUserCart()
        {
            try
            {
                var userId = GetUserIdFromToken();
                var result = await _cartBL.GetUserCart(userId);

                return Ok(new ResponseModel<IEnumerable<CartResponseModel>>
                {
                    Success = true,
                    Message = "Cart retrieved",
                    Data = result
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex.Message);
                return Unauthorized(new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Updates quantity or purchase status of a cart item
        /// </summary>
        /// <param name="updateModel">Model containing book ID, quantity, and purchase status</param>
        /// <returns>Response indicating success or failure</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemModel updateModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Message = "Invalid input model",
                    Data = null
                });
            }

            try
            {
                var userId = GetUserIdFromToken();
                var result = await _cartBL.UpdateCartItem(
                    userId,
                    updateModel.BookId,
                    updateModel.Quantity,
                    updateModel.IsPurchased
                );

                if (result == null)
                {
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Cart item not found",
                        Data = null
                    });
                }

                return Ok(new ResponseModel<Cart>
                {
                    Success = true,
                    Message = "Cart updated",
                    Data = result
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex.Message);
                return Unauthorized(new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Removes a book from the user's shopping cart
        /// </summary>
        /// <param name="bookId">ID of the book to remove</param>
        /// <returns>Response indicating success or failure</returns>
        [HttpDelete]
        public async Task<IActionResult> RemoveFromCart([FromQuery] int bookId)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var result = await _cartBL.RemoveFromCart(userId, bookId);

                if (!result)
                {
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Cart item not found",
                        Data = null
                    });
                }

                return Ok(new ResponseModel<string>
                {
                    Success = true,
                    Message = "Item removed from cart",
                    Data = null
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex.Message);
                return Unauthorized(new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing from cart");
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
