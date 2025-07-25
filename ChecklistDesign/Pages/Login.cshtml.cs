using ChecklistDesign.Models; // Your LoginData model namespace
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChecklistDesign.Pages
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [BindProperty]
        public LoginData Login { get; set; } = new();

        public IActionResult OnGet()
        {
            // Prevent caching the login page
            Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            // If already logged in, redirect to StudentList or Profile
            if (HttpContext.Session.GetString("IsAuthenticated") == "true")
            {
                return RedirectToPage("/CampusList");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Prevent caching
            Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (string.IsNullOrEmpty(Login.Username) || string.IsNullOrEmpty(Login.Password))
            {
                Login.Message = "❗ Please fill in all fields.";
                return Page();
            }

            var response = await _httpClient.PostAsync(
                $"https://localhost:7116/api/auth/login?username={Login.Username}&password={Login.Password}", null);

            if (!response.IsSuccessStatusCode)
            {
                Login.Message = "❌ Login failed!";
                return Page();
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var token = doc.RootElement.GetProperty("token").GetString();
            var data = doc.RootElement.GetProperty("data");

            HttpContext.Session.SetString("IsAuthenticated", "true");
            HttpContext.Session.SetString("Token", token ?? "");
            HttpContext.Session.SetInt32("UserId", data.GetProperty("id").GetInt32());
            HttpContext.Session.SetString("Username", data.GetProperty("username").GetString() ?? "");
            HttpContext.Session.SetString("FirstName", data.GetProperty("firstName").GetString() ?? "");
            HttpContext.Session.SetString("LastName", data.GetProperty("lastName").GetString() ?? "");
            HttpContext.Session.SetString("Role", data.GetProperty("role").GetString() ?? "");
            HttpContext.Session.SetString("Profilepic", data.GetProperty("profilepic").GetString() ?? "");

            return RedirectToPage("/CampusList");
        }
    }
}
