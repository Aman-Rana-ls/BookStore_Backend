using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Interfaces;
using RepoLayer.Entity;
using System;
using System.Threading.Tasks;
using ModelLayer;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace BookStoreBackEnd.Controllers
{
    [Route("Address")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressBL _addressBL;
        private readonly ILogger<AddressController> _logger;

        public AddressController(IAddressBL addressBL, ILogger<AddressController> logger)
        {
            _addressBL = addressBL;
            _logger = logger;
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                _logger.LogWarning("Unauthorized access attempt - Invalid user ID in token.");
                throw new UnauthorizedAccessException("Invalid user ID in token");
            }
            return userId;
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress(AddressInputModel addressInput)
        {
            try
            {
                int userId = GetUserIdFromToken();
                _logger.LogInformation("Adding new address for user {UserId}", userId);
                var result = await _addressBL.AddAddress(addressInput, userId);

                return Ok(new ResponseModel<Address>
                {
                    Success = true,
                    Message = "Address added successfully.",
                    Data = result
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access in AddAddress");
                return Unauthorized(new ResponseModel<string> { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in AddAddress");
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = "An error occurred." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                _logger.LogInformation("Fetching all addresses.");
                var result = await _addressBL.GetAllAddresses();

                return Ok(new ResponseModel<List<Address>>
                {
                    Success = true,
                    Message = "Addresses fetched successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAll");
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = "An error occurred." });
            }
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching address with ID {AddressId}", id);
                var result = await _addressBL.GetAddressById(id);

                if (result != null)
                {
                    return Ok(new ResponseModel<Address>
                    {
                        Success = true,
                        Message = "Address found.",
                        Data = result
                    });
                }

                return NotFound(new ResponseModel<string>
                {
                    Success = false,
                    Message = "Address not found."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetById");
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = "An error occurred." });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching addresses for user {UserId}", userId);
                var result = await _addressBL.GetAddressesByUserId(userId);

                return Ok(new ResponseModel<List<Address>>
                {
                    Success = true,
                    Message = "User addresses fetched successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetByUserId");
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = "An error occurred." });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(AddressInputModel addressInput)
        {
            try
            {
                int userId = GetUserIdFromToken();
                _logger.LogInformation("Updating address for user {UserId}", userId);
                var result = await _addressBL.UpdateAddress(addressInput, userId);

                if (result != null)
                {
                    return Ok(new ResponseModel<Address>
                    {
                        Success = true,
                        Message = "Address updated successfully.",
                        Data = result
                    });
                }

                return NotFound(new ResponseModel<string>
                {
                    Success = false,
                    Message = "Address not found."
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access in Update");
                return Unauthorized(new ResponseModel<string> { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Update");
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = "An error occurred." });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Deleting address with ID {AddressId}", id);
                var success = await _addressBL.DeleteAddress(id);

                if (success)
                {
                    return Ok(new ResponseModel<string>
                    {
                        Success = true,
                        Message = "Address deleted successfully."
                    });
                }

                return NotFound(new ResponseModel<string>
                {
                    Success = false,
                    Message = "Address not found."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Delete");
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = "An error occurred." });
            }
        }
    }
}
