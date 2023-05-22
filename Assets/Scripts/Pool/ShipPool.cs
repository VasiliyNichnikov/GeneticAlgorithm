using System.Collections.Generic;
using ShipLogic;
using UnityEngine;

namespace Pool
{
    public class ShipPool
    {
        private readonly Queue<ShipBase> _cacheShips = new Queue<ShipBase>();

        public ShipBase GetOrCreateShip()
        {
            if (_cacheShips.Count == 0)
            {
                var createdShip = Main.Instance.FactoryShip.CreateShip();
                createdShip.Show();
                createdShip.InitCache(() => AddShipOnCache(createdShip));
                return createdShip;
            }

            var cachedShip = _cacheShips.Dequeue();
            cachedShip.Show();
            return cachedShip;
        }

        private void AddShipOnCache(ShipBase ship)
        {
            if (_cacheShips.Contains(ship))
            {
                Debug.LogWarning("Ship is already in cache");
                return;
            }
            
            _cacheShips.Enqueue(ship);
        }
    }
}