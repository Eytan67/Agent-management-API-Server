﻿using AgentManagementAPIServer.DAL;
using AgentManagementAPIServer.Models;
using AgentManagementAPIServer.Intrfaces;
using Microsoft.EntityFrameworkCore;

namespace AgentManagementAPIServer.Services
{
    public class AgentsService : IService<Agent>
    {
        private readonly MyDbContext _DbContext;

        public AgentsService( MyDbContext context)
        {
            this._DbContext = context;
        }

        public async Task<List<Agent>> GetAllAsync()
        {
            var agents = await _DbContext.Agents.Include(l => l.Location).ToListAsync();
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

        public async Task<int> CreateAsync(Agent newAgent)
        {
            _DbContext.Agents.Add(newAgent);
            await _DbContext.SaveChangesAsync();
            return newAgent.Id;
        }

        public async Task<Agent> UpdateLocationAsync(int id, Coordinates newLocation)
        {
            var agent = await _DbContext.Agents.Include(a => a.Location).FirstOrDefaultAsync(a => a.Id == id);
            if(agent != null && agent.Location == null)
            {
                agent.Location = newLocation;

                _DbContext.Agents.Update(agent);
                await _DbContext.SaveChangesAsync();
            }

            return agent;
        }

        

    }
}
