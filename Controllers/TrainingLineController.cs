using ApiMySQL.Repositories;
using ApiMySQL.Model;
using Microsoft.AspNetCore.Mvc;

namespace ApiMySQL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingLineController : Controller
    {
        private readonly ITrainingLineRepository _trainingLineRepository;

        public TrainingLineController(ITrainingLineRepository trainingLineRepository)
        {
            _trainingLineRepository = trainingLineRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetTrainingLinesOfTraining(int id)
        {
            return Ok(await _trainingLineRepository.GetTrainingLinesOfTraining(id));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrainingLine(int id)
        {
            return Ok(await _trainingLineRepository.GetTrainingLine(id));
        }

        [HttpPost]
        public async Task<IActionResult> InsertTrainingLine([FromBody] TrainingLine trainingLine)
        {
            if (trainingLine == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _trainingLineRepository.InsertTrainingLine(trainingLine);
            return Created("created", created);

        }

        [HttpPut]
        public async Task<IActionResult> UpdateTrainingLine([FromBody] TrainingLine trainingLine)
        {
            if (trainingLine == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _trainingLineRepository.UpdateTrainingLine(trainingLine);
            return NoContent();

        }
        [HttpDelete]
        public async Task<IActionResult> DeleteTrainingLine(int id)
        {
            await _trainingLineRepository.DeleteTrainingLine(id);
            return NoContent();
        }
    }
}
