using ApiMySQL.Repositories;
using ApiMySQL.Model;
using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;

namespace ApiMySQL.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingLineController : ControllerBase
    {
        private readonly ITrainingLineRepository _trainingLineRepository;
        private readonly ILogger<TrainingLineController> _logger;

        public TrainingLineController(ITrainingLineRepository trainingLineRepository, ILogger<TrainingLineController> logger)
        {
            _trainingLineRepository = trainingLineRepository;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las líneas de entrenamiento de un entrenamiento específico
        /// </summary>
        /// <remarks>
        /// Devuelve una lista de objetos de tipo TrainingLine asociados al Training con el ID especificado.
        /// 
        /// Sample request:
        /// 
        ///     Get /api/TrainingLine/GetTrainingLinesOfTraining?id=10
        ///     
        /// </remarks>
        /// <param name="id">Identificador del Training</param>
        /// <response code="200">Lista de TrainingLines asociadas al Training</response>
        /// <response code="204">No se encontraron TrainingLines para el Training especificado</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("GetTrainingLinesOfTraining")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponse(statusCode: 200, type: typeof(List<TrainingLine>), description: "successful operation")]
        public async Task<IActionResult> GetTrainingLinesOfTraining(int id)
        {
            try
            {
                var trainingLines = await _trainingLineRepository.GetTrainingLinesOfTraining(id);

                if (trainingLines == null || trainingLines.Count() == 0)
                {
                    _logger.LogError("****Error en la operación GetTrainingLinesOfTraining, no se encontraron TrainingLines para el Training especificado");
                    return NoContent();
                }

                _logger.LogInformation("****Operación GetTrainingLinesOfTraining ejecutada correctamente.");
                return Ok(trainingLines);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "****Error en la operación GetTrainingLinesOfTraining.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Obtiene una línea de entrenamiento específica por su ID
        /// </summary>
        /// <remarks>
        /// Devuelve un objeto de tipo TrainingLine con el ID especificado.
        /// 
        /// Sample request:
        /// 
        ///     Get /api/TrainingLine/1
        ///     
        /// </remarks>
        /// <param name="id">Identificador de la TrainingLine</param>
        /// <response code="200">TrainingLine encontrada</response>
        /// <response code="204">TrainingLine no encontrada</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponse(statusCode: 200, type: typeof(TrainingLine), description: "successful operation")]
        public async Task<IActionResult> GetTrainingLine(int id)
        {
            try
            {
                var existingTrainingLine = await _trainingLineRepository.GetTrainingLine(id);

                if (existingTrainingLine == null)
                {
                    // El entrenamiento no existe, devolver código 204
                    _logger.LogError("****Error en la operación GetTrainingLine, no existe el training a eliminar");
                    return NoContent();
                }
                _logger.LogInformation("****Operación GetTrainingLine ejecutada correctamente.");
                return Ok(existingTrainingLine);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "****Error en la operación GetTrainingLine.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Inserta una nueva línea de entrenamiento
        /// </summary>
        /// <remarks>
        /// Recibe un objeto TrainingLine y lo inserta en la BBDD.
        /// 
        /// Sample request:
        /// 
        ///     {
        ///      "idExercise": 142,
        ///      "idTraining": 10,
        ///      "series": "4",
        ///      "repetition": "12-10-8-6",
        ///      "weight": "a tope",
        ///      "recovery": "poco descanso",
        ///      "others": "otras cosas",
        ///      "notes": "dia 3"
        ///     }
        ///     
        /// </remarks>
        /// <param name="trainingLine">Objeto TrainingLine a insertar</param>
        /// <response code="201">TrainingLine insertada correctamente</response>
        /// <response code="500">Internal server error</response>
        /// <response code="400">Datos incorrectos o Training no encontrado</response>        
        [HttpPost("InsertTrainingLine")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InsertTrainingLine([FromBody] TrainingLine trainingLine)
        {
            try
            {
                if (trainingLine == null)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var created = await _trainingLineRepository.InsertTrainingLine(trainingLine);
                _logger.LogInformation("****Operación InsertTrainingLine ejecutada correctamente.");
                return Created("created", created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "****Error en la operación InsertTrainingLine.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Actualiza una línea de entrenamiento existente
        /// </summary>
        /// <remarks>
        /// Recibe un objeto TrainingLine y lo actualiza en la BBDD.
        /// 
        /// Sample request:
        /// 
        ///     {
        ///      "id": 4,
        ///      "idExercise": 142,
        ///      "idTraining": 10,
        ///      "series": "4",
        ///      "repetition": "12-10-8-6",
        ///      "weight": "a tope",
        ///      "recovery": "poco descanso",
        ///      "others": "otras cosas",
        ///      "notes": "dia 3"
        ///     }
        ///     
        /// </remarks>
        /// <param name="trainingLine">Objeto TrainingLine a actualizar</param>
        /// <response code="204">TrainingLine actualizada correctamente</response>
        /// <response code="500">Internal server error</response>
        /// <response code="400">Datos incorrectos o TrainingLine no encontrada</response>        
        [HttpPut("UpdateTrainingLine")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateTrainingLine([FromBody] TrainingLine trainingLine)
        {
            try
            {
                if (trainingLine == null)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                // Verificar si el entrenamiento existe antes de intentar eliminarlo
                var existingTrainingLine = await _trainingLineRepository.GetTrainingLine(trainingLine.ID);
                if (existingTrainingLine == null)
                {
                    // El entrenamiento no existe, devolver código 400
                    _logger.LogError("****Error en la operación UpdateTrainingLine, no existe el training a modificar");
                    return BadRequest();
                }

                await _trainingLineRepository.UpdateTrainingLine(trainingLine);
                _logger.LogInformation("****Operación UpdateTrainingLine ejecutada correctamente.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "****Error en la operación UpdateTrainingLine.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Elimina una línea de entrenamiento por su ID
        /// </summary>
        /// <remarks>
        /// Recibe el ID de la TrainingLine que se desea eliminar y la borra de la BBDD.
        /// 
        /// Sample request:
        /// 
        ///     Delete /api/TrainingLine?id=10
        ///     
        /// </remarks>
        /// <param name="id">Identificador de la TrainingLine</param>
        /// <response code="200">TrainingLine eliminada</response>
        /// <response code="204">TrainingLine no encontrada</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("DeleteTrainingLine")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponse(statusCode: 200, type: typeof(bool), description: "TrainingLine eliminada")]
        public async Task<IActionResult> DeleteTrainingLine(int id)
        {
            try
            {
                // Verificar si el entrenamiento existe antes de intentar eliminarlo
                var existingTrainingLine = await _trainingLineRepository.GetTrainingLine(id);
                if (existingTrainingLine == null)
                {
                    // El entrenamiento no existe, devolver código 204
                    _logger.LogError("****Error en la operación DeleteTrainingLine, no existe el training a eliminar");
                    return NoContent();
                }
                await _trainingLineRepository.DeleteTrainingLine(id);
                _logger.LogInformation("****Operación DeleteTrainingLine ejecutada correctamente.");
                return Ok(true); // Devuelve true si la eliminación fue exitosa
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "****Error en la operación DeleteTrainingLine.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }       
    }
}
