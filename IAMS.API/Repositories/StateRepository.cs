using IAMS.API.Data;
using IAMS.API.Repositories.Contract;
using IAMS.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text;

namespace IAMS.API.Repositories
{
    public class StateRepository : IStateRepository
    {

        private const string stateListCacheKey = "stateList";
        private readonly IAMSDBContext _dbContext;
        private readonly IDistributedCache cache_;
        private static readonly SemaphoreSlim semaphore = new(1, 1);

        public StateRepository(IAMSDBContext dbContext, IDistributedCache cache)
        {
            _dbContext = dbContext;
            cache_ = cache ?? throw new ArgumentNullException(nameof(cache));
            GetStateFromCache().Wait();
        }

        //int pageNumber, int pageSize
        //public async Task<List<State>> GetStatesAsync(int countryId = 0, int lastId = 0, int pageSize = 10)
        public async Task<PagedResponse<State>> GetStatesAsync(int countryId, string searchString, int pageNumber, int pageSize)
        {
            var countryId_ = new SqlParameter
            {
                ParameterName = "countryId",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = countryId
            };
            var searchString_ = new SqlParameter
            {
                ParameterName = "searchString",
                SqlDbType = System.Data.SqlDbType.NVarChar,
                Value = searchString
            };
            var pageNumber_ = new SqlParameter
            {
                ParameterName = "pageNumber",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = pageNumber
            };
            var pageSize_ = new SqlParameter
            {
                ParameterName = "pageSize",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = pageSize
            };
            var totalRecords_ = new SqlParameter
            {
                ParameterName = "totalRecords",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output,
            };

            try
            {
                var states = _dbContext.States
                  .FromSqlRaw("EXECUTE dbo.GetStatesByCountryAndSearch @countryId, @searchString, @pageNumber, @pageSize, @totalRecords OUTPUT", countryId_, searchString_, pageNumber_, pageSize_, totalRecords_)
                  .ToList();
                int totalCount = (int)totalRecords_.Value;
                var pagedResponse = new PagedResponse<State>(states, pageNumber, pageSize, totalCount);

                return pagedResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<State> CreateState(State state)
        {
            throw new NotImplementedException();
        }

        public async Task<List<State>> GetStatesByCountryId(int countryId)
        {
            return await this._dbContext.States.Where(x => x.CountryId == countryId).ToListAsync();
        }
        private async Task<List<State>> GetStateFromCache()
        {
            List<State> states;
            var cacheKey = stateListCacheKey;
            var cacheValue = await cache_.GetAsync(cacheKey);
            if (cacheValue != null)
            {
                states = JsonSerializer.Deserialize<List<State>>(Encoding.UTF8.GetString(cacheValue));
            }
            else
            {
                states = await this._dbContext.States.ToListAsync();
                var options = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(DateTimeOffset.Now.AddHours(1));
                await cache_.SetAsync(cacheKey, states, options);
            }
            return states;          

        }
    }
}
