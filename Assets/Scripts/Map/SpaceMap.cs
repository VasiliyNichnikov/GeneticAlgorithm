using System;
using System.Collections.Generic;
using System.Linq;
using Players;
using ShipLogic;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Map
{
    public class SpaceMap : MonoBehaviour
    {
        public GridViewing GridViewing { get; private set; }

        // Двигаем не статичные объекты (В нашем случае корабли)
        public event Action OnMoveNonStaticObjects;
        [SerializeField] private HeatMapVisual _heatMapVisualPrefab;
        [SerializeField] private GridSettings _gridSettings;

        private readonly List<IObjectOnMap> _objectsOnMap = new();
        private readonly ShipsOnMap _shipsOnMap = new ();
        
        private GridInt _gridForMovement;
        
        public static SpaceMap Map { get; private set; }
        private SectorHandler _sectorHandler;

        private void Awake()
        {
            Map = this;
            Init();
        }

        private void Init()
        {
            _sectorHandler = new SectorHandler(_gridSettings);
            
            _gridForMovement = _gridSettings.GridForMovement;
            GridViewing = new GridViewing(_gridSettings, transform, _heatMapVisualPrefab);
        }

        private void Update()
        {
            // Очищаем Grid
            ClearNotStaticObjectsFromGrid();

            // Отрисовываем заново все статичные объекты
            DrawStaticObjectOnGrid();

            // Двигаем все движущиеся объекты
            OnMoveNonStaticObjects?.Invoke();

            // Отрисовываем заново все движущиеся объекты
            DrawNotStaticObjectOnGrid();
            
            // Пересчитываем веса
            _sectorHandler.CheckAndUpdateWeightsInSectors();
        }


        public void AddObjectOnMap(IObjectOnMap objectOnMap)
        {
            if (_objectsOnMap.Contains(objectOnMap))
            {
                Debug.LogError("Current object contains in list");
                return;
            }

            _objectsOnMap.Add(objectOnMap);
            if (objectOnMap.TypeObject == MapObjectType.Ship && objectOnMap is ShipBase ship)
            {
                _shipsOnMap.AddShip(ship);
            }

            if (objectOnMap.IsStatic)
            {
                _gridForMovement.SetValuesAroundPerimeter(objectOnMap.RightTopPosition, objectOnMap.LeftBottomPosition,
                    (int)objectOnMap.TypeObject);
            }
        }

        public void RemoveObjectOnMap(IObjectOnMap objectOnMap)
        {
            if (!_objectsOnMap.Contains(objectOnMap))
            {
                return;
            }
            
            if (objectOnMap.TypeObject == MapObjectType.Ship && objectOnMap is ShipBase ship)
            {
                _shipsOnMap.RemoveShip(ship);
            }

            _gridForMovement.SetValue(objectOnMap.ObjectPosition, (int)MapObjectType.Empty);
            _objectsOnMap.Remove(objectOnMap);
        }

        public Vector3 TryGetRandomEmptyPointAroundObject(IObjectOnMap objectOnMap, int range, out bool isFound)
        {
            var emptyPoints =
                _gridForMovement.GetEmptyPointsAroundSelectedObject(objectOnMap.ObjectPosition, (int)MapObjectType.Empty,
                    range);
            isFound = emptyPoints.Length != 0;

            if (!isFound)
            {
                return Vector3.zero;
            }

            var randomPoint = Random.Range(0, emptyPoints.Length);
            return emptyPoints[randomPoint];
        }

        public IEnumerable<Vector3> TryGetRandomEmptyPointsAroundPosition(Vector3 position, int numberPoints, int range, out bool isFound)
        {
            var emptyPoints =
                _gridForMovement.GetEmptyPointsAroundSelectedObject(position, (int)MapObjectType.Empty, range);
            isFound = emptyPoints.Length != 0;
            if (!isFound)
            {
                return new[] { Vector3.zero };
            }

            numberPoints = Mathf.Min(numberPoints, emptyPoints.Length);
            var randomPoints = new List<int>();
            while (randomPoints.Count != numberPoints)
            {
                var randomPoint = Random.Range(0, emptyPoints.Length);
                if (randomPoints.Contains(randomPoint))
                {
                    continue;
                }
            
                randomPoints.Add(randomPoint);
            }

            return randomPoints.Select(index => emptyPoints[index]);
        }

        public IEnumerable<ICommanderInfo> GetAlliedShipsOnMap(PlayerType player)
        {
            return _shipsOnMap.GetAlliedShipsOnMap(player);
        }

        public IReadOnlyCollection<ShipBase> GetAlliedShipsInRange(PlayerType player, Vector3 center, float range)
        {
            return _shipsOnMap.GetAlliedShipsInRange(player, center, range);
        }

        public IReadOnlyCollection<ShipBase> GetAlliedShipsInRangeWithAdditionalCheck(PlayerType player, Vector3 center,
            float range, Func<ShipBase, bool> additionalCheck)
        {
            return _shipsOnMap.GetAlliedShipsInRange(player, center, range, additionalCheck);
        }

        private void DrawStaticObjectOnGrid()
        {
            foreach (var objectOnMap in _objectsOnMap)
            {
                if (!objectOnMap.IsStatic)
                {
                    continue;
                }

                _gridForMovement.SetValuesAroundPerimeter(objectOnMap.RightTopPosition, objectOnMap.LeftBottomPosition,
                    (int)objectOnMap.TypeObject);
            }
        }

        private void DrawNotStaticObjectOnGrid()
        {
            foreach (var objectOnMap in _objectsOnMap)
            {
                if (objectOnMap.IsStatic)
                {
                    continue;
                }

                if (_gridForMovement.GetValue(objectOnMap.ObjectPosition) == (int)MapObjectType.Empty)
                {
                    _gridForMovement.SetValue(objectOnMap.ObjectPosition, (int)objectOnMap.TypeObject);
                }
            }
        }

        private void ClearNotStaticObjectsFromGrid()
        {
            foreach (var objectOnMap in _objectsOnMap)
            {
                if (objectOnMap.IsStatic)
                {
                    continue;
                }

                _gridForMovement.SetValue(objectOnMap.ObjectPosition, (int)MapObjectType.Empty);
            }
        }

        public Vector3[] TryToFindPath(Vector3 startPosition, Vector3 endPosition, out bool isFound)
        {
            var startGridPoint = _gridForMovement.GetXZ(startPosition);
            var endGridPoint = _gridForMovement.GetXZ(endPosition);
            var endNode = Node.Create(endGridPoint);
            var reachable = new List<Node> { Node.Create(startGridPoint) };
            var explored = new List<Node>();

            while (reachable.Count > 0)
            {
                var node = ChoosePoint(reachable, endGridPoint);

                if (node.Equals(endNode))
                {
                    isFound = true;
                    return BuildPath(node);
                }

                reachable.Remove(node);
                explored.Add(node);

                var newPointsForMovement = GetAdjacentNodes(node.GridPoint, endGridPoint, explored);
                foreach (var adjacent in newPointsForMovement)
                {
                    if (!reachable.Contains(adjacent))
                    {
                        adjacent.SetPreviewNode(node);
                        reachable.Add(adjacent);
                    }
                }
            }

            isFound = false;
            return Array.Empty<Vector3>();
        }

        private Vector3[] BuildPath(Node node)
        {
            var path = new List<Vector3>();
            while (node != null)
            {
                path.Add(_gridForMovement.GetWorldPositionCenterCell(node.GridPoint));
                node = node.Preview;
            }

            path.Reverse();
            return path.ToArray();
        }

        /// <summary>
        /// Выбираем точку до которой мы можем добраться
        /// </summary>
        private static Node ChoosePoint(List<Node> reachable, Vector3Int endPoint)
        {
            if (reachable.Count == 1)
            {
                return reachable[0];
            }

            var minDistance = float.MaxValue;
            Node selectedNode = null;

            foreach (var node in reachable)
            {
                var distance = Vector3Int.Distance(node.GridPoint, endPoint);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    selectedNode = node;
                }
            }

            return selectedNode ?? reachable[0];
        }

        public (int x, int z) GetIndexSectorForObjectOnMap(IPosition positionObject)
        {
            _gridSettings.GetGridPlayerForSector(PlayerType.Player1)!.GetXZ(positionObject.ObjectPosition, out var x, out var z);
            return (x, z);
        }


        private IEnumerable<Node> GetAdjacentNodes(Vector3Int point, Vector3Int endGridPosition, List<Node> explored)
        {
            var x = point.x;
            const int y = 0;
            var z = point.z;

            // Ячейки сверху
            var topLeft = new Vector3Int(x - 1, y, z + 1);
            var top = new Vector3Int(x, y, z + 1);
            var topRight = new Vector3Int(x + 1, y, z + 1);

            // Ячейки по бокам
            var left = new Vector3Int(x - 1, y, z);
            var right = new Vector3Int(x + 1, y, z);

            // Ячейки снизу
            var bottomLeft = new Vector3Int(x - 1, y, z - 1);
            var bottom = new Vector3Int(x, y, z - 1);
            var bottomRight = new Vector3Int(x + 1, y, z - 1);

            var preparePoints = new List<Vector3Int>
            {
                topLeft, top, topRight,
                left, right,
                bottomLeft, bottom, bottomRight
            };

            return preparePoints
                .Where(p => _gridForMovement.GetValue(p.x, p.z) == (int)MapObjectType.Empty || p == endGridPosition)
                .Select(Node.Create)
                .Where(node => !explored.Contains(node));
        }

        private void OnDestroy()
        {
            GridViewing.Dispose();
        }

        /// <summary>
        /// Возвращает все точки, которые принадлежат сектору
        /// </summary>
        public IReadOnlyCollection<Vector3> GetObjectOnMapPointsInSectors(IObjectOnMap objectOnMap,
            Func<MapObjectType, bool> additionCheck = null)
        {
            var result = new List<Vector3>();

            var sectors = _gridSettings.GetGridPlayerForSector(PlayerType.Player1)!.GetPositionsOfCellsAroundPerimeter(
                    objectOnMap.RightTopPosition,
                    objectOnMap.LeftBottomPosition);

            var difference = DiffBetweenMovementAndSector();

            var hasAdditionalCheck = additionCheck != null;

            foreach (var sector in sectors)
            {
                var startX = sector.x * difference.x;
                var startZ = sector.z * difference.z;

                var endX = startX + difference.x;
                var endZ = startZ + difference.z;

                for (var x = startX; x < endX; x++)
                {
                    for (var z = startZ; z < endZ; z++)
                    {
                        if (hasAdditionalCheck)
                        {
                            if (additionCheck.Invoke((MapObjectType)_gridForMovement.GetValue(x, z)))
                            {
                                result.Add(_gridForMovement.GetWorldPosition(x, z));
                            }

                            continue;
                        }

                        result.Add(_gridForMovement.GetWorldPosition(x, z));
                    }
                }
            }

            return result;
        }

        private (int x, int z) DiffBetweenMovementAndSector() =>
            DifferenceBetweenGrids(_gridSettings.GridWrapperForMovement, _gridSettings.GetGridWrapperPlayerForSector(PlayerType.Player1));

        private (int x, int z) DifferenceBetweenGrids(GridWrapper one, GridWrapper two)
        {
            var diffX = one.GetWidth() > two.GetWidth()
                ? Mathf.RoundToInt(one.GetWidth() / two.GetWidth())
                : Mathf.RoundToInt(one.GetWidth() / two.GetWidth());
            var diffZ = one.GetLength() > two.GetLength()
                ? Mathf.RoundToInt(one.GetLength() / two.GetLength())
                : Mathf.RoundToInt(one.GetLength() / two.GetLength());
            return (diffX, diffZ);
        }
    }
}