using ElectroPortal.ContextModel;
using ElectroPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRewardsController : ControllerBase
    {
        private readonly ElectroPortalDbContext _context;

        public UserRewardsController(ElectroPortalDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AwardReward([FromBody] UserReward userReward)
        {
            _context.UserRewards.Add(userReward);
            await _context.SaveChangesAsync();
            return Ok(userReward);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserRewards(string userId)
        {
            var userRewards = await _context.UserRewards
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            var rewards = await _context.Rewards
                .Where(r => userRewards.Select(ur => ur.RewardId).Contains(r.Id))
                .ToListAsync();

            var detailedRewards = userRewards.Join(rewards, ur => ur.RewardId, r => r.Id, (ur, r) => new
            {
                r.Name,
                r.Description,
                r.IsUsed,
                ur.Id,
                ur.UserId,
                ur.RewardId
            });

            return Ok(detailedRewards);
        }
    }
}
