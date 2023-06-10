using System;
using Planets;
using UnityEngine;

namespace Group
{
    public interface IShipGroup : IDisposable
    {
        /// <summary>
        /// Жива ли группа, есть ли в ней участники (Корабли)
        /// </summary>
        bool IsAlive { get; }
        int GetIndexShip(ISupportedGroup ship);

        Vector3 CenterGroup { get; }
        event Action<Vector3[]> OnUpdatePositionsInGroup;

        void UpdatePointForMovement(ITarget target);
        bool CanAddShipInGroup { get; }
        void AddShipInGroup(ISupportedGroup ship);
    }
}