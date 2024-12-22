using IAMS.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace IAMS.Web.Pages.State
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _options;
        //public IList<Model.State>? States { get; set; }
        public string CurrentFilter { get; set; }
        public IAMS.Model.PagedResponse<IAMS.Model.State>? States { get; set; }

        public SelectList? CountryList { get; set; }
        [BindProperty]
        public int SelectedCountryId { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        private async Task LoadContries()
        {
            var httpClient = _httpClientFactory.CreateClient("localAPI");

            using (var response = await httpClient.GetAsync("/country", HttpCompletionOption.ResponseHeadersRead))
            {
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var countries = JsonSerializer.Deserialize<List<IAMS.Model.Country>>(content, _options);
                    CountryList = new SelectList(countries, "CountryId", "CountryName");
                }
            }
        }

        public async Task OnGetAsync(string currentFilter, int selectedCountryId, int countryId, string searchString, int? pageIndex)
        {
            await LoadContries();

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            if (pageIndex == null)
            {
                pageIndex = 1;
            }
            CurrentFilter = searchString;


            if (searchString == null)
            {
                searchString = "";
            }
            if (countryId == 0)
                countryId = selectedCountryId;

            SelectedCountryId = countryId;

            string query = $"search?countryId={SelectedCountryId}&searchString={searchString}&pageNumber={pageIndex}&pageSize=10";
            //string query = $"search?countryId=1&pageIndex={pageIndex}&pageSize=10";

            var httpClient = _httpClientFactory.CreateClient("localAPI");
            //using (var response = await httpClient.GetAsync($"/state/1/{searchString}/{pageIndex}/10", HttpCompletionOption.ResponseHeadersRead))
            using (var response = await httpClient.GetAsync($"/state/{query}", HttpCompletionOption.ResponseHeadersRead))
            {
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    // States = JsonSerializer.Deserialize<List<Model.State>>(content, _options);
                    States = JsonSerializer.Deserialize<PagedResponse<IAMS.Model.State>>(content, _options);
                }
            }
        }
    }
}
