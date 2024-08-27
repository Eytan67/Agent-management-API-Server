using AgentManagementAPIServer.DAL;
using AgentManagementAPIServer.Enums;
using AgentManagementAPIServer.Intrfaces;
using AgentManagementAPIServer.Models;
using AgentManagementAPIServer.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

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


        public async Task<Mission> GetAsync(int id)
        {
            var mission = await _DbContext.Missions.FirstOrDefaultAsync(m => m.Id == id);
            return mission;
        }

        //return active missions.
        public async Task<List<Mission>> GetActiveMissionsAsync()
        {
            var missions = await GetAllAsync();
            foreach (var mission in missions)
            {
                var fullMissions = await GetFullActiveMissionAsync(mission);
                await UpdatLeftTime(fullMissions);
            }
            return missions;
        }

        public async Task<int> CreateAsync(Mission newMission)
        {
            _DbContext.Missions.Add(newMission);
            await _DbContext.SaveChangesAsync();
            return 1;
        }


        public async Task UpdateMissionsAsync()
        {
            var activMissions = await _DbContext.Missions
                .Where(m => m.Status == EMissionsStatus.CommandForMission)
                .ToListAsync();
            //for each miision get her Ajent and Target
            foreach (var mission in activMissions)
            {
                await NextMove(mission);
            }
        }

        //Activeate mission.
        public async Task UpdateStatusAsync(int id)
        {
            var mission = await _DbContext.Missions.FindAsync(id);
            if(mission == null)
            {
                throw new Exception("the mission dosent exist."); 
            }
            var agent = await GetAgentAsync(mission.AgentId);
            var target = await GetTargetAsync(mission.TargetId);

            await ChecksMissionAsync(mission, agent, target);

            await RmoveIrrelvantMissions(mission);

            await StartMissionAsync(mission, agent, target);

        }

        //Checks for optional missions.
        public async Task TryToAddMissionsAsync(IPerson person)
        {
            switch (person)
            {
                case Agent:
                    //filter only free targets.
                    var targets = await GetAllTargtsAsync();
                    foreach (var target in targets)
                    {
                        CreateOrFindMissionAsync(person as Agent, target);
                    }
                    break;
                case Target:
                    var agents = await GetAllAgentsAsync();
                    foreach (var agent in agents)
                    {
                         CreateOrFindMissionAsync(agent, person as Target);
                    }
                    break;
                default:
                    break;
            }
        }

        public async Task CreateOrFindMissionAsync(Agent agent, Target target)
        {
            var mission = await SearchMissionAsync(agent.Id, target.Id);
            //create new mission.
            if (mission == null && MoveLogic.IsDistanceAppropriate(agent.Location, target.Location))
            {
                Mission newMission = new Mission(agent.Id, target.Id);

                _DbContext.Missions.Add(newMission);
                await _DbContext.SaveChangesAsync();
            }
            else
            {   //remove an relvant missions.
                if(!MoveLogic.IsDistanceAppropriate(agent.Location, target.Location))
                {
                    _DbContext.Missions.Remove(mission);
                    await _DbContext.SaveChangesAsync();
                }
            }
        }
        //Search mission by her Targte and Agent.
        public async Task<Mission> SearchMissionAsync(int agentId, int targetId)
        {
            var mission = await _DbContext.Missions
                                    .FirstOrDefaultAsync(m => m.AgentId == agentId
                                    && m.TargetId == targetId);
            return mission;
        }

        //Checks mission before activated it.
        private async Task ChecksMissionAsync (Mission mission, Agent agent, Target target)
        {

            if (agent.Status == EAgentStatus.Active 
                || target.Status != ETargetStatus.Alive 
                || !MoveLogic.IsDistanceAppropriate(agent.Location, target.Location))
            {
                _DbContext.Remove(mission);
                await _DbContext.SaveChangesAsync();
                throw new Exception("mission is not more valid");
            }

        }

        //Deletes all unnecessary tasks.
        private async Task RmoveIrrelvantMissions(Mission mission)
        {
            var canceledMissions = await _DbContext.Missions
                .Where(m => m.AgentId == mission.AgentId || m.TargetId == mission.TargetId)
                .ToListAsync();

            //get out our mission from the list.
            canceledMissions.Remove(mission);

            _DbContext.Missions.RemoveRange(canceledMissions);
            await _DbContext.SaveChangesAsync();
        }

        private async Task NextMove(Mission mission)
        {
            var agent = await GetAgentAsync(mission.AgentId);
            var target = await GetTargetAsync(mission.TargetId);
            
            var newLocation = MoveLogic.NextLocationByCoordinates(agent.Location, target.Location);

            agent.Location.X = newLocation.X;
            agent.Location.Y = newLocation.Y;

            _DbContext.Agents.Update(agent);
            await _DbContext.SaveChangesAsync();
            await CheckMissionProgresAsync(mission, agent, target);
        }

        //Checks whether there was an elimination.
        private async Task CheckMissionProgresAsync(Mission mission, Agent agent, Target target)
        {
            if (agent.Location == target.Location)
            {
                agent.Stars += 1;
                agent.Status = EAgentStatus.Dormant;
                mission.Status = EMissionsStatus.finished;
                mission.finalTime = DateTime.Now - mission.StartTime;
                target.Status = ETargetStatus.Eeliminated;

                _DbContext.Targets.Update(target);
                _DbContext.Missions.Update(mission);
                _DbContext.Agents.Update(agent);

                await _DbContext.SaveChangesAsync();
            }
        }

        private async Task StartMissionAsync(Mission mission, Agent agent, Target target)
        {
            mission.StartTime = DateTime.Now;
            mission.leftTime = MoveLogic.CalculateLeftTime(agent.Location, target.Location);
            mission.Status = EMissionsStatus.CommandForMission;

            agent.Status = EAgentStatus.Active;
            target.Status = ETargetStatus.Targeted;

            _DbContext.Agents.Update(agent);
            _DbContext.Targets.Update(target);
            _DbContext.Missions.Update(mission);

            await _DbContext.SaveChangesAsync();
        }

        private async Task UpdatLeftTime(FullMission fullMission)
        {
            fullMission.Mission.leftTime = MoveLogic.CalculateLeftTime(fullMission.Agent.Location, fullMission.Target.Location);
            _DbContext.Missions.Update(fullMission.Mission);
            await _DbContext.SaveChangesAsync();
        }
        private async Task<FullMission> GetFullActiveMissionAsync(Mission mission)
        {
            FullMission fullMission = new FullMission()
            {
                Mission = mission,
                Agent = await GetAgentAsync(mission.AgentId),
                Target = await GetTargetAsync(mission.TargetId),
            };
            return fullMission;
        }
        private async Task<Agent> GetAgentAsync(int id)
        {
            var agent = await _DbContext.Agents
                .Include(a => a.Location)
                .FirstOrDefaultAsync(a => a.Id == id);
            return agent;
        }
        private async Task<List<Agent>> GetAllAgentsAsync()
        {
            var agents = await _DbContext.Agents
                .Include(a => a.Location)
                .Where(a => a.Status == EAgentStatus.Dormant)
                .ToListAsync();
            return agents;
        }
        private async Task<Target> GetTargetAsync(int id)
        {
            var target = await _DbContext.Targets
                .Include(t => t.Location)
                .FirstOrDefaultAsync(t => t.Id == id);
            return target;
        }

        private async Task<List<Target>> GetAllTargtsAsync()
        {
            var targets = await _DbContext.Targets
                .Include(t => t.Location)
                .Where(t => t.Status == ETargetStatus.Alive)
                .ToListAsync();
            return targets;
        }


    }
}
