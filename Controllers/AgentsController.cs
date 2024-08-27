using Microsoft.AspNetCore.Mvc;

using AgentManagementAPIServer.Intrfaces;
using AgentManagementAPIServer.Models;
using AgentManagementAPIServer.Services;



namespace AgentManagementAPIServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly AgentsService _agentsService;
        private readonly MissionsService _missionsService;

        public AgentsController(IService<Agent> agentsService, IService<Mission> missionsService)
        {
            _agentsService = agentsService as AgentsService;
            _missionsService = missionsService as MissionsService;
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

            return StatusCode(StatusCodes.Status200OK, agents );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAgent(int id)
        {
            var agent = await _agentsService.GetAsync(id);

            return StatusCode(StatusCodes.Status200OK, agent );
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
            try
            {
                Agent agent = await _agentsService.MoveAsync(id, dict["direction"]);
                await _missionsService.TryToAddMissionsAsync(agent);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = ex.Message });
            }

            return StatusCode(StatusCodes.Status200OK);
        }

    }

}

