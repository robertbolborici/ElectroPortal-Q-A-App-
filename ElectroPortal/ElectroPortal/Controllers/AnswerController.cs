using ElectroPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElectroPortal.ContextModel;
using ElectroPortal.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ElectroPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswersController : ControllerBase
    {
        private readonly ElectroPortalDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ProfanityBase _profanityFilter = new ProfanityBase();

        public AnswersController(UserManager<User> userManager, ElectroPortalDbContext context)
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

        private bool AnswerExists(int id) => _context.Answers.Any(e => e.Id == id);

        [HttpGet("{id}")]
        public async Task<ActionResult<Answer>> GetAnswer(int id)
        {
            var answer = await _context.Answers.FindAsync(id);

            if (answer == null)
            {
                return NotFound();
            }

            return answer;
        }

        [HttpGet("ByQuestion/{questionId}")]
        public async Task<ActionResult<IEnumerable<AnswerWithVotesDto>>> GetAnswersByQuestionId(int questionId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var answers = await _context.Answers
                .Where(a => a.QuestionId == questionId)
                .Include(a => a.User)
                .Include(a => a.Votes)
                .ToListAsync();

            var answersWithVotes = answers.Select(a => new AnswerWithVotesDto
            {
                Id = a.Id,
                Content = a.Content,
                CreatedAt = a.CreatedAt,
                UserId = a.UserId,
                QuestionId = a.QuestionId,
                UserName = a.User.UserName,
                UpVotes = a.Votes.Count(v => v.Upvote),
                DownVotes = a.Votes.Count(v => !v.Upvote),
                UserVote = a.Votes.Any(v => v.UserId == currentUserId) ?
                            (a.Votes.First(v => v.UserId == currentUserId).Upvote ? 1 : -1) : 0
            }).ToList();

            return Ok(answersWithVotes);
        }

        [HttpGet("ByUser/{userId}")]
        public async Task<ActionResult<IEnumerable<Answer>>> GetAnswersByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid user ID");
            }

            var answers = await _context.Answers
                .Where(a => a.UserId == userId)
                .ToListAsync();

            if (!answers.Any())
            {
                return NoContent();
            }

            return Ok(answers);
        }

        [HttpGet("ResponsesToUserQuestions/{userId}")]
        public async Task<ActionResult<IEnumerable<AnswerWithVotesDto>>> GetResponsesToUserQuestions(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var questionsByUser = _context.Questions.Where(q => q.UserId == userId).Select(q => q.Id).ToList();
            var responses = await _context.Answers
                .Where(a => questionsByUser.Contains(a.QuestionId) && a.UserId != userId)
                .Include(a => a.User)
                .Include(a => a.Votes)
                .ToListAsync();

            var answersWithVotes = responses.Select(a => new AnswerWithVotesDto
            {
                Id = a.Id,
                Content = a.Content,
                CreatedAt = a.CreatedAt,
                UserId = a.UserId,
                QuestionId = a.QuestionId,
                UserName = a.User.UserName,
                UpVotes = a.Votes.Count(v => v.Upvote),
                DownVotes = a.Votes.Count(v => !v.Upvote),
                UserVote = a.Votes.FirstOrDefault(v => v.UserId == currentUserId) == null ? 0 :
                            a.Votes.First(v => v.UserId == currentUserId).Upvote ? 1 : -1
            }).ToList();

            return Ok(answersWithVotes);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<Answer>> PostAnswer([FromBody] CreateAnswerDto answerDto)
        {
            if (_profanityFilter.ContainsProfanity(answerDto.Content))
            {
                return BadRequest("Your answer contains inappropriate language.");
            }

            var answer = new Answer
            {
                Content = answerDto.Content,
                CreatedAt = answerDto.CreatedAt,
                UserId = answerDto.UserId,
                QuestionId = answerDto.QuestionId
            };

            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAnswer), new { id = answer.Id }, answer);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> PutAnswer(int id, AnswerUpdateDto updateAnswerDto)
        {
            if (_profanityFilter.ContainsProfanity(updateAnswerDto.Content))
            {
                return BadRequest("Your answer contains inappropriate language.");
            }

            var answer = await _context.Answers.FindAsync(id);
            if (answer == null)
            {
                return NotFound();
            }

            if (!(await IsOwnerOrAdmin(answer.UserId.ToString())))
            {
                return Problem(
                title: "Authenticated user is not allowed.",
                detail: $"You don't have permission to perform this action. Only the owner of the resource or an admin have the ability" +
                $"to perform this.",
                statusCode: StatusCodes.Status403Forbidden,
                instance: HttpContext.Request.Path
                );
            }

            answer.Content = updateAnswerDto.Content;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnswerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeleteAnswer(int id)
        {
            var answer = await _context.Answers.FindAsync(id);
            if (answer == null)
            {
                return NotFound();
            }

            if (!(await IsOwnerOrAdmin(answer.UserId.ToString())))
            {
                return Problem(
                title: "Authenticated user is not allowed.",
                detail: $"You don't have permission to perform this action. Only the owner of the resource or an admin have the ability" +
                $"to perform this.",
                statusCode: StatusCodes.Status403Forbidden,
                instance: HttpContext.Request.Path
                );
            }

            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
