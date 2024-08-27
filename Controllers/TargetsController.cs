using Microsoft.AspNetCore.Mvc;

using AgentManagementAPIServer.Intrfaces;
using AgentManagementAPIServer.Models;
using AgentManagementAPIServer.Services;



namespace AgentManagementAPIServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TargetsController : ControllerBase
    {
        private readonly TargetService _targetService;
        private readonly MissionsService _missionsService;

        public TargetsController(IService<Target> agentsService, IService<Mission> missionsService)
        {
            _targetService = agentsService as TargetService;
            _missionsService = missionsService as MissionsService;

        }


        [HttpPost]
        public async Task<IActionResult> Create(Target? target)
        {
            int id = await _targetService.CreateAsync(target);

            return StatusCode(StatusCodes.Status201Created, new { Id = id});
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var targets = await _targetService.GetAllAsync();

            return StatusCode(StatusCodes.Status200OK, targets );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAll(int id)
        {
            var target = await _targetService.GetAsync(id);

            return StatusCode(StatusCodes.Status200OK, target );
        }

        [HttpPut("{id}/pin")]
        public async Task<IActionResult> Pin([FromRoute] int id, [FromBody] Coordinates? location)
        {
            Target updatedTarget = await _targetService.UpdateLocationAsync(id, location);

            //Chack if have posibility to create mission.
            await _missionsService.TryToAddMissionsAsync(updatedTarget);

            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpPut("{id}/move")]
        public async Task<IActionResult> Move([FromRoute] int id, [FromBody] Dictionary<string, string> dict)
        {
            try
            {
                Target target = await _targetService.MoveAsync(id, dict["direction"]);
                await _missionsService.TryToAddMissionsAsync(target);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = ex.Message });

            }
            return StatusCode(StatusCodes.Status200OK);
        }

    }

}

