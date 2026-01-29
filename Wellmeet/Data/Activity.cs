using Wellmeet.Core.Enums;

namespace Wellmeet.Data
{
    public class Activity : BaseEntity
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public ActivityCategory Category { get; set; } = ActivityCategory.Other;

        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        public int MaxParticipants { get; set; } = 10;

        // FK  creator of the activity
        public int CreatorId { get; set; }

        // Navigation property - to match User.CreatedActivities
        public virtual User Creator { get; set; } = null!;
        public virtual ICollection<ActivityParticipant> Participants { get; set; } = new List<ActivityParticipant>();
    }
}
