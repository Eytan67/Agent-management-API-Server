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
            var distance = Math.Sqrt(Math.Pow(agentLocation.X - targetLocation.X,2) + Math.Pow(agentLocation.Y - targetLocation.Y,2));

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
            vector.X = vector.X > 0 ? 1 : -1;
            vector.Y = vector.Y > 0 ? 1 : -1;
            return vector;
        }
        private static Dictionary<string, Coordinates> DirectionDictionary
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
    }
}
