using ApiMySQL.DTOs;
using ApiMySQL.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ApiMySQL.Repositories;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ApiMySQL.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseController : ControllerBase
    {
        private readonly IExerciseRepository _exerciseRepository;
        private readonly ILogger<ExerciseController> _logger;
        private readonly IMapper _mapper;

        public ExerciseController(IExerciseRepository exerciseRepository, ILogger<ExerciseController> logger, IMapper mapper)
        {
            _exerciseRepository = exerciseRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene los ejercicios de una categoría específica.
        /// </summary>
        /// <remarks>
        /// Devuelve una lista de objetos de tipo ExerciseDto para una categoría dada.
        /// 
        /// Sample request:
        /// 
        ///     Get /api/Exercise/GetCategoryExercises/1
        ///     
        /// </remarks>
        /// <param name="id">Identificador de la categoría</param>
        /// <response code="200">Lista de ejercicios de la categoría</response>
        /// <response code="204">No se encontraron ejercicios para la categoría</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("GetCategoryExercises")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponse(statusCode: 200, type: typeof(List<ExerciseDto>), description: "successful operation")]
        public async Task<IActionResult> GetCategoryExercises(int id)
        {
            try
            {
                var exercises = await _exerciseRepository.GetCategoryExercises(id);

                if (exercises == null || exercises.Count() == 0)
                {
                    _logger.LogError("****Error en la operación GetCategoryExercises, no se encontraron ejercicios para la categoría especificada");
                    return NoContent();
                }

                var exerciseDtos = _mapper.Map<List<ExerciseDto>>(exercises);
                _logger.LogInformation("****Operación GetCategoryExercises ejecutada correctamente.");
                return Ok(exerciseDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "****Error en la operación GetCategoryExercises.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Obtiene un ejercicio por su ID
        /// </summary>
        /// <remarks>
        /// Devuelve un objeto de tipo ExerciseDto con el ID especificado.
        /// 
        /// Sample request:
        /// 
        ///     Get /api/Exercise/1
        ///     
        /// </remarks>
        /// <param name="id">Identificador del ejercicio</param>
        /// <response code="200">Ejercicio encontrado</response>
        /// <response code="204">Ejercicio no encontrado</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponse(statusCode: 200, type: typeof(ExerciseDto), description: "successful operation")]
        public async Task<IActionResult> GetExercise(int id)
        {
            try
            {
                var existingExercise = await _exerciseRepository.GetExercise(id);

                if (existingExercise == null)
                {
                    _logger.LogError("****Error en la operación GetExercise, no existe el ejercicio especificado");
                    return NoContent();
                }

                var exerciseDto = _mapper.Map<ExerciseDto>(existingExercise);
                _logger.LogInformation("****Operación GetExercise ejecutada correctamente.");
                return Ok(exerciseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "****Error en la operación GetExercise.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Inserta un nuevo ejercicio
        /// </summary>
        /// <remarks>
        /// Recibe un objeto ExerciseDto y lo inserta en la BBDD.
        /// 
        /// Sample request:
        /// 
        ///     {
        ///      "description": "string",
        ///      "categoryID": 1,
        ///      "image": "imagen_del_ejercicio"
        ///     }
        ///     
        /// </remarks>
        /// <param name="exerciseDto">Objeto ExerciseDto a insertar</param>
        /// <response code="201">Ejercicio insertado correctamente</response>
        /// <response code="500">Internal server error</response>
        /// <response code="400">Datos incorrectos</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InsertExercise([FromBody] ExerciseDto exerciseDto)
        {
            try
            {
                if (exerciseDto == null)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var exercise = _mapper.Map<Exercise>(exerciseDto);
                exercise.LastUpdated = DateTime.Now;

                var created = await _exerciseRepository.InsertExercise(exercise);
                _logger.LogInformation("****Operación InsertExercise ejecutada correctamente.");
                return Created("created", created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "****Error en la operación InsertExercise.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Actualiza un ejercicio existente
        /// </summary>
        /// <remarks>
        /// Recibe un objeto ExerciseDto y lo actualiza en la BBDD.
        /// 
        /// Sample request:
        /// 
        ///     {
        ///      "id": 4,
        ///      "name": "Ejercicio Actualizado",
        ///      "categoryId": 1,
        ///      "image": "imagen_del_ejercicio"
        ///     }
        ///     
        /// </remarks>
        /// <param name="id">Identificador del ejercicio a actualizar</param>
        /// <param name="exerciseDto">Objeto ExerciseDto a actualizar</param>
        /// <response code="204">Ejercicio actualizado correctamente</response>
        /// <response code="500">Internal server error</response>
        /// <response code="400">Datos incorrectos o ejercicio no encontrado</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateExercise([FromBody] ExerciseDto exerciseDto)
        {
            try
            {
                if (exerciseDto == null)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existingExercise = await _exerciseRepository.GetExercise(exerciseDto.ID);
                if (existingExercise == null)
                {
                    _logger.LogError("****Error en la operación UpdateExercise, no existe el ejercicio a actualizar");
                    return BadRequest();
                }

                var exercise = _mapper.Map<Exercise>(exerciseDto);
                exercise.LastUpdated = DateTime.Now;

                await _exerciseRepository.UpdateExercise(exercise);
                _logger.LogInformation("****Operación UpdateExercise ejecutada correctamente.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "****Error en la operación UpdateExercise.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Elimina un ejercicio por su ID
        /// </summary>
        /// <remarks>
        /// Elimina un ejercicio de la BBDD según el ID especificado.
        /// 
        /// Sample request:
        /// 
        ///     Delete /api/Exercise/1
        ///     
        /// </remarks>
        /// <param name="id">Identificador del ejercicio</param>
        /// <response code="204">Ejercicio eliminado correctamente</response>
        /// <response code="500">Internal server error</response>
        /// <response code="400">Ejercicio no encontrado</response>
        [HttpDelete("DeleteExercise")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteExercise(int id)
        {
            try
            {
                var existingExercise = await _exerciseRepository.GetExercise(id);
                if (existingExercise == null)
                {
                    _logger.LogError("****Error en la operación DeleteExercise, no existe el ejercicio a eliminar");
                    return BadRequest();
                }

                await _exerciseRepository.DeleteExercise(id);
                _logger.LogInformation("****Operación DeleteExercise ejecutada correctamente.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "****Error en la operación DeleteExercise.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}
