using AgentManagementAPIServer.Enums;

namespace AgentManagementAPIServer.Models
{
    public class Mission
    {
        public EMissionsStatus Status { get; set; }
        public Agent Agent { get; set; }
        public Target Target { get; set; }

    }
}
