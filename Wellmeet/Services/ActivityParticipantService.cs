using AutoMapper;
using Serilog;
using Wellmeet.Data;
using Wellmeet.DTO;
using Wellmeet.Exceptions;
using Wellmeet.Repositories.Interfaces;
using Wellmeet.Services.Interfaces;

namespace Wellmeet.Services
{
    public class ActivityParticipantService : IActivityParticipantService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<ActivityParticipantService> _logger = new LoggerFactory().AddSerilog().CreateLogger<ActivityParticipantService>();
        public ActivityParticipantService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        // JOIN -------------------------------------------------------------------
        public async Task<ActivityParticipantReadOnlyDTO> JoinAsync(int userId, int activityId)
        {
            ActivityParticipant? participant;

            try
            {
                // 1. Get activity
                var activity = await _uow.ActivityRepository.GetAsync(activityId)
                    ?? throw new EntityNotFoundException("Activity", "Activity not found.");

                // 2. Business rules
                if (activity.CreatorId == userId)
                    throw new EntityForbiddenException("Activity", "You cannot join your own activity.");

                if (activity.StartDateTime <= DateTime.UtcNow)
                    throw new EntityForbiddenException(
                        "Activity",
                        "You cannot join an activity that has already started or ended.");

                if (activity.MaxParticipants > 0 &&
                    activity.Participants.Count >= activity.MaxParticipants)
                    throw new EntityForbiddenException("Activity", "Activity is full.");

                // 3. 🔥 IMPORTANT: check INCLUDING soft-deleted
                var existing = await _uow.ActivityParticipantRepository
                    .GetIncludingDeletedAsync(activityId, userId);

                if (existing != null)
                {
                    if (!existing.IsDeleted)
                    {
                        throw new EntityAlreadyExistsException(
                            "Participant",
                            "You already joined this activity.");
                    }

                    // 4. RESTORE soft-deleted participation
                    existing.IsDeleted = false;
                    existing.DeletedAt = null;
                    existing.JoinDate = DateTime.UtcNow;
                    existing.ModifiedAt = DateTime.UtcNow;

                    await _uow.ActivityParticipantRepository.UpdateAsync(existing);
                    participant = existing;
                }
                else
                {
                    // 5. First-time join
                    participant = new ActivityParticipant
                    {
                        ActivityId = activityId,
                        UserId = userId,
                        JoinDate = DateTime.UtcNow
                    };

                    await _uow.ActivityParticipantRepository.AddAsync(participant);
                }

                // 6. Save
                await _uow.SaveAsync();

                _logger.LogInformation(
                    "User {UserId} joined Activity {ActivityId}",
                    userId,
                    activityId
                );
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (EntityForbiddenException)
            {
                throw;
            }
            catch (EntityAlreadyExistsException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while joining Activity {ActivityId} by User {UserId}",
                    activityId,
                    userId
                );

                throw new ServerException(
                    "Participant",
                    "Unexpected error while joining activity.");
            }

            return _mapper.Map<ActivityParticipantReadOnlyDTO>(participant);
        }

        //public async Task<ActivityParticipantReadOnlyDTO> JoinAsync(int userId, int activityId)
        //{
        //    ActivityParticipant? participant = null;

        //    try
        //    {
        //        var activity = await _uow.ActivityRepository.GetAsync(activityId)
        //            ?? throw new EntityNotFoundException("Activity", "Activity not found.");

        //        if (activity.CreatorId == userId)
        //            throw new EntityForbiddenException("Activity", "You cannot join your own activity.");

        //        // Cannot join past activities
        //        if (activity.StartDateTime <= DateTime.UtcNow)
        //            throw new EntityForbiddenException("Activity", "You cannot join an activity that has already started or ended.");


        //        if (await _uow.ActivityParticipantRepository.IsUserParticipantAsync(activityId, userId))
        //            throw new EntityAlreadyExistsException("Participant", "You already joined this activity.");

        //        if (activity.MaxParticipants > 0 &&
        //            activity.Participants.Count >= activity.MaxParticipants)
        //            throw new EntityForbiddenException("Activity", "Activity is full.");

        //        participant = new ActivityParticipant
        //        {
        //            ActivityId = activityId,
        //            UserId = userId,
        //            JoinDate = DateTime.UtcNow
        //        };

        //        await _uow.ActivityParticipantRepository.AddAsync(participant);
        //        await _uow.SaveAsync();

        //        _logger.LogInformation("User {UserId} joined Activity {ActivityId}", userId, activityId);
        //    }
        //    catch (EntityNotFoundException ex)
        //    {
        //        _logger.LogError("Join failed: Activity {ActivityId} not found. {Message}", activityId, ex.Message);
        //        throw;
        //    }
        //    catch (EntityForbiddenException ex)
        //    {
        //        _logger.LogError("Join forbidden for User {UserId} on Activity {ActivityId}. {Message}",
        //            userId, activityId, ex.Message);
        //        throw;
        //    }
        //    catch (EntityAlreadyExistsException ex)
        //    {
        //        _logger.LogError("Join failed: User {UserId} already joined Activity {ActivityId}. {Message}",
        //            userId, activityId, ex.Message);
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Unexpected error while joining Activity {ActivityId} by User {UserId}", activityId, userId);
        //        throw new ServerException("Participant", "Unexpected error while joining activity.");
        //    }

        //    return _mapper.Map<ActivityParticipantReadOnlyDTO>(participant);
        //}

        // LEAVE -------------------------------------------------------------------
        public async Task<bool> LeaveAsync(int userId, int activityId)
        {
            try
            {
                var participants = await _uow.ActivityParticipantRepository.GetParticipantsByActivityAsync(activityId);

                var entry = participants.FirstOrDefault(p => p.UserId == userId)
                    ?? throw new EntityNotFoundException("Participant", "User is not a participant of this activity.");

                await _uow.ActivityParticipantRepository.DeleteAsync(entry.Id);
                await _uow.SaveAsync();

                _logger.LogInformation("User {UserId} left Activity {ActivityId}", userId, activityId);

                return true;
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogError("Leave failed: User {UserId} not participant of Activity {ActivityId}. {Message}",
                    userId, activityId, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while User {UserId} attempted to leave Activity {ActivityId}",
                    userId, activityId);
                throw new ServerException("Participant", "Unexpected error while leaving activity.");
            }
        }

        // GET PARTICIPANTS ----------------------------------------------------------
        public async Task<IEnumerable<ActivityParticipantReadOnlyDTO>> GetParticipantsAsync(int activityId)
        {
            var participants = await _uow.ActivityParticipantRepository.GetParticipantsByActivityAsync(activityId);
            return _mapper.Map<IEnumerable<ActivityParticipantReadOnlyDTO>>(participants);
        }

        // GET JOINED ACTIVITIES ---------------------------------------------------
        public async Task<IEnumerable<ActivityParticipantReadOnlyDTO>> GetUserJoinedActivitiesAsync(int userId)
        {
            var list = await _uow.ActivityParticipantRepository.GetActivitiesByUserAsync(userId);
            return _mapper.Map<IEnumerable<ActivityParticipantReadOnlyDTO>>(list);
        }
    }
}
