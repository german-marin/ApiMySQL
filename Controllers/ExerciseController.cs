using ApiMySQL.Repositories;
using ApiMySQL.Model;
using Microsoft.AspNetCore.Mvc;

namespace ApiMySQL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseController : ControllerBase
    {
        private readonly IExerciseRepository _exerciseRepository;

        public ExerciseController(IExerciseRepository exerciseRepository)
        {
            _exerciseRepository = exerciseRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetCategoryExercises(int id)
        {
            return Ok(await _exerciseRepository.GetCategoryExercises(id));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetExercise(int id)
        {
            return Ok(await _exerciseRepository.GetExercise(id));
        }

        [HttpPost]
        public async Task<IActionResult> InsertExercise([FromBody] Exercise exercise)
        {
            if (exercise == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _exerciseRepository.InsertExercise(exercise);
            return Created("created", created);

        }

        [HttpPut]
        public async Task<IActionResult> UpdateExercise([FromBody] Exercise exercise)
        {
            if (exercise == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _exerciseRepository.UpdateExercise(exercise);
            return NoContent();

        }
        [HttpDelete]
        public async Task<IActionResult> DeleteExercise(int id)
        {
            await _exerciseRepository.DeleteExercise(id);
            return NoContent();
        }



    }
}
