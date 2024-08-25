using AgentManagementAPIServer.Models;

namespace AgentManagementAPIServer.Intrfaces
{
    public interface IPerson
    {
        public Coordinates? Location { get; set; }
    }
}
