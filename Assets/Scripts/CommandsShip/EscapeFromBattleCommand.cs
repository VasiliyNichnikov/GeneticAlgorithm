using NavigatorShip;
using ShipLogic;
using UnityEngine;

namespace CommandsShip
{
    public class EscapeFromBattleCommand : ICommandLogic
    {
        private readonly IShipCommander _commander;

        public EscapeFromBattleCommand(IShipCommander commander)
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

            Debug.LogWarning("Command: EscapeFromBattleCommand");
            _commander.SetPointForEscapeFromBattle(command.Target);
        }
    }
}