using Microsoft.EntityFrameworkCore;
using Wellmeet.Data;
using Wellmeet.Repositories.Interfaces;

namespace Wellmeet.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly WellmeetDbContext context;
        protected readonly DbSet<T> dbSet;

        public BaseRepository(WellmeetDbContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        public virtual async Task AddAsync(T entity) => await dbSet.AddAsync(entity);

        public virtual async Task AddRangeAsync(IEnumerable<T> entities) => await dbSet.AddRangeAsync(entities);

        public virtual Task UpdateAsync(T entity)  //async is not needed here
        {
            dbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Modified; // Mark the entity as modified
            return Task.CompletedTask;
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            T? existingEntity = await GetAsync(id);
            if (existingEntity == null) return false;
            existingEntity.IsDeleted = true;
            existingEntity.DeletedAt = DateTime.UtcNow; //soft delete. For hard delete : dbSet.Remove(existingEntity);
            existingEntity.ModifiedAt = DateTime.UtcNow; 
            return true;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync() => await dbSet.ToListAsync();

        public virtual async Task<T?> GetAsync(int id) => await dbSet.FindAsync(id);

        public virtual async Task<int> GetCountAsync() => await dbSet.CountAsync();
    }
}
