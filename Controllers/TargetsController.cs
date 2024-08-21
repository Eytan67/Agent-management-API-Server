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
        public TargetsController(IService<Target> agentsService)
        {
            this._targetService = agentsService as TargetService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Target? target)
        {
            await _targetService.CreateAgentAsync(target);

            return StatusCode(StatusCodes.Status201Created);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var targets = await _targetService.GetAllAsync();

            return StatusCode(StatusCodes.Status200OK, new { targets });
        }
        [HttpPut("{id}/pin")]
        public async Task<IActionResult> Pin([FromQuery] int id, [FromBody] Coordinates? location)
        {
            await _targetService.UpdateLocationAsync(id, location);
            //Chack if have posibility to create mission.

            return StatusCode(StatusCodes.Status200OK);
        }
        [HttpPut("{id}/move")]
        public async Task<IActionResult> Move([FromQuery] int id, [FromBody] EDirection direction)
        {
            Target target = await _targetService.GetAsync(id);
            if (target.Status == ETargetStatus.Eeliminated)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = $"{target.Name} alredy ded!" });
            }
            var newLocation = MoveLogic.NextLocation(target.Location, direction);
            await _targetService.UpdateLocationAsync(id, newLocation);
            //Chack if have posibility to create mission.

            return StatusCode(StatusCodes.Status200OK);
        }
    }
}
