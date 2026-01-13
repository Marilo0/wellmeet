using Wellmeet.Core.Enums;

namespace Wellmeet.DTO
{
    public record UserReadOnlyDTO
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public UserRole UserRole { get; set; }  
    }
}
