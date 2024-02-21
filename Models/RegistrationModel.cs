using System.ComponentModel.DataAnnotations;

namespace WebApi7.Models
{
    public class RegistrationModel
    {
        public RegistrationModel(string username, string email, string password)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Password = password ?? throw new ArgumentNullException(nameof(password));
        }

        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        // Optional ???
        public string? PhoneNumber { get; set; }

    }
}
