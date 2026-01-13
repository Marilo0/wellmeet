namespace Wellmeet.Core.Filters
{
    public record ActivityFiltersDTO
    {   
        public string? Title { get; set; }
        public string? City { get; set; }
        public string? Category { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }

        public bool IsJoinable { get; set; } = false;

        // Manual filters chosen by user
        public bool UpcomingOnly { get; set; } = false;
        public bool PastOnly { get; set; } = false;
    }
}
