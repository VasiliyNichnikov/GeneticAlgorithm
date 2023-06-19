using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Loaders;
using Planets;
using Players;
using ShipLogic;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Map
{
    public class SectorWrapper : ITarget
    {
        public float ThreatLevel { get; }
        private readonly Vector3 _pointToApproximate;
        private readonly IReadOnlyCollection<Vector3> _pointsInSector;

        public SectorWrapper(float weight, Vector3 pointToApproximate, IReadOnlyCollection<Vector3> pointsInSector)
        {
            ThreatLevel = weight;
            _pointToApproximate = pointToApproximate;
            _pointsInSector = pointsInSector;
        }

        public Vector3 GetPointToApproximate()
        {
            return _pointToApproximate;
        }

        public IReadOnlyCollection<Vector3> GetPointsInSector()
        {
            return _pointsInSector;
        }
    }

    public class SectorHandler
    {
        private readonly struct SectorAndShips
        {
            public readonly float Weight;
            public readonly Vector3Int SectorLocalPosition;
            public readonly List<ICommanderInfo> Ships;

            public SectorAndShips(float weight, Vector3Int sector, List<ICommanderInfo> ships)
            {
                Weight = weight;
                SectorLocalPosition = sector;
                Ships = ships;
            }

            public void AddShip(ICommanderInfo commanderInfo)
            {
                if (Ships == null)
                {
                    Debug.LogError("List ships is null");
                    return;
                }

                Ships.Add(commanderInfo);
            }
        }

        private readonly GridSettings _settings;

        private float _minValueWeight;
        private float _maxValueWeight;
        private readonly SpaceMap _spaceMap;
        
        public SectorHandler(SpaceMap spaceMap, GridSettings settings)
        {
            _spaceMap = spaceMap;
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

        public SectorWrapper GetSectorForSelectedShip(PlayerType player, ICommanderInfo commanderInfo)
        {
            var grid = _settings.GetGridPlayerForSector(player);
            if (grid == null)
            {
                Debug.LogError($"Grid for player: {player} is null");
                return null;
            }

            var selectedSector = grid.GetXZ(commanderInfo.PositionShip);
            
            var sectorWrapper = CreateSectorWrapper(grid, selectedSector);
            return sectorWrapper;
        }
        
        [CanBeNull]
        public SectorWrapper GetSectorWithHighWeightForShip(PlayerType player)
        {
            var grid = _settings.GetGridPlayerForSector(player);
            if (grid == null)
            {
                Debug.LogError($"Grid for player: {player} is null");
                return null;
            }

            var sectors = grid.GetAllSectors();

            if (sectors.Length == 0)
            {
                Debug.LogError("Sectors not found");
                return null;
            }
            
            var selectedSector = sectors[Random.Range(0, sectors.Length)];
            var sectorWrapper = CreateSectorWrapper(grid, selectedSector);

            return sectorWrapper;
        }

        private static Vector3Int[] GetAllAvailableSectors(GridPlayerSector grid)
        {
            var allSectors = grid.GetAllSectors();
            var result = new List<Vector3Int>();

            foreach (var sector in allSectors)
            {
                var weight = grid.GetValue(sector.x, sector.z);
                if (weight >= 0.01f)
                {
                    result.Add(sector);
                }
            }

            return result.Count == 0 ? allSectors : result.ToArray();
        }
        
        private Vector3Int[] GetAllAvailableSectors(GridPlayerSector grid, PlayerType player)
        {
            var allSectors = grid.GetAllSectors();
            var sectorAndShips = new List<SectorAndShips>();

            foreach (var sector in allSectors)
            {
                var weight = grid.GetValue(sector.x, sector.z);
                var sectorAndShip = new SectorAndShips(weight, sector, new List<ICommanderInfo>());
                sectorAndShips.Add(sectorAndShip);
            }

            var ships = _spaceMap.GetAlliedShipsOnMap(player);

            foreach (var commanderInfo in ships)
            {
                var sectorLocalPosition = grid.GetXZ(commanderInfo.PositionShip);
                var sectorData =
                    sectorAndShips.FirstOrDefault(data => data.SectorLocalPosition.Equals(sectorLocalPosition));
                if (sectorData.Ships == null)
                {
                    sectorAndShips.Add(new SectorAndShips(grid.GetValue(sectorLocalPosition.x, sectorLocalPosition.z),
                        sectorLocalPosition,
                        new List<ICommanderInfo> { commanderInfo }));
                }
                else
                {
                    sectorData.AddShip(commanderInfo);
                }
            }

            var result = new List<Vector3Int>();
            foreach (var sectorAndShip in sectorAndShips)
            {
                if (sectorAndShip.Weight > 0.1f)
                {
                    result.Add(sectorAndShip.SectorLocalPosition);
                }
            }

            if (result.Count != 0)
            {
                return result.ToArray();
            }

            return sectorAndShips.Select(data => data.SectorLocalPosition).ToArray();
        }

        private static int SortSectorAndShips(SectorAndShips a, SectorAndShips b)
        {
            var shipsA = a.Ships.Count;
            var weightA = a.Weight;

            var shipsB = b.Ships.Count;
            var weightB = b.Weight;

            float balanceShips = shipsA;
            if (shipsA != 0)
            {
                balanceShips = (float)shipsB / shipsA;
            }

            var balanceWeights = weightB / weightA;

            // // Первая проверка на высокий вес и малое кол-во кораблей
            // if (balanceWeights >= 1 && balanceShips < 1)
            // {
            //     // Сортировка от большего к меньшему
            //     return Mathf.RoundToInt(shipsB - shipsA);
            // }
            //
            // // Вторая проверка на высокий вес и большое кол-во кораблей
            // if (balanceWeights >= 1 && balanceShips >= 1)
            // {
            //     return 0;
            // }
            //
            // // Третья проверка на низкий вес и малое кол-во кораблей
            // if (balanceWeights < 1 && balanceShips < 1)
            // {
            //     return 0;
            // }
            //
            // // Четвертая проверка на низкий вес и большое кол-во кораблей
            // if (balanceWeights < 1 && balanceShips >= 1)
            // {
            //     // Сортировка от большего к меньшему
            //     return Mathf.RoundToInt(weightB - weightA);
            // }

            return Mathf.RoundToInt(shipsA - shipsB);
        }

        private void SetWeighsForSelectedPlayer(PlayerType player)
        {
            var sectorAndShips = new Dictionary<Vector3Int, List<ICommanderInfo>>();

            var grid = _settings.GetGridPlayerForSector(player);
            if (grid == null)
            {
                Debug.LogError($"Grid for player: {player} is null");
                return;
            }

            // Распределяем корабли на сектора
            // и говорим в каком секторе какой корабль должен быть расположен
            var commanderInfos = _spaceMap.GetAlliedShipsOnMap(player);

            foreach (var commanderInfo in commanderInfos)
            {
                var sectorPosition = grid.GetXZ(commanderInfo.PositionShip);

                if (sectorAndShips.TryGetValue(sectorPosition, out var ships))
                {
                    ships.Add(commanderInfo);
                }
                else
                {
                    sectorAndShips[sectorPosition] = new List<ICommanderInfo> { commanderInfo };
                }
            }

            // Рассчитываем вес для каждого корабля в секторе и складываем в сумму
            foreach (var (sector, ships) in sectorAndShips)
            {
                var weight = ships.Sum(ship => ship.CalculateWeight());
                var weightClamp01 = Mathf.Clamp(weight, _minValueWeight, _maxValueWeight);
                weight = Converter.ConvertFromOneRangeToAnother(_minValueWeight, _maxValueWeight,
                    GridInt.HeatMapMinValue, GridInt.HeatMapMaxValue, weightClamp01);
                grid.SetValue(sector, weight);

                // var sectorWrapper = CreateSectorWrapper(grid, sector);
                // OnUpdateWeightInSector?.Invoke(sectorWrapper);
            }
        }

        private SectorWrapper CreateSectorWrapper(GridPlayerSector grid, Vector3Int sectorGridPosition)
        {
            var weight = grid.GetValue(sectorGridPosition.x, sectorGridPosition.z);
            var sectorWorldPosition = grid.GetWorldPositionCenterCell(sectorGridPosition);
            return new SectorWrapper(weight, sectorWorldPosition,
                _spaceMap.GetPointsForMovementInSector(grid, sectorWorldPosition));
        }
    }
}