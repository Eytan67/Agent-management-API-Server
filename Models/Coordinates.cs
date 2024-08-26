using System.ComponentModel.DataAnnotations;

namespace AgentManagementAPIServer.Models
{
    
    public class Coordinates
    {
        [Key]
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public Coordinates(int x, int y) { this.X = x; this.Y = y; }
        public Coordinates() { }

        public static Coordinates operator -(Coordinates c1, Coordinates c2)
        {
            return new Coordinates(c1.X - c2.X, c1.Y - c2.Y);
        }
        public static Coordinates operator +(Coordinates c1, Coordinates c2)
        {
            return new Coordinates(c1.X + c2.X, c1.Y + c2.Y);
        }
        public static bool operator ==(Coordinates c1, Coordinates c2)
        {
            if (ReferenceEquals(c1, c2))
                return true;
            if (ReferenceEquals(c1, null) || ReferenceEquals(c2, null))
                return false;
            return c1.X == c2.X && c1.Y == c2.Y;
        }
        public static bool operator !=(Coordinates c1, Coordinates c2)
        {
            return c1.X != c2.X || c1.Y != c2.Y;
        }
        public override bool Equals(object obj)
        {
            if (obj is Coordinates other)
            {
                return X == other.X && Y == other.Y;
            }
            return false;
        }
    }
}
