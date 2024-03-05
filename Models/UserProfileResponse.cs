namespace WebApi7.Models;

// The UserProfileResponse class
public class UserProfileResponse
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public bool EmailVerified { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; } 
    public string Token { get; set; }
}
