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
            var missions = await _DbContext.Missions.ToListAsync();
            if (missions == null)
            {
                throw new Exception("sumsing wrong!");
            }

            return missions;
        }
        //public async Task<List<Mission>> GetActivMissionsAsync()
        //{
        //    var missions = await _DbContext.Missions.Where(m => m.Status == EMissionsStatus.CommandForMission).ToListAsync();
        //    foreach (var mission in missions)
        //    {
        //        mission.AgentId
        //    }
        //}
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
            var activMissions = _DbContext.Missions
                .Where(m => m.Status == EMissionsStatus.CommandForMission).ToList();
            //for each miision get her Ajent and Target
            foreach (var mission in activMissions)
            {
                var agent = await _DbContext.Agents.Include(a => a.Location)
                    .FirstOrDefaultAsync( a => a.Id == mission.AgentId);
                var target = await _DbContext.Targets.Include(t => t.Location)
                    .FirstOrDefaultAsync(t => t.Id == mission.TargetId);
                //find the next move direction
                var newLocation = MoveLogic.NextLocationByCoordinates(agent.Location, target.Location);

                //update new location
                agent.Location.X = newLocation.X;
                agent.Location.Y = newLocation.Y;

                if(agent.Location == target.Location)
                {
                    agent.Status = EAgentStatus.Dormant;
                    agent.Stars += 1;
                    mission.Status = EMissionsStatus.finished;
                    target.Status = ETargetStatus.Eeliminated;
                    _DbContext.Targets.Update(target);
                    _DbContext.Missions.Update(mission);
                }
                _DbContext.Agents.Update(agent);
                await _DbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateStatusAsync(int id)
        {
            var mission = await _DbContext.Missions.FindAsync(id);
            if(mission == null) { return; }//to handle_____________________
            var agent = await _DbContext.Agents.Include(a => a.Location).FirstOrDefaultAsync(a => a.Id == mission.AgentId); 
            var target = await _DbContext.Targets.Include(l => l.Location).FirstOrDefaultAsync(t => t.Id == mission.TargetId); 
            if (agent == null || target == null
                || !MoveLogic.IsDistanceAppropriate(agent.Location, target.Location))
            {  return; }//to handle_____________________

            mission.Status = EMissionsStatus.CommandForMission;
            //get all missions with the same Agent or Target.
            var canceledMissions = await _DbContext.Missions
                .Where(m => m.AgentId == agent.Id || m.TargetId == target.Id)
                .ToListAsync();
            //get out our mission from the list.
            canceledMissions.Remove(mission);
            //delete all canceledMissions.
            _DbContext.Missions.RemoveRange(canceledMissions);
            //update our mission.
            _DbContext.Update(mission);
            await _DbContext.SaveChangesAsync();

        }

        public async Task TryToAddMissionsAsync(IPerson person)
        {
            switch (person)
            {
                case Agent:
                    //filter only an on missions targets //async?
                    var targets =  await _DbContext.Targets.Include(t => t.Location)
                        .Where(t => t.Status == ETargetStatus.Alive).ToListAsync();
                    foreach (var target in targets)
                    {
                        if(MoveLogic.IsDistanceAppropriate(person.Location, target.Location))
                        {
                            //anshure that this mission not created before
                            var mission = await _DbContext.Missions
                                    .FirstOrDefaultAsync(m => m.AgentId == person.Id 
                                    && m.TargetId == target.Id);
                            if (mission != null)
                            {
                                continue;
                            }
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
                            var mission = await _DbContext.Missions.
                                    FirstOrDefaultAsync(m => m.TargetId == person.Id
                                    && m.AgentId == agent.Id);
                            if (mission != null)
                            {
                                continue;
                            }
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
                Status = EMissionsStatus.proposal,
            };
            _DbContext.Missions.Add(newMission);
            await _DbContext.SaveChangesAsync();
        }
    }
}
