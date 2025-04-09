using BusinessLayer.Interfaces;
using ModelLayer;
using RepoLayer.Entity;
using RepoLayer.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace BusinessLayer.Services
{
    public class AddressBL : IAddressBL
    {
        private readonly IAddressRL _addressRL;
        private readonly ILogger<AddressBL> _logger;

        public AddressBL(IAddressRL addressRL, ILogger<AddressBL> logger)
        {
            _addressRL = addressRL;
            _logger = logger;
        }

        public async Task<Address> AddAddress(AddressInputModel addressInput, int userId)
        {
            try
            {
                _logger.LogInformation("Adding new address for user {UserId}", userId);
                Address address = new Address
                {
                    FullName = addressInput.FullName,
                    MobileNumber = addressInput.MobileNumber,
                    AddressLine = addressInput.AddressLine,
                    PinCode = addressInput.PinCode,
                    City = addressInput.City,
                    State = addressInput.State,
                    Type = addressInput.AddressType,
                    UserId = userId
                };

                return await _addressRL.AddAddress(address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BL while adding address for user {UserId}", userId);
                throw;
            }
        }

        public async Task<List<Address>> GetAllAddresses()
        {
            try
            {
                _logger.LogInformation("Fetching all addresses");
                return await _addressRL.GetAllAddresses();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BL while retrieving all addresses");
                throw;
            }
        }

        public async Task<Address> GetAddressById(int addressId)
        {
            try
            {
                _logger.LogInformation("Fetching address with ID {AddressId}", addressId);
                return await _addressRL.GetAddressById(addressId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BL while fetching address by ID {AddressId}", addressId);
                throw;
            }
        }

        public async Task<List<Address>> GetAddressesByUserId(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching addresses for user {UserId}", userId);
                return await _addressRL.GetAddressesByUserId(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BL while fetching addresses for user {UserId}", userId);
                throw;
            }
        }

        public async Task<Address> UpdateAddress(AddressInputModel addressInput, int userId)
        {
            try
            {
                _logger.LogInformation("Updating address for user {UserId}", userId);
                Address address = new Address
                {
                    FullName = addressInput.FullName,
                    MobileNumber = addressInput.MobileNumber,
                    AddressLine = addressInput.AddressLine,
                    PinCode = addressInput.PinCode,
                    City = addressInput.City,
                    State = addressInput.State,
                    Type = addressInput.AddressType,
                    UserId = userId
                };

                return await _addressRL.UpdateAddress(address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BL while updating address for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> DeleteAddress(int addressId)
        {
            try
            {
                _logger.LogInformation("Deleting address with ID {AddressId}", addressId);
                return await _addressRL.DeleteAddress(addressId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BL while deleting address with ID {AddressId}", addressId);
                throw;
            }
        }
    }
}
