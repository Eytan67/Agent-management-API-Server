using AgentManagementAPIServer.Enums;
using AgentManagementAPIServer.Intrfaces;
using System.ComponentModel.DataAnnotations;

namespace AgentManagementAPIServer.Models
{
    public class Target : IPerson
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public Coordinates? Location { get; set; }
        public string Position {  get; set; }
        public string photoUrl {  get; set; }
        public ETargetStatus Status {  get; set; }
    }
}
