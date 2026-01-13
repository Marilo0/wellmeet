namespace Wellmeet.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        UserRepository UserRepository { get; }
        ActivityRepository ActivityRepository { get; }
        ActivityParticipantRepository ActivityParticipantRepository { get; }

        Task<bool> SaveAsync();
    }
}
