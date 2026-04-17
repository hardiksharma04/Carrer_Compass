using CareerCompass.API.Data;
using CareerCompass.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerCompass.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student")]
    public class FavoriteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FavoriteController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost("{careerId}")]
        public IActionResult AddToFavorite(int careerId, int studentId)
        {
            // Check if career exists
            var career = _context.Careers.Find(careerId);
            if (career == null)
                return NotFound("Career not found");

            // Check if already added
            var alreadyExists = _context.FavoriteCareers
                .Any(f => f.CareerId == careerId && f.StudentId == studentId);

            if (alreadyExists)
                return BadRequest("Career already added to favorites");

            var favorite = new FavoriteCareer
            {
                CareerId = careerId,
                StudentId = studentId
            };

            _context.FavoriteCareers.Add(favorite);
            _context.SaveChanges();

            return Ok("Career added to favorites successfully");
        }
[HttpGet("student/{studentId}")]
    public IActionResult GetStudentFavorites(int studentId)
    {
        var favorites = _context.FavoriteCareers
            .Include(f => f.Career)
            .Where(f => f.StudentId == studentId)
            .Select(f => f.Career)
            .Where(c => c != null)
            .Select(c => new
            {
                c!.Id,
                c.Title,
                c.Description,
                c.Category,
                c.RequiredSkills,
                c.AverageSalary
            })
            .ToList();

        if (!favorites.Any())
            return NotFound("No favorites found for this student");

        return Ok(favorites);
    }
        [HttpDelete]
        public IActionResult RemoveFromFavorite(int careerId, int studentId)
        {
            var favorite = _context.FavoriteCareers
                .FirstOrDefault(f => f.CareerId == careerId && f.StudentId == studentId);

            if (favorite == null)
                return NotFound("Favorite record not found");

            _context.FavoriteCareers.Remove(favorite);
            _context.SaveChanges();

            return Ok("Career removed from favorites successfully");
        }
    }
}