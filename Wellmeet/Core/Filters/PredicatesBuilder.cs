using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Wellmeet.Core.Enums;
using Wellmeet.Data;

namespace Wellmeet.Core.Filters
{
    public static class PredicateBuilder
    {
        
        // USER FILTERS
           public static List<Expression<Func<User, bool>>> BuildUserPredicates(UserFiltersDTO filters)
        {
            var predicates = new List<Expression<Func<User, bool>>>();

            if (!string.IsNullOrWhiteSpace(filters.Username))
                predicates.Add(u => u.Username.Contains(filters.Username!.Trim()));

            if (!string.IsNullOrWhiteSpace(filters.Email))
                predicates.Add(u => u.Email.Contains(filters.Email!.Trim()));

            if (!string.IsNullOrWhiteSpace(filters.Firstname))
                predicates.Add(u => u.Firstname.Contains(filters.Firstname!.Trim()));

            if (!string.IsNullOrWhiteSpace(filters.Lastname))
                predicates.Add(u => u.Lastname.Contains(filters.Lastname!.Trim()));

            return predicates;
        }

       
        // ACTIVITY FILTERS
        public static List<Expression<Func<Activity, bool>>> BuildActivityPredicates(ActivityFiltersDTO filters)
        {
            var predicates = new List<Expression<Func<Activity, bool>>>();

            if (!string.IsNullOrWhiteSpace(filters.Title))
            {
                var title = filters.Title.Trim();
                predicates.Add(a =>
                    EF.Functions.Like(a.Title, $"%{title}%")
                );
            }

            if (!string.IsNullOrWhiteSpace(filters.City))
            {
                var city = filters.City.Trim();
                predicates.Add(a =>
                    EF.Functions.Like(a.City, $"%{city}%")
                );
            }

            if (!string.IsNullOrWhiteSpace(filters.Category) &&
            Enum.TryParse<ActivityCategory>(filters.Category.Trim(), true, out var category))
            {
                predicates.Add(a => a.Category == category);
            }


            if (filters.StartDateFrom.HasValue)
                predicates.Add(a => a.StartDateTime >= filters.StartDateFrom.Value);

            if (filters.StartDateTo.HasValue)
                predicates.Add(a => a.StartDateTime <= filters.StartDateTo.Value);

            if (filters.IsJoinable == true)
            {
                predicates.Add(a =>
                    a.StartDateTime > DateTime.UtcNow &&
                    a.Participants.Count < a.MaxParticipants
                );
            }


            if (filters.UpcomingOnly && !filters.PastOnly)
                predicates.Add(a => a.StartDateTime > DateTime.UtcNow);

            if (filters.PastOnly && !filters.UpcomingOnly)
                predicates.Add(a => a.StartDateTime <= DateTime.UtcNow);


            return predicates;
        }

        // ACTIVITIES CREATED BY SPECIFIC USER
        public static List<Expression<Func<Activity, bool>>> CreatedByUser(int userId)
        {
            return new List<Expression<Func<Activity, bool>>>
            {
                a => a.CreatorId == userId
            };
        }
    }
}
