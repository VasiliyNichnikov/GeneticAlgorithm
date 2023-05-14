using FindingPath;
using ShipLogic.Stealth;
using SpaceObjects;
using UnityEngine;

namespace ShipLogic
{
    public interface ITargetToAttack : IDetectedObject, IObjectOnMap
    {
        Vector3 Position { get; }
        PlayerType PlayerType { get; }
        /// <summary>
        /// Видим для выбранного корабля
        /// </summary>
        bool SeeOtherShip(ITargetToAttack ship);
    }
}