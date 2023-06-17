using NavigatorShip;
using ShipLogic;
using UnityEngine;

namespace CommandsShip
{
    public class MovementCommand : ICommandLogic
    {
        private readonly ICommanderCommander _commanderCommander;


        public MovementCommand(ICommanderCommander commanderCommander)
        {
            _commanderCommander = commanderCommander;
        }
        
        public void ExecuteRequest(Command command)
        {
            if (command.Target == null)
            {
                Debug.LogError("Target is null");
                return;
            }
            
            _commanderCommander.SetPointForMovement(command.Target);
        }
    }
}