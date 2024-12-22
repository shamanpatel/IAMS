
using IAMS.API.Repositories.Contract;
using IAMS.Model;

namespace IAMS.API.Endpoints
{
    public static class StateEndpoints
    {
        public static void RegisterStateEndpoints(this WebApplication app)
        {
            var country = app.MapGroup("/state");

            country.MapGet("/{countryId}/{pageNumber}/{pageSize}", GetAllStates);
            country.MapGet("/search", SearchStates);
            country.MapGet("/getByCountryId/{countryId}", GetStatesByCountryId);
            country.MapGet("/{id}", GetStateById);
            country.MapPost("/", CreateState);
            country.MapPut("/{id}", UpdateState);
            country.MapDelete("/{id}", DeleteState);

        }

        private static async Task DeleteState(IStateRepository addressRepository)
        {
            throw new NotImplementedException();
        }

        private static async Task UpdateState(IStateRepository addressRepository)
        {
            throw new NotImplementedException();
        }

        private static async Task<IResult> CreateState(IStateRepository stateRepository, State state)
        {
            try
            {
                await stateRepository.CreateState(state);
                return TypedResults.Created($"/state/{state.StateId}", state);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        private static async Task GetStateById(IStateRepository addressRepository)
        {
            throw new NotImplementedException();
        }
        public static async Task<IResult> GetAllStates(IStateRepository stateRepository, int countryId, string searchString, int pageNumber, int pageSize)
        //public static async Task<IResult> GetAllStates(IStateRepository stateRepository)
        {
            //throw new NotImplementedException();
            //return TypedResults.Ok(await stateRepository.GetStatesAsync( countryId,  pageNumber,  pageSize));
            return TypedResults.Ok(await stateRepository.GetStatesAsync(countryId, searchString, pageNumber, pageSize));
        }
        public static async Task<IResult> SearchStates(IStateRepository stateRepository, int countryId, string searchString, int pageNumber, int pageSize)

        {
            return TypedResults.Ok(await stateRepository.GetStatesAsync(countryId, searchString, pageNumber, pageSize));
        }
        public static async Task<IResult> GetStatesByCountryId(IStateRepository stateRepository, int countryId)
        {
            return TypedResults.Ok(await stateRepository.GetStatesByCountryId(countryId));
        }     

    }
}
