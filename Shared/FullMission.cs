using AgentManagementAPIServer.Models;

namespace AgentManagementAPIServer.Shared
{
    public class FullMission
    {
        public Mission Mission { get; set; }
        public Agent Agent { get; set; }
        public Target Target { get; set; }
    }
}
