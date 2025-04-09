using ModelLayer;
using RepoLayer.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IAddressBL
    {
        Task<Address> AddAddress(AddressInputModel addressInput,int userId);
        Task<List<Address>> GetAllAddresses();
        Task<Address> GetAddressById(int addressId);
        Task<List<Address>> GetAddressesByUserId(int userId);
        Task<Address> UpdateAddress(AddressInputModel addressInput, int userId);
        Task<bool> DeleteAddress(int addressId);
    }
}
