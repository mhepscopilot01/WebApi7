using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApi7.Data;

namespace WebApi7.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly DataContext _context;

        public ProfileController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves the authenticated user's profile
        /// </summary>
        /// <response code="200">Returns user profile data</response>
        /// <response code="401">If the user is unauthorized</response>
        /// <response code="400">If user ID is in an invalid format</response>
        /// <response code="404">If the user cannot be found</response>
        
        [HttpGet]
        public async Task<IActionResult> GetUserProfile()
        {
            // Extract the user ID from the JWT token claims.
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" && int.TryParse(c.Value, out _)); // This is the default claim type for user ID in JWT tokens

            // Check if the user ID claim is present
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "User ID claim not found or in incorrect format." });
            }
            // Check if the user ID is in a valid format
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "User ID claim not found or in incorrect format." });
            }
            
            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest(new { message = "Invalid user ID format." });
            }
            // Retrieve the user from the database
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Return essential user profile data
            return Ok(new
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                EmailVerified = user.EmailVerified,
                CreatedDate = user.CreatedDate,
                UpdatedDate = user.UpdateDate
            });
        }
    }
}
