namespace Wellmeet.Data
{
    public class ActivityParticipant : BaseEntity
    {
        // Id as primary key, composite key as unique constraint in DbContext
        public int Id { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;

        public int ActivityId { get; set; }
        public virtual Activity Activity { get; set; } = null!;

        // Extra fields
        public DateTime JoinDate { get; set; } = DateTime.UtcNow;  
        public string Status { get; set; } = "Joined";  // not in use yet
    }
}
