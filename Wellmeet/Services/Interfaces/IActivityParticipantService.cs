using Wellmeet.DTO;

namespace Wellmeet.Services.Interfaces
{
    public interface IActivityParticipantService
    {
        Task<ActivityParticipantReadOnlyDTO> JoinAsync(int userId, int activityId);
        Task<bool> LeaveAsync(int userId, int activityId);

        Task<IEnumerable<ActivityParticipantReadOnlyDTO>> GetParticipantsAsync(int activityId);
        Task<IEnumerable<ActivityParticipantReadOnlyDTO>> GetUserJoinedActivitiesAsync(int userId);
    }
}
