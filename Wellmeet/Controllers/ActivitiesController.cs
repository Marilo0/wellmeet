using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wellmeet.Core.Filters;
using Wellmeet.DTO;
using Wellmeet.Exceptions;
using Wellmeet.Models;
using Wellmeet.Services.Interfaces;

namespace Wellmeet.Controllers
{
          [ApiController]
        [Route("api/activities")]
        public class ActivitiesController : BaseController
        {
            public ActivitiesController(IApplicationService applicationService)
                : base(applicationService) { }

            // GET: /api/activities
            [HttpGet]
            public async Task<ActionResult<PaginatedResult<ActivityReadOnlyDTO>>> GetActivities(
                [FromQuery] ActivityFiltersDTO filters,  //first required params 
                [FromQuery] int pageNumber = 1,   //then optional params
                [FromQuery] int pageSize = 10)
               
            {
                var result = await ApplicationService.ActivityDetailsService.GetPaginatedAsync(pageNumber, pageSize, filters);
                return Ok(result);
            }

            // GET: /api/activities/{id}
            [HttpGet("{id:int}")]
            public async Task<ActionResult<ActivityReadOnlyDTO>> GetById(int id)
            {
                var dto = await ApplicationService.ActivityDetailsService.GetByIdAsync(id);
                return Ok(dto);
            }

            // POST: /api/activities
            [Authorize]
            [HttpPost]
            public async Task<ActionResult<ActivityReadOnlyDTO>> Create([FromBody] ActivityCreateDTO dto)
            {
                if (AppUser == null)
                    return Unauthorized();

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(e => e.Value!.Errors.Any())
                        .Select(e => new {
                            Field = e.Key,
                            Errors = e.Value!.Errors.Select(er => er.ErrorMessage).ToArray()
                        });

                    throw new InvalidArgumentException("Activity",
                        "Invalid activity data: " +
                        System.Text.Json.JsonSerializer.Serialize(errors));   //againn serialize not needed(?)
                }

                var created = await ApplicationService.ActivityDetailsService.CreateAsync(AppUser.Id, dto);

                return StatusCode(201, created);
            }

            // PUT: /api/activities/{id}
            [Authorize]
            [HttpPut("{id:int}")]
            public async Task<ActionResult<ActivityReadOnlyDTO>> Update(int id, [FromBody] ActivityUpdateDTO dto)
            {
                if (AppUser == null)
                    return Unauthorized();

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(e => e.Value!.Errors.Any())
                        .Select(e => new
                        {
                            Field = e.Key,
                            Errors = e.Value!.Errors.Select(er => er.ErrorMessage).ToArray()
                        });

                    throw new InvalidArgumentException("Activity",
                        "Invalid update data: " +
                        System.Text.Json.JsonSerializer.Serialize(errors));  //agaaaaaaaaaaaaain
                }
                var updated = await ApplicationService.ActivityDetailsService
                                    .UpdateAsync(id, AppUser.Id, dto);

                return Ok(updated);
            }

            // DELETE: /api/activities/{id}
            [Authorize]
            [HttpDelete("{id:int}")]
            public async Task<IActionResult> Delete(int id)
            {
                if (AppUser == null)
                    return Unauthorized();

                await ApplicationService.ActivityDetailsService.DeleteAsync(id, AppUser.Id);
                return NoContent();
            }

            // GET: /api/activities/upcoming
            [HttpGet("upcoming")]
            public async Task<ActionResult<List<ActivityReadOnlyDTO>>> GetUpcoming()
            {
                var result = await ApplicationService.ActivityDetailsService.GetUpcomingAsync();
                return Ok(result);
            }


        // GET /api/activities/{id}/has-joined     FOR THE NEW DTO WITH HasJoined PROPERTY FOR THE FRONTEND LOGGED IN USER
        [Authorize]
            [HttpGet("{id:int}/has-joined")]
            public async Task<ActionResult<ActivityHasJoinedReadOnlyDTO>> GetDetails(int id)
            {
                if (AppUser == null)
                    return Unauthorized();

                var dto = await ApplicationService.ActivityHasJoinedService
                    .GetActivityHasJoinedAsync(id, AppUser.Id);

                return Ok(dto);
            }



    }



}
