using ApiMySQL.Repositories;
using ApiMySQL.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiMySQL.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MuscleGroupController : ControllerBase
    {
        private readonly IMuscleGroupRepository _muscleGroupRepository;
        private readonly ILogger<MuscleGroupController> _logger;

        public MuscleGroupController(IMuscleGroupRepository muscleGroupRepository, ILogger<MuscleGroupController> logger)
        {
            _muscleGroupRepository = muscleGroupRepository;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los grupos musculares
        /// </summary>
        /// <remarks>
        /// Devuelve una lista de objetos de tipo MuscleGroup con todos los grupos musculares.
        /// 
        /// Sample request:
        /// 
        ///     Get /api/MuscleGroup
        ///     
        /// </remarks>
        /// <response code="200">Lista de grupos musculares</response>
        /// <response code="204">No se encontraron grupos musculares</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponse(statusCode: 200, type: typeof(List<MuscleGroup>), description: "successful operation")]
        public async Task<IActionResult> GetAllMuscleGroup()
        {
            try
            {                
                var muscleGroups = await _muscleGroupRepository.GetAllMuscleGroup();

                if (muscleGroups == null || muscleGroups.Count() == 0)
                {
                    _logger.LogError("****Error en la operación GetAllMuscleGroup, no se encontraron grupos musculares");
                    return NoContent();
                }

                _logger.LogInformation("****Operación GetAllMuscleGroup ejecutada correctamente.");
                return Ok(muscleGroups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "****Error en la operación GetAllMuscleGroup.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Obtiene un grupo muscular por su ID
        /// </summary>
        /// <remarks>
        /// Devuelve un objeto de tipo MuscleGroup con el ID especificado.
        /// 
        /// Sample request:
        /// 
        ///     Get /api/MuscleGroup/1
        ///     
        /// </remarks>
        /// <param name="id">Identificador del grupo muscular</param>
        /// <response code="200">Grupo muscular encontrado</response>
        /// <response code="204">Grupo muscular no encontrado</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponse(statusCode: 200, type: typeof(MuscleGroup), description: "successful operation")]
        public async Task<IActionResult> GetMuscleGroup(int id)
        {
            try
            {
                var existingMuscleGroup = await _muscleGroupRepository.GetMuscleGroup(id);

                if (existingMuscleGroup == null)
                {
                    _logger.LogError("****Error en la operación GetMuscleGroup, no existe el grupo muscular especificado");
                    return NoContent();
                }

                _logger.LogInformation("****Operación GetMuscleGroup ejecutada correctamente.");
                return Ok(existingMuscleGroup);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "****Error en la operación GetMuscleGroup.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Inserta un nuevo grupo muscular
        /// </summary>
        /// <remarks>
        /// Recibe un objeto MuscleGroup y lo inserta en la BBDD.
        /// 
        /// Sample request:
        /// 
        ///     {
        ///      "name": "Nuevo Grupo Muscular"
        ///     }
        ///     
        /// </remarks>
        /// <param name="muscleGroup">Objeto MuscleGroup a insertar</param>
        /// <response code="201">Grupo muscular insertado correctamente</response>
        /// <response code="500">Internal server error</response>
        /// <response code="400">Datos incorrectos</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InsertMuscleGroup([FromBody] MuscleGroup muscleGroup)
        {
            try
            {
                if (muscleGroup == null)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var created = await _muscleGroupRepository.InsertMuscleGroup(muscleGroup);
                _logger.LogInformation("****Operación InsertMuscleGroup ejecutada correctamente.");
                return Created("created", created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "****Error en la operación InsertMuscleGroup.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Actualiza un grupo muscular existente
        /// </summary>
        /// <remarks>
        /// Recibe un objeto MuscleGroup y lo actualiza en la BBDD.
        /// 
        /// Sample request:
        /// 
        ///     {
        ///      "id": 4,
        ///      "name": "Grupo Muscular Actualizado",
        ///      "description": "Descripción actualizada del grupo muscular"
        ///     }
        ///     
        /// </remarks>
        /// <param name="muscleGroup">Objeto MuscleGroup a actualizar</param>
        /// <response code="204">MuscleGroup actualizado correctamente</response>
        /// <response code="500">Internal server error</response>
        /// <response code="400">Datos incorrectos o MuscleGroup no encontrado</response>        
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateMuscleGroup([FromBody] MuscleGroup muscleGroup)
        {
            try
            {
                if (muscleGroup == null)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existingMuscleGroup = await _muscleGroupRepository.GetMuscleGroup(muscleGroup.ID);
                if (existingMuscleGroup == null)
                {
                    _logger.LogError("****Error en la operación UpdateMuscleGroup, no existe el grupo muscular a modificar");
                    return BadRequest();
                }

                await _muscleGroupRepository.UpdateMuscleGroup(muscleGroup);
                _logger.LogInformation("****Operación UpdateMuscleGroup ejecutada correctamente.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "****Error en la operación UpdateMuscleGroup.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }
        /// <summary>
        /// Elimina un grupo muscular por su ID
        /// </summary>
        /// <remarks>
        /// Recibe el ID del MuscleGroup que se desea eliminar y lo borra de la BBDD.
        /// 
        /// Sample request:
        /// 
        ///     Delete /api/MuscleGroup?id=10
        ///     
        /// </remarks>
        /// <param name="id">Identificador del MuscleGroup</param>
        /// <response code="200">MuscleGroup eliminado</response>
        /// <response code="204">MuscleGroup no encontrado</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponse(statusCode: 200, type: typeof(bool), description: "MuscleGroup eliminado")]
        public async Task<IActionResult> DeleteMuscleGroup(int id)
        {
            try
            {
                var existingMuscleGroup = await _muscleGroupRepository.GetMuscleGroup(id);
                if (existingMuscleGroup == null)
                {
                    _logger.LogError("****Error en la operación DeleteMuscleGroup, no existe el grupo muscular a eliminar");
                    return NoContent();
                }
                await _muscleGroupRepository.DeleteMuscleGroup(id);
                _logger.LogInformation("****Operación DeleteMuscleGroup ejecutada correctamente.");
                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "****Error en la operación DeleteMuscleGroup.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}
