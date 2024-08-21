using AgentManagementAPIServer.DAL;
using AgentManagementAPIServer.Models;
using AgentManagementAPIServer.Intrfaces;

namespace AgentManagementAPIServer.Services
{
    public class AgentsService : IService
    {
        private readonly MyDbContext _DbContext;

        public AgentsService( MyDbContext context)
        {
            this._DbContext = context;
        }

        public async Task<List<Agent>> GetAllAsync()
        {
            var agents = _DbContext.Agents.ToList();
            if (agents == null)
            {
                throw new Exception("sumsing wrong!");
            }

            return agents;
        }

        public async Task<Agent> GetAsync(int id)
        {
            var agent = await _DbContext.Agents.FindAsync(id);
            if (agent == null)
            {
                throw new Exception("sumsing wrong!");
            }

            return agent;
        }

        public async Task CreateAgentAsync(Agent agent)
        {
            _DbContext.Agents.Add(agent);
            await _DbContext.SaveChangesAsync();
        }

        public async Task UpdateLocationAsync(int id, Location newLocation)
        {
            var agent = await _DbContext.Agents.FindAsync(id);
            agent.Location = newLocation;
        }

        

    }
}
