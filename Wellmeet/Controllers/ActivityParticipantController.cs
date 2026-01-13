using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wellmeet.DTO;
using Wellmeet.Services.Interfaces;

namespace Wellmeet.Controllers
{
    [ApiController]
    [Route("api/participations")]
    public class ActivityParticipantController : BaseController
    {
        public ActivityParticipantController(IApplicationService applicationService)
            : base(applicationService) { }

        // POST: /api/participations/{id}/join
        [Authorize]
        [HttpPost("{id:int}/join")]
        public async Task<ActionResult<ActivityParticipantReadOnlyDTO>> Join(int id)
        {
            if (AppUser == null)
                return Unauthorized();

            var dto = await ApplicationService.ActivityParticipantService.JoinAsync(AppUser.Id, id);
            return Ok(dto);
        }

        // DELETE: /api/participations/{id}/leave
        [Authorize]
        [HttpDelete("{id:int}/leave")]
        public async Task<IActionResult> Leave(int id)
        {
            if (AppUser == null)
                return Unauthorized();

            await ApplicationService.ActivityParticipantService.LeaveAsync(AppUser.Id, id);
            return NoContent();
        }

        // GET: /api/participations/{id}/participants
        [HttpGet("{id:int}/participants")]
        public async Task<ActionResult<IEnumerable<ActivityParticipantReadOnlyDTO>>> GetParticipants(int id)
        {
            var participants = await ApplicationService.ActivityParticipantService.GetParticipantsAsync(id);
            return Ok(participants);
        }

        // GET: /api/participations/mine
        [Authorize]
        [HttpGet("mine")]
        public async Task<ActionResult<List<ActivityParticipantReadOnlyDTO>>> GetMyJoinedActivities()
        {
            if (AppUser == null)
                return Unauthorized();
          
            var list = await ApplicationService.ActivityParticipantService.GetUserJoinedActivitiesAsync(AppUser.Id);
            return Ok(list);
        }
    }
}
