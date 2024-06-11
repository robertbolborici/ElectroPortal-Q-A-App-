using ElectroPortal.ContextModel;
using ElectroPortal.DTOs;
using ElectroPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ElectroPortal.NewFolder;
using System.Text.Encodings.Web;
using System.Web;

namespace ElectroPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ElectroPortalDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;

        public UsersController(UserManager<User> userManager, ElectroPortalDbContext context, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IEmailSender emailSender)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }   

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetUser(string id)
        {
            var user = await _userManager.Users
                .Select(u => new {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.Country,
                    u.IsFirstNameVisible,
                    u.IsLastNameVisible,
                    u.IsCountryVisible
                })
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("IsAdmin")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> IsAdmin()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized("User is not authenticated.");
            }

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            return Ok(new { IsAdmin = isAdmin });
        }

        [HttpGet("ConfirmEmail", Name = "ConfirmEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Redirect($"http://localhost:4200/login?emailConfirmation=NotFound");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return Redirect($"http://localhost:4200/login?emailConfirmation=Success");
            }
            else
            {
                return Redirect($"http://localhost:4200/login?emailConfirmation=Failure");
            }
        }

        [HttpPost]
        public async Task<ActionResult> PostUser(CreateUserDto createUserDto)
        {
            var user = new User
            {
                UserName = createUserDto.Username,
                Email = createUserDto.Email,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Country = createUserDto.Country,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, createUserDto.Password);
            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action(
                    "ConfirmEmail",
                    "Users",
                    new { userId = user.Id, code = code },
                    protocol: HttpContext.Request.Scheme);

                if (string.IsNullOrEmpty(callbackUrl))
                {
                    return Problem("Failed to generate email confirmation link.");
                }

                var encodedUrl = HtmlEncoder.Default.Encode(callbackUrl);
                await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{encodedUrl}'>clicking here</a>.");

                return Ok(new { message = "Registration successful. Please check your email to confirm your account." });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            if (!user.EmailConfirmed)
            {
                return Unauthorized(new { message = "You must confirm your email before you can log in." });
            }

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token, Username = user.UserName });
        }

        [HttpPatch("settings")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> UpdateUserSettings(UpdateUserSettingsDto settingsDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (settingsDto.IsFirstNameVisible.HasValue)
                user.IsFirstNameVisible = settingsDto.IsFirstNameVisible.Value;
            if (settingsDto.IsLastNameVisible.HasValue)
                user.IsLastNameVisible = settingsDto.IsLastNameVisible.Value;
            if (settingsDto.IsCountryVisible.HasValue)
                user.IsCountryVisible = settingsDto.IsCountryVisible.Value;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(result.Errors);

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
