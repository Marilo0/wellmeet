using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Wellmeet.Data;
using Wellmeet.Models;
using Wellmeet.Repositories.Interfaces;

namespace Wellmeet.Repositories
{
    public class ActivityRepository : BaseRepository<Activity>, IActivityRepository
    {
        public ActivityRepository(WellmeetDbContext context) : base(context)
        {
        }

        public async Task<PaginatedResult<Activity>> GetActivitiesAsync(int pageNumber, int pageSize,
            List<Expression<Func<Activity, bool>>> predicates)
        {
            var query = context.Activities
                .Include(a => a.Creator)
                .Include(a => a.Participants)
                    .ThenInclude(ap => ap.User)
                .AsQueryable();

            if (predicates != null && predicates.Any())
            {
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate);
                }
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(a => a.StartDateTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Activity>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<Activity?> GetActivityWithParticipantsAsync(int activityId)
        {
            return await context.Activities
                .Include(a => a.Creator)
                .Include(a => a.Participants)
                    .ThenInclude(ap => ap.User)
                .FirstOrDefaultAsync(a => a.Id == activityId);
        }

        public async Task<IEnumerable<Activity>> GetUpcomingActivitiesAsync()
        {
            return await context.Activities
                .Where(a => a.StartDateTime > DateTime.UtcNow)
                .Include(a => a.Creator)
                .OrderBy(a => a.StartDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Activity>> GetActivitiesCreatedByUserAsync(int userId)
        {
            return await context.Activities
                .Where(a => a.CreatorId == userId)
                .Include(a => a.Creator)
                .Include(a => a.Participants)
                    .ThenInclude(ap => ap.User)
                .OrderBy(a => a.StartDateTime)
                .ToListAsync();
        }

    }
}
