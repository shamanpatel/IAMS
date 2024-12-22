using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace IAMS.Web
{
    public class BasePageModel : PageModel
    {
        protected readonly ILogger<BasePageModel> _logger;
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly JsonSerializerOptions _options;

        public BasePageModel(ILogger<BasePageModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }
    }
}
