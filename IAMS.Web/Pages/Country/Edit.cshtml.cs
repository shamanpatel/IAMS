using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace IAMS.Web.Pages.Country
{
    public class EditModel : BasePageModel
    {
        [BindProperty]
        public IAMS.Model.Country? Country { get; set; }
        public EditModel(ILogger<EditModel> logger, IHttpClientFactory httpClientFactory) : base(logger, httpClientFactory)
        {
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var httpClient = _httpClientFactory.CreateClient("localAPI");
            using (var response = await httpClient.GetAsync($"/country/{id}", HttpCompletionOption.ResponseHeadersRead))
            {
                //response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Country = JsonSerializer.Deserialize<IAMS.Model.Country>(content, _options);
                }
            }

            if (Country == null)
            {
                return NotFound();
            }
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Country != null)
            {
                var httpClient = _httpClientFactory.CreateClient("localAPI");
                var jsonContent = new StringContent(JsonSerializer.Serialize<IAMS.Model.Country>(Country), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PutAsync($"/country/{Country.CountryId}", jsonContent))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToPage("./Index");
                    }
                    return Page();
                }
            }

            return RedirectToPage("./Index");
        }



    }
}
