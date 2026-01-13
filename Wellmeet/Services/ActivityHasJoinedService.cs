using AutoMapper;
using Wellmeet.DTO;
using Wellmeet.Exceptions;
using Wellmeet.Repositories.Interfaces;
using Wellmeet.Services.Interfaces;

namespace Wellmeet.Services
{
    public class ActivityHasJoinedService : IActivityHasJoinedService
    {
        private readonly IUnitOfWork _uow;

        private readonly IMapper _mapper;

        public ActivityHasJoinedService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

    

        public async Task<ActivityHasJoinedReadOnlyDTO> GetActivityHasJoinedAsync(int activityId, int userId)
        {
            var activity = await _uow.ActivityRepository.GetActivityWithParticipantsAsync(activityId)
     ?? throw new EntityNotFoundException("Activity", "Activity not found.");


            var hasJoined = await _uow.ActivityParticipantRepository
                .IsUserParticipantAsync(activityId, userId);

            var dto = _mapper.Map<ActivityHasJoinedReadOnlyDTO>(activity);
            dto.HasJoined = hasJoined;

            return dto;
        }


    }
}
