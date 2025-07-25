using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentFilesFrontend.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace ChecklistDesign.Pages
{
    public class SchoolyearPageModel : PageModel
    {
        private readonly HttpClient _http;

        public SchoolyearPageModel(IHttpClientFactory factory)
        {
            _http = factory.CreateClient();
            _http.BaseAddress = new Uri("https://localhost:7116/");
        }

        [BindProperty(SupportsGet = true)]
        public string? CampusName { get; set; }

        [BindProperty]
        public int SchoolyearID { get; set; }

        [BindProperty]
        public string? SchoolyearName { get; set; }

        public List<Schoolyear> SchoolYears { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            if (string.IsNullOrWhiteSpace(CampusName))
            {
                ErrorMessage = "No campus selected.";
                return;
            }

            var encodedCampus = Uri.EscapeDataString(CampusName);
            var response = await _http.GetAsync($"api/schoolyear/by-campus?campusName={encodedCampus}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                SchoolYears = JsonSerializer.Deserialize<List<Schoolyear>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new();
            }
            else
            {
                ErrorMessage = "No data found for this campus.";
            }
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (string.IsNullOrWhiteSpace(CampusName) || string.IsNullOrWhiteSpace(SchoolyearName))
            {
                return new JsonResult(new { success = false, message = "All fields are required." });
            }

            string url = $"api/schoolyear/AddSchoolyear?schoolyearName={Uri.EscapeDataString(SchoolyearName)}&campusName={Uri.EscapeDataString(CampusName)}";
            var response = await _http.PostAsync(url, null); // null body, data is in query string

            string apiResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new JsonResult(new { success = false, message = apiResponse });
            }

            return new JsonResult(new { success = true });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/schoolyear/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage(new { campusName = CampusName });
            }

            return BadRequest("Failed to delete the record.");
        }
    }
}
