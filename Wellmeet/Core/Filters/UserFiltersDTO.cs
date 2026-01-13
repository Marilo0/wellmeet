namespace Wellmeet.Core.Filters
{
    public record UserFiltersDTO
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
    }
}
