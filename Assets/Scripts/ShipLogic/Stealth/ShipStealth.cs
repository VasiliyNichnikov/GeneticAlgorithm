using System;
using System.Collections.Generic;
using Map;
using UnityEngine;

namespace ShipLogic.Stealth
{
    public class ShipStealth : ShipBase
    {
        protected override float MinAngleRotation => 5f;
        public override ShipType Type => ShipType.Stealth;

        public override bool CanAttackOtherShip(IObjectOnMap ship)
        {
            return SeeSelectedPointAngle(ship.ObjectPosition) && SeeOtherShipDistance(ship);
        }

        public override IReadOnlyCollection<Vector3> GetPointsInSector()
        {
            throw new NotImplementedException();
        }
    }
}