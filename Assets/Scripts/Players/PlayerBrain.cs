using System;
using System.Collections.Generic;
using System.Linq;
using CommandsShip;
using Map;
using Planets;
using Planets.MiningPlanet;
using Planets.PlayerPlanet;
using ShipLogic;
using Random = UnityEngine.Random;

namespace Players
{
    public class PlayerBrain : IDisposable
    {
        private readonly IPlayerPlanet _playerPlanet;
        private readonly IMiningPlanet[] _miningPlanets;

        public PlayerBrain(IPlayerPlanet playerPlanet, IMiningPlanet[] miningPlanet)
        {
            _playerPlanet = playerPlanet;
            _miningPlanets = miningPlanet;

            if (!Main.Instance.IsDebugMode)
            {
                Main.Instance.OnUpdateGame += CustomUpdate;
            }
            else
            {
                Main.Instance.OnUpdateGame += CustomUpdateDebug;
            }
        }

        public void InitPlanet()
        {
            _playerPlanet.Init();
        }

        /// <summary>
        /// В начале игры у каждого игрока есть 3 разведчика и 2 добыдчика
        /// </summary>
        public void CreateFirstShips()
        {
            if (Main.Instance.IsDebugMode)
            {
                return;
            }

            for (var i = 0; i < 3; i++)
            {
                _playerPlanet.AddShipToProduction(ShipType.Stealth, ship =>
                {
                    var commander = ship.GetCommander();
                    var pointForMovement =
                        SpaceMap.Map.GetSectorWithHighWeightForShip(_playerPlanet.PlayerType);
                    commander.SetPointForMovement(pointForMovement);
                });
            }

            for (var i = 0; i < 2; i++)
            {
                var index = i;
                _playerPlanet.AddShipToProduction(ShipType.Mining, ship =>
                {
                    var commander = ship.GetCommander();
                    commander.SetPointForMovement(_miningPlanets[index]);
                });
            }
        }

        private void CustomUpdate()
        {
            var playerLiveShips = SpaceMap.Map.GetAlliedShipsOnMap(_playerPlanet.PlayerType);
            TryToBuildNewShip(playerLiveShips);
        }

        private void CustomUpdateDebug()
        {
            var playerLiveShips = SpaceMap.Map.GetAlliedShipsOnMap(_playerPlanet.PlayerType);
            foreach (var commanderInfo in playerLiveShips)
            {
                commanderInfo.ExecuteCommand(Command.Movement(_miningPlanets[0]));
            }
        }

        /// <summary>
        /// Проверяем можем ли построить корабль
        /// </summary>
        private void TryToBuildNewShip(IEnumerable<ICommanderInfo> ships)
        {
            // Проверяем можем ли построить корабль
            var numberMiningShips = ships.Count(ship => ship.ShipType == ShipType.Mining);

            // Если у нас меньше двух буров, создаем новый

            if (numberMiningShips < 2)
            {
                if (PlayerGoldManager.CanMakeTransaction(_playerPlanet.CurrentGold, ShipType.Mining))
                {
                    // todo тут нужно выбрать в зависимости от меньшего коэффициента 
                    var miningPlanet = _miningPlanets[Random.Range(0, _miningPlanets.Length)];
                    AddShipToProduction(ShipType.Mining, miningPlanet);
                    
                    // Как вариант, добавить:
                    // Если буров достаточно смотрим на уровень важности, при низком уровне важности, создаем новый бур
                }
            }
            else
            {
                // todo в будущем тут будет генетика
                var randomShips = new[] { ShipType.Stealth, ShipType.Fighter, ShipType.AircraftCarrier };
                var ship = randomShips[Random.Range(0, randomShips.Length)];
                var pointForMovement = SpaceMap.Map.GetSectorWithHighWeightForShip(_playerPlanet.PlayerType);
                AddShipToProduction(ship, pointForMovement);
            }
        }

        private void AddShipToProduction(ShipType shipType, ITarget pointForMovement)
        {
            _playerPlanet.AddShipToProduction(shipType, ship =>
            {
                var commander = ship.GetCommander();
                commander.SetPointForMovement(pointForMovement);
            });
        }
        
        public void Dispose()
        {
            if (!Main.Instance.IsDebugMode)
            {
                Main.Instance.OnUpdateGame -= CustomUpdate;
            }
            else
            {
                Main.Instance.OnUpdateGame -= CustomUpdateDebug;
            }
        }
    }
}