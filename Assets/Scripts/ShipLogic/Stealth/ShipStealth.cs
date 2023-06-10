using System.Collections.Generic;
using SpaceObjects;
using UnityEngine;

namespace ShipLogic.Stealth
{
    public class ShipStealth : ShipBase
    {
        protected override float MinAngleRotation => 5f;
        public override float ThreatLevel => 0.2f;
        public override ShipType Type => ShipType.Stealth;

        protected override IShipCommander GetNewCommander()
        {
            return new StealthCommander(this);
        }

        public override bool CanAttackOtherShip(IDetectedObject ship)
        {
            return SeeSelectedPointAngle(ship.ObjectPosition) && SeeOtherShipDistance(ship);
        }

        public override IReadOnlyCollection<Vector3> GetPointsInSector()
        {
            throw new System.NotImplementedException();
        }
    }
}