using Wellmeet.Core.Enums;

namespace Wellmeet.Data
{
    public class User : BaseEntity
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
        public UserRole UserRole { get; set; }

        // Navigation properties 
        public virtual ICollection<Activity> CreatedActivities { get; set; } = new List<Activity>();
        public virtual ICollection<ActivityParticipant> ActivityParticipants { get; set; } = new List<ActivityParticipant>();
    }
}
