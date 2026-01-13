using AutoMapper;
using Wellmeet.Repositories.Interfaces;
using Wellmeet.Services.Interfaces;

namespace Wellmeet.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;  // this is for JwtService configuration needed in UserService EXTRA!! Microsoft.Extensions.Configuration;

        public ApplicationService(IUnitOfWork uow, IMapper mapper, IConfiguration config)
        {
            _uow = uow;
            _mapper = mapper;
            _config = config;
        }

        public IUserService UserService =>
            new UserService(_uow, _mapper, new JwtService(_config));

        public IActivityService ActivityDetailsService =>     // renamed from ActivityService to ActivityDetailsService to avoid confusion with ActivityService class which is being instantiated here
            new ActivityService(_uow, _mapper);

        public IActivityParticipantService ActivityParticipantService =>
            new ActivityParticipantService(_uow, _mapper);

        public IDashboardService DashboardService =>         
        new DashboardService(_uow, _mapper);

        public IActivityHasJoinedService ActivityHasJoinedService =>
            new ActivityHasJoinedService(_uow, _mapper);

    }
}
