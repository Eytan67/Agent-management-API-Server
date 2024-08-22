using AgentManagementAPIServer.DAL;
using AgentManagementAPIServer.Enums;
using AgentManagementAPIServer.Intrfaces;
using AgentManagementAPIServer.Models;
using AgentManagementAPIServer.Shared;

namespace AgentManagementAPIServer.Services
{
    public class MissionsService : IService<Mission>
    {
        private readonly MyDbContext _DbContext;

        public MissionsService(MyDbContext context)
        {
            this._DbContext = context;
        }
        public async Task<List<Mission>> GetAllAsync()
        {
            var missions = _DbContext.Missions.ToList();//__________________ASYNC?
            if (missions == null)
            {
                throw new Exception("sumsing wrong!");
            }

            return missions;
        }
        public async Task<Mission> GetAsync(int id)
        {
            var mission = await _DbContext.Missions.FindAsync(id);
            if (mission == null)
            {
                throw new Exception("sumsing wrong!");
            }

            return mission;
        }

        public async Task CreateAsync(Mission newMission)
        {
            _DbContext.Missions.Add(newMission);
            await _DbContext.SaveChangesAsync();
        }

        public async Task updateMissionsAsync()
        {
            //get all activ missions
            var activMissions = _DbContext.Missions.Where(m => m.Status == EMissionsStatus.Active).ToList();
            //for each miision get her Ajent and Target
            foreach (var activMission in activMissions)
            {
                Agent agent = await _DbContext.Agents.FindAsync(activMission.Agent.Id);
                Target target = await _DbContext.Targets.FindAsync(activMission.Target.Id);
                //find the next move direction
                var direction = MoveLogic.ChoosDirection(agent.Coordinates, target.Coordinates);
                //find the next location
                Coordinates newLocation = MoveLogic.NextLocation(agent.Coordinates, direction);

                //update new location
                agent.Coordinates = newLocation;
                await _DbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateStatusAsync(int id, EMissionsStatus status)
        {
            var mission = await _DbContext.Missions.FindAsync(id);
            mission.Status = status;
            _DbContext.Update(mission);
            await _DbContext.SaveChangesAsync();

        }
    }
}
