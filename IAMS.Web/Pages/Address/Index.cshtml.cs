using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IAMS.Model;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http;
using System.Text.Json;

namespace IAMS.Web.Pages.Address
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _options;
        public IAMS.Model.PagedResponse<IAMS.Model.Address>? Address { get; set; }
        public string? CurrentRecipient { get; set; }
        public string? CurrentAddress1 { get; set; }
        public string? CurrentAddress2 { get; set; }
        public string? CurrentAddress3 { get; set; }
        public string? CurrentCity { get; set; }
        public string? CurrentPostalCode { get; set; }
        public SelectList? CountryList { get; set; }
        public SelectList? StateList { get; set; }
        [BindProperty]
        public int[]? SelectedCountryId { get; set; }
        public int[]? SelectedStateId { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task OnGetAsync(
            string currentRecipient,
            string searchRecipient,
            int[] selectedCountryId,
            int[] searchCountryId,
            int[] selectedStateId,
            int[] stateId,
            string currentCity,
            string searchCity,
            string currentPostCode,
            string searchPostalCode,
            int? pageIndex)
        {
            await LoadContries();

            searchRecipient = searchRecipient ?? currentRecipient;
            CurrentRecipient = searchRecipient;

            searchCity = searchCity ?? currentCity;
            CurrentCity = searchCity;

            searchPostalCode = searchPostalCode ?? currentPostCode;
            CurrentPostalCode = searchPostalCode;


            pageIndex = pageIndex ?? 1;
            searchRecipient = searchRecipient ?? "";
            searchCity = searchCity ?? "";
            searchPostalCode = searchPostalCode ?? "";


            if (searchCountryId.Length == 0)
                searchCountryId = selectedCountryId;

            SelectedCountryId = searchCountryId;

            var countryId_ = string.Join(",", SelectedCountryId);

            if (countryId_ == "")
                countryId_ = "0";
            string query = $"search?countryId={countryId_}&searchRecipient={searchRecipient}&searchCity={searchCity}&searchPostalCode={searchPostalCode}&pageNumber={pageIndex}&pageSize=10";


            var httpClient = _httpClientFactory.CreateClient("localAPI");
            using (var response = await httpClient.GetAsync($"/address/{query}", HttpCompletionOption.ResponseHeadersRead))
            {
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Address = JsonSerializer.Deserialize<PagedResponse<IAMS.Model.Address>>(content, _options);
                }
            }
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
    }
}
