using IAMS.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace IAMS.Web.Pages.Country
{
    public class CreateModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _options;

        public CreateModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }
        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public IAMS.Model.Country? Country { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var httpClient = _httpClientFactory.CreateClient("localAPI");
            var jsonContent = new StringContent(JsonSerializer.Serialize<IAMS.Model.Country>(Country), Encoding.UTF8, "application/json");
            using (var response = await httpClient.PostAsync("/country", jsonContent))
            {
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./Index");
                }
                return Page();
            }
        }
    }
}
