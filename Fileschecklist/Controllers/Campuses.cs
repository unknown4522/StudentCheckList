using Fileschecklist.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Fileschecklist.Models.Checklistmodel;

namespace Fileschecklist.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Campuses : ControllerBase
    {
        private readonly AppDbContext _context;

        public Campuses(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/campuses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Campus>>> GetCampuses()
        {
            var Campus = await _context.Campuses.ToListAsync();

            if (Campus == null || Campus.Count == 0)
            {
                return NotFound();
            }

            return Ok(Campus);
        }
    }
}
