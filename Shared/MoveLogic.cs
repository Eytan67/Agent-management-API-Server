using AgentManagementAPIServer.Enums;
using AgentManagementAPIServer.Models;

namespace AgentManagementAPIServer.Shared
{
    public class MoveLogic
    {
        public static Location NextLocation(Location location, EDirection direction)
        {
            Location newLocation = new Location();
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
        //public static void 
    }
}
