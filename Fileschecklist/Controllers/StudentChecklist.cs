using Fileschecklist.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fileschecklist.Models;

namespace Fileschecklist.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentfilesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentfilesController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET ALL - always return 200 OK, even if empty
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Studentfiles>>> GetStudentfiles()
        {
            var studentFilesList = await _context.Studentfiles.ToListAsync();
            return Ok(studentFilesList); // ← This avoids the 404 error
        }

        // ✅ GET BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Studentfiles>> GetById(string id)
        {
            var student = await _context.Studentfiles.FindAsync(id);

            if (student == null)
            {
                return NotFound($"Student with ID {id} not found.");
            }

            return Ok(student);
        }

        // ✅ POST - Add new student
        [HttpPost("add")]
        public async Task<ActionResult<Studentfiles>> Create(
            [FromQuery] string studentID,
            [FromQuery] string studentname,
            [FromQuery] string gradelevel,
            [FromQuery] string remarks,
            [FromQuery] string status,
            [FromQuery] string section,
            [FromQuery] string lrn,
            [FromQuery] string formerschool,
            [FromQuery] string location,
            [FromQuery] string psa,
            [FromQuery] string card,
            [FromQuery] string enrollmentform,
            [FromQuery] string passportpicture,
            [FromQuery] string academichonors,
            [FromQuery] string indegency,
            [FromQuery] string form137)
        {
            if (string.IsNullOrWhiteSpace(studentID) || string.IsNullOrWhiteSpace(lrn))
            {
                return BadRequest("StudentID and LRN are required.");
            }

            // Check for duplicate
            bool exists = await _context.Studentfiles.AnyAsync(s =>
                s.StudentID == studentID || s.Lrn == lrn);

            if (exists)
            {
                return Conflict("This information already exists.");
            }

            var student = new Studentfiles
            {
                StudentID = studentID,
                Studentname = studentname,
                Gradelevel = gradelevel,
                Remarks = remarks,
                Status = status,
                Section = section,
                Lrn = lrn,
                Formerschool = formerschool,
                Location = location,
                PSA = psa,
                Card = card,
                Enrollmentform = enrollmentform,
                Passportpicture = passportpicture,
                Academichonors = academichonors,
                Indegency = indegency,
                Form137 = form137
            };

            _context.Studentfiles.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = student.StudentID }, student);
        }
    }
}
