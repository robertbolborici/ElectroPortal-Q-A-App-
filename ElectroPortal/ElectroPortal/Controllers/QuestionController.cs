using Microsoft.AspNetCore.Mvc;
using ElectroPortal.ContextModel;
using ElectroPortal.Models;
using Microsoft.EntityFrameworkCore;
using ElectroPortal.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace ElectroPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly ElectroPortalDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ProfanityBase _profanityFilter = new ProfanityBase();

        public QuestionsController(UserManager<User> userManager, ElectroPortalDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        private async Task<bool> IsOwnerOrAdmin(string userId)
        {
            var currentUserId = GetCurrentUserId();
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            return currentUserId == userId || isAdmin;
        }

        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestions()
        {
            var questions = await _context.Questions
                .Include(q => q.User)
                .Include(q => q.QuestionTags).ThenInclude(qt => qt.Tag)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var questionDtos = questions.Select(q => new QuestionDto
            {
                Id = q.Id,
                Title = q.Title,
                Content = q.Content,
                CreatedAt = q.CreatedAt,
                UserId = q.UserId,
                UserName = q.User?.UserName,
                CategoryId = q.CategoryId,
                QuestionTags = q.QuestionTags.Select(qt => new TagDto
                {
                    Id = qt.Tag.Id,
                    Name = qt.Tag.Name,
                    Description = qt.Tag.Description
                }).ToList()
            }).ToList();

            return Ok(questionDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Question>> GetQuestion(int id)
        {
            var question = await _context.Questions.FindAsync(id);

            if (question == null)
            {
                return NotFound();
            }

            return question;
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> SearchQuestions(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query is required.");
            }

            var searchResults = await _context.Questions
                .Where(q => q.Title.Contains(query) || q.Content.Contains(query))
                .Include(q => q.User)
                .Include(q => q.QuestionTags).ThenInclude(qt => qt.Tag)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var questionDtos = searchResults.Select(q => new QuestionDto
            {
                Id = q.Id,
                Title = q.Title,
                Content = q.Content,
                CreatedAt = q.CreatedAt,
                UserId = q.UserId,
                UserName = q.User?.UserName,
                CategoryId = q.CategoryId,
                QuestionTags = q.QuestionTags.Select(qt => new TagDto
                {
                    Id = qt.Tag.Id,
                    Name = qt.Tag.Name,
                    Description = qt.Tag.Description
                }).ToList()
            }).ToList();

            return Ok(questionDtos);
        }

        [HttpGet("byTag/{tagId}")]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestionsByTag(int tagId)
        {
            var questions = await _context.Questions
                .Where(q => q.QuestionTags.Any(qt => qt.TagId == tagId))
                .Include(q => q.User)
                .Include(q => q.QuestionTags).ThenInclude(qt => qt.Tag)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var questionDtos = questions.Select(q => new QuestionDto
            {
                Id = q.Id,
                Title = q.Title,
                Content = q.Content,
                CreatedAt = q.CreatedAt,
                UserId = q.UserId,
                UserName = q.User?.UserName,
                CategoryId = q.CategoryId,
                QuestionTags = q.QuestionTags.Select(qt => new TagDto
                {
                    Id = qt.Tag.Id,
                    Name = qt.Tag.Name,
                    Description = qt.Tag.Description
                }).ToList()
            }).ToList();

            return Ok(questionDtos);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Question>>> GetUserQuestions(string userId)
        {
            var questions = await _context.Questions
                                          .Where(q => q.UserId == userId)
                                          .Include(q => q.Answers)
                                          .Include(q => q.QuestionTags).ThenInclude(qt => qt.Tag)
                                          .ToListAsync();

            if (!questions.Any())
            {
                return NotFound("No questions found for the specified user.");
            }

            return Ok(questions);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<Question>> PostQuestion(CreateQuestionDto questionDto)
        {
            if (_profanityFilter.ContainsProfanity(questionDto.Title) || _profanityFilter.ContainsProfanity(questionDto.Content))
            {
                return BadRequest("Your question contains inappropriate language.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Unable to determine user ID from token.");
            }

            var question = new Question
            {
                Title = questionDto.Title,
                Content = questionDto.Content,
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                CategoryId = questionDto.CategoryId,
                QuestionTags = questionDto.TagIds.Select(tagId => new QuestionTag { TagId = tagId }).ToList()
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, question);
        }


        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> UpdateQuestion(int id, [FromBody] QuestionUpdateDto questionUpdateDto)
        {
            if (_profanityFilter.ContainsProfanity(questionUpdateDto.Title) || _profanityFilter.ContainsProfanity(questionUpdateDto.Content))
            {
                return BadRequest("Your question contains inappropriate language.");
            }

            var question = await _context.Questions
                .Include(q => q.QuestionTags)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            if (!IsOwnerOrAdmin(question.UserId).Result)
            {
                return Forbid("You do not have permission to update this question.");
            }

            question.Title = questionUpdateDto.Title;
            question.Content = questionUpdateDto.Content;
            question.CategoryId = questionUpdateDto.CategoryId;

            var currentTagIds = question.QuestionTags.Select(qt => qt.TagId).ToList();
            var newTagIds = questionUpdateDto.TagIds.Except(currentTagIds).ToList();
            var removedTagIds = currentTagIds.Except(questionUpdateDto.TagIds).ToList();

            foreach (var tagId in removedTagIds)
            {
                var tagToRemove = question.QuestionTags.FirstOrDefault(qt => qt.TagId == tagId);
                if (tagToRemove != null)
                {
                    _context.QuestionTags.Remove(tagToRemove);
                }
            }

            foreach (var tagId in newTagIds)
            {
                var tagToAdd = new QuestionTag { QuestionId = id, TagId = tagId };
                _context.QuestionTags.Add(tagToAdd);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var question = await _context.Questions.FindAsync(id);
                    if (question == null)
                    {
                        return NotFound();
                    }

                    var answers = _context.Answers.Where(a => a.QuestionId == id);
                    _context.Answers.RemoveRange(answers);

                    var questionTags = _context.QuestionTags.Where(qt => qt.QuestionId == id);
                    _context.QuestionTags.RemoveRange(questionTags);

                    _context.Questions.Remove(question);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, "An error occurred while deleting the question.");
                }
            }

            return NoContent();
        }
    }
}
