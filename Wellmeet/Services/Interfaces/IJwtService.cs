using Wellmeet.Core.Enums;

namespace Wellmeet.Services.Interfaces
{
    public interface IJwtService
    {
        string CreateToken(int userId, string username, string email, UserRole role);
    }
}
