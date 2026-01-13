namespace Wellmeet.DTO
{
    public class ActivityReadOnlyDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? City { get; set; }
        public string? Location { get; set; }
        public string? Category { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int MaxParticipants { get; set; }
        public int CurrentParticipants { get; set; }
        public UserReadOnlyDTO? Creator { get; set; }
        public DateTime InsertedAt { get; set; }

        // computed properties 
        public bool IsJoinable
        {
            get
            {
                var timeUntilStart = StartDateTime - DateTime.UtcNow;
                return timeUntilStart > TimeSpan.FromMinutes(30); // Can join if >30 min away
            }
        }

        public bool HasStarted => DateTime.UtcNow >= StartDateTime;
        public bool HasEnded => DateTime.UtcNow >= EndDateTime;
        public bool IsOngoing => HasStarted && !HasEnded;

        // For UI to show time status
        public string TimeStatus
        {
            get
            {
                if (HasEnded) return "Completed";
                if (IsOngoing) return "In Progress";
                if (!IsJoinable) return "Starting Soon";
                return "Upcoming";
            }
        }
    }
}
