using Fileschecklist.Data;
using Fileschecklist.Models; // ✅ Replace with your actual namespace for the model
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fileschecklist.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolyearController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SchoolyearController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Schoolyear>>> GetAll()
        {
            var schoolyears = await _context.Schoolyears.ToListAsync();
            return Ok(schoolyears);
        }
        // Example: GET /api/schoolyear/by-campus?campusName=Cebu Main
        [HttpGet("by-campus")]
        public async Task<ActionResult<IEnumerable<Schoolyear>>> GetByCampusName([FromQuery] string campusName)
        {
            if (string.IsNullOrWhiteSpace(campusName))
                return BadRequest("Campus name is required.");

            var result = await _context.Schoolyears
                .Where(sy => sy.CampusName == campusName)
                .ToListAsync();

            if (result.Count == 0)
                return NotFound($"No school years found for campus '{campusName}'");

            return Ok(result);
        }


        // ✅ GET by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Schoolyear>> GetById(int id)
        {
            var schoolyear = await _context.Schoolyears.FindAsync(id);

            if (schoolyear == null)
                return NotFound();

            return schoolyear;
        }

        [HttpPost("AddSchoolyear")]
        public async Task<ActionResult<Schoolyear>> Create([FromQuery] string schoolyearName, [FromQuery] string campusName)
        {
            if (string.IsNullOrWhiteSpace(schoolyearName))
                return BadRequest(new { message = "SchoolyearName is required." });

            if (string.IsNullOrWhiteSpace(campusName))
                return BadRequest(new { message = "CampusName is required." });

            // Check if same schoolyear name already exists for the same campus
            bool sameNameAndCampus = await _context.Schoolyears
                .AnyAsync(s => s.SchoolyearName == schoolyearName && s.CampusName == campusName);

            if (sameNameAndCampus)
                return BadRequest(new { message = "sigeg balik eyy Mana na eyy" });

            // Optional: check if the name already exists in a different campus (just for info/logging)
            bool sameNameDifferentCampus = await _context.Schoolyears
                .AnyAsync(s => s.SchoolyearName == schoolyearName && s.CampusName != campusName);

            if (sameNameDifferentCampus)
            {
                // You could log this or add a warning message if needed.
                // But allow it to proceed.
            }

            var schoolyear = new Schoolyear
            {
                SchoolyearName = schoolyearName,
                CampusName = campusName
            };

            _context.Schoolyears.Add(schoolyear);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = schoolyear.SchoolyearID }, schoolyear);
        }


    }
}
