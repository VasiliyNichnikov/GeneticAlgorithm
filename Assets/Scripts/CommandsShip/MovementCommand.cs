using NavigatorShip;
using ShipLogic;
using UnityEngine;

namespace CommandsShip
{
    public class MovementCommand : ICommandLogic
    {
        private readonly IShipCommander _commander;


        public MovementCommand(IShipCommander commander)
        {
            _commander = commander;
        }
        
        public void ExecuteRequest(Command command)
        {
            if (command.Target == null)
            {
                Debug.LogError("Target is null");
                return;
            }
            
            _commander.SetPointForMovement(command.Target);
        }
    }
}