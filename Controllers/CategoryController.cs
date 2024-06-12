using ApiMySQL.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using ApiMySQL.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace ApiMySQL.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryRepository categoryRepository, ILogger<CategoryController> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las categorías de grupos musculares asociadas a un grupo muscular específico
        /// </summary>
        /// <remarks>
        /// Devuelve una lista de objetos de tipo Category asociados al grupo muscular con el ID especificado.
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
        [SwaggerResponse(statusCode: 200, type: typeof(List<Category>), description: "successful operation")]
        public async Task<IActionResult> GetMuscleGroupCategories(int id)
        {
            try
            {
                var categories = await _categoryRepository.GetMuscleGroupCategories(id);

                if (categories == null || categories.Count() == 0)
                {
                    _logger.LogError("****Error en la operación GetMuscleGroupCategories, no se encontraron categorías para el grupo muscular especificado");
                    return NoContent();
                }

                _logger.LogInformation("****Operación GetMuscleGroupCategories ejecutada correctamente.");
                return Ok(categories);
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
        /// Devuelve un objeto de tipo Category con el ID especificado.
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
        [SwaggerResponse(statusCode: 200, type: typeof(Category), description: "successful operation")]
        public async Task<IActionResult> GetCategory(int id)
        {
            try
            {
                var existingCategory = await _categoryRepository.GetCategory(id);

                if (existingCategory == null)
                {
                    _logger.LogError("****Error en la operación GetCategory, no existe la categoría especificada");
                    return NoContent();
                }

                _logger.LogInformation("****Operación GetCategory ejecutada correctamente.");
                return Ok(existingCategory);
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
        /// Recibe un objeto Category y lo inserta en la BBDD.
        /// 
        /// Sample request:
        /// 
        ///     {
        ///      "name": "Nueva Categoría",
        ///      "idMuscleGroup": 10
        ///     }
        ///     
        /// </remarks>
        /// <param name="category">Objeto Category a insertar</param>
        /// <response code="201">Categoría insertada correctamente</response>
        /// <response code="500">Internal server error</response>
        /// <response code="400">Datos incorrectos o grupo muscular no encontrado</response>        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InsertCategory([FromBody] Category category)
        {
            try
            {
                if (category == null)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (category.MuscleGroupID == 0)
                {
                    _logger.LogError("****Error en la operación InsertCategory, no existe el grupo muscular");
                    return BadRequest();
                }                    

                var created = await _categoryRepository.InsertCategory(category);
                _logger.LogInformation("****Operación InsertCategory ejecutada correctamente.");
                return Created("created", created);
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
        /// Recibe un objeto Category y lo actualiza en la BBDD.
        /// 
        /// Sample request:
        /// 
        ///     {
        ///      "id": 4,
        ///      "name": "Categoría Actualizada",
        ///      "idMuscleGroup": 10
        ///     }
        ///     
        /// </remarks>
        /// <param name="category">Objeto Category a actualizar</param>
        /// <response code="204">Categoría actualizada correctamente</response>
        /// <response code="500">Internal server error</response>
        /// <response code="400">Datos incorrectos o categoría no encontrada</response>        
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateCategory([FromBody] Category category)
        {
            try
            {
                if (category == null)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Verificar si la categoría existe antes de intentar actualizarla
                var existingCategory = await _categoryRepository.GetCategory(category.ID);
                if (existingCategory == null)
                {
                    _logger.LogError("****Error en la operación UpdateCategory, no existe la categoría a actualizar");
                    return BadRequest();
                }

                await _categoryRepository.UpdateCategory(category);
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
