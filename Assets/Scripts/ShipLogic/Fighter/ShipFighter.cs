using System.Collections.Generic;
using DebugShip;
using Map;
using SpaceObjects;
using UnityEngine;

namespace ShipLogic.Fighter
{
    public class ShipFighter : ShipBase
    {
        protected override float MinAngleRotation => 5f;
        public override ShipType Type => ShipType.Fighter;
        
        public override bool CanAttackOtherShip(IObjectOnMap ship)
        {
            return SeeSelectedPointAngle(ship.ObjectPosition) && SeeOtherShipDistance(ship);
        }

        public override IReadOnlyCollection<Vector3> GetPointsInSector()
        {
            throw new System.NotImplementedException();
        }
    }
}