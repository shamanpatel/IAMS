
using IAMS.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics.Eventing.Reader;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace IAMS.Web.Pages.Address
{
    public class CreateModel : PageModel
    {

        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _options;
        public AddressLabels AddressLabel { get; set; }
        //public List<Web.Model.Car> Cars { get; set; }
        [BindProperty]
        public IAMS.Model.Address Address { get; set; }
        //[BindProperty]
        public SelectList? StateList { get; set; }
        //[BindProperty]
        public SelectList? CountryList { get; set; }
        //[BindProperty(SupportsGet = true)]
        //public int SelectedCountryId { get; set; }
        //[BindProperty]
        //public int SelectedStateId { get; set; }
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
        public JsonResult OnGetStates(int countryId)
        {           
            AddressLabels labels = new AddressLabels();           
            var httpClient = _httpClientFactory.CreateClient("localAPI");
            //var response = httpClient.GetAsync($"/state/getByCountryId/{countryId}", HttpCompletionOption.ResponseHeadersRead).Result;
            var response = httpClient.GetAsync($"/address/getAddressLabels/{countryId}", HttpCompletionOption.ResponseHeadersRead).Result;

            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                labels = JsonSerializer.Deserialize<IAMS.Model.AddressLabels>(content, _options);
            }
            return new JsonResult(labels);
        }

        private async Task LoadContries()
        {
            var httpClient = _httpClientFactory.CreateClient("localAPI");

            using (var response = await httpClient.GetAsync("/state", HttpCompletionOption.ResponseHeadersRead))
            {
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var states = JsonSerializer.Deserialize<List<IAMS.Model.State>>(content, _options);
                    StateList = new SelectList(states, "StateId", "StateName");
                }
            }
        }

        public JsonResult OnGetLabels(int countryId)
        {
            int cou = countryId;
            AddressLabels prop = new AddressLabels();
 

            return new JsonResult(prop);
        }

        public List<Web.Model.Car> GetAll()
        {
            List<Web.Model.Car> cars = new List<Web.Model.Car>{
            new Web.Model.Car{Id = 1, Make="Audi",Model="R8",Year=2014,Doors=2,Colour="Red",Price=79995},
            new Web.Model.Car{Id = 2, Make="Aston Martin",Model="Rapide",Year=2010,Doors=2,Colour="Black",Price=54995},
            new Web.Model.Car{Id = 3, Make="Porsche",Model=" 911 991",Year=2016,Doors=2,Colour="White",Price=155000},
            new Web.Model.Car{Id = 4, Make="Mercedes-Benz",Model="GLE 63S",Year=2017,Doors=5,Colour="Blue",Price=83995},
            new Web.Model.Car{Id = 5, Make="BMW",Model="X6 M",Year=2016,Doors=5,Colour="Silver",Price=62995},
        };
            return cars;
        }



        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            // Address.CountryId = countryId;
            var httpClient = _httpClientFactory.CreateClient("localAPI");
            var jsonContent = new StringContent(JsonSerializer.Serialize<IAMS.Model.Address>(Address), Encoding.UTF8, "application/json");
            using (var response = await httpClient.PostAsync("/address", jsonContent))
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
