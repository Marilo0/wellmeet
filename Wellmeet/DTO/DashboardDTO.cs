namespace Wellmeet.DTO
{
    public class DashboardDTO

   {
        public UserReadOnlyDTO User { get; init; } 

        public List<ActivityReadOnlyDTO> CreatedActivities { get; set; } = new(); //new() TO AVOID NULL REFERENCE because IT IS A LIST
        public List<ActivityParticipantReadOnlyDTO> JoinedActivities { get; set; } = new(); //using lightweight DTO because we don't need full activity details here 
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
