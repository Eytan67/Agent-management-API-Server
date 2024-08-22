using AgentManagementAPIServer.Enums;
using System.ComponentModel.DataAnnotations;

namespace AgentManagementAPIServer.Models
{
    public class Mission
    {
        [Key]
        public int Id { get; set; }
        public EMissionsStatus Status { get; set; }
        public int AgentId { get; set; }
        public int TargetId { get; set; }

    }
}
