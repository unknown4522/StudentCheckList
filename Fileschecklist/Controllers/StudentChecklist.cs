using Fileschecklist.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fileschecklist.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Linq.Dynamic.Core;
using System.IO;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Studentfiles>>> GetStudentfiles(
                    [FromQuery] string? schoolyearName,
                    [FromQuery] string? campusName)
        {
            if (string.IsNullOrWhiteSpace(schoolyearName) || string.IsNullOrWhiteSpace(campusName))
            {
                return BadRequest(new { message = "School year and campus name are required." });
            }

            var studentFilesList = await _context.Studentfiles
                .Where(s =>
                    (s.SchoolyearName ?? "").ToLower() == schoolyearName.ToLower() &&
                    (s.CampusName ?? "").ToLower() == campusName.ToLower())
                .ToListAsync();

            if (!studentFilesList.Any())
            {
                return NotFound(new { message = "No students found for the selected school year and campus." });
            }

            return Ok(studentFilesList);
        }

        // ✅ GET BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Studentfiles>> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Student ID is required.");

            var student = await _context.Studentfiles.FindAsync(id);

            if (student == null)
            {
                return NotFound($"Student with ID '{id}' not found.");
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
             [FromQuery] string? section,
             [FromQuery] string? lrn,
             [FromQuery] string formerschool,
             [FromQuery] string location,
             [FromQuery] string psa,
             [FromQuery] string card,
             [FromQuery] string enrollmentform,
             [FromQuery] string passportpicture,
             [FromQuery] string academichonors,
             [FromQuery] string indegency,
             [FromQuery] string form137,
             [FromQuery] string campusName,
             [FromQuery] string schoolyearName // ✅ Newly added
 )
        {
            // ✅ Validate studentID
            if (string.IsNullOrWhiteSpace(studentID))
                return BadRequest("Student ID is required.");

            // ✅ Reject if studentID contains letters (only digits and hyphens allowed)
            if (studentID.Any(c => char.IsLetter(c)))
                return BadRequest("Student ID is not valid. Letters are not allowed.");

            // ✅ Check if student already exists
            bool exists = await _context.Studentfiles.AnyAsync(s =>
                s.StudentID == studentID);

            if (exists)
                return Conflict("This Student ID already exists.");

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
                Form137 = form137,
                CampusName = campusName,
                SchoolyearName = schoolyearName // ✅ Set here
            };

            _context.Studentfiles.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = student.StudentID }, student);
        }


        [HttpPut("update")]
        public async Task<IActionResult> UpdateStudent([FromQuery] Studentfiles updatedStudent)
        {
            if (string.IsNullOrEmpty(updatedStudent.StudentID))
            {
                return BadRequest("StudentID is required.");
            }

            var student = await _context.Studentfiles.FindAsync(updatedStudent.StudentID);
            if (student == null)
            {
                return NotFound($"Student with ID {updatedStudent.StudentID} not found.");
            }

            // 🔍 Check for duplicate Studentname or Lrn (excluding current student)
            var duplicate = await _context.Studentfiles
                .Where(s => s.StudentID != updatedStudent.StudentID &&
                       (s.Studentname == updatedStudent.Studentname || s.Lrn == updatedStudent.Lrn))
                .FirstOrDefaultAsync();

            if (duplicate != null)
            {
                if (duplicate.Studentname == updatedStudent.Studentname)
                {
                    return Conflict("A student with this name already exists.");
                }
                if (duplicate.Lrn == updatedStudent.Lrn)
                {
                    return Conflict("A student with this LRN already exists.");
                }
            }

            // ✅ Update all fields
            _context.Entry(student).CurrentValues.SetValues(updatedStudent);

            try
            {
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return StatusCode(500, "Update failed. No records were changed.");
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }

            return Ok(new { message = "Update successful." });
        }

        [HttpGet("generate-pdf")]
        public async Task<IActionResult> GenerateFilteredPdf(
                 [FromQuery] string field,
                 [FromQuery] string value,
                 [FromQuery] string schoolyear,
                 [FromQuery] string campusName)
        {
            try
            {
                // Validate the field using reflection
                var property = typeof(Studentfiles).GetProperties()
                    .FirstOrDefault(p => p.Name.Equals(field, StringComparison.OrdinalIgnoreCase));

                if (property == null)
                    return BadRequest("Invalid field name.");

                // Use the actual casing of the field name
                string actualFieldName = property.Name;

                // Filter the data using System.Linq.Dynamic.Core
                var filteredStudents = await _context.Studentfiles
                    .Where($"{actualFieldName} != null && {actualFieldName}.ToLower() == @0 && SchoolyearName.ToLower() == @1 && CampusName.ToLower() == @2",
                           value.ToLower(), schoolyear.ToLower(), campusName.ToLower())
                    .ToListAsync();

                if (filteredStudents.Count == 0)
                    return NotFound("No students found for the given filter.");

                // Generate PDF
                var pdfStream = new MemoryStream();
                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);
                        page.Size(PageSizes.A4);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        // Header
                        page.Header().Column(col =>
                        {
                            col.Item().Text($"{actualFieldName.ToUpper()} : {value.ToUpper()} STUDENTS")
                                .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                            col.Item().Text($"School Year: {schoolyear}")
                                .FontSize(14).FontColor(Colors.Grey.Darken2).Italic();

                            col.Item().Text($"Campus: {campusName}")
                                .FontSize(14).FontColor(Colors.Grey.Darken2).Italic();

                            col.Item().Text($"Total Students: {filteredStudents.Count}")
                                .FontSize(14).FontColor(Colors.Grey.Darken2).Italic();
                        });

                        // Table
                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40);    // #
                                columns.RelativeColumn();      // Student ID
                                columns.RelativeColumn();      // Student Name
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("#").SemiBold();
                                header.Cell().Text("Student ID").SemiBold();
                                header.Cell().Text("Student Name").SemiBold();
                            });

                            int index = 1;
                            foreach (var student in filteredStudents)
                            {
                                table.Cell().Text(index++.ToString());
                                table.Cell().Text(student.StudentID ?? "-");
                                table.Cell().Text(student.Studentname ?? "-");
                            }
                        });
                    });
                }).GeneratePdf(pdfStream);

                pdfStream.Position = 0;

                var filename = $"StudentList_{actualFieldName}_{value}_{schoolyear}_{campusName}.pdf";
                return File(pdfStream, "application/pdf", filename);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating PDF: {ex.Message}");
            }
        }


    }
}