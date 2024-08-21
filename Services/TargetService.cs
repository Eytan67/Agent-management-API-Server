using AgentManagementAPIServer.Intrfaces;
using AgentManagementAPIServer.DAL;
using AgentManagementAPIServer.Models;


namespace AgentManagementAPIServer.Services
{
    public class TargetService : IService<Target>
    {
        private readonly MyDbContext _DbContext;

        public TargetService(MyDbContext context)
        {
            this._DbContext = context;
        }
        public async Task<List<Target>> GetAllAsync()
        {
            var targets = _DbContext.Targets.ToList();
            if (targets == null)
            {
                throw new Exception("sumsing wrong!");
            }

            return targets;
        }
        public async Task<Target> GetAsync(int id)
        {
            var target = await _DbContext.Targets.FindAsync(id);
            if (target == null)
            {
                throw new Exception("sumsing wrong!");
            }

            return target;
        }
        public async Task CreateAgentAsync(Target newTarget)
        {
            _DbContext.Targets.Add(newTarget);
            await _DbContext.SaveChangesAsync();
        }
        public async Task UpdateLocationAsync(int id, Coordinates newLocation)
        {
            var target = await _DbContext.Targets.FindAsync(id);
            target.Location = newLocation;
            _DbContext.Targets.Update(target);
            await _DbContext.SaveChangesAsync();

        }

    }
}
