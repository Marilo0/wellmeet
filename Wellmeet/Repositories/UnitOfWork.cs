using Wellmeet.Data;
using Wellmeet.Repositories.Interfaces;


namespace Wellmeet.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WellmeetDbContext _context;

        // These fields store a single instance of each repository.
        // They are kept private to prevent external modification
        // and to maintain full control inside the UnitOfWork.
        private UserRepository _userRepository;
        private ActivityRepository _activityRepository;
        private ActivityParticipantRepository _activityParticipantRepository;

        public UnitOfWork(WellmeetDbContext context)
        {
            _context = context;
        }


        // Public properties to access repositories
        // These use lazy loading: the repository is created only when first needed,
        // then the same instance is returned every time after that
        public UserRepository UserRepository =>
            _userRepository ??= new UserRepository(_context);

        
        public ActivityRepository ActivityRepository =>
            _activityRepository ??= new ActivityRepository(_context);

       
        public ActivityParticipantRepository ActivityParticipantRepository =>
            _activityParticipantRepository ??= new ActivityParticipantRepository(_context);

        // Persists all changes managed by this UnitOfWork
        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
