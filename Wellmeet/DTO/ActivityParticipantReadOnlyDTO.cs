namespace Wellmeet.DTO
{
    public class ActivityParticipantReadOnlyDTO
    {
        public int Id { get; set; }

        // Minimal user info to avoid circular references --instead of full UserReadOnlyDTO being nested
        public int UserId { get; set; }
        public string? Username { get; set; }

        // Minimal activity info needed for joined-activities view --instead of full ActivityReadOnlyDTO
        public int ActivityId { get; set; }
        public string? ActivityTitle { get; set; }
        public DateTime ActivityStartDateTime { get; set; }

        public DateTime JoinDate { get; set; }
    }
}
