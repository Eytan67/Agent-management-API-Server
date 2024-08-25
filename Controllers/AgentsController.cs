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
        private readonly MissionsService _missionsService;

        public AgentsController(IService<Agent> agentsService, IService<Mission> missionsService)
        {
            this._agentsService = agentsService as AgentsService;
            this._missionsService = missionsService as MissionsService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Agent agent)
        {
            int id = await _agentsService.CreateAsync(agent);

            return StatusCode(StatusCodes.Status201Created, new { Id = id});
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var agents =  await _agentsService.GetAllAsync();

            return StatusCode(StatusCodes.Status200OK, new { agents });
        }
        [HttpPut("{id}/pin")]
        public async Task<IActionResult> Pin( [FromRoute]int id, [FromBody] Coordinates location)
        {
            Agent updatedAgent = await _agentsService.UpdateLocationAsync(id, location);
            //Chack if have posibility to create mission.
            await _missionsService.TryToAddMissionsAsync(updatedAgent);

            return StatusCode(StatusCodes.Status200OK);
        }
        [HttpPut("{id}/move")]
        public async Task<IActionResult> Move([FromRoute]int id, [FromBody] Dictionary<string, string> dict)
        {
            EDirection direction = Translate(dict["direction"]);
            Agent agent = await _agentsService.GetAsync(id);
            if (agent.Status == EAgentStatus.Active)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new {message = "This agent is active!" });
            }
            var newLocation = MoveLogic.NextLocation(agent.Location, direction);
            Agent updatedAgent = await _agentsService.UpdateLocationAsync(id, newLocation);
            //Chack if have posibility to create mission.
            await _missionsService.TryToAddMissionsAsync(updatedAgent);

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
