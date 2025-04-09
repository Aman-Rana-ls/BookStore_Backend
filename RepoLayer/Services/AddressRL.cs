using Microsoft.Extensions.Logging;
using RepoLayer.Context;
using RepoLayer.Entity;
using RepoLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

public class AddressRL : IAddressRL
{
    private readonly AppDbContext _context;
    private readonly ILogger<AddressRL> _logger;

    public AddressRL(AppDbContext context, ILogger<AddressRL> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Address> AddAddress(Address address)
    {
        try
        {
            _logger.LogInformation("Adding a new address for user {UserId}", address.UserId);
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while adding address.");
            throw new Exception("An unexpected error occurred while adding the address.", ex);
        }
    }

    public async Task<List<Address>> GetAllAddresses()
    {
        try
        {
            _logger.LogInformation("Retrieving all addresses.");
            return await _context.Addresses.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve all addresses.");
            throw new Exception("Failed to retrieve addresses from the database.", ex);
        }
    }

    public async Task<Address> GetAddressById(int addressId)
    {
        try
        {
            _logger.LogInformation("Retrieving address with ID {AddressId}.", addressId);
            var address = await _context.Addresses.FindAsync(addressId);
            if (address == null)
            {
                throw new KeyNotFoundException($"Address with ID {addressId} not found.");
            }
            return address;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve address by ID.");
            throw new Exception("An error occurred while fetching the address.", ex);
        }
    }

    public async Task<List<Address>> GetAddressesByUserId(int userId)
    {
        try
        {
            _logger.LogInformation("Retrieving addresses for user ID {UserId}.", userId);
            var addresses = await _context.Addresses.Where(a => a.UserId == userId).ToListAsync();
            return addresses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve addresses by user ID.");
            throw new Exception("An error occurred while retrieving addresses for the user.", ex);
        }
    }

    public async Task<Address> UpdateAddress(Address address)
    {
        try
        {
            _logger.LogInformation("Updating address with ID {AddressId}.", address.AddressId);
            var existing = await _context.Addresses.FindAsync(address.AddressId);

            if (existing == null)
            {
                throw new KeyNotFoundException($"Address with ID {address.AddressId} not found.");
            }

            existing.FullName = address.FullName;
            existing.MobileNumber = address.MobileNumber;
            existing.AddressLine = address.AddressLine;
            existing.PinCode = address.PinCode;
            existing.City = address.City;
            existing.State = address.State;
            existing.Type = address.Type;

            await _context.SaveChangesAsync();
            return existing;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating address.");
            throw new Exception("An unexpected error occurred while updating the address.", ex);
        }
    }

    public async Task<bool> DeleteAddress(int addressId)
    {
        try
        {
            _logger.LogInformation("Deleting address with ID {AddressId}.", addressId);
            var address = await _context.Addresses.FindAsync(addressId);

            if (address == null)
            {
                throw new KeyNotFoundException($"Address with ID {addressId} not found.");
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while deleting address.");
            throw new Exception("An unexpected error occurred while deleting the address.", ex);
        }
    }
}
