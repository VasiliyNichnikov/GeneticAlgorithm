using Loaders;
using Map;
using NavigatorShip;
using UnityEngine;

namespace CommandsShip
{
    public class HelpCommand : ICommandLogic
    {
        private float _radiusOfHelp;

        public HelpCommand()
        {
            Main.Instance.LoaderManager.GetAsync<SignalsLoader>(result =>
            {
                _radiusOfHelp = result.GetHelpSignal().RadiusOfHelp;
            }, false);
        }
        
        public void ExecuteRequest(Command command)
        {
            if (command.Target == null)
            {
                Debug.LogError("Target is null");
                return;
            }

            var target = command.Target;
            var ships = SpaceMap.Map.GetAlliedShipsInRange(command.PlayerType, target.GetPointToApproximate(), _radiusOfHelp);
            foreach (var ship in ships)
            {
                ship.GetCommander().ExecuteCommand(Command.Movement(command.Target));
            }
        }
    }
}