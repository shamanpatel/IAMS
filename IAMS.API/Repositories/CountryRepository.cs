using IAMS.API.Data;
using IAMS.API.Repositories.Contract;
using IAMS.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text;

namespace IAMS.API.Repositories
{
    public class CountryRepository : ICountryRepository
    {

        private const string countryListCacheKey = "countryList";
        private readonly IAMSDBContext _dbContext;
        private readonly IDistributedCache cache_;


        public CountryRepository(IAMSDBContext dbContext, IDistributedCache cache)
        {
            _dbContext = dbContext;
            cache_ = cache ?? throw new ArgumentNullException(nameof(cache));
            GetCountryFromCache().Wait();
        }


        public async Task<List<Country>> GetCountriesAsync()
        {
            return await this._dbContext.Countries.OrderBy(x => x.CountryName).ToListAsync();
        }









        private async Task<List<Country>> GetCountryFromCache()
        {
            List<Country>? countries;
            var cacheKey = countryListCacheKey;
            var cacheValue = await cache_.GetAsync(cacheKey);
            if (cacheValue != null)
            {
                countries = JsonSerializer.Deserialize<List<Country>>(Encoding.UTF8.GetString(cacheValue));
            }
            else
            {
                countries = await this._dbContext.Countries.ToListAsync();
                var options = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(DateTimeOffset.Now.AddHours(1));
                await cache_.SetAsync(cacheKey, countries, options);
            }
            return countries;
        }
    }
}
