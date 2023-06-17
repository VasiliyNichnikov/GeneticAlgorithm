using System.Collections.Generic;
using SpaceObjects;
using UnityEngine;

namespace ShipLogic.AircraftCarrier
{
    public class ShipAircraftCarrier : ShipBase
    {
        protected override float MinAngleRotation => 5f;
        public override float ThreatLevel => 1.0f;
        public override ShipType Type => ShipType.AircraftCarrier;

        protected override ICommanderCommander GetNewCommander()
        {
            return new AircraftCarrierCommander(this);
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