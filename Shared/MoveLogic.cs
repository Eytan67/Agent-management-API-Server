using AgentManagementAPIServer.Enums;
using AgentManagementAPIServer.Models;

namespace AgentManagementAPIServer.Shared
{
    public class MoveLogic
    {
        public static Coordinates NextLocation(Coordinates location, EDirection direction)
        {
            //throw an eror if it out of table
            Coordinates newLocation = new Coordinates();
            switch (direction)
            {
                case EDirection.s:
                    newLocation.X = location.X;
                    newLocation.Y = location.Y+1;
                    break;
                default:
                    break;
            }

            return newLocation;
        }
        public static EDirection ChoosDirection(Coordinates agentLocation, Coordinates targetLocation)
        {
            EDirection direction;
            if(agentLocation.X != targetLocation.X && agentLocation.Y != targetLocation.Y)
            {
                if (agentLocation.X < targetLocation.X)
                {
                    direction = agentLocation.Y < targetLocation.Y ? EDirection.ne : EDirection.se;
                }
                else
                {
                    direction = agentLocation.Y < targetLocation.Y ? EDirection.nw : EDirection.sw;
                }
            }
            else
            {
                if(agentLocation.X != targetLocation.X)
                {
                    direction = agentLocation.X < targetLocation.X ? EDirection.e : EDirection.w;
                }
                else
                {
                    direction = agentLocation.Y < targetLocation.Y ? EDirection.n : EDirection.s;
                }
            }
            return direction;

        }
        public static bool IsDistanceAppropriate(Coordinates agentLocation, Coordinates targetLocation)
        {
            var distance = Math.Sqrt(Math.Pow(agentLocation.X - targetLocation.X,2) + Math.Pow(agentLocation.Y - targetLocation.Y,2));

            return distance < 200;
        }
    }
}
