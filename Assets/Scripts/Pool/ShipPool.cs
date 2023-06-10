using System;
using System.Collections.Generic;
using Map;
using ShipLogic;
using UnityEngine;

namespace Pool
{
    public class ShipPool
    {
        public event Action<ShipBase> OnHideShip;

        private readonly Dictionary<ShipType, Queue<ShipBase>> _cacheShips = new();

        public ShipBase GetOrCreateShip(ShipType ship)
        {
            if (!_cacheShips.ContainsKey(ship) || _cacheShips[ship].Count == 0)
            {
                var createdShip = Main.Instance.FactoryShip.CreateShip(ship);
                createdShip.Show();
                createdShip.InitCache(() => AddShipOnCache(createdShip));
                return createdShip;
            }
            
            var cacheShip = _cacheShips[ship].Dequeue();
            cacheShip.Show();
            return cacheShip;
        }

        private void AddShipOnCache(ShipBase ship)
        {
            if (!_cacheShips.ContainsKey(ship.Type))
            {
                _cacheShips[ship.Type] = new Queue<ShipBase>();
            }
            else
            {
                if (_cacheShips[ship.Type].Contains(ship))
                {
                    Debug.LogWarning("Ship is already in cache");
                    return;
                }
            }
            
            SpaceMap.Map.RemoveObjectOnMap(ship);
            OnHideShip?.Invoke(ship);
            _cacheShips[ship.Type].Enqueue(ship);
        }
    }
}