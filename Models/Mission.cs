using AgentManagementAPIServer.Enums;
using System.ComponentModel.DataAnnotations;

namespace AgentManagementAPIServer.Models
{
    public class Mission
    {
        [Key]
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int TargetId { get; set; }
        public EMissionsStatus Status { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan finalTime { get; set; }
        public TimeSpan leftTime { get; set; }
        public Mission(int agentId, int targetId)
        {
            AgentId = agentId;
            TargetId = targetId;
            Status = EMissionsStatus.proposal;
        }
    }
}
