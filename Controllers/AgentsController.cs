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
    public class AgentsController : ControllerBase
    {
        private readonly AgentsService _agentsService;
        public AgentsController(IService<Agent> agentsService)
        {
            this._agentsService = agentsService as AgentsService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Agent? agent)
        {
            await _agentsService.CreateAgentAsync(agent);

            return StatusCode(StatusCodes.Status201Created);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var agents =  await _agentsService.GetAllAsync();

            return StatusCode(StatusCodes.Status200OK, new { agents });
        }
        [HttpPut("{id}/pin")]
        public async Task<IActionResult> Pin( [FromQuery]int id, [FromBody] Coordinates? location)
        {
            await _agentsService.UpdateLocationAsync(id, location);
            //Chack if have posibility to create mission.

            return StatusCode(StatusCodes.Status200OK);
        }
        [HttpPut("{id}/move")]
        public async Task<IActionResult> Move([FromQuery]int id, [FromBody] EDirection direction)
        {
            Agent agent = await _agentsService.GetAsync(id);
            if (agent.Status == EAgentStatus.Active)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new {message = "This agent is active!" });
            }
            var newLocation = MoveLogic.NextLocation(agent.Location, direction);
            await _agentsService.UpdateLocationAsync(id, newLocation);
            //Chack if have posibility to create mission.

            return StatusCode(StatusCodes.Status200OK);
        }


    }
}
