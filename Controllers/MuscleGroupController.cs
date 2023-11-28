using ApiMySQL.Repositories;
using ApiMySQL.Model;
using Microsoft.AspNetCore.Mvc;

namespace ApiMySQL.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        public class MuscleGroupController : ControllerBase
        {
            private readonly IMuscleGroupRepository _muscleGroupRepository;

            public MuscleGroupController(IMuscleGroupRepository muscleGroupRepository)
            {
                _muscleGroupRepository = muscleGroupRepository;
            }
            [HttpGet]
            public async Task<IActionResult> GetAllMuscleGroup()
            {
                return Ok(await _muscleGroupRepository.GetAllMuscleGroup());
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetMuscleGroup(int id)
            {
                return Ok(await _muscleGroupRepository.GetMuscleGroup(id));
            }

            [HttpPost]
            public async Task<IActionResult> InsertMuscleGroup([FromBody] MuscleGroup muscleGroup)
            {
                if (muscleGroup == null)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var created = await _muscleGroupRepository.InsertMuscleGroup(muscleGroup);
                return Created("created", created);

            }

            [HttpPut]
            public async Task<IActionResult> UpdateMuscleGroup([FromBody] MuscleGroup muscleGroup)
            {
                if (muscleGroup == null)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _muscleGroupRepository.UpdateMuscleGroup(muscleGroup);
                return NoContent();

            }
            [HttpDelete]
            public async Task<IActionResult> DeleteMuscleGroup(int id)
            {
                await _muscleGroupRepository.DeleteMuscleGroup(id);
                return NoContent();
            }



        }
    }
