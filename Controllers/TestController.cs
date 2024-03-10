using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi7.Data;

// Just Azure SQL Database connection test

namespace WebApi7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly DataContext _context;

        public TestController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("connection")] // api/test/connection
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                // Verify the connection
                await _context.Users.CountAsync();
                return Ok("Connection successful!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Connection failed: {ex.Message}");
            }
        }
    }
}
