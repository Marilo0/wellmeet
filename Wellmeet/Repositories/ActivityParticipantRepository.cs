using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Wellmeet.Data;
using Wellmeet.Models;
using Wellmeet.Repositories.Interfaces;

namespace Wellmeet.Repositories
{
    public class ActivityParticipantRepository : BaseRepository<ActivityParticipant>, IActivityParticipantRepository
    {
        public ActivityParticipantRepository(WellmeetDbContext context) : base(context)
        {
        }

        public async Task<ActivityParticipant?> GetByActivityAndUserAsync(int activityId, int userId)
        {
            return await dbSet
                .FirstOrDefaultAsync(ap => ap.ActivityId == activityId && ap.UserId == userId);
        }

        public async Task<IEnumerable<ActivityParticipant>> GetParticipantsByActivityAsync(int activityId)
        {
            return await dbSet
                .Where(ap => ap.ActivityId == activityId)
                .Include(ap => ap.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<ActivityParticipant>> GetActivitiesByUserAsync(int userId)
        {
            return await dbSet
                .Where(ap => ap.UserId == userId)
                .Include(ap => ap.Activity)
                    .ThenInclude(a => a.Creator)
                .ToListAsync();
        }

        public async Task<bool> IsUserParticipantAsync(int activityId, int userId)
        {
            return await dbSet
                .AnyAsync(ap => ap.ActivityId == activityId && ap.UserId == userId);
        }

        public async Task<PaginatedResult<ActivityParticipant>> GetActivityParticipantsAsync(int pageNumber, int pageSize,
            List<Expression<Func<ActivityParticipant, bool>>> predicates)
        {
            var query = dbSet.AsQueryable();

            if (predicates != null && predicates.Any())
            {
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate);
                }
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Include(ap => ap.User)
                .Include(ap => ap.Activity)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<ActivityParticipant>(items, totalCount, pageNumber, pageSize);
        }

        //THIS METHOD INCLUDES DELETED RECORDS- WHEN THE USER HAS LEFT THE ACTIVITY AND THE RECORD IS SOFT DELETED.. IT CAN BE RETRIEVED USING THIS METHOD
        public async Task<ActivityParticipant?> GetIncludingDeletedAsync(int activityId, int userId)
        {
            return await dbSet
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(ap =>
                    ap.ActivityId == activityId &&
                    ap.UserId == userId);
        }

    }
}
