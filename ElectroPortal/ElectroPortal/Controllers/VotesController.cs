using ElectroPortal.ContextModel;
using ElectroPortal.DTOs;
using ElectroPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ElectroPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotesController : ControllerBase
    {
        private readonly ElectroPortalDbContext _context;
        private readonly UserManager<User> _userManager;

        public VotesController(UserManager<User> userManager, ElectroPortalDbContext context)
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

        private bool VoteExists(int id)
        {
            return _context.Votes.Any(e => e.Id == id);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Vote>> GetVote(int id)
        {
            var vote = await _context.Votes
                .Include(v => v.User)
                .Include(v => v.Answer)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vote == null)
            {
                return NotFound("Vote not found.");
            }

            return Ok(vote);
        }

        [HttpGet("ByUserId")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<IEnumerable<Vote>>> GetVotesByUserId(string userId)
        {
            var votes = await _context.Votes
                .Include(v => v.Answer)
                .Where(v => v.Answer.UserId == userId)
                .ToListAsync();

            return Ok(votes);
        }

        [HttpGet("Leaderboard")]
        public async Task<ActionResult<IEnumerable<UserScoreDto>>> GetLeaderboard()
        {
            var today = DateTime.UtcNow;
            var daysSinceMonday = (int)today.DayOfWeek - (int)DayOfWeek.Monday;
            if (daysSinceMonday < 0)
            {
                daysSinceMonday += 7;
            }
            var mondayDate = today.AddDays(-daysSinceMonday).Date;

            var lastRewardedConfig = await _context.SystemConfigurations
                                                   .FirstOrDefaultAsync(c => c.Key == "LastRewardedDate");

            DateTime lastRewardedDate;
            if (lastRewardedConfig != null && DateTime.TryParse(lastRewardedConfig.Value, out lastRewardedDate))
            {
                lastRewardedDate = DateTime.SpecifyKind(lastRewardedDate, DateTimeKind.Utc);
            }
            else
            {
                lastRewardedDate = DateTime.MinValue;
            }

            var userScores = await _context.Answers
                .Include(a => a.Votes)
                .Include(a => a.User)
                .SelectMany(a => a.Votes.Where(v => v.Date >= mondayDate).Select(v => new { a.UserId, a.User.UserName, v.Upvote }))
                .GroupBy(v => new { v.UserId, v.UserName })
                .Select(group => new
                {
                    UserId = group.Key.UserId,
                    UserName = group.Key.UserName,
                    Score = group.Sum(v => v.Upvote ? 10 : -10)
                })
                .ToListAsync();

            var adjustedScores = userScores.Select(us => new UserScoreDto
            {
                UserId = us.UserId,
                UserName = us.UserName,
                Score = Math.Max(0, us.Score)
            })
            .OrderByDescending(us => us.Score)
            .ToList();

            if (today.DayOfWeek == DayOfWeek.Monday && today.Date > lastRewardedDate.Date)
            {

                if (adjustedScores.Any())
                {
                    var topUser = adjustedScores.First();
                    var reward = await _context.Rewards.FirstOrDefaultAsync(r => !r.IsUsed);
                    if (reward != null)
                    {
                        var userReward = new UserReward
                        {
                            UserId = topUser.UserId,
                            RewardId = reward.Id
                        };

                        _context.UserRewards.Add(userReward);
                        reward.IsUsed = true;

                        if (lastRewardedConfig != null)
                        {
                            lastRewardedConfig.Value = today.ToString("o");
                        }
                        else
                        {
                            _context.SystemConfigurations.Add(new SystemConfiguration { Key = "LastRewardedDate", Value = today.ToString("o"), LastModified = today });
                        }

                        await _context.SaveChangesAsync();
                    }
                }
            }

            return Ok(adjustedScores);
        }


        [HttpGet("Score/{userId}")]
        public async Task<ActionResult<UserScoreDto>> GetScore(string userId)
        {
            var userScore = await _context.Answers
                .Include(a => a.Votes)
                .Include(a => a.User)
                .Where(a => a.UserId == userId)
                .GroupBy(a => new { a.UserId, a.User.UserName })
                .Select(group => new
                {
                    group.Key.UserName,
                    Score = group.SelectMany(g => g.Votes).Sum(v => v.Upvote ? 10 : -10)
                })
                .FirstOrDefaultAsync();

            if (userScore == null)
            {
                return NotFound($"No score found for user with ID {userId}");
            }

            var adjustedScore = new UserScoreDto
            {
                UserName = userScore.UserName,
                Score = Math.Max(0, userScore.Score)
            };

            return Ok(adjustedScore);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> PostVote(VoteDto voteDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Vote vote = await _context.Votes.FirstOrDefaultAsync(v => v.UserId == userId && v.AnswerId == voteDto.AnswerId);

            if (vote != null)
            {
                if (vote.Upvote == voteDto.Upvote)
                {
                    _context.Votes.Remove(vote);
                    await _context.SaveChangesAsync();
                    return Ok(new { Message = "Vote removed", VoteRemoved = true });
                }
                else
                {
                    vote.Upvote = voteDto.Upvote;
                }
            }
            else
            {
                vote = new Vote
                {
                    UserId = userId,
                    AnswerId = voteDto.AnswerId,
                    Upvote = voteDto.Upvote,
                    Date = DateTime.UtcNow
                };
                _context.Votes.Add(vote);
            }

            await _context.SaveChangesAsync();
            return Ok(vote);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> PutVote(int id, VoteUpdateDto voteUpdateDto)
        {
            var vote = await _context.Votes.FindAsync(id);
            if (vote == null)
            {
                return NotFound();
            }

            if (!(await IsOwnerOrAdmin(vote.UserId.ToString())))
            {
                {
                    return Problem(
                    title: "Authenticated user is not allowed.",
                    detail: $"You don't have permission to perform this action. Only the owner of the resource or an admin have the ability" + 
                    $"to perform this.",
                    statusCode: StatusCodes.Status403Forbidden,
                    instance: HttpContext.Request.Path
                );
                }
            }

            vote.Upvote = voteUpdateDto.Upvote;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VoteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeleteVote(int id)
        {
            var vote = await _context.Votes.FindAsync(id);
            if (vote == null)
            {
                return NotFound();
            }

            if (!(await IsOwnerOrAdmin(vote.UserId.ToString())))
            {
                return Problem(
                title: "Authenticated user is not allowed.",
                detail: $"You don't have permission to perform this action. Only the owner of the resource or an admin have the ability" +
                $"to perform this.",
                statusCode: StatusCodes.Status403Forbidden,
                instance: HttpContext.Request.Path
                );
            }

            _context.Votes.Remove(vote);
            await _context.SaveChangesAsync();

            return NoContent();
        }   
    }
}
