using ApiMySQL.Model;
using ApiMySQL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace ApiMySQL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerRepository customerRepository, ILogger<CustomerController> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los clientes
        /// </summary>
        /// <remarks>
        /// Devuelve una lista de objetos de tipo Customer.
        /// </remarks>
        /// <response code="200">Lista de clientes</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<Customer>), Description = "successful operation")]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                var customers = await _customerRepository.GetAllCustomers();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la operación GetAllCustomers.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Obtiene un cliente específico por su ID
        /// </summary>
        /// <remarks>
        /// Devuelve un objeto de tipo Customer con el ID especificado.
        /// </remarks>
        /// <param name="id">Identificador del cliente</param>
        /// <response code="200">Cliente encontrado</response>
        /// <response code="404">Cliente no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Customer), Description = "successful operation")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            try
            {
                var customer = await _customerRepository.GetCustomer(id);
                if (customer == null)
                {
                    _logger.LogError("Error en la operación GetCustomer, no existe el cliente especificado");
                    return NotFound();
                }
                return Ok(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la operación GetCustomer.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Inserta un nuevo cliente
        /// </summary>
        /// <remarks>
        /// Recibe un objeto Customer y lo inserta en la BBDD.
        /// </remarks>
        /// <param name="customer">Objeto Customer a insertar</param>
        /// <response code="201">Cliente insertado correctamente</response>
        /// <response code="400">Datos incorrectos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertCustomer([FromBody] Customer customer)
        {
            try
            {
                if (customer == null || !ModelState.IsValid)
                {
                    _logger.LogError("Error en la operación InsertCustomer, datos incorrectos");
                    return BadRequest();
                }

                var result = await _customerRepository.InsertCustomer(customer);
                if (result)
                {
                    _logger.LogInformation("Operación InsertCustomer ejecutada correctamente.");
                    return CreatedAtAction(nameof(GetCustomer), new { id = customer.ID }, customer);
                }
                return StatusCode(500, "A problem happened while handling your request.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la operación InsertCustomer.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Actualiza un cliente existente
        /// </summary>
        /// <remarks>
        /// Recibe un objeto Customer y lo actualiza en la BBDD.
        /// </remarks>
        /// <param name="id">Identificador del cliente</param>
        /// <param name="customer">Objeto Customer a actualizar</param>
        /// <response code="204">Cliente actualizado correctamente</response>
        /// <response code="400">Datos incorrectos</response>
        /// <response code="404">Cliente no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] Customer customer)
        {
            try
            {
                if (customer == null || customer.ID != id || !ModelState.IsValid)
                {
                    _logger.LogError("Error en la operación UpdateCustomer, datos incorrectos");
                    return BadRequest();
                }

                var existingCustomer = await _customerRepository.GetCustomer(id);
                if (existingCustomer == null)
                {
                    _logger.LogError("Error en la operación UpdateCustomer, no existe el cliente a actualizar");
                    return NotFound();
                }

                var result = await _customerRepository.UpdateCustomer(customer);
                if (result)
                {
                    _logger.LogInformation("Operación UpdateCustomer ejecutada correctamente.");
                    return NoContent();
                }
                return StatusCode(500, "A problem happened while handling your request.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la operación UpdateCustomer.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Elimina un cliente por su ID
        /// </summary>
        /// <remarks>
        /// Recibe el ID del cliente que se desea eliminar y lo borra de la BBDD.
        /// </remarks>
        /// <param name="id">Identificador del cliente</param>
        /// <response code="204">Cliente eliminado correctamente</response>
        /// <response code="404">Cliente no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerResponse(StatusCodes.Status204NoContent, Type = typeof(bool), Description = "Cliente eliminado")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                var existingCustomer = await _customerRepository.GetCustomer(id);
                if (existingCustomer == null)
                {
                    _logger.LogError("Error en la operación DeleteCustomer, no existe el cliente a eliminar");
                    return NotFound();
                }

                var result = await _customerRepository.DeleteCustomer(id);
                if (result)
                {
                    _logger.LogInformation("Operación DeleteCustomer ejecutada correctamente.");
                    return NoContent();
                }
                return StatusCode(500, "A problem happened while handling your request.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la operación DeleteCustomer.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}
