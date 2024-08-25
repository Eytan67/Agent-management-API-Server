using AgentManagementAPIServer.Models;

namespace AgentManagementAPIServer.Intrfaces
{
    public interface IPerson
    {
        public int Id { get; set; }
        public Coordinates? Location { get; set; }
    }
}
