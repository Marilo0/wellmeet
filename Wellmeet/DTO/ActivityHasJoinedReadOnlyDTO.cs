namespace Wellmeet.DTO
{
    public class ActivityHasJoinedReadOnlyDTO : ActivityReadOnlyDTO
    {
        // user-specific, computed field in the service layer
        public bool HasJoined { get; set; }

    }
}
