using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Players;
using UnityEngine;

namespace Map
{
    public class GridSettings : MonoBehaviour
    {
        private enum GridDataType
        {
            Movement,
            SectorPlayer1,
            SectorPlayer2
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
        public GridPlayerSector GetGridPlayerForSector(PlayerType player)
        {
            if (player == PlayerType.None)
            {
                return null;
            }
            
            var grid = GetGridWrapperPlayerForSector(player);
            return grid.HasGridSector ? grid.GridPlayerSector : null;
        }

        public GridWrapper GetGridWrapperPlayerForSector(PlayerType player)
        {
            if (player == PlayerType.None)
            {
                return default;
            }
            var sectorType = player == PlayerType.Player1 ? GridDataType.SectorPlayer1 : GridDataType.SectorPlayer2;
            var grid = GetGrid(sectorType);
            return grid;
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
                case GridDataType.Movement:
                    var gridInt = new GridInt(data.Weight, data.Length, data.CellSize, data.OriginPosition, transform, false);
                    return GridWrapper.CreateGridInt(gridInt);
                
                case GridDataType.SectorPlayer1:
                case GridDataType.SectorPlayer2:
                    var gridSector = new GridPlayerSector(data.Weight, data.Length, data.CellSize, data.OriginPosition, transform, false);
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