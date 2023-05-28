using System;
using System.Collections.Generic;
using FindingPath;
using ShipLogic;
using UnityEngine;

namespace Pool
{
    public class ShipPool
    {
        public event Action<ShipBase> OnHideShip; 

        private readonly Queue<ShipBase> _cacheShips = new Queue<ShipBase>();

        public ShipBase GetOrCreateShip()
        {
            if (_cacheShips.Count == 0)
            {
                var createdShip = Main.Instance.FactoryShip.CreateShip();
                createdShip.Show();
                createdShip.InitCache(() => AddShipOnCache(createdShip));
                TrafficMap.Map.AddObjectOnMap(createdShip);
                return createdShip;
            }

            var cachedShip = _cacheShips.Dequeue();
            cachedShip.Show();
            TrafficMap.Map.AddObjectOnMap(cachedShip);
            return cachedShip;
        }

        private void AddShipOnCache(ShipBase ship)
        {
            TrafficMap.Map.RemoveObjectOnMap(ship);
            if (_cacheShips.Contains(ship))
            {
                Debug.LogWarning("Ship is already in cache");
                return;
            }

            OnHideShip?.Invoke(ship);
            _cacheShips.Enqueue(ship);
        }
    }
}