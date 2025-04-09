using RepoLayer.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepoLayer.Interfaces
{
    public interface IAddressRL
    {
        Task<Address> AddAddress(Address address);
        Task<List<Address>> GetAllAddresses();
        Task<Address> GetAddressById(int addressId);
        Task<List<Address>> GetAddressesByUserId(int userId);
        Task<Address> UpdateAddress(Address address);
        Task<bool> DeleteAddress(int addressId);
    }
}
