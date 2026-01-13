using System.Linq.Expressions;
using Wellmeet.Data;
using Wellmeet.Models;

namespace Wellmeet.Repositories.Interfaces
{
    public interface IActivityRepository
    {
        Task<PaginatedResult<Activity>> GetActivitiesAsync(int pageNumber, int pageSize,
           List<Expression<Func<Activity, bool>>> predicates);
        Task<Activity?> GetActivityWithParticipantsAsync(int activityId);
        Task<IEnumerable<Activity>> GetUpcomingActivitiesAsync();

        Task<IEnumerable<Activity>> GetActivitiesCreatedByUserAsync(int userId);
    }
}
