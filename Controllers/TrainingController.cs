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
using System;
using ApiMySQL.DTOs;

namespace ApiMySQL.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingController : ControllerBase
    {
        private readonly ITrainingRepository _trainingRepository;
        private readonly ILogger<TrainingController> _logger;
        private readonly IMapper _mapper;

        public TrainingController(ITrainingRepository trainingRepository, ILogger<TrainingController> logger, IMapper mapper)
        {
            _trainingRepository = trainingRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Devuelve un único Training
        /// </summary>
        /// <remarks>
        /// Devuelve un objeto del tipo training con el id seleccionado 
        /// 
        /// Sample request:
        /// 
        ///     Get /api/Training/10
        ///     
        /// </remarks>
        /// <param name="id">Identificador del ID a recuperar</param>
        /// <response code="200">JSON con un objeto del tipo Training</response>
        /// <response code="204">Training no encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponse(statusCode: 200, type: typeof(TrainingDto), description: "successful operation")]
        public async Task<IActionResult> GetTraining(int id)
        {
            try
            {
                _logger.LogInformation("****Operación GetTraining ejecutada correctamente.");
                var training = await _trainingRepository.GetTraining(id);
                if (training == null)
                {
                    _logger.LogError("****Error en la operación GetTraining, no se encontró el training");
                    return NoContent();
                }
                var trainingDto = _mapper.Map<TrainingDto>(training);
                return Ok(trainingDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "****Error en la operación GetTraining.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Inserta un nuevo Training
        /// </summary>
        /// <remarks>
        /// Recibe un objeto Training y lo inserta en la BBDD 
        /// 
        /// Sample request:
        /// 
        ///     {          
        ///      "description": "rutina buena",
        ///      "startDate": "2023-11-30T00:00:00",
        ///      "endDate": "2023-12-14T00:00:00",     
        ///      "idClient": 2,
        ///      "notes": "rutina suave de tres dias"
        ///     }
        ///     
        /// </remarks>
        /// <param name="training">Crea un nuevo training</param>
        /// <response code="201">Successful operation</response>
        /// <response code="500">Internal server error</response>
        /// <response code="400">Incorrect Data or client not exist</response>  
        [Route("InsertTraining")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InsertTraining([FromBody] TrainingDto trainingDto)
        {
            try
            {
                if (trainingDto == null)
                {
                    _logger.LogError("****Error en la operación InsertTraining. Training nulo");
                    return BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("****Error en la operación InsertTraining. ModelState Invalid");
                    return BadRequest(ModelState);
                }
                //comprobamos si existe el cliente
                if (await _trainingRepository.CustomerExist(trainingDto.CustomerID) is false)
                {
                    _logger.LogInformation("****El cliente seleccionado no existe");
                    return BadRequest();
                }

                var training = _mapper.Map<Training>(trainingDto);
                training.LastUpdate = DateTime.Now;
                var created = await _trainingRepository.InsertTraining(training);
                _logger.LogInformation("****Operación InsertTraining ejecutada correctamente.");
                return Created("created", created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "****Error en la operación InsertTraining.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Modifica un Training existente
        /// </summary>
        /// <remarks>
        /// Recibe un objeto Training y lo updata en la BBDD 
        /// 
        /// Sample request:
        /// 
        ///     { 
        ///      "id": 1,
        ///      "description": "rutina buena",
        ///      "startDate": "2023-11-30T00:00:00",
        ///      "endDate": "2023-12-14T00:00:00",     
        ///      "idClient": 2,
        ///      "notes": "rutina suave de tres dias"
        ///     }
        ///     
        /// </remarks>
        /// <param name="training">Objeto training a modificar</param>
        /// <response code="204">Successful operation</response>
        /// <response code="500">Internal server error</response>
        /// <response code="400">Incorrect Data or client not exist</response>        
        [Route("UpdateTraining")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateTraining([FromBody] TrainingDto trainingDto)
        {
            try
            {
                if (trainingDto == null)
                    return BadRequest();
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                //comprobamos si existe el cliente
                if (await _trainingRepository.CustomerExist(trainingDto.CustomerID) is false)
                {
                    _logger.LogInformation("****El cliente seleccionado no existe");
                    return BadRequest();
                }

                var training = _mapper.Map<Training>(trainingDto);
                training.LastUpdate = DateTime.Now;
                await _trainingRepository.UpdateTraining(training);
                _logger.LogInformation("****Operación UpdateTraining ejecutada correctamente.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "****Error en la operación UpdateTraining.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Elimina un único Training
        /// </summary>
        /// <remarks>
        /// Recibe el ID del Training que se desea eliminar, y se borra de la BBDD
        /// 
        /// Sample request:
        /// 
        ///     Delete /api/Training/DeleteTraining?id=10
        ///     
        /// </remarks>
        /// <param name="id">Identificador del ID a eliminar</param>
        /// <response code="200">Training deleted</response>
        /// <response code="204">Training not found</response>
        /// <response code="500">Internal server error</response>
        [Route("DeleteTraining")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponse(statusCode: 200, type: typeof(bool), description: "successful operation")]
        public async Task<IActionResult> DeleteTraining(int id)
        {
            try
            {
                // Verificar si el entrenamiento existe antes de intentar eliminarlo
                var existingTraining = await _trainingRepository.GetTraining(id);
                if (existingTraining == null)
                {
                    // El entrenamiento no existe, devolver código 204
                    _logger.LogError("****Error en la operación DeleteTraining, no existe el training a eliminar");
                    return NoContent();
                }

                // El entrenamiento existe, proceder con la eliminación
                await _trainingRepository.DeleteTraining(id);
                _logger.LogInformation("****Operación DeleteTraining ejecutada correctamente.");
                return Ok(true); // Devuelve true si la eliminación fue exitosa
            }
            catch (Exception ex)
            {
                // Loguear el error
                _logger.LogError(ex, "****Error en la operación DeleteTraining.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Elimina un Training y sus TrainingsLines
        /// </summary>
        /// <remarks>
        /// Recibe el ID del Training que se desea eliminar, y se borra de la BBDD tanto el training como todas sus TrainingLines asociadas.
        /// 
        /// Sample request:
        /// 
        ///     Delete /api/Training/DeleteTrainingAndTrainingLines?id=10
        ///     
        /// </remarks>
        /// <param name="id">Identificador del ID a eliminar</param>
        /// <response code="200">Training deleted</response>
        /// <response code="204">Training not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("DeleteTrainingAndTrainingLines")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponse(statusCode: 200, type: typeof(bool), description: "successful operation")]
        public async Task<IActionResult> DeleteTrainingAndTrainingLines(int id)
        {
            try
            {
                // Verificar si el entrenamiento existe antes de intentar eliminarlo
                var existingTraining = await _trainingRepository.GetTraining(id);
                if (existingTraining == null)
                {
                    // El entrenamiento no existe, devolver código 204
                    _logger.LogError("****Error en la operación DeleteTrainingAndTrainingLines, no existe el training a eliminar");
                    return NoContent();
                }

                // El entrenamiento existe, proceder con la eliminación
                await _trainingRepository.DeleteTrainingAndTrainingLines(id);
                _logger.LogInformation("****Operación DeleteTrainingAndTrainingLines ejecutada correctamente.");
                return Ok(true); // Devuelve true si la eliminación fue exitosa
            }
            catch (Exception ex)
            {
                // Loguear el error
                _logger.LogError(ex, "****Error en la operación DeleteTrainingAndTrainingLines.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }
        /// <summary>
        /// Devuelve todos los Trainings
        /// </summary>
        /// <remarks>
        /// Devuelve la lista completa de objetos del tipo training  
        /// 
        /// Sample request:
        /// 
        ///     Get /api/GetAllTrainings
        ///     
        /// </remarks>
        /// <response code="200">JSON con un objeto del tipo Training</response>
        /// <response code="204">Trainings no encontrados</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("GetAllTrainings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Training>), description: "successful operation")]
        public async Task<IActionResult> GetAllTrainings()
        {
            try
            {
                var trainings = await _trainingRepository.GetAllTrainings();

                if (trainings == null || !trainings.Any())
                {
                    _logger.LogError("****Error en la operación GetAllTrainings, trainings no encontrados");
                    return NoContent();
                }

                _logger.LogInformation("****Operación GetAllTrainings ejecutada correctamente.");

                // Mapear los trainings a TrainingDTOs antes de devolverlos
                var trainingsDTOs = _mapper.Map<IEnumerable<TrainingDto>>(trainings);

                return Ok(trainingsDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "****Error en la operación GetAllTrainings.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}
