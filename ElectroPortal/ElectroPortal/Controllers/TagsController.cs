using ElectroPortal.ContextModel;
using ElectroPortal.DTOs;
using ElectroPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ElectroPortalDbContext _context;

        public TagsController(ElectroPortalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTags()
        {
            return await _context.Tags.ToListAsync();
        }

        [HttpGet("question/{questionId}")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTagsForQuestion(int questionId)
        {
            var question = await _context.Questions
                .Include(q => q.QuestionTags)
                .ThenInclude(qt => qt.Tag)
                .FirstOrDefaultAsync(q => q.Id == questionId);

            if (question == null)
            {
                return NotFound();
            }

            var tags = question.QuestionTags.Select(qt => qt.Tag);

            return Ok(tags);
        }

        [HttpGet("ByUser/{userId}")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTagsByUserId(string userId)
        {
            var userTags = await _context.Questions
                .Where(q => q.UserId == userId)
                .SelectMany(q => q.QuestionTags)
                .Select(qt => qt.Tag)
                .Distinct()
                .ToListAsync();

            return Ok(userTags);
        }

        [HttpGet("ByCategory/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTagsByCategory(int categoryId)
        {
            var tags = await _context.Tags
                .Where(t => t.CategoryId == categoryId)
                .ToListAsync();

            if (!tags.Any())
            {
                return NotFound("No tags found for the specified category.");
            }

            return Ok(tags);
        }

        private bool TagExists(int id)
        {
            return _context.Tags.Any(e => e.Id == id);
        }
    }
}
