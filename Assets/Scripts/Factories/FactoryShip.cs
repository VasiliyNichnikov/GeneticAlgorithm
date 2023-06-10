using System.Linq;
using ShipLogic;
using UnityEngine;

namespace Factories
{
    public class FactoryShip : MonoBehaviour
    {
        [SerializeField] private ShipBase[] _shipsPrefab;
        [SerializeField] private Transform _parentShips;

        public ShipBase CreateShip(ShipType shipType)
        {
            var prefab = _shipsPrefab.First(ship => ship.Type == shipType);
            var newShip = Instantiate(prefab, _parentShips, false);
            return newShip;
        }
    }
}