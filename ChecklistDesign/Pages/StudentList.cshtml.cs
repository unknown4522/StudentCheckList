using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentFilesFrontend.Models;
using StudentFilesFrontend.Services;

public class StudentListModel : PageModel
{
    private readonly StudentfilesService _service;

    public List<Studentfiles>? Students { get; set; }

    [BindProperty]
    public Studentfiles NewStudent { get; set; } = new Studentfiles();

    [BindProperty(SupportsGet = true)]
    public string? Schoolyear { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? CampusName { get; set; }

    public bool FilteredButNoResults { get; set; } = false;

    public StudentListModel(StudentfilesService service)
    {
        _service = service;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";

        if (HttpContext.Session.GetString("IsAuthenticated") != "true")
        {
            return RedirectToPage("/Login");
        }

        if (!string.IsNullOrEmpty(Schoolyear) && !string.IsNullOrEmpty(CampusName))
        {
            Students = await _service.GetBySchoolyearAndCampusAsync(Schoolyear, CampusName);

            if (Students == null || Students.Count == 0)
            {
                FilteredButNoResults = true;
            }
        }
        else
        {
            Students = await _service.GetAllAsync();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Students = await _service.GetAllAsync(); // reload students to stay on page
            return Page();
        }

        var response = await _service.AddAsync(NewStudent);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToPage(); // Refresh to clear form and update list
        }

        ModelState.AddModelError(string.Empty, "Failed to add student.");
        Students = await _service.GetAllAsync();
        return Page();
    }
}
