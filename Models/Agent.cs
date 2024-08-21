using AgentManagementAPIServer.Enums;
using System.ComponentModel.DataAnnotations;
namespace AgentManagementAPIServer.Models
{
    public class Agent : IPerson
    {
        [Key]
        public int id {  get; set; }
        public string AgentImage { get; set; }
        public string AgentNickname { get; set; }
        public Coordinates Location { get; set; }
        public EAgentStatus Status { get; set; }
    }
}
