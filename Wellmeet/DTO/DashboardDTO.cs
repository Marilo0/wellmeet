namespace Wellmeet.DTO
{
    public class DashboardDTO

   {
        public UserReadOnlyDTO User { get; init; } //NEW ADD

        // Activities the user created
        public List<ActivityReadOnlyDTO> CreatedActivities { get; set; } = new(); //new() TO AVOID NULL REFERENCE because IT IS A LIST

        // Activities the user joined (using lightweight DTO)
        public List<ActivityParticipantReadOnlyDTO> JoinedActivities { get; set; } = new(); // lightweight DTO because we don't need full activity details here

        // Summary stats
        public int TotalCreatedActivities => CreatedActivities.Count;
        public int TotalJoinedActivities => JoinedActivities.Count;

        // Upcoming = activities user joined that start in the future   I SHOULD COMPUTE THIS IN THE SERVICE
        public List<ActivityParticipantReadOnlyDTO> UpcomingJoinedActivities =>
            JoinedActivities
                .Where(ap => ap.ActivityStartDateTime > DateTime.UtcNow)
                .OrderBy(ap => ap.ActivityStartDateTime)
                .ToList();
    }
}
