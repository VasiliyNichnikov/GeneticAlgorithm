using Players;
using SpaceObjects;
using UnityEngine;

namespace ShipLogic
{
    public interface ITargetToAttack : IDetectedObject
    {
        Vector3 Position { get; }
        /// <summary>
        /// Видим для выбранного корабля
        /// </summary>
        bool SeeOtherShip(ITargetToAttack ship);
    }
}