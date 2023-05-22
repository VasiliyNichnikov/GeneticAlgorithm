using ShipLogic;
using UnityEngine;

namespace Factories
{
    public class FactoryShip : MonoBehaviour
    {
        [SerializeField] private ShipBase _shipPrefab;
        [SerializeField] private Transform _parentShips;

        public ShipBase CreateShip()
        {
            var newShip = Instantiate(_shipPrefab, _parentShips, false);
            return newShip;
        }
    }
}