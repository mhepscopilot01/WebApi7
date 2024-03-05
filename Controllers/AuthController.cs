using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; // ControllerBase icin gerekli
using Microsoft.EntityFrameworkCore;
using WebApi7.Data;
using WebApi7.Models;
using WebApi7.Services;

namespace WebApi7.Controllers
{
    // The AuthController class
    // localhost:5000/api/authcontroller

    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
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
            // Input Validation !!!
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check for existing user 
            if (await _context.Users.AnyAsync(x => x.Username == model.Username))
            {
                return BadRequest($"Username already exists : {model.Username}"); // KALDIR. {model.Username} sadece developer icin, kullaniciya gosterilmez.
            }

            // Check for existing email
            if (await _context.Users.AnyAsync(x => x.Email == model.Email))
            {
                return BadRequest($"Email already exists : {model.Email}"); // KALDIR. {model.Email} sadece developer icin, kullaniciya gosterilmez.
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

            // Generate JWT and return it
            string token = _authService.GenerateJwtToken(user);

            // Construct a Response Object and return it
            var response = new RegistrationResponse { Token = token };

            // Return the Response Object
            return Ok(response);
        }

        // The Login action method
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            // Input Validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify user in the database 
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username.ToLower() == model.Username.ToLower());
            if (user == null || !_authService.VerifyPasswordHash(model.Password, user.PasswordHash, user.PasswordSalt))
            {
                return Unauthorized("Invalid username or password");
            }

            // User found and password valid - generate JWT
            string token = _authService.GenerateJwtToken(user);
            return Ok(new { token });
        }

        // The GetUserProfile action method
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            // 1. Get User ID from JWT
            // "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" is the default claim type for user ID
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" && int.TryParse(c.Value, out _));

            if (userIdClaim == null)
            {
                Console.WriteLine("User ID claim not found or in incorrect format.");
                return Unauthorized();
            }

            string userId = userIdClaim.Value;

            // <summary>
            // Console.WriteLine($"Converted userIdClaim: {userIdClaim}"); // Debugging
            // Console.WriteLine($"Extracted userId1 (string): {userId}"); // Debugging
            // Console.WriteLine($"------------------------------------------------"); // Debugging

            // Debug aninda tum Claims Type ve Value doner.
            // Console.WriteLine("User claims:");
            // foreach (var claim in User.Claims)
            // {
            //     Console.WriteLine($"   - Type: {claim.Type}, Value: {claim.Value}");
            // }
            // /</summary>

            if (userId == null)
            {
                return Unauthorized();
            }

            // 2. Fetch User with Error Handling
            // Convert userId to int
            if (!int.TryParse(userId, out int userIdInt))
            {
                return BadRequest("Invalid user ID format");
            }
            // Fetch user from database
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userIdInt);
            if (user == null)
            {
                return NotFound();
            }

            // Console.WriteLine($"Converted userId3 (int): {userIdInt}"); // Debugging
            // Console.WriteLine($"Extracted userId3 (string): {userId}"); // Debugging

            // 3. Construct Response (Unchanged)
            var response = new UserProfileResponse // Construct a Response Object
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                EmailVerified = user.EmailVerified,
                CreatedDate = user.CreatedDate,
                UpdatedDate = user.UpdateDate,
                Token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "") // Extract token from request header
            };

            return Ok(response);
        }
    }
}