using IAMS.API.Data;

using IAMS.API.Repositories.Contract;
using IAMS.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace IAMS.API.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private const string addressListCacheKey = "addressList";

        private readonly IAMSDBContext _dbContext;
        private readonly IDistributedCache cache_;
        private static readonly SemaphoreSlim semaphore = new(1, 1);

        public AddressRepository(IAMSDBContext dbContext, IDistributedCache cache)
        {
            _dbContext = dbContext;
            cache_ = cache ?? throw new ArgumentNullException(nameof(cache));
            GetAddressesFromCache().Wait();
        }

        public async Task<List<Address>> GetAddressesAsync()
        {
            return await this._dbContext.Addresses.ToListAsync();
        }

        /* STATES */
        public async Task<List<State>> GetStatesAsync(int countryId = 0, int lastId = 0, int pageSize = 10)
        {
            List<State> states;
            if (countryId > 0)
                //return await this._dbContext.States.Where(x => x.CountryId == countryId).ToListAsync();
                states = await _dbContext.States.AsNoTracking()
                .OrderBy(x => x.CountryId)
                .Where(x => x.CountryId == countryId)
                .Where(p => p.StateId > lastId) // in this example, reference is the "lastId"
                .Take(pageSize)
                .ToListAsync();
            else
                //return await this._dbContext.States.ToListAsync();
                states = await _dbContext.States.AsNoTracking()
                .OrderBy(x => x.StateId)
                .Where(p => p.StateId > lastId) // in this example, reference is the "lastId"
                .Take(pageSize)
                .ToListAsync();

            return states;
        }
        public async Task<State> CreateState(State state)
        {
            var existingStateCount = this._dbContext.States.Count(a => a.CountryId == state.CountryId && a.StateName == state.StateName);
            if (existingStateCount == 0)
            {
                var result = await this._dbContext.States.AddAsync(state);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            else
            {
                throw new Exception("State already exists");
            }
        }

        public async Task<Address> CreateAddress(Address address)
        {
            var existingAddressCount = this._dbContext.Addresses.Count(a => a.CountryId == address.CountryId
                                                                        && a.StateId == address.StateId
                                                                        && a.RecipientName == address.RecipientName
                                                                        && a.City.ToLower() == address.City.ToLower()
                                                                        );
            if (existingAddressCount == 0)
            {
                var result = await this._dbContext.Addresses.AddAsync(address);
                try
                {
                    await this._dbContext.SaveChangesAsync();
                    cache_.Remove(addressListCacheKey);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return result.Entity;
            }
            else
            {
                throw new Exception("address already exists");
            }
        }

        public async Task<PagedResponse<Address>> GetAddressesAsync(string countryIds, string searchRecipient, string searchCity, string searchPostalCode, int pageNumber, int pageSize)
        {
            if (cache_.TryGetValue("countryList", out IEnumerable<Country>? countryList))
            {
                string msg = "Country list found in cache.";
            }
            else
            {
                countryList = await this._dbContext.Countries.ToListAsync();
                var options = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(DateTimeOffset.Now.AddHours(1));
                await cache_.SetAsync("countryList", countryList, options);
            }

            if (cache_.TryGetValue("stateList", out IEnumerable<State>? stateList))
            {
                string msg = "State list found in cache.";
            }else
            {
                stateList = await this._dbContext.States.ToListAsync();
                var options = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(DateTimeOffset.Now.AddHours(1));
                await cache_.SetAsync("stateList", stateList, options);
            }

            if (cache_.TryGetValue(addressListCacheKey, out IEnumerable<Address>? addressList))
            {
                string msg = "Address list found in cache.";
            }

            IQueryable<Address>? addressIQ = null;
            //List<Address>? addressIQ = null;


            int[] countryId = countryIds.Split(',').Select(int.Parse).ToArray();

            try
            {
                if (countryId.Length == 1 && countryId[0] == 0)
                {
                    //addressIQ = from a in _dbContext.Addresses
                    //            join c in _dbContext.Countries on a.CountryId equals c.CountryId
                    //            join st in _dbContext.States on a.StateId equals st.StateId

                    var addressIQ_ = from a in addressList
                                     join c in countryList on a.CountryId equals c.CountryId
                                     join st in stateList on a.StateId equals st.StateId
                                     select new Address
                                     {
                                         AddressId = a.AddressId,
                                         StateId = a.StateId,
                                         StateName = st.StateName,
                                         CountryId = a.CountryId,
                                         CountryName = c.CountryName,
                                         RecipientName = a.RecipientName,
                                         AddressLine1 = a.AddressLine1,
                                         AddressLine2 = a.AddressLine2,
                                         AddressLine3 = a.AddressLine3,
                                         City = a.City,
                                         District = a.District,
                                         PostalCode = a.PostalCode,
                                         AdditionalInfo = a.AdditionalInfo
                                     };
                    addressIQ = addressIQ_.AsQueryable();
                }
                else
                {
                    var addressIQ_ = from a in addressList
                                     join c in countryList on a.CountryId equals c.CountryId
                                     join st in stateList on a.StateId equals st.StateId
                                     where countryId.Contains(a.CountryId)
                                     select new Address
                                     {
                                         AddressId = a.AddressId,
                                         StateId = a.StateId,
                                         StateName = st.StateName,
                                         CountryId = a.CountryId,
                                         CountryName = c.CountryName,
                                         RecipientName = a.RecipientName,
                                         AddressLine1 = a.AddressLine1,
                                         AddressLine2 = a.AddressLine2,
                                         AddressLine3 = a.AddressLine3,
                                         City = a.City,
                                         District = a.District,
                                         PostalCode = a.PostalCode,
                                         AdditionalInfo = a.AdditionalInfo
                                     };
                    addressIQ = addressIQ_.AsQueryable();
                }

                if (!String.IsNullOrEmpty(searchRecipient.Trim()))
                {
                    addressIQ = addressIQ.Where(s => s.RecipientName!.ToLower().Contains(searchRecipient.ToLower()));
                }

                if (!String.IsNullOrEmpty(searchCity.Trim()))
                {
                    addressIQ = addressIQ.Where(s => s.City!.ToLower().Contains(searchCity.ToLower()));
                }

                if (!String.IsNullOrEmpty(searchPostalCode.Trim()))
                {
                    addressIQ = addressIQ.Where(s => s.PostalCode!.Contains(searchPostalCode));
                }

                // var totalRecords = await addressIQ.AsNoTracking().CountAsync();
                var totalRecords = addressIQ.Count();

                //var states = await _dbContext.States.AsNoTracking()
                var addresses = addressIQ
                    .OrderBy(x => x.StateId)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var pagedResponse = new PagedResponse<Address>(addresses, pageNumber, pageSize, totalRecords);

                return pagedResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AddressLabels> GetAddressLabels(int countryId)
        {
            AddressLabels labels = new AddressLabels();
            try
            {
                var formats = this._dbContext.AddressFormats.Where(x => x.CountryId == countryId).ToList();
                if (formats != null && formats.Count > 0)
                {
                    labels.AddressFormats = formats;
                }
                labels.States = this._dbContext.States.Where(x => x.CountryId == countryId)
                    .Select(x => new State
                    {
                        StateId = x.StateId,
                        StateName = x.StateName
                    }).ToList();
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
            }
            return labels;
        }

        private async Task<List<Address>> GetAddressesFromCache()
        {
            List<Address>? addresses;
            var cacheKey = addressListCacheKey;
            var cacheValue = await cache_.GetAsync(cacheKey);
            if (cacheValue != null)
            {
                addresses = JsonSerializer.Deserialize<List<Address>>(Encoding.UTF8.GetString(cacheValue));
            }
            else
            {
                addresses = await this._dbContext.Addresses.ToListAsync();
                var options = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(DateTimeOffset.Now.AddHours(1));
                await cache_.SetAsync(cacheKey, addresses, options);
            }
            return addresses;
        }

        internal void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
