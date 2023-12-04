using Microsoft.AspNetCore.Mvc;
using ApiMySQL.Repositories;
using ApiMySQL.Model;

namespace ApiMySQL.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TrainingController : ControllerBase
    {
        private readonly ITrainingRepository _trainingRepository;

        public TrainingController(ITrainingRepository trainingRepository)
        {
            _trainingRepository = trainingRepository;
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTraining(int id)
        {
            return Ok(await _trainingRepository.GetTraining(id));
        }
        [Route("InsertTraining")]
        [HttpPost]
        public async Task<IActionResult> InsertTraining([FromBody] Training training)
        {
            if (training == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _trainingRepository.InsertTraining(training);
            return Created("created", created);

        }
        [Route("UpdateTraining")]
        [HttpPut]
        public async Task<IActionResult> UpdateTraining([FromBody] Training training)
        {
            if (training == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _trainingRepository.UpdateTraining(training);
            return NoContent();

        }
        
        [HttpDelete("DeleteTraining/{id}")]
        public async Task<IActionResult> DeleteTraining(int id)
        {
            await _trainingRepository.DeleteTraining(id);
            return NoContent();
        }
        [HttpGet("GetAllTrainings")]
        public async Task<IActionResult> GetAllTrainings()
        {
            var trainings = await _trainingRepository.GetAllTrainings();

            if (trainings == null || !trainings.Any())
            {
                return NoContent();
            }

            return Ok(trainings);
        }

    }
}
