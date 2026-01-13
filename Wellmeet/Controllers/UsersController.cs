
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
    [Route("api/users")]
    public class UsersController : BaseController
    {
        public UsersController(IApplicationService applicationService)
            : base(applicationService) { }

        // GET /api/users/me
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserReadOnlyDTO>> GetCurrentUser()
        {
            if (AppUser == null)
                return Unauthorized();

            var dto = await ApplicationService.UserService.GetByIdAsync(AppUser.Id);
            return Ok(dto);
        }


        // GET /api/users/{id}
        // IS THIS SUPPOSED TO BE REACHED BY ADMIN ONLY OR ALSO BY THE USER HIMSELF?IT SEEMS TO BE BOTH. but users see themselves in dashboard 
        //but can users see other users' info? 
        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserReadOnlyDTO>> GetUserById(int id)
        {
            if (AppUser == null)
                return Unauthorized();

            // Only admin OR the same user can view this account
            if (AppUser.Id != id && !User.IsInRole("Admin"))
                return Forbid();

            UserReadOnlyDTO userReadOnlyDTO = await ApplicationService.UserService.GetByIdAsync(id);
            return Ok(userReadOnlyDTO);
        }


        // GET /api/users
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<UserReadOnlyDTO>>> GetUsers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? username = null,
            [FromQuery] string? email = null,
            [FromQuery] string? userRole = null)
        {
            var filters = new UserFiltersDTO
            {
                Username = username,
                Email = email,
                //UserRole = userRole
            };

            var result = await ApplicationService.UserService.GetPaginatedAsync(pageNumber, pageSize, filters);
            return Ok(result);
        }

        // PUT /api/users/{id}
        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<UserReadOnlyDTO>> UpdateUser(int id, [FromBody] UserUpdateDTO dto)
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

                throw new InvalidArgumentException("User",
                    "Invalid update data: " + System.Text.Json.JsonSerializer.Serialize(errors));     //DO I NEED THIS SERIALIZATION???i THINK NOT BECAUSE IT IS JUST STRING
            }

            // Only the owner or an admin can update
            if (AppUser.Id != id && !User.IsInRole("Admin"))
                return Forbid();

            var updated = await ApplicationService.UserService.UpdateAsync(id, dto);
            return Ok(updated);
        }


        // DELETE /api/users/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await ApplicationService.UserService.DeleteAsync(id);
            return NoContent();
        }


        [Authorize]
        [HttpGet("me/dashboard")]
        public async Task<ActionResult<DashboardDTO>> GetDashboard()
        {
            if (AppUser == null) return Unauthorized();

            var dashboard = await ApplicationService.DashboardService
                .GetDashboardAsync(AppUser.Id);


            return Ok(dashboard);
        }

    }
}
