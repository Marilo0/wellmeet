namespace Wellmeet.Models;

public class ApplicationUser
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string UserRole { get; set; } = "User";   // FIXED (string instead of enum)
}
