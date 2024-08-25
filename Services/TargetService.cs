using AgentManagementAPIServer.Intrfaces;
using AgentManagementAPIServer.DAL;
using AgentManagementAPIServer.Models;
using Microsoft.EntityFrameworkCore;
using AgentManagementAPIServer.Enums;
using AgentManagementAPIServer.Shared;


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
            var targets = await _DbContext.Targets.Include(l => l.Location).ToListAsync();
            if (targets == null)
            {
                throw new Exception("sumsing wrong!");
            }

            return targets;
        }
        public async Task<Target> GetAsync(int id)
        {
            var target = await _DbContext.Targets.Include(l => l.Location).FirstOrDefaultAsync(t => t.Id == id); ;
            if (target == null)
            {
                throw new Exception("sumsing wrong!");
            }

            return target;
        }
        public async Task<int> CreateAsync(Target newTarget)
        {
            _DbContext.Targets.Add(newTarget);
            await _DbContext.SaveChangesAsync();
            return newTarget.Id;
        }
        public async Task<Target> UpdateLocationAsync(int id, Coordinates newLocation)
        {
            var target = await _DbContext.Targets.Include(t => t.Location).FirstOrDefaultAsync(t => t.Id == id);
            if (target != null && target.Location == null)
            {
                target.Location = newLocation;

                _DbContext.Targets.Update(target);
                await _DbContext.SaveChangesAsync();
            }

            return target;
        }

        public async Task<Target> MoveAsync(int id, string direction)
        {
            Target target = await GetAsync(id);
            if (target.Status == ETargetStatus.Eeliminated)
            {
                throw new InvalidOperationException($"{target.Name} alredy ded!");
            }
            if (target.Location == null)
            {
                throw new InvalidOperationException($"{target.Name} not pind yet!");

            }
            var newLocation = MoveLogic.NextLocationByDirection(target.Location, direction);
            target.Location.X = newLocation.X;
            target.Location.Y = newLocation.Y;
            _DbContext.Targets.Update(target);
            await _DbContext.SaveChangesAsync();
            return target;
        }

    }
}
