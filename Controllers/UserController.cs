using ApiMySQL.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using ApiMySQL.Services;

namespace ApiMySQL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("authenticate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [SwaggerOperation(Summary = "Authenticate user", Description = "Authenticates a user and generates a JWT token.")]
        public async Task<IActionResult> Authenticate([FromBody] User model)
        {
            try
            {
                var user = await _userService.GetUserByUsernameAndPassword(model.Username, model.Password);

                if (user == null)
                {
                    _logger.LogWarning("User authentication failed. Invalid username or password.");
                    return Unauthorized();
                }

                var token = _userService.GenerateJwtToken(user);

                _logger.LogInformation("User authentication successful.");
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating user.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
