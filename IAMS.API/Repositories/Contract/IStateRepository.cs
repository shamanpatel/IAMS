using IAMS.Model;

namespace IAMS.API.Repositories.Contract
{
    public interface IStateRepository
    {
        //STATES
        Task<PagedResponse<State>> GetStatesAsync(int countryId, string searchString, int pageNumber, int pageSize);
        Task<List<State>> GetStatesByCountryId(int countryId);
        Task<State> CreateState(State state);
    }
}
