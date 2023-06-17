using NavigatorShip;
using ShipLogic;
using UnityEngine;

namespace CommandsShip
{
    public class EscapeFromBattleCommand : ICommandLogic
    {
        private readonly ICommanderCommander _commanderCommander;

        public EscapeFromBattleCommand(ICommanderCommander commanderCommander)
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

            Debug.LogWarning("Command: EscapeFromBattleCommand");
            _commanderCommander.SetPointForEscapeFromBattle(command.Target);
        }
    }
}