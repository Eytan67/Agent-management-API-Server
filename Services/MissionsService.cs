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
            var activMissions = _DbContext.Missions.Where(m => m.Status == EMissionsStatus.CommandForMission).ToList();
            //for each miision get her Ajent and Target
            foreach (var activMission in activMissions)
            {
                Agent agent = await _DbContext.Agents.FindAsync(activMission.AgentId);
                Target target = await _DbContext.Targets.FindAsync(activMission.TargetId);
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

        public async Task TryToAddMissionsAsync(IPerson person)
        {
            switch (person)
            {
                case Agent:
                    var targets = _DbContext.Targets;//filter only an on missions targets //async?
                    foreach (var target in targets)
                    {
                        if(MoveLogic.IsDistanceAppropriate(person.Coordinates, target.Coordinates))
                        {
                            CreateMissionAsync(person as Agent, target);
                        }
                    }
                    break;
                case Target:
                    var agents = _DbContext.Agents;//filter only an on missions agents //async?
                    foreach (var agent in agents)
                    {
                        if (MoveLogic.IsDistanceAppropriate(person.Coordinates, agent.Coordinates))
                        {
                            CreateMissionAsync(agent, person as Target);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public async Task CreateMissionAsync(Agent agent, Target target)
        {
            Mission newMission = new Mission()
            {
                AgentId = agent.Id,
                TargetId = target.Id,
                Status = EMissionsStatus.proposal
            };
            _DbContext.Missions.Add(newMission);
            await _DbContext.SaveChangesAsync();
        }
    }
}
