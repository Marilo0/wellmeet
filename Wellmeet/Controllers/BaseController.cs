
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wellmeet.Models;
using Wellmeet.Services.Interfaces;


namespace Wellmeet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        protected readonly IApplicationService ApplicationService;
        //protected readonly IMapper Mapper;

        public BaseController(IApplicationService applicationService )
        {
            ApplicationService = applicationService;
        }

        private ApplicationUser? _appUser;

        protected ApplicationUser? AppUser
        {
            get
            {
                if (_appUser != null)
                    return _appUser;  // Return cached user

                if (User?.Claims == null || !User.Claims.Any())
                    return null; // No JWT -> no user

                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (idClaim == null || !int.TryParse(idClaim.Value, out int userId))
                    return null;  // JWT missing userId ->  invalid token

                _appUser = new ApplicationUser
                {
                    Id = userId,
                    Username = User.FindFirst(ClaimTypes.Name)?.Value,
                    Email = User.FindFirst(ClaimTypes.Email)?.Value,
                    // Role = User.FindFirst(ClaimTypes.Role)?.Value  //ASP.NET Core does NOT use ApplicationUser class for authorization.
                                                // So what is ApplicationUser used for?
                                                // Only for:
                                                //✔ Controllers reading info about the current user
                                                //✔ Convenience
                                                //✔ Avoiding manually reading claims every time
                                                // It is NOT used by the framework.
                                                                        // Then how does ASP.NET know my role?
                                                                        //Because in JWT creation,you already did: new Claim(ClaimTypes.Role, userRole.ToString())
                };

                return _appUser;
            }
        }
    }
}
