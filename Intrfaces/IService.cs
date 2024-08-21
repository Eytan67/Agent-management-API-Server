using AgentManagementAPIServer.Models;

namespace AgentManagementAPIServer.Intrfaces
{
    public interface IService<T>
    {
        public Task<List<T>> GetAllAsync();
        public Task<T> GetAsync(int id);
        public Task CreateAgentAsync(T newAgent);
        public Task UpdateLocationAsync(int id, Coordinates newLocation);


    }
}
