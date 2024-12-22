
using IAMS.API.Repositories;
using IAMS.API.Repositories.Contract;
using IAMS.Model;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace IAMS.API.Endpoints
{
    public static class AddressEndpoints
    {
        public static void RegisterAddressEndpoints(this WebApplication app)
        {
            var address = app.MapGroup("/address");
            address.MapGet("/search", SearchAddresses);
            address.MapGet("/getAddressLabels/{countryId}", GetAddressLabels);
            address.MapGet("/", GetAllAddresses);
            address.MapGet("/{id}", GetAddressById);
            address.MapPost("/", CreateAddress);
            address.MapPut("/{id}", UpdateAddress);
            address.MapDelete("/{id}", DeleteAddress);

        }

        private static async Task DeleteAddress(IAddressRepository addressRepository)
        {
            throw new NotImplementedException();
        }

        private static async Task UpdateAddress(IAddressRepository addressRepository)
        {
            throw new NotImplementedException();
        }

        private static async Task<IResult> CreateAddress(IAddressRepository addressRepository, Address address)
        {
            try
            {
                await addressRepository.CreateAddress(address);
                return TypedResults.Created($"/address/{address.AddressId}", address);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        private static async Task GetAddressById(IAddressRepository addressRepository)
        {
            throw new NotImplementedException();
        }

        private static async Task GetAllAddresses(IAddressRepository addressRepository)
        {
            throw new NotImplementedException();
        }
        public static async Task<IResult> GetAddressLabels(IAddressRepository addressRepository, int countryId)
        {
            return TypedResults.Ok(await addressRepository.GetAddressLabels(countryId));
        }
        public static async Task<IResult> SearchAddresses(IAddressRepository addressRepository, string countryId, string searchRecipient, string searchCity, string searchPostalCode, int pageNumber, int pageSize)
        {
            return TypedResults.Ok(await addressRepository.GetAddressesAsync(countryId, searchRecipient, searchCity, searchPostalCode, pageNumber, pageSize));
        }
    }
}
