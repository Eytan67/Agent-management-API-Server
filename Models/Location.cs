using System.ComponentModel.DataAnnotations;

namespace AgentManagementAPIServer.Models
{
    public class Location
    {
        [Key]
        public int Id { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
    }
}
