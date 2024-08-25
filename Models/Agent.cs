using AgentManagementAPIServer.Enums;
using AgentManagementAPIServer.Intrfaces;
using System.ComponentModel.DataAnnotations;
namespace AgentManagementAPIServer.Models
{
    public class Agent : IPerson
    {
        [Key]
        public int Id {  get; set; }
        public string photoUrl { get; set; }
        public string Nickname { get; set; }
        public Coordinates? Location { get; set; }
        public EAgentStatus Status { get; set; }
        public int Stars {  get; set; }
    }
}
