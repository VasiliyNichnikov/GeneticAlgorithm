using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using StateMachineLogic;
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

        private readonly Dictionary<GridDataType, GridWrapper> _gridsCache = new ();

        public GridWrapper GridWrapperForMovement => _gridsCache[GridDataType.Movement];
        public GridWrapper GridWrapperForSector => _gridsCache[GridDataType.Sector];

        [CanBeNull]
        public GridInt GridForMovement
        {
            get
            {
                var grid = GetGrid(GridDataType.Movement);
                return grid.HasGridInt ? grid.GridInt : null;
            }
        }

        [CanBeNull]
        public GridSector GridForSector
        {
            get
            {
                var grid = GetGrid(GridDataType.Sector);
                return grid.HasGridSector ? grid.GridSector : null;
            }
        }

        [CanBeNull]
        public GridInt GridForClick
        {
            get
            {
                var grid = GetGrid(GridDataType.ClickDebug);
                return grid.HasGridInt ? grid.GridInt : null;
            }
        }
        
        
        private GridWrapper GetGrid(GridDataType type)
        {
            if (!_gridsCache.ContainsKey(type))
            {
                _gridsCache[type] = CreateGridAndHeatMapVisual(type);
            }

            return _gridsCache[type];
        }
        
        private GridWrapper CreateGridAndHeatMapVisual(GridDataType type)
        {
            var data = GetGridDataByType(type);
            switch (type)
            {
                case GridDataType.ClickDebug:
                case GridDataType.Movement:
                    var gridInt = new GridInt(data.Weight, data.Length, data.CellSize, data.OriginPosition, transform, false);
                    return GridWrapper.CreateGridInt(gridInt);
                
                case GridDataType.Sector:
                    var gridSector = new GridSector(data.Weight, data.Length, data.CellSize, data.OriginPosition, transform, false);
                    return GridWrapper.CreateGridSector(gridSector);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        private GridData GetGridDataByType(GridDataType type)
        {
            return _gridsData.FirstOrDefault(d => d.Type == type);
        }
        
    }
}