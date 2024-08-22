using AgentManagementAPIServer.Enums;
using AgentManagementAPIServer.Intrfaces;
using System.ComponentModel.DataAnnotations;
namespace AgentManagementAPIServer.Models
{
    public class Agent : IPerson
    {
        [Key]
        public int Id {  get; set; }
        public string AgentImage { get; set; }
        public string AgentNickname { get; set; }
        public Coordinates Coordinates { get; set; }
        public EAgentStatus Status { get; set; }
    }
}
