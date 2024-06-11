using ElectroPortal.ContextModel;
using ElectroPortal.Models;
using Microsoft.AspNetCore.Mvc;

namespace ElectroPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RewardsController : ControllerBase
    {
        private readonly ElectroPortalDbContext _context;

        public RewardsController(ElectroPortalDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Reward>> GetReward(int id)
        {
            var reward = await _context.Rewards.FindAsync(id);
            if (reward == null)
                return NotFound("Reward not found.");

            return Ok(reward);
        }
    }
}
