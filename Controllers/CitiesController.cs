using ApiMySQL.Data.Repositories;
using ApiMySQL.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;

namespace ApiMySQL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ICityRepository _cityRepository;

        public CitiesController(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCities()
        {
            return Ok(await _cityRepository.GetAllCities());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCityDetails(int id)
        {
            return Ok(await _cityRepository.GetDetails(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCity([FromBody] City city)
        {
            if (city == null)
                return BadRequest();

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _cityRepository.InsertCity(city);
            return Created("created", created);

        }

        [HttpPut]
        public async Task<IActionResult> UpdateCity([FromBody] City city)
        {
            if (city == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _cityRepository.UpdateCity(city);
            return NoContent();          

        }
        [HttpDelete]
        public async Task<IActionResult> DeleteCity(int id)
        { 
            await _cityRepository.DeleteCity(id);
            return NoContent();
        }
            


    }
}
