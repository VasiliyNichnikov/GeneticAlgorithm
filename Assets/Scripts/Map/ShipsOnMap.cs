using System;
using System.Collections.Generic;
using System.Linq;
using Players;
using ShipLogic;
using UnityEngine;

namespace Map
{
    public class ShipsOnMap
    {
        private readonly Dictionary<PlayerType, List<ShipBase>> _ships = new ();
        
        public void AddShip(ShipBase ship)
        {
            if (ship.IsDead)
            {
                Debug.LogError("Ship is dead");
                return;
            }

            if (ship.PlayerType == PlayerType.None)
            {
                Debug.LogError($"Unsupported ship type: {ship.PlayerType}");
                return;
            }
            
            AddShipInDictionary(ship);
        }

        public void RemoveShip(ShipBase ship)
        {
            if (!_ships.ContainsKey(ship.PlayerType))
            {
                Debug.LogWarning($"Ship with type: {ship.PlayerType} is null");
                return;
            }

            var ships = _ships[ship.PlayerType];
            if (!ships.Contains(ship))
            {
                Debug.LogError("Ship is not in list ships");
                return;
            }

            _ships[ship.PlayerType].Remove(ship);
        }

        public IEnumerable<ICommander> GetAlliedShipsOnMap(PlayerType player)
        {
            if (!_ships.ContainsKey(player))
            {
                return ArraySegment<ICommander>.Empty;
            }
            
            return _ships[player].Where(s => !s.IsDead).Select(s => s.GetCommander());
        }

        public IReadOnlyCollection<ShipBase> GetAlliedShipsInRange(PlayerType player, Vector3 center, float range, Func<ShipBase, bool> additionalCheck = null)
        {
            if (!_ships.ContainsKey(player))
            {
                return ArraySegment<ShipBase>.Empty;
            }

            var result = new List<ShipBase>();
            foreach (var ship in _ships[player])
            {
                var distance = Vector3.Distance(center, ship.ObjectPosition);
                // distance >= ship.ShipRadius сделано для того, чтобы мы не добавили свой же корабль
                if (distance <= range && distance >= ship.Radius)
                {
                    if ((additionalCheck != null && additionalCheck.Invoke(ship)) || additionalCheck == null)
                    {
                        result.Add(ship);
                    }
                }
                
            }

            return result;
        }

        private void AddShipInDictionary(ShipBase ship)
        {
            if (_ships.TryGetValue(ship.PlayerType, out var ships))
            {
                if (ships.Contains(ship))
                {
                    Debug.LogError("Ship is already in list ships");
                    return;
                }
                _ships[ship.PlayerType].Add(ship);
            }
            else
            {
                _ships.Add(ship.PlayerType, new List<ShipBase> { ship });
            }
        }
    }
}