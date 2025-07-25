using StudentFilesFrontend.Models;
using System.Text.Json;

namespace StudentFilesFrontend.Services
{
    public class CampusService
    {
        private readonly HttpClient _httpClient;

        public CampusService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Campus>> GetAllAsync()
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

                return campuses ?? new List<Campus>();
            }
            catch
            {
                return new List<Campus>();
            }
        }
    }
}
