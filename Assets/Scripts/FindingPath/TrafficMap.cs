using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FindingPath
{
    public class TrafficMap : MonoBehaviour, IFindingPath
    {
        [SerializeField] private int _weight;
        [SerializeField] private int _length;
        [SerializeField] private float _cellSize;
        [SerializeField] private Vector3 _originPosition;
        [SerializeField] private Transform _parentForText;

        public event Action OnMovementObjects;

        private readonly List<IObjectOnMap> _objectsOnMap = new List<IObjectOnMap>();

        private Grid _grid;


        private void Update()
        {
            // Очищаем Grid
            ClearNotStaticObjectsFromGrid();

            // Двигаем все движущиеся объекты
            OnMovementObjects?.Invoke();

            // Отрисовываем заново все движущиеся объекты
            DrawNotStaticObjectOnGrid();
        }

        public void Init()
        {
            _grid = new Grid(_weight, _length, _cellSize, _originPosition, _parentForText);
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

                _grid.SetValuesAroundPerimeter(objectOnMap.RightTopPosition, objectOnMap.LeftBottomPosition,
                    (int)objectOnMap.TypeObject);
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

                _grid.SetValuesAroundPerimeter(objectOnMap.RightTopPosition, objectOnMap.LeftBottomPosition,
                    (int)MapObjectType.Empty);
            }
        }

        /// <summary>
        /// todo Не работает
        /// НУЖНО РАЗОБРАТЬСЯ
        /// СЕЙЧАС ВОЗВРАЩАЕТСЯ В ТУ ЖЕ ТОЧКУ ОТ КУДА ВЫЛЕТЕЛ
        /// </summary>
        public Vector3[] TryToFindPath(Vector3 startPosition, Vector3 endPosition, out bool isFound)
        {
            var endGridPoint = _grid.GetXZ(endPosition);
            var endNode = Node.Create(endGridPoint);
            var reachable = new List<Node> { Node.Create(_grid.GetXZ(startPosition)) };
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
                path.Add(_grid.GetWorldPosition(node.GridPoint));
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
        
        /// <summary>
        /// Возвращает примыкающие точки
        /// todo не самое лучшее решение с endGridPosition, но иначе мы не будем находить конечную точку
        /// </summary>
        private IEnumerable<Node> GetAdjacentNodes(Vector3Int point, Vector3Int endGridPosition, List<Node> explored)
        {
            var x = point.x;
            var y = 0;
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
    }
}