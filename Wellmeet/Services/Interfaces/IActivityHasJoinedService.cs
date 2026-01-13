using Wellmeet.DTO;

namespace Wellmeet.Services.Interfaces
{
    public interface IActivityHasJoinedService
    {
        Task<ActivityHasJoinedReadOnlyDTO> GetActivityHasJoinedAsync(int activityId, int userId);
    }
}
