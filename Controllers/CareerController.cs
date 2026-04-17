using CareerCompass.API.Data;
using CareerCompass.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CareerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CareerController(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 Admin: Add Career
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddCareer(Career career)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Careers.Add(career);
            _context.SaveChanges();
            return Ok("Career added successfully");
        }

        // 🔹 Student & Admin: View All Careers
        [Authorize]
        [HttpGet]
        public IActionResult GetAll(int pageNumber = 1, int pageSize = 5, string? sortBy = null)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest("Invalid pagination values");

            var query = _context.Careers.AsQueryable();

            // 🔹 Sorting Logic
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "salary":
                        query = query.OrderByDescending(c => c.AverageSalary);
                        break;

                    case "title":
                        query = query.OrderBy(c => c.Title);
                        break;

                    case "category":
                        query = query.OrderBy(c => c.Category);
                        break;

                    default:
                        return BadRequest("Invalid sort parameter");
                }
            }

            var totalRecords = query.Count();

            var careers = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                Data = careers
            };

            return Ok(result);
        }

        // 🔹 Admin: Delete Career
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteCareer(int id)
        {
            var career = _context.Careers.Find(id);

            if (career == null)
                return NotFound("Career not found");

            _context.Careers.Remove(career);
            _context.SaveChanges();

            return Ok("Career deleted successfully");
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateCareer(int id, Career updatedCareer)
        {
            var career = _context.Careers.Find(id);

            if (career == null)
                return NotFound("Career not found");

            career.Title = updatedCareer.Title;
            career.Description = updatedCareer.Description;
            career.Category = updatedCareer.Category;
            career.RequiredSkills = updatedCareer.RequiredSkills;
            career.AverageSalary = updatedCareer.AverageSalary;

            _context.SaveChanges();

            return Ok("Career updated successfully");
        }
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetCareerById(int id)
        {
            var career = _context.Careers.Find(id);

            if (career == null)
                return NotFound("Career not found");

            return Ok(career);
        }
        [Authorize]
        [HttpGet("category")]
        public IActionResult GetByCategory(string category)
        {
            var careers = _context.Careers
                .Where(c => c.Category == category)
                .ToList();

            if (!careers.Any())
                return NotFound("No careers found in this category");

            return Ok(careers);
        }
        [Authorize]
        [HttpGet("salary")]
        public IActionResult FilterBySalary(decimal minSalary, decimal maxSalary)
        {
            var careers = _context.Careers
                .Where(c => c.AverageSalary >= minSalary && c.AverageSalary <= maxSalary)
                .ToList();

            if (!careers.Any())
                return NotFound("No careers found in this salary range");

            return Ok(careers);
        }
        [HttpGet("search")]
        public IActionResult SearchCareer(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest("Keyword is required");

            var careers = _context.Careers
                .Where(c =>
                    c.Title.Contains(keyword) ||
                    c.Category.Contains(keyword) ||
                    c.RequiredSkills.Contains(keyword))
                .ToList();

            if (!careers.Any())
                return NotFound("No careers found");

            return Ok(careers);
        }
        [Authorize]
        [HttpGet("recommend")]
        public IActionResult RecommendCareers(string skills)
        {
            if (string.IsNullOrEmpty(skills))
                return BadRequest("Please provide skills");

            var skillList = skills.ToLower().Split(",");

            var careers = _context.Careers
                .Where(c => skillList.Any(skill =>
                    c.RequiredSkills.ToLower().Contains(skill.Trim())))
                .ToList();

            return Ok(careers);
        }
        [Authorize]
        [HttpPost("save/{careerId}")]
        public IActionResult SaveCareer(int careerId)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);

            // Check if career exists
            var careerExists = _context.Careers.Any(c => c.Id == careerId);

            if (!careerExists)
                return NotFound("Career not found");

            // Check duplicate
            var existing = _context.SavedCareers
                .FirstOrDefault(sc => sc.UserId == userId && sc.CareerId == careerId);

            if (existing != null)
                return BadRequest("Career already saved");

            var savedCareer = new SavedCareer
            {
                UserId = userId,
                CareerId = careerId
            };

            _context.SavedCareers.Add(savedCareer);
            _context.SaveChanges();

            return Ok("Career saved successfully");
        }
        [Authorize]
        [HttpDelete("unsave/{careerId}")]
        public IActionResult UnsaveCareer(int careerId)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);

            var savedCareer = _context.SavedCareers
                .FirstOrDefault(sc => sc.UserId == userId && sc.CareerId == careerId);

            if (savedCareer == null)
                return NotFound("Saved career not found");

            _context.SavedCareers.Remove(savedCareer);
            _context.SaveChanges();

            return Ok("Career removed successfully");
        }

        [Authorize]
        [HttpGet("saved")]
        public IActionResult GetSavedCareers()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var savedCareers = _context.SavedCareers
                .Where(sc => sc.UserId == userId)
                .Select(sc => new
                {
                    sc.Career!.Id,
                    sc.Career.Title,
                    sc.Career.Description,
                    sc.Career.Category,
                    sc.Career.RequiredSkills
                })
                .ToList();

            return Ok(savedCareers);
        }
    }
}