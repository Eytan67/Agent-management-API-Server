using AgentManagementAPIServer.Enums;
using AgentManagementAPIServer.Intrfaces;
using AgentManagementAPIServer.Models;
using AgentManagementAPIServer.Services;
using AgentManagementAPIServer.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AgentManagementAPIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TargetsController : ControllerBase
    {
        private readonly TargetService _targetService;
        private readonly MissionsService _missionsService;

        public TargetsController(IService<Target> agentsService, IService<Mission> missionsService)
        {
            this._targetService = agentsService as TargetService;
            this._missionsService = missionsService as MissionsService;

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

            return StatusCode(StatusCodes.Status200OK, new { targets });
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
            EDirection direction = Translate(dict["direction"]);
            Target target = await _targetService.GetAsync(id);
            if (target.Status == ETargetStatus.Eeliminated)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = $"{target.Name} alredy ded!" });
            }
            var newLocation = MoveLogic.NextLocation(target.Location, direction);
            Target updatedTarget = await _targetService.UpdateLocationAsync(id, newLocation);
            //Chack if have posibility to create mission.
            await _missionsService.TryToAddMissionsAsync(updatedTarget);

            return StatusCode(StatusCodes.Status200OK);
        }
        private EDirection Translate(string dir)
        {
            Dictionary<string, EDirection> dict = new Dictionary<string, EDirection>();
            dict["ne"] = EDirection.ne;
            return dict[dir];
        }
    }
}
