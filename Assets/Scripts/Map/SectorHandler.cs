using System.Collections.Generic;
using System.Linq;
using Loaders;
using Players;
using ShipLogic;
using UnityEngine;
using Utils;

namespace Map
{
    public class SectorHandler
    {
        private readonly GridSettings _settings;

        private float _minValueWeight;
        private float _maxValueWeight;

        public SectorHandler(GridSettings settings)
        {
            _settings = settings;
            
            Main.Instance.LoaderManager.LoadAsync<WeightsLoader>(loader =>
            {
                _minValueWeight = loader.MinWeight;
                _maxValueWeight = loader.MaxWeight;
            }, false);
        }


        public void CheckAndUpdateWeightsInSectors()
        {
            SetWeighsForSelectedPlayer(PlayerType.Player1);
            SetWeighsForSelectedPlayer(PlayerType.Player2);
        }

        private void SetWeighsForSelectedPlayer(PlayerType player)
        {
            var sectorAndShips = new Dictionary<Vector3Int, List<ICommanderInfo>>();

            // Распределяем корабли на сектора
            // и говорим в каком секторе какой корабль должен быть расположен
            var commanderInfos = SpaceMap.Map.GetAlliedShipsOnMap(player);
            
            foreach (var commanderInfo in commanderInfos)
            {
                var sectorPosition = _settings.GetGridPlayerForSector(player)?.GetXZ(commanderInfo.PositionShip);
                if (sectorPosition == null)
                {
                    return;
                }

                if (sectorAndShips.TryGetValue(sectorPosition.Value, out var ships))
                {
                    ships.Add(commanderInfo);
                }
                else
                {
                    sectorAndShips[sectorPosition.Value] = new List<ICommanderInfo> { commanderInfo };
                }
            }

            // Рассчитываем вес для каждого корабля в секторе и складываем в сумму
            foreach (var (sector, ships) in sectorAndShips)
            {
                var weight = ships.Sum(ship => ship.CalculateWeight());
                weight = Mathf.Clamp(weight, _minValueWeight, _maxValueWeight);
                weight = Converter.ConvertFromOneRangeToAnother(_minValueWeight, _maxValueWeight,
                    GridInt.HeatMapMinValue, GridInt.HeatMapMaxValue, weight);
                _settings.GetGridPlayerForSector(player)?.SetValue(sector, weight);
            }
        }
    }
}