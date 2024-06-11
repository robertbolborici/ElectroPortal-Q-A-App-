using Microsoft.AspNetCore.Mvc;
using ElectroPortal.ContextModel;
using ElectroPortal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace ElectroPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentationController : ControllerBase
    {
        private readonly ElectroPortalDbContext _context;
        private readonly UserManager<User> _userManager;

        public DocumentationController(UserManager<User> userManager, ElectroPortalDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }
        private async Task<bool> IsAdmin()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            return await _userManager.IsInRoleAsync(user, "Admin");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Documentation>>> GetAllDocumentation()
        {
            return await _context.Documentations.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Documentation>> GetDocumentation(int id)
        {
            var documentation = await _context.Documentations.FindAsync(id);
            if (documentation == null)
            {
                return NotFound();
            }
            return documentation;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<Documentation>> CreateDocumentation(Documentation documentation)
        {
            if (!await IsAdmin())
            {
                return Problem(
                title: "Authenticated user is not allowed.",
                detail: $"You need to be admin.",
                statusCode: StatusCodes.Status403Forbidden,
                instance: HttpContext.Request.Path
                );
            }

            _context.Documentations.Add(documentation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDocumentation), new { id = documentation.Id }, documentation);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> UpdateDocumentation(int id, [FromBody] Documentation updatedDocumentation)
        {
            if (!await IsAdmin())
            {
                return Forbid("You are not authorized to update documentation.");
            }

            var documentation = await _context.Documentations.FindAsync(id);
            if (documentation == null)
            {
                return NotFound();
            }

            documentation.Name = updatedDocumentation.Name;
            documentation.Url = updatedDocumentation.Url;

            _context.Documentations.Update(documentation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeleteDocumentation(int id)
        {
            var documentation = await _context.Documentations.FindAsync(id);
            if (documentation == null)
            {
                return NotFound();
            }

            _context.Documentations.Remove(documentation);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
