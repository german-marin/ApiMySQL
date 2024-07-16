using ApiMySQL.Repositories;
using ApiMySQL.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ApiMySQL.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ApiMySQL.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingLineController : ControllerBase
    {
        private readonly ITrainingLineRepository _trainingLineRepository;
        private readonly ILogger<TrainingLineController> _logger;
        private readonly IMapper _mapper;

        public TrainingLineController(ITrainingLineRepository trainingLineRepository, ILogger<TrainingLineController> logger, IMapper mapper)
        {
            _trainingLineRepository = trainingLineRepository;
            _logger = logger;
            _mapper = mapper;
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
        [SwaggerResponse(statusCode: 200, type: typeof(List<TrainingLineDto>), description: "successful operation")]
        public async Task<IActionResult> GetTrainingLinesOfTraining(int id)
        {
            try
            {
                var trainingLines = await _trainingLineRepository.GetTrainingLinesOfTraining(id);
                var trainingLineDtos = _mapper.Map<IEnumerable<TrainingLineDto>>(trainingLines);
                return Ok(trainingLineDtos);
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
        [SwaggerResponse(statusCode: 200, type: typeof(TrainingLineDto), description: "successful operation")]
        public async Task<IActionResult> GetTrainingLine(int id)
        {
            try
            {
                var trainingLine = await _trainingLineRepository.GetTrainingLine(id);
                if (trainingLine == null)
                {
                    return NotFound();
                }

                var trainingLineDto = _mapper.Map<TrainingLineDto>(trainingLine);
                return Ok(trainingLineDto);
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
        ///        "ExerciseID": 1,
        ///        "TrainingID": 1,
        ///        "Sets": "3x10",
        ///        "Repetitions": "10",
        ///        "Weight": "20kg",
        ///        "Recovery": "1 min",
        ///        "Others": "",
        ///        "Notes": "Ninguna",
        ///        "Grip": "Neutral",
        ///        "LastUpdated": "2022-01-01T00:00:00Z"
        ///     }
        /// </remarks>
        /// <param name="trainingLineDto">Objeto TrainingLine que se va a insertar</param>
        /// <response code="200">TrainingLine insertada correctamente</response>
        /// <response code="500">Internal server error</response>
        /// <response code="400">Datos incorrectos o Training no encontrado</response>        
        [HttpPost("InsertTrainingLine")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertTrainingLine([FromBody] TrainingLineDto trainingLineDto)
        {
            try
            {
                if (trainingLineDto == null)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var trainingLine = _mapper.Map<TrainingLine>(trainingLineDto);
                trainingLine.LastUpdated = DateTime.Now;
                var result = await _trainingLineRepository.InsertTrainingLine(trainingLine);

                if (!result)
                {
                    _logger.LogError("****Error en la operación InsertTrainingLine.");
                    return StatusCode(500, "No se pudo insertar la línea de entrenamiento.");
                }

                _logger.LogInformation("****Operación InsertTrainingLine ejecutada correctamente.");
                return Created("created", result);
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
        /// Recibe un objeto TrainingLine y actualiza la línea de entrenamiento correspondiente en la BBDD.
        /// 
        /// Sample request:
        /// 
        ///     {
        ///        "ID": 1,
        ///        "ExerciseID": 1,
        ///        "TrainingID": 1,
        ///        "Sets": "3x10",
        ///        "Repetitions": "10",
        ///        "Weight": "20kg",
        ///        "Recovery": "1 min",
        ///        "Others": "",
        ///        "Notes": "Ninguna",
        ///        "Grip": "Neutral",
        ///        "LastUpdated": "2022-01-01T00:00:00Z"
        ///     }
        /// </remarks>
        /// <param name="trainingLineDto">Objeto TrainingLine que se va a actualizar</param>
        /// <response code="200">TrainingLine actualizada correctamente</response>
        /// <response code="500">Internal server error</response>
        /// <response code="400">Datos incorrectos o TrainingLine no encontrada</response>     
        [HttpPut("UpdateTrainingLine")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateTrainingLine([FromBody] TrainingLineDto trainingLineDto)
        {
            try
            {
                if (trainingLineDto == null)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var trainingLine = _mapper.Map<TrainingLine>(trainingLineDto);
                // Verificar si el entrenamiento existe antes de intentar eliminarlo
                //var existingTrainingLine = await _trainingLineRepository.GetTrainingLine(trainingLine.ID);
                //if (existingTrainingLine == null)
                //{
                //    // El entrenamiento no existe, devolver código 400
                //    _logger.LogError("****Error en la operación UpdateTrainingLine, no existe el training a modificar");
                //    return BadRequest();
                //}
                //trainingLine.LastUpdated = DateTime.Now;

                var result = await _trainingLineRepository.UpdateTrainingLine(trainingLine);

                if (!result)
                {
                    _logger.LogError("****Error en la operación UpdateTrainingLine.");
                    return StatusCode(500, "No se pudo actualizar la línea de entrenamiento.");
                }

                _logger.LogInformation("****Operación UpdateTrainingLine ejecutada correctamente.");
                return Ok();
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
        /// Elimina una línea de entrenamiento de la BBDD según su ID.
        /// 
        /// Sample request:
        /// 
        ///     Delete /api/TrainingLine/1
        ///     
        /// </remarks>
        /// <param name="id">Identificador de la línea de entrenamiento que se va a eliminar</param>
        /// <response code="200">TrainingLine eliminada correctamente</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("DeleteTrainingLine")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTrainingLine(int id)
        {
            try
            {
                var result = await _trainingLineRepository.DeleteTrainingLine(id);

                if (!result)
                {
                    _logger.LogError("****Error en la operación DeleteTrainingLine.");
                    return StatusCode(500, "No se pudo eliminar la línea de entrenamiento.");
                }

                _logger.LogInformation("****Operación DeleteTrainingLine ejecutada correctamente.");
                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "****Error en la operación DeleteTrainingLine.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}
