using System;
using System.ComponentModel.DataAnnotations;

// This snippet to Models/User.cs

namespace WebApi7.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required] 
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        public bool EmailVerified { get; set; } 

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string? ApiToken { get; set; }

        [Required]
        public byte[] PasswordHash { get; set; }
         
        [Required]
        public byte[] PasswordSalt { get; set; } 
    }
}
