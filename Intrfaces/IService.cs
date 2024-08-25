using AgentManagementAPIServer.Models;

namespace AgentManagementAPIServer.Intrfaces
{
    public interface IService<T>
    {
        public Task<List<T>> GetAllAsync();
        public Task<T> GetAsync(int id);
        public Task<int> CreateAsync(T newItem);
        //public Task UpdateAsync(int id, Coordinates newLocation);


    }
}
