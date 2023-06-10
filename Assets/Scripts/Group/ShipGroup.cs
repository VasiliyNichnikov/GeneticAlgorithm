using System;
using System.Collections.Generic;
using System.Linq;
using Map;
using Planets;
using ShipLogic;
using UnityEngine;

namespace Group
{
    public class ShipGroup : IShipGroup
    {
        public bool IsAlive => _ships.Count > 0;
        public Vector3 CenterGroup => GetCenterPosition();
        
        public event Action<Vector3[]> OnUpdatePositionsInGroup;
        public bool CanAddShipInGroup => _ships.Count < _maxCountInGroup;
        
        private readonly List<ISupportedGroup> _ships = new();
        private readonly int _maxCountInGroup;
        private ITarget _targetForMovement;

        public ShipGroup(int maxCountInGroup)
        {
            _maxCountInGroup = maxCountInGroup;
            Main.Instance.ShipFactory.OnDestroyShip += RemoveShipInGroup;
        }

        public void UpdatePointForMovement(ITarget target)
        {
            _targetForMovement = target;
            RecalculatePositionShips();
        }
        
        public void AddShipInGroup(ISupportedGroup ship)
        {
            if (_ships.Contains(ship))
            {
                return;
            }

            _ships.Add(ship);
            RecalculatePositionShips();
        }

        public int GetIndexShip(ISupportedGroup ship)
        {
            for (var i = 0; i < _ships.Count; i++)
            {
                if (ship.Equals(_ships[i]))
                {
                    return i;
                }
            }

            return -1;
        }
        
        private void RemoveShipInGroup(ShipBase ship)
        {
            if (ship.GetCommander() is not ISupportedGroup supportedGroup)
            {
                return;
            }
            
            if (!_ships.Contains(supportedGroup))
            {
                return;
            }

            _ships.Remove(supportedGroup);
            RecalculatePositionShips();
        }

        private void RecalculatePositionShips()
        {
            if (_targetForMovement == null)
            {
                return;
            }
            
            var positions = SpaceMap.Map.TryGetRandomEmptyPointsAroundPosition(_targetForMovement.GetPointToApproximate(), _ships.Count, 2, out var isFound);
            var positionArray = positions.ToArray();
            if (positionArray.Length != _ships.Count)
            {
                Debug.LogError($"Number positions not equal number ships. Position array: {positionArray.Length}. Ships: {_ships.Count}");
                return;
            }

            OnUpdatePositionsInGroup?.Invoke(positionArray);
        }

        private Vector3 GetCenterPosition()
        {
            var totalPosition = new Vector3();

            foreach (var ship in _ships)
            {
                totalPosition += ship.ObjectPosition;
            }

            return totalPosition / _ships.Count;
        }
        
        public void Dispose()
        {
            Main.Instance.ShipFactory.OnDestroyShip -= RemoveShipInGroup;
        }
    }
}