using NavigatorShip;
using ShipLogic;
using UnityEngine;

namespace CommandsShip
{
    public class ToHelpCommand : ICommandLogic
    {
        private readonly ICommander _commander;
        
        public ToHelpCommand(ICommander commander)
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
            
            _commander.SetPointToHelp(command.Target);
        }
    }
}