using AutoMapper;
using Serilog;
using Wellmeet.Core.Filters;
using Wellmeet.Data;
using Wellmeet.DTO;
using Wellmeet.Exceptions;
using Wellmeet.Models;
using Wellmeet.Repositories.Interfaces;
using Wellmeet.Services.Interfaces;

namespace Wellmeet.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<ActivityService> _logger = new LoggerFactory().AddSerilog().CreateLogger<ActivityService>();

        public ActivityService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }


        // CREATE --------------------------------------------------------------------
        public async Task<ActivityReadOnlyDTO> CreateAsync(int creatorId, ActivityCreateDTO dto)
        {
            Activity? activity = null;

            try
            {
                activity = _mapper.Map<Activity>(dto);
                activity.CreatorId = creatorId;
                activity.InsertedAt = DateTime.UtcNow;

                await _uow.ActivityRepository.AddAsync(activity);
                await _uow.SaveAsync();

                _logger.LogInformation("Activity '{Title}' created by User {UserId}", activity.Title, creatorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating activity for User {UserId}", creatorId);
                throw new ServerException("Activity", "Unexpected error while creating activity.");
            }

            return _mapper.Map<ActivityReadOnlyDTO>(activity);
        }

        // UPDATE --------------------------------------------------------------------
        public async Task<ActivityReadOnlyDTO> UpdateAsync(int activityId, int userId, ActivityUpdateDTO dto)
        {
            Activity? activity = null;

            try
            {
                activity = await _uow.ActivityRepository.GetAsync(activityId)
                    ?? throw new EntityNotFoundException("Activity", "Activity not found.");

                if (activity.CreatorId != userId)
                    throw new EntityForbiddenException("Activity", "You cannot modify this activity.");

                _mapper.Map(dto, activity);
                activity.ModifiedAt = DateTime.UtcNow;

                await _uow.ActivityRepository.UpdateAsync(activity);
                await _uow.SaveAsync();

                _logger.LogInformation("Activity {Id} updated by User {UserId}", activityId, userId);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogError("Error updating Activity {Id}: {Message}", activityId, ex.Message);
                throw;
            }
            catch (EntityForbiddenException ex)
            {
                _logger.LogError("Forbidden update attempt by User {UserId} on Activity {ActivityId}. {Message}",
                    userId, activityId, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating Activity {Id}", activityId);
                throw new ServerException("Activity", "Unexpected error while updating activity.");
            }

            return _mapper.Map<ActivityReadOnlyDTO>(activity);
        }

        // DELETE --------------------------------------------------------------------
        public async Task<bool> DeleteAsync(int activityId, int userId)
        {
            try
            {
                var activity = await _uow.ActivityRepository.GetAsync(activityId)
                    ?? throw new EntityNotFoundException("Activity", "Activity not found.");

                if (activity.CreatorId != userId)
                    throw new EntityForbiddenException("Activity", "You cannot delete this activity.");

                await _uow.ActivityRepository.DeleteAsync(activityId);
                await _uow.SaveAsync();

                _logger.LogInformation("Activity {Id} deleted by User {UserId}", activityId, userId);
                return true;
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogError("Error deleting Activity {Id}: {Message}", activityId, ex.Message);
                throw;
            }
            catch (EntityForbiddenException ex)
            {
                _logger.LogError("Forbidden delete attempt by User {UserId} on Activity {ActivityId}. {Message}",
                    userId, activityId, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting Activity {Id}", activityId);
                throw new ServerException("Activity", "Unexpected error while deleting activity.");
            }
        }

        // GET BY ID -----------------------------------------------------------------
        public async Task<ActivityReadOnlyDTO> GetByIdAsync(int id)   
        {
            Activity? activity = null;

            try
            {
                activity = await _uow.ActivityRepository.GetActivityWithParticipantsAsync(id)   //changed the get by id cause it gave the baserepository activity although we need the creator too
                    ?? throw new EntityNotFoundException("Activity", "Activity not found.");
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogError("Error retrieving Activity {Id}: {Message}", id, ex.Message);
                throw;
            }

            return _mapper.Map<ActivityReadOnlyDTO>(activity);
        }

        // PAGINATED -----------------------------------------------------------------
        public async Task<PaginatedResult<ActivityReadOnlyDTO>> GetPaginatedAsync(
            int page, int size, ActivityFiltersDTO filters)
        {
            var predicates = PredicateBuilder.BuildActivityPredicates(filters);

            var result = await _uow.ActivityRepository.GetActivitiesAsync(page, size, predicates);

            return new PaginatedResult<ActivityReadOnlyDTO>
            {
                Data = _mapper.Map<List<ActivityReadOnlyDTO>>(result.Data),
                TotalRecords = result.TotalRecords,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };
        }

        // UPCOMING ------------------------------------------------------------------
        public async Task<IEnumerable<ActivityReadOnlyDTO>> GetUpcomingAsync()
        {
            var upcoming = await _uow.ActivityRepository.GetUpcomingActivitiesAsync();
            return _mapper.Map<IEnumerable<ActivityReadOnlyDTO>>(upcoming);
        }
    }
}
