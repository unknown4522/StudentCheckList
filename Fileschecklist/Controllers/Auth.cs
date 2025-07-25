using Fileschecklist.Data;
using Fileschecklist.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fileschecklist.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Auth : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public Auth(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromQuery] string username, [FromQuery] string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            if (user == null)
            {
                return Unauthorized(new { message = "Login failed" }); 
            }

            // ✅ Create the token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing from configuration."));


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role ?? string.Empty),
            new Claim("UserId", user.Id.ToString())
        }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // ✅ Return the token with user data
            return Ok(new
            {
                message = "Login successful",
                token = tokenString,
                data = new
                {
                    user.Id,
                    user.Username,
                    user.FirstName,
                    user.LastName,
                    user.Role,
                    user.Profilepic
                }
            });
        }

        [HttpPost("upload-profilepic")]
        [ApiExplorerSettings(IgnoreApi = true)] // Hides from Swagger UI
        public async Task<IActionResult> UploadProfilePic([FromForm] int userId, [FromForm] IFormFile file)
        {
            // Validate file input
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded." });
            }
            // Validate user
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Optional: Limit file size (e.g., max 2MB)
            if (file.Length > 2 * 1024 * 1024)
            {
                return BadRequest(new { message = "File is too large. Max size is 2MB." });
            }

            // Optional: Validate image type
            var permittedTypes = new[] { "image/jpeg", "image/png", "image/jpg" };
            if (!permittedTypes.Contains(file.ContentType))
            {
                return BadRequest(new { message = "Only JPEG and PNG images are allowed." });
            }

            // Read file and convert to base64
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var imageBytes = memoryStream.ToArray();
            var base64String = Convert.ToBase64String(imageBytes);

            // Save to database
            user.Profilepic = base64String;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Profile picture uploaded successfully.",
                profilepic = base64String
            });
        }

    }
}
