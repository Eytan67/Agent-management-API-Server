using AgentManagementAPIServer.Enums;
using AgentManagementAPIServer.Intrfaces;
using AgentManagementAPIServer.Models;
using AgentManagementAPIServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

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

            return StatusCode(StatusCodes.Status200OK, missions );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMission(int id)
        {
            var mission = await _missionsService.GetAsync(id);

            return StatusCode(StatusCodes.Status200OK, mission);
        }

        [HttpGet("actives")]
        public async Task<IActionResult> GetActiveMission()
        {
            var activeMissions= await _missionsService.GetActiveMissionsAsync();

            return StatusCode(StatusCodes.Status200OK, activeMissions);
        }


        [HttpPost("update")]
        public async Task<IActionResult> UpdateMissionsProgres()
        {
            await _missionsService.UpdateMissionsAsync();

            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> CommandForMission([FromRoute] int id, [FromBody]Dictionary<string, string> status)
        {
            await _missionsService.UpdateStatusAsync(id);

            return StatusCode(StatusCodes.Status200OK);
        }
    }
}
