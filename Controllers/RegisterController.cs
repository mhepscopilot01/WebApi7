using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi7.Data;
using WebApi7.Models;
using WebApi7.Services;

namespace WebApi7.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly DataContext _context;

        public RegisterController(IAuthService authService, DataContext context)
        {
            _authService = authService;
            _context = context;
        }

        [HttpPost] // POST /api/register
        public async Task<IActionResult> Register(RegistrationModel model)
        {
            // Validate the model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Check if the user already exists
            if (await _context.Users.AnyAsync(x => x.Username == model.Username))
            {
                // For better security, consider using a generic error message
                return BadRequest("An account with the given details already exists.");
            }
              
            if (await _context.Users.AnyAsync(x => x.Email == model.Email))
            {
                // For better security, consider using a generic error message
                return BadRequest("An account with the given details already exists.");
            }
            // Create the user
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                CreatedDate = DateTime.UtcNow
            };
            // Hash the password
            _authService.CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            // Save the user
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            // Generate a JWT token
            string token = _authService.GenerateJwtToken(user);
            var response = new RegistrationResponse { Token = token };

            return Ok(response);
        }
    }
}
