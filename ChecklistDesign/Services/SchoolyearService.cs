using StudentFilesFrontend.Models;
using System.Text.Json;

namespace StudentFilesFrontend.Services
{
    public class SchoolyearService
    {
        private readonly HttpClient _httpClient;

        public SchoolyearService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Schoolyear>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7116/api/Schoolyear");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var years = JsonSerializer.Deserialize<List<Schoolyear>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return years ?? new List<Schoolyear>();
            }
            catch
            {
                return new List<Schoolyear>();
            }
        }
    }
}
