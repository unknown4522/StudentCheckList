using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentFilesFrontend.Models;
using StudentFilesFrontend.Services;

namespace StudentFilesFrontend.Pages.Students
{
    public class StudentdataModel : PageModel
    {
        private readonly StudentfilesService _service;

        public StudentdataModel(StudentfilesService service)
        {
            _service = service;
        }

        public List<Studentfiles>? Students { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Schoolyear { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? CampusName { get; set; }

        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            if (!string.IsNullOrWhiteSpace(Schoolyear) && !string.IsNullOrWhiteSpace(CampusName))
            {
                try
                {
                    Students = await _service.GetBySchoolyearAndCampusAsync(Schoolyear, CampusName);
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
            }
            else
            {
                ErrorMessage = "Missing school year or campus name.";
            }
        }
    }
}
