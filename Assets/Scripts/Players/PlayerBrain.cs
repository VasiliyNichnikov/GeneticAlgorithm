using System;
using System.Linq;
using CommandsShip;
using Map;
using Planets.MiningPlayer;
using ShipLogic;
using UnityEngine;

namespace Players
{
    public class PlayerBrain : IDisposable
    {
        private readonly IMiningPlanet[] _miningPlanets;
        private readonly PlayerType _player;

        public PlayerBrain(PlayerType player, IMiningPlanet[] miningPlanet)
        {
            _player = player;
            _miningPlanets = miningPlanet;

            if (!Main.Instance.IsDebugMode)
            {
                Main.Instance.OnUpdateGame += CustomUpdate;
            }
        }


        private void CustomUpdate()
        {
            GenerateTasks();
        }
        
        private void GenerateTasks()
        {
            var playerLiveShips = SpaceMap.Map.GetAlliedShipsOnMap(_player);

            foreach (var ship in playerLiveShips)
            {
                // Debug.LogWarning($"Health percentage: {ship.HealthPercentages}. Player: {_player}");
                // Если с кораблем все ок, пытаемся долететь до точки с добычой ресурсов
                if (ship.HealthPercentages >= 0.7f)
                {
                    GiveCommandToMoveToMiningPlanet(ship);
                    continue;
                }
                
                // Если корабль почти погибает и численость на стороне врага, пытаемся сбежать из боя
                if (ship.IsInBattle && 
                    // ship.NumberOfEnemiesNearby > ship.NumberOfAlliesNearby &&
                    ship.HealthPercentages <= 0.45f)
                {
                    GiveCommandEscape(ship);
                    continue;
                }

                continue;
                // Если корабль в бою и у него мало здоровья и численность врагом, то просим о помощи
                if (ship.IsInBattle &&
                    ship.NumberOfEnemiesNearby > ship.NumberOfAlliesNearby && 
                    ship.HealthPercentages <= 0.5f)
                {
                    GiveCommandHelp(ship);
                    continue;
                }
            }
        }

        private void GiveCommandToMoveToMiningPlanet(ICommanderInfo commanderInfo)
        {
            commanderInfo.ExecuteCommand(Command.Movement(_miningPlanets[0]));
        }

        private void GiveCommandHelp(ICommanderInfo commanderInfo)
        {
            commanderInfo.ExecuteCommand(Command.Help(_player, commanderInfo.ShipTarget));
        }

        private void GiveCommandEscape(ICommanderInfo commanderInfo)
        {
            commanderInfo.ExecuteCommand(Command.EscapeFromBattle(_player));
        }

        public void Dispose()
        {
            if (!Main.Instance.IsDebugMode)
            {
                Main.Instance.OnUpdateGame -= CustomUpdate;
            }
        }
    }
}