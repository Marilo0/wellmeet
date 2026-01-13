using System.Linq.Expressions;
using Wellmeet.Data;
using Wellmeet.Models;

namespace Wellmeet.Repositories.Interfaces
{
    public interface IActivityParticipantRepository
    {
        Task<ActivityParticipant?> GetByActivityAndUserAsync(int activityId, int userId);
        Task<IEnumerable<ActivityParticipant>> GetParticipantsByActivityAsync(int activityId);
        Task<IEnumerable<ActivityParticipant>> GetActivitiesByUserAsync(int userId);
        Task<bool> IsUserParticipantAsync(int activityId, int userId);
        Task<PaginatedResult<ActivityParticipant>> GetActivityParticipantsAsync(int pageNumber, int pageSize,
            List<Expression<Func<ActivityParticipant, bool>>> predicates);
    }
}
