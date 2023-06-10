using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map
{
    public class GridSettings : MonoBehaviour
    {
        private enum GridDataType
        {
            Movement,
            Sector,
            ClickDebug
        }
        
        [Serializable]
        private struct GridData
        {
            public GridDataType Type => _type;
            public int Weight => _weight;
            public int Length => _length;
            public float CellSize => _cellSize;
            public Vector3 OriginPosition => _originPosition;
            
            [SerializeField] private GridDataType _type;
            [SerializeField] private int _weight;
            [SerializeField] private int _length;
            [SerializeField] private float _cellSize;
            [SerializeField] private Vector3 _originPosition;
        }

        
        [SerializeField]
        private GridData[] _gridsData;

        private readonly Dictionary<GridDataType, Grid> _gridsCache = new ();
        
        public Grid GridForMovement =>  GetGrid(GridDataType.Movement);
        public Grid GridForSector => GetGrid(GridDataType.Sector);
        public Grid GridForClick => GetGrid(GridDataType.ClickDebug);
        
        private Grid GetGrid(GridDataType type)
        {
            if (!_gridsCache.ContainsKey(type))
            {
                _gridsCache[type] = CreateGridAndHeatMapVisual(type);
            }

            return _gridsCache[type];
        }
        
        private Grid CreateGridAndHeatMapVisual(GridDataType type)
        {
            var data = GetGridDataByType(type);
            var grid = new Grid(data.Weight, data.Length, data.CellSize, data.OriginPosition, transform, false);
            return grid;
        }
        
        private GridData GetGridDataByType(GridDataType type)
        {
            return _gridsData.FirstOrDefault(d => d.Type == type);
        }
        
    }
}