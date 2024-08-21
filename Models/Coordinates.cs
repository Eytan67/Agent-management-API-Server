using System.ComponentModel.DataAnnotations;

namespace AgentManagementAPIServer.Models
{
    
    public class Coordinates
    {
        [Key]
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
