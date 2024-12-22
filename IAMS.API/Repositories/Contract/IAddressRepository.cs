using IAMS.Model;
namespace IAMS.API.Repositories.Contract
{
    public interface IAddressRepository
    {
        Task<List<Address>> GetAddressesAsync();
        Task<PagedResponse<Address>> GetAddressesAsync(string countryIds, string searchString, string searchCity, string searchPostalCode, int pageNumber, int pageSize);

        Task<Address> CreateAddress(Address address);
        Task<AddressLabels> GetAddressLabels(int countryId);
    }
}
