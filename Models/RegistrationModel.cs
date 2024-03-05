using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
namespace WebApi7.Models
{
    public class RegistrationModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationModel" /> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        public RegistrationModel(string username, string email, string password)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Password = password ?? throw new ArgumentNullException(nameof(password));
        }

        /// <summary>
        /// Gets or sets the username.
        /// Example: johnsmith
        /// </summary>
        [Required]
        [SwaggerSchema(Description = "Example: johnsmith")] // Swagger documentation
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// Example: john.smith@gmail.com. Must adhere to the email format.
        /// </summary>
        [Required]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s\.]+$", ErrorMessage = "Invalid Email Format")]
        [SwaggerSchema(Description = "Example: john.smith@gmail.com. Must adhere to the email format.")]

        // Email format: local-part@domain.com
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// Example: P@ssw0rd! (Minimum 8 characters, lowercase, uppercase, digit, special character)
        /// </summary>
        [Required]
        [MinLength(8)] // Minimum 8 characters
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*_=+-]).{8,}$")]
        // Require: lowercase, uppercase, digit, special character. Minimum 8 chars
        [SwaggerSchema(Description = "Example: P@ssw0rd! (Minimum 8 characters, lowercase, uppercase, digit, special character)")]

        public string Password { get; set; }

        // Optional ???
        //public string? PhoneNumber { get; set; }

    }
}