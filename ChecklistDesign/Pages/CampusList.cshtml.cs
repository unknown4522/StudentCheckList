using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentFilesFrontend.Models; // ? Make sure this is correct
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentFilesFrontend.Pages // ? Must match your folder structure
{
    public class CampusListModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CampusListModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            Campuses = new List<Campus>();
        }

        public List<Campus> Campuses { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7116/api/campuses");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var campuses = JsonSerializer.Deserialize<List<Campus>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (campuses != null)
                    Campuses = campuses;
            }
            catch
            {
                // Log error or handle fallback
            }
        }
    }
}
