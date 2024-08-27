using AgentManagementAPIServer.Enums;
using AgentManagementAPIServer.Models;

namespace AgentManagementAPIServer.Shared
{
    public class MoveLogic
    {
        public static Coordinates NextLocationByDirection(Coordinates location, string direction)
        {
            Coordinates vector = DirectionDictionary[direction];
            return NextLocation(location, vector);

        }
        public static Coordinates NextLocationByCoordinates(Coordinates location, Coordinates targetLocation)
        {
            var vector = DefineVector(location, targetLocation);
            return NextLocation(location, vector);
        }
        public static Coordinates NextLocation(Coordinates location, Coordinates vector)
        {
            Coordinates newLocation = location + vector;
            if (!IsInRange(newLocation))
            {
                //throw in valid move
                return location;
            }

            return newLocation;
        }
        public static bool IsDistanceAppropriate(Coordinates agentLocation, Coordinates targetLocation)
        {
            var distance = Distance(agentLocation, targetLocation);
            return distance < 200;

        }
        private static bool IsInRange(Coordinates coordinates)
        {
            return coordinates.X <= 1000
                && coordinates.Y <= 1000
                && coordinates.X >= 0
                && coordinates.Y >= 0;
        }
        private static Coordinates DefineVector(Coordinates agentPoints, Coordinates targetPoints)
        {
            var vector = targetPoints - agentPoints;
            vector.X = vector.X > 0 ? 1 : vector.X < 0 ? -1 : 0;
            vector.Y = vector.Y > 0 ? 1 : vector.Y > 0 ? -1 : 0;
            return vector;
        }
        private static readonly Dictionary<string, Coordinates> DirectionDictionary
            = new Dictionary<string, Coordinates>()
            {
                { "n", new Coordinates(0, 1) },
                { "ne", new Coordinates(1, 1) },
                { "e", new Coordinates(1, 0) },
                { "se", new Coordinates(1, -1) },
                { "s", new Coordinates(0, -1) },
                { "sw", new Coordinates(-1, -1) },
                { "w", new Coordinates(-1, 0) },
                { "nw", new Coordinates(-1, 1) }
            };

        public static TimeSpan CalculateLeftTime(Coordinates c1, Coordinates c2)
        {
            double distance = Distance(c1, c2);
            double seconds = (distance / 5) * 3600;
            TimeSpan leftTime = TimeSpan.FromSeconds(seconds);
            return leftTime;
        }
        public static double Distance(Coordinates c1,  Coordinates c2)
        {
          return Math.Sqrt( Math.Pow(c1.X - c2.X, 2) + Math.Pow(c1.Y - c2.Y, 2) );
        }

    }
}
