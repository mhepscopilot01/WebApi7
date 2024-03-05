using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApi7.Data;
using WebApi7.Models;

namespace WebApi7.Services
{
    public interface IAuthService
    {
        Task<bool> Register(User user, string password);
        Task<User> Login(string username, string password);
        string GenerateJwtToken(User user);
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    }

    public class AuthService : IAuthService // My interface methods
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration; // For our JWT configuration

        public AuthService(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Methods: Login, Register, CreatePasswordHash, VerifyPasswordHash

        public Task<User> Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        //
        public string GenerateJwtToken(User user)
        {
            // JWT Claims - Standard information and custom data
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // "jti" - unique token identifier
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            // Fetch JWT configuration settings
            var keyString = _configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(keyString))
            {
                throw new Exception("JWT Key is missing in configuration. Check appsettings.json");
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"]));

            // Create the JWT token itself
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> Register(User user, string password)
        {
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return true;
        }

        ///The VerifyPasswordHash method below:

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)

        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        // Password hashing helper 
        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}

