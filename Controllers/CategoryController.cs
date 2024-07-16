using ApiMySQL.DTOs;
using ApiMySQL.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using ApiMySQL.Repositories;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

namespace ApiMySQL.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryController> _logger;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository, ILogger<CategoryController> logger, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene todas las categorías de grupos musculares asociadas a un grupo muscular específico
        /// </summary>
        /// <remarks>
        /// Devuelve una lista de objetos de tipo CategoryDto asociados al grupo muscular con el ID especificado.
        /// 
        /// Sample request:
        /// 
        ///     Get /api/Category?id=10
        ///     
        /// </remarks>
        /// <param name="id">Identificador del grupo muscular</param>
        /// <response code="200">Lista de categorías asociadas al grupo muscular</response>
        /// <response code="204">No se encontraron categorías para el grupo muscular especificado</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponse(statusCode: 200, type: typeof(List<CategoryDto>), description: "successful operation")]
        public async Task<IActionResult> GetMuscleGroupCategories(int id)
        {
            try
            {
                var categories = await _categoryRepository.GetMuscleGroupCategories(id);
                var categoriesDTO = _mapper.Map<List<CategoryDto>>(categories);

                if (categoriesDTO == null || categoriesDTO.Count() == 0)
                {
                    _logger.LogError("****Error en la operación GetMuscleGroupCategories, no se encontraron categorías para el grupo muscular especificado");
                    return NoContent();
                }

                _logger.LogInformation("****Operación GetMuscleGroupCategories ejecutada correctamente.");
                return Ok(categoriesDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "****Error en la operación GetMuscleGroupCategories.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Obtiene una categoría específica por su ID
        /// </summary>
        /// <remarks>
        /// Devuelve un objeto de tipo CategoryDto con el ID especificado.
        /// 
        /// Sample request:
        /// 
        ///     Get /api/Category/1
        ///     
        /// </remarks>
        /// <param name="id">Identificador de la categoría</param>
        /// <response code="200">Categoría encontrada</response>
        /// <response code="204">Categoría no encontrada</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponse(statusCode: 200, type: typeof(CategoryDto), description: "successful operation")]
        public async Task<IActionResult> GetCategory(int id)
        {
            try
            {
                var existingCategory = await _categoryRepository.GetCategory(id);
                var existingCategoryDto = _mapper.Map<CategoryDto>(existingCategory);

                if (existingCategoryDto == null)
                {
                    _logger.LogError("****Error en la operación GetCategory, no existe la categoría especificada");
                    return NoContent();
                }

                _logger.LogInformation("****Operación GetCategory ejecutada correctamente.");
                return Ok(existingCategoryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "****Error en la operación GetCategory.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Inserta una nueva categoría
        /// </summary>
        /// <remarks>
        /// Recibe un objeto CategoryDto y lo inserta en la BBDD.
        /// 
        /// Sample request:
        /// 
        ///     {
        ///      "name": "Nueva Categoría",
        ///      "muscleGroupID": 10
        ///     }
        ///     
        /// </remarks>
        /// <param name="CategoryDto">Objeto CategoryDto a insertar</param>
        /// <response code="201">Categoría insertada correctamente</response>
        /// <response code="500">Internal server error</response>
        /// <response code="400">Datos incorrectos o grupo muscular no encontrado</response>        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InsertCategory([FromBody] CategoryDto CategoryDto)
        {
            try
            {
                if (CategoryDto == null)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (CategoryDto.MuscleGroupID == 0)
                {
                    _logger.LogError("****Error en la operación InsertCategory, no existe el grupo muscular");
                    return BadRequest();
                }

                var category = _mapper.Map<Category>(CategoryDto);
                category.LastUpdate = DateTime.Now;

                var created = await _categoryRepository.InsertCategory(category);

                if (!created)
        {
            _logger.LogError("****Error en la operación InsertCategory, no se pudo insertar la categoría");
            return StatusCode(500, "Error interno del servidor.");
        }

        var categoryDtoResult = _mapper.Map<CategoryDto>(category);
        _logger.LogInformation("****Operación InsertCategory ejecutada correctamente.");
        return CreatedAtAction(nameof(GetCategory), new { id = categoryDtoResult.ID }, categoryDtoResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "****Error en la operación InsertCategory.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Actualiza una categoría existente
        /// </summary>
        /// <remarks>
        /// Recibe un objeto CategoryDto y lo actualiza en la BBDD.
        /// 
        /// Sample request:
        /// 
        ///     {
        ///      "id": 4,
        ///      "name": "Categoría Actualizada",
        ///      "muscleGroupID": 10
        ///     }
        ///     
        /// </remarks>
        /// <param name="CategoryDto">Objeto CategoryDto a actualizar</param>
        /// <response code="204">Categoría actualizada correctamente</response>
        /// <response code="500">Internal server error</response>
        /// <response code="400">Datos incorrectos o categoría no encontrada</response>        
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryDto CategoryDto)
        {
            try
            {
                if (CategoryDto == null)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Verificar si la categoría existe antes de intentar actualizarla
                var existingCategory = await _categoryRepository.GetCategory(CategoryDto.ID);
                if (existingCategory == null)
                {
                    _logger.LogError("****Error en la operación UpdateCategory, no existe la categoría a actualizar");
                    return BadRequest();
                }

                var categoryToUpdate = _mapper.Map<Category>(CategoryDto);
                categoryToUpdate.LastUpdate = DateTime.Now;

                await _categoryRepository.UpdateCategory(categoryToUpdate);

                _logger.LogInformation("****Operación UpdateCategory ejecutada correctamente.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "****Error en la operación UpdateCategory.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Elimina una categoría por su ID
        /// </summary>
        /// <remarks>
        /// Recibe el ID de la categoría que se desea eliminar y la borra de la BBDD.
        /// 
        /// Sample request:
        /// 
        ///     Delete /api/Category/1
        ///     
        /// </remarks>
        /// <param name="id">Identificador de la categoría</param>
        /// <response code="204">Categoría eliminada correctamente</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerResponse(statusCode: 204, type: typeof(bool), description: "Category eliminada")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                // Verificar si la categoría existe antes de intentar eliminarla
                var existingCategory = await _categoryRepository.GetCategory(id);
                if (existingCategory == null)
                {
                    _logger.LogError("****Error en la operación DeleteCategory, no existe la categoría a eliminar");
                    return NoContent();
                }

                await _categoryRepository.DeleteCategory(id);

                _logger.LogInformation("****Operación DeleteCategory ejecutada correctamente.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "****Error en la operación DeleteCategory.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}
