using System;
using System.Collections.Generic;
using UnityEngine;

namespace FindingPath
{
    public class TrafficMap : MonoBehaviour
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
    }
}