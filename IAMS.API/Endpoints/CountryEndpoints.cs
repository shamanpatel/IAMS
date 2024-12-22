using IAMS.API.Data;
using IAMS.API.Repositories.Contract;
using IAMS.Model;
using Microsoft.EntityFrameworkCore;
namespace IAMS.API.Endpoints
{
    public static class CountryEndpoints
    {
        public static void RegisterCountryEndpoints(this WebApplication app) {
            var country = app.MapGroup("/country");

            country.MapGet("/", GetAllCountries);
            country.MapGet("/{id}", GetCountryById);
            country.MapPost("/", CreateCountry);
            country.MapPut("/{id}", UpdateCountry);
            country.MapDelete("/{id}", DeleteCountry);

        }
        public static async Task<IResult> GetAllCountries(ICountryRepository countryRepository)
        {
            return TypedResults.Ok(await countryRepository.GetCountriesAsync());// Countries.ToListAsync());
        }
        public static async Task<IResult> GetCountryById(int id, IAMSDBContext db)
        {
            return await db.Countries.FindAsync(id)
                is Country country
                    ? TypedResults.Ok(country)
                    : TypedResults.NotFound();
        }
        public static async Task<IResult> CreateCountry(Country country, IAMSDBContext db)
        {
            db.Countries.Add(country);
            await db.SaveChangesAsync();

            return TypedResults.Created($"/country/{country.CountryId}", country);
        }
        public static async Task<IResult> UpdateCountry(int id, Country inputCountry, IAMSDBContext db)
        {
            var country = await db.Countries.FindAsync(id);

            if (country is null) return Results.NotFound();

            country.CountryName = inputCountry.CountryName;
            country.CountryCode = inputCountry.CountryCode;

            await db.SaveChangesAsync();

            return TypedResults.Ok(country);
        }
        public static async Task<IResult> DeleteCountry(int id, IAMSDBContext db)
        {
            var country = await db.Countries.FindAsync(id);

            if (country is null) return Results.NotFound();

            db.Countries.Remove(country);
            await db.SaveChangesAsync();

            return Results.NoContent();
        }       
    }
}
