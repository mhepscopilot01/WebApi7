using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi7.Data;
using WebApi7.Models;
using WebApi7.Services;

// Login controller.

namespace WebApi7.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase 
    {
        private readonly IAuthService _authService;
        private readonly DataContext _context;

        public LoginController(IAuthService authService, DataContext context)
        {
            _authService = authService;
            _context = context;
        }

        [HttpPost] // POST: /api/login
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username.ToLower() == model.Username.ToLower()); 
            if (user == null || !_authService.VerifyPasswordHash(model.Password, user.PasswordHash, user.PasswordSalt))
            {
                // To enhance security, consider using a more generic error message
                return Unauthorized("Invalid credentials. Please try again.");
            }

            string token = _authService.GenerateJwtToken(user); // Generate JWT token
            return Ok(new { token });
        }
    }
}