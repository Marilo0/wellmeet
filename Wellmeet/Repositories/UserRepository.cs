using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Wellmeet.Data;
using Wellmeet.Models;
using Wellmeet.Repositories.Interfaces;
using Wellmeet.Security;

namespace Wellmeet.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(WellmeetDbContext context) : base(context)
        {
        }

        //maybe i can add .AsNoTracking() to improve performance to all methods that only read data

        public async Task<User?> GetUserAsync(string username, string password)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username
            || u.Email == username);

            if (user == null) return null;

            if (!EncryptionUtil.IsValidPassword(password, user.Password)) return null;

            return user;
        }

        public async Task<User?> GetUserByUsernameAsync(string username) =>
            await context.Users.FirstOrDefaultAsync(u => u.Username == username);



        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<PaginatedResult<User>> GetUsersAsync(int pageNumber, int pageSize,
       List<Expression<Func<User, bool>>> predicates)
        {
            IQueryable<User> query = context.Users; // δεν εκτελείται

            //soft delete
            //query = query.Where(u => !u.IsDeleted);

            if (predicates != null && predicates.Count > 0)
            {
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate); // εκτελείται, υπονοείται το AND
                }
            }

            int totalRecords = await query.CountAsync(); // εκτελείται

            int skip = (pageNumber - 1) * pageSize;

            var data = await query
                .OrderBy(u => u.Id) // πάντα να υπάρχει ένα OrderBy πριν το Skip
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(); // εκτελείται

            return new PaginatedResult<User>(data, totalRecords, pageNumber, pageSize);
        }
    }
}
