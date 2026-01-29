using Microsoft.EntityFrameworkCore;
using Serilog;
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

        //not in use yet -> login flow uses VerifyAndGetUserAsync ->  LoginAsync
        //public async Task<User?> GetUserAsync(string username, string password)
        //{
        //    var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username
        //    || u.Email == username);

        //    if (user == null) return null;

        //    if (!EncryptionUtil.IsValidPassword(password, user.Password)) return null;

        //    return user;
        //}

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
            IQueryable<User> query = context.Users; // query is not executed yet

            //soft delete
            //query = query.Where(u => !u.IsDeleted);

            if (predicates != null && predicates.Count > 0)
            {
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate); // applied to the query (AND logic)
                }
            }

            int totalRecords = await query.CountAsync(); // query executes here

            int skip = (pageNumber - 1) * pageSize;

            var data = await query
                .OrderBy(u => u.Id) // always apply OrderBy before Skip
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();  // query executes here

            return new PaginatedResult<User>(data, totalRecords, pageNumber, pageSize);
        }
    }
}
