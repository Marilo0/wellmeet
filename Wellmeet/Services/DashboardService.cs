using AutoMapper;
using Serilog;
using Wellmeet.DTO;
using Wellmeet.Exceptions;
using Wellmeet.Repositories.Interfaces;
using Wellmeet.Services.Interfaces;

namespace Wellmeet.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<ActivityService> _logger = new LoggerFactory().AddSerilog().CreateLogger<ActivityService>();

        public DashboardService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }
        public async Task<DashboardDTO> GetDashboardAsync(int userId)
        {
            var user = await _uow.UserRepository.GetAsync(userId)
                ?? throw new EntityNotFoundException("User", "User not found");

            var createdActivities = await _uow.ActivityRepository
                .GetActivitiesCreatedByUserAsync(userId);

            var joinedActivities = await _uow.ActivityParticipantRepository
                .GetActivitiesByUserAsync(userId);

            var dto = new DashboardDTO
            {
                User = _mapper.Map<UserReadOnlyDTO>(user),
                CreatedActivities = _mapper.Map<List<ActivityReadOnlyDTO>>(createdActivities),
                JoinedActivities = _mapper.Map<List<ActivityParticipantReadOnlyDTO>>(joinedActivities),
            };

            //dto.TotalCreatedActivities = dto.CreatedActivities.Count;   // I HAVE THEM IN COMPUTED PROPERTIES IN DTO BUT MAYBE IT IS BETTER HERE
            //dto.TotalJoinedActivities = dto.JoinedActivities.Count;

            //dto.UpcomingJoinedActivities = dto.JoinedActivities
            //    .Where(ap => ap.ActivityStartDateTime > DateTime.UtcNow)
            //    .OrderBy(ap => ap.ActivityStartDateTime)
            //    .ToList();

            return dto;
        }

    }
}
