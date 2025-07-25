using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using ChecklistDesign.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ChecklistDesign.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _env;

        public ProfileModel(IHttpClientFactory httpClientFactory, IWebHostEnvironment env)
        {
            _httpClient = httpClientFactory.CreateClient();
            _env = env;
        }

        [BindProperty]
        public LoginData UserProfile { get; set; } = new();


        [BindProperty]
        public IFormFile? UploadedFile { get; set; } // ✅ made nullable to avoid constructor error

        public async Task<IActionResult> OnGetAsync()
        {
            await Task.CompletedTask; // ✅ ensures async method behaves properly

            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["Error"] = "User ID not found in session.";
                return RedirectToPage("/Login");
            }
            UserProfile.Id = userId.ToString();

            UserProfile.FirstName = HttpContext.Session.GetString("FirstName") ?? "";
            UserProfile.LastName = HttpContext.Session.GetString("LastName") ?? "";
            UserProfile.Role = HttpContext.Session.GetString("Role") ?? "";
            UserProfile.Username = HttpContext.Session.GetString("Username") ?? "";
            UserProfile.Profilepic = HttpContext.Session.GetString("Profilepic") ?? "/images/default.png";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["Error"] = "User ID not found in session.";
                return RedirectToPage("/Login");
            }

            if (UploadedFile == null || UploadedFile.Length == 0)
            {
                TempData["Error"] = "Please select a file.";
                return RedirectToPage();
            }

            try
            {
                var content = new MultipartFormDataContent();

                var stream = UploadedFile.OpenReadStream();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(UploadedFile.ContentType);

                content.Add(fileContent, "file", UploadedFile.FileName);
                content.Add(new StringContent(userId.ToString()!), "UserId");

                var response = await _httpClient.PostAsync($"https://localhost:7116/api/studentfiles/upload-profilepic", content);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Failed to upload image.";
                    return RedirectToPage();
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<LoginData>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (!string.IsNullOrEmpty(result?.Profilepic))
                {
                    HttpContext.Session.SetString("Profilepic", result.Profilepic);
                    UserProfile.Profilepic = result.Profilepic;
                    TempData["Success"] = "✅ Profile picture updated successfully.";
                }
                else
                {
                    TempData["Error"] = "Server did not return a valid image path.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred: " + ex.Message;
            }

            return RedirectToPage();
        }
    }
}
