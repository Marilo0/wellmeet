using Wellmeet.Core.Filters;
using Wellmeet.Data;
using Wellmeet.DTO;
using Wellmeet.Models;

namespace Wellmeet.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> VerifyAndGetUserAsync(UserLoginDTO credentials);
        Task<JwtTokenDTO> LoginAsync(UserLoginDTO dto); // maybe remove this line later
        Task<UserReadOnlyDTO> RegisterAsync(UserRegisterDTO dto);

        Task<UserReadOnlyDTO> GetByIdAsync(int id);
        Task<UserReadOnlyDTO> UpdateAsync(int id, UserUpdateDTO dto);
        Task<bool> DeleteAsync(int id);

        Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedAsync(
            int pageNumber,
            int pageSize,
            UserFiltersDTO filters);

        //Task<DashboardDTO> GetDashboardAsync(int userId);
    }
}
