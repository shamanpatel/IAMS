using IAMS.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;


namespace IAMS.Web.Pages.State
{
    public class CreateModel : PageModel
    {

        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _options;

        [BindProperty]
        public IAMS.Model.State? State { get; set; }
  
        public SelectList? CountryList { get; set; }
        [BindProperty]
        public int SelectedCountryId { get; set; }
        public CreateModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public IActionResult OnGet()
        {
            var httpClient = _httpClientFactory.CreateClient("localAPI");
            var response = httpClient.GetAsync("/country").Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                var countries = JsonSerializer.Deserialize<List<IAMS.Model.Country>>(content, _options);
                CountryList = new SelectList(countries, "CountryId", "CountryName");
            }
            return Page();
        }


        public async Task<IActionResult> OnPostAsync(int countryId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            State.CountryId = countryId;
            var httpClient = _httpClientFactory.CreateClient("localAPI");
            var jsonContent = new StringContent(JsonSerializer.Serialize<IAMS.Model.State>(State), Encoding.UTF8, "application/json");
            using (var response = await httpClient.PostAsync("/state", jsonContent))
            {
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./Index");
                }
                else
                {
                    string content = await response.Content.ReadAsStringAsync();
                    JsonNode result = JsonSerializer.Deserialize<JsonNode>(content, _options);
                   
                    string message = result["detail"].GetValue<string>();
                    ModelState.AddModelError(string.Empty, message);
                      
                    
                }

                
                return Page();
            }
        }
    }
}
