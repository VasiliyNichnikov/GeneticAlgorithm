using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FindingPath
{
    public class TrafficMap : MonoBehaviour, IFindingPath, IDisposable
    {
        [SerializeField] private int _weight;
        [SerializeField] private int _length;
        [SerializeField] private float _cellSize;
        [SerializeField] private Vector3 _originPosition;
        [SerializeField] private Transform _parentForText;
        [SerializeField] private HeatMapVisual _heatMapVisual;
        [SerializeField] private bool _isShowDebugMap;

        // Двигаем не статичные корабли
        public event Action OnMoveNonStaticObjects;

        private readonly List<IObjectOnMap> _objectsOnMap = new List<IObjectOnMap>();

        private Grid _grid;
        
        public static TrafficMap Map { get; private set; }


        private void Awake()
        {
            Map = this;
            Init();
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
        }

        private void Init()
        {
            _grid = new Grid(_weight, _length, _cellSize, _originPosition, _parentForText, _isShowDebugMap);
            _heatMapVisual.gameObject.SetActive(_isShowDebugMap);

            if (_isShowDebugMap)
            {
                _heatMapVisual.SetGrid(_grid);
                _grid.OnChangeCellValue += UpdateHeatMapVisual;
            }
        }

        private void UpdateHeatMapVisual((int x, int z) positionGrid)
        {
            _heatMapVisual.UpdateHeatMapVisual();
        }
        
        public void AddObjectOnMap(IObjectOnMap objectOnMap)
        {
            if (_objectsOnMap.Contains(objectOnMap))
            {
                Debug.LogError("Current object contains in list");
                return;
            }

            _objectsOnMap.Add(objectOnMap);

            if (objectOnMap.IsStatic)
            {
                _grid.SetValuesAroundPerimeter(objectOnMap.RightTopPosition, objectOnMap.LeftBottomPosition,
                    (int)objectOnMap.TypeObject);
            }
        }

        public void RemoveObjectOnMap(IObjectOnMap objectOnMap)
        {
            if (!_objectsOnMap.Contains(objectOnMap))
            {
                Debug.LogError("Current object not contains in list");
                return;
            }

            _objectsOnMap.Remove(objectOnMap);
        }

        public Vector3 TryGetRandomEmptyPointAroundObject(IObjectOnMap objectOnMap, int range, out bool isFound)
        {
            var emptyPoints =
                _grid.GetEmptyPointsAroundSelectedObject(objectOnMap.WorldPosition, (int)MapObjectType.Empty, range);
            isFound = emptyPoints.Length != 0;

            if (!isFound)
            {
                return Vector3.zero;
            }
            
            var randomPoint = Random.Range(0, emptyPoints.Length);
            return emptyPoints[randomPoint];
        }
        
        public void DrawAroundObject(IObjectOnMap objectOnMap, int range)
        {
            _grid.AddValue(objectOnMap.WorldPosition, 5, range);
        }

        private void DrawStaticObjectOnGrid()
        {
            foreach (var objectOnMap in _objectsOnMap)
            {
                if (!objectOnMap.IsStatic)
                {
                    continue;
                }

                _grid.SetValuesAroundPerimeter(objectOnMap.RightTopPosition, objectOnMap.LeftBottomPosition,
                    (int)objectOnMap.TypeObject);
            }
        }
        
        /// <summary>
        /// todo нужно сделать так, чтобы нельзя было стирать, если тип не равен empty
        /// </summary>
        private void DrawNotStaticObjectOnGrid()
        {
            foreach (var objectOnMap in _objectsOnMap)
            {
                if (objectOnMap.IsStatic)
                {
                    continue;
                }

                if (_grid.GetValue(objectOnMap.WorldPosition) == (int)MapObjectType.Empty)
                {
                    _grid.SetValue(objectOnMap.WorldPosition, (int)objectOnMap.TypeObject);
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

                _grid.SetValue(objectOnMap.WorldPosition, (int)MapObjectType.Empty);
            }
        }
        
        public Vector3[] TryToFindPath(Vector3 startPosition, Vector3 endPosition, out bool isFound)
        {
            var startGridPoint = _grid.GetXZ(startPosition);
            var endGridPoint = _grid.GetXZ(endPosition);
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
                path.Add(_grid.GetWorldPositionCenterCell(node.GridPoint));
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
                .Where(p => _grid.GetValue(p.x, p.z) == (int)MapObjectType.Empty || p == endGridPosition)
                .Select(Node.Create)
                .Where(node => !explored.Contains(node));
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_isShowDebugMap)
            {
                _grid.OnChangeCellValue -= UpdateHeatMapVisual;
            }
        }
    }
}