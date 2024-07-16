using ApiMySQL.DTOs;
using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, IMapper mapper, ILogger<UserController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Authenticates a user and generates a JWT token.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/user/authenticate
        ///     {
        ///        "username": "sampleuser",
        ///        "password": "samplepassword"
        ///     }
        ///
        /// </remarks>
        /// <param name="model">User credentials.</param>
        /// <returns>Returns a JWT token if authentication is successful.</returns>
        [HttpPost("authenticate")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [SwaggerOperation(Summary = "Authenticate user", Description = "Authenticates a user and generates a JWT token.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns JWT token if authentication is successful.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication failed.")]
        public async Task<IActionResult> Authenticate([FromBody] UserDto model)
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
