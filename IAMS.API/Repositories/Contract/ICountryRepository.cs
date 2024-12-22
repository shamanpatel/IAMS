using IAMS.Model;

namespace IAMS.API.Repositories.Contract
{
    public interface ICountryRepository
    {
        Task<List<Country>> GetCountriesAsync();
    }
}
