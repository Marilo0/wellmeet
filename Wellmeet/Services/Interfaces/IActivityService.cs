using Wellmeet.Core.Filters;
using Wellmeet.DTO;
using Wellmeet.Models;

namespace Wellmeet.Services.Interfaces
{
    public interface IActivityService
    {
        Task<ActivityReadOnlyDTO> CreateAsync(int creatorId, ActivityCreateDTO dto);
        Task<ActivityReadOnlyDTO> UpdateAsync(int activityId, int userId, ActivityUpdateDTO dto);
        Task<bool> DeleteAsync(int activityId, int userId);

        Task<ActivityReadOnlyDTO> GetByIdAsync(int id);
        Task<PaginatedResult<ActivityReadOnlyDTO>> GetPaginatedAsync(
            int pageNumber, int pageSize, ActivityFiltersDTO filters);

        Task<IEnumerable<ActivityReadOnlyDTO>> GetUpcomingAsync();
    }
}
