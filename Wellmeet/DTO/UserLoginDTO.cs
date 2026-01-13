using System.ComponentModel.DataAnnotations;

namespace Wellmeet.DTO
{
    public record UserLoginDTO
    {
        [Required(ErrorMessage = "Username is required")] 
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Username must be between 2 and 50 characters.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")] 
        public string? Password { get; set; }

        public bool KeepLoggedIn { get; set; } //to check if it is needed and what to do with it
        //Keep it for future “remember me” implementation e.g., for setting longer token expiration
    }
}
