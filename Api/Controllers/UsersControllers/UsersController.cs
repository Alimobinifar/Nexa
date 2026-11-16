using Application.Interfaces.Users;
using Contracts.Cmmon.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.UsersControllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _users;

        public UsersController(IUserService users) => _users = users;

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateUserDto request, CancellationToken ct)
        {
            try
            {
                var id = await _users.CreateUserAsync(request, ct);
                return Ok(new { UserId = id });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

