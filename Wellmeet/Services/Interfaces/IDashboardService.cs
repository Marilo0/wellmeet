using Wellmeet.DTO;

namespace Wellmeet.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDTO> GetDashboardAsync(int userId);
    }
}
