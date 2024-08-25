using AgentManagementAPIServer.Enums;
using AgentManagementAPIServer.Intrfaces;
using AgentManagementAPIServer.Models;
using AgentManagementAPIServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AgentManagementAPIServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MissionsController : ControllerBase
    {
        private readonly MissionsService _missionsService;
        public MissionsController(IService<Mission> missionsService)
        {
            this._missionsService = missionsService as MissionsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var missions = await _missionsService.GetAllAsync();

            return StatusCode(StatusCodes.Status200OK, new { missions });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateMissionsProgres()
        {
            var activMissions = _missionsService.UpdateMissionsAsync();

            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> CommandForMission([FromQuery] int id, [FromBody] EMissionsStatus status)
        {
            await _missionsService.UpdateStatusAsync(id, status);

            return StatusCode(StatusCodes.Status200OK);
        }
    }
}
