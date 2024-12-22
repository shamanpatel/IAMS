using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using IAMS.Model;
namespace IAMS.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _options;
        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task OnGetAsync()
        {
         
            var httpClient = _httpClientFactory.CreateClient("localAPI");
            using (var response = await httpClient.GetAsync("/country", HttpCompletionOption.ResponseHeadersRead))
            {
                //response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var countries = JsonSerializer.Deserialize<List<IAMS.Model.Country>>(content, _options);
                  
                }               
            }
        }
    }
}
