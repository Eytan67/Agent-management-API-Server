using AgentManagementAPIServer.Enums;
using System.ComponentModel.DataAnnotations;

namespace AgentManagementAPIServer.Models
{
    public class Target
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public Coordinates Location { get; set; }
        public ETargetStatus Status {  get; set; }
    }
}
