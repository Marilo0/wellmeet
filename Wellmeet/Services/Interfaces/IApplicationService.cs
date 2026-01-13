namespace Wellmeet.Services.Interfaces
{
    public interface IApplicationService
    {
        IUserService UserService { get; }
        IActivityService ActivityDetailsService { get; }
        IActivityParticipantService ActivityParticipantService { get; }

        IDashboardService DashboardService { get; }

        IActivityHasJoinedService ActivityHasJoinedService { get; }
    }
}