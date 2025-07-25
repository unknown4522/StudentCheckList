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

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    public int PageSize { get; set; } = 5;
    public int TotalPages { get; set; }
    public bool FilteredButNoResults { get; set; } = false;

    public StudentListModel(StudentfilesService service)
    {
        _service = service;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        // Prevent caching
        Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";

        // Redirect to login if not authenticated
        if (HttpContext.Session.GetString("IsAuthenticated") != "true")
        {
            return RedirectToPage("/Login");
        }

        List<Studentfiles>? allStudents;

        // ✅ FILTERED
        if (!string.IsNullOrEmpty(Schoolyear) && !string.IsNullOrEmpty(CampusName))
        {
            allStudents = await _service.GetBySchoolyearAndCampusAsync(Schoolyear, CampusName);

            if (allStudents == null || allStudents.Count == 0)
            {
                FilteredButNoResults = true;
                Students = new List<Studentfiles>();
                return Page();
            }
        }
        else
        {
            // 🟡 UNFILTERED
            allStudents = await _service.GetAllAsync();
        }

        // ✅ PAGINATION
        int totalItems = allStudents?.Count ?? 0;
        TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
        CurrentPage = Math.Clamp(CurrentPage, 1, TotalPages);

        Students = allStudents!
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Students = await _service.GetAllAsync();
            return Page();
        }

        var response = await _service.AddAsync(NewStudent);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToPage(); // reload with GET
        }

        ModelState.AddModelError(string.Empty, "Failed to add student.");
        Students = await _service.GetAllAsync();
        return Page();
    }
}
