using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi7.Data;
using WebApi7.Models;
using WebApi7.Services;

namespace WebApi7.Controllers
{
    // The AuthController class
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // The AuthService and DataContext are injected into the controller
        private readonly IAuthService _authService;
        private readonly DataContext _context;

        // The constructor
        public AuthController(IAuthService authService, DataContext context)
        {
            _authService = authService;
            _context = context;
        }

        // The Register action method
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistrationModel model)
        {
            // Input Validation (Important!)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check for existing user 
            if (await _context.Users.AnyAsync(x => x.Username == model.Username))
            {
                return BadRequest("Username already exists");
            }

            // Create the new User entity
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                CreatedDate = DateTime.UtcNow
            };

            // Hash the password (using AuthService)
            _authService.CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            // Save the user 
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate JWT
            string token = _authService.GenerateJwtToken(user);

            // Construct a Response Object 
            var response = new RegistrationResponse { Token = token };

            // Return the Response Object
            return Ok(response);
        }

        // The Login action method
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            // Input Validation (Important!)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Returns validation errors
            }

            // Verify user in the database
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username.ToLower() == model.Username.ToLower());
            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            // Verify password against the stored hash

            if (!_authService.VerifyPasswordHash(model.Password, user.PasswordHash, user.PasswordSalt))
            {
                return Unauthorized("Invalid username or password");
            }

            // User found and password valid - generate JWT
            string token = _authService.GenerateJwtToken(user);
            return Ok(new { token });
        }
    }
}
