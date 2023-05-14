using ShipLogic.Stealth;
using SpaceObjects;
using UnityEngine;

namespace ShipLogic
{
    public interface ITargetToAttack : IDetectedObject
    {
        Vector3 Position { get; }
        bool IsDead { get; }
        bool IsShip { get; }
        PlayerType PlayerType { get; }
        /// <summary>
        /// Видим для выбранного корабля
        /// </summary>
        bool SeeOtherShip(ITargetToAttack ship);
    }
}