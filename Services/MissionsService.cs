using AgentManagementAPIServer.DAL;
using AgentManagementAPIServer.Enums;
using AgentManagementAPIServer.Intrfaces;
using AgentManagementAPIServer.Models;
using AgentManagementAPIServer.Shared;
using Microsoft.EntityFrameworkCore;

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

        public async Task<int> CreateAsync(Mission newMission)
        {
            _DbContext.Missions.Add(newMission);
            await _DbContext.SaveChangesAsync();
            return 1;
        }

        public async Task UpdateMissionsAsync()
        {
            //get all activ missions
            var activMissions = _DbContext.Missions.Where(m => m.Status == EMissionsStatus.CommandForMission).ToList();
            //for each miision get her Ajent and Target
            foreach (var activMission in activMissions)
            {
                Agent agent = await _DbContext.Agents.FindAsync(activMission.AgentId);
                Target target = await _DbContext.Targets.FindAsync(activMission.TargetId);
                //find the next move direction
                var newLocation = MoveLogic.NextLocationByCoordinates(agent.Location, target.Location);
                //find the next location
                //Coordinates newLocation = MoveLogic.NextLocation(agent.Location, direction);

                //update new location
                agent.Location = newLocation;
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
                    var targets1 = _DbContext.Targets;//filter only an on missions targets //async?
                    var targets =  await _DbContext.Targets.Include(t => t.Location)
                        .Where(t => t.Status == ETargetStatus.Alive).ToListAsync();
                    foreach (var target in targets)
                    {
                        if(MoveLogic.IsDistanceAppropriate(person.Location, target.Location))
                        {
                            CreateMissionAsync(person as Agent, target);
                        }
                    }
                    break;
                case Target:
                    var agents = await _DbContext.Agents.Include(a => a.Location)
                        .Where(a => a.Status == EAgentStatus.Dormant).ToListAsync();
                    foreach (var agent in agents)
                    {
                        if (MoveLogic.IsDistanceAppropriate(person.Location, agent.Location))
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
