using System.Collections.Generic;
using Loaders;
using Map;
using SpaceObjects;
using UnityEngine;

namespace ShipLogic.AircraftCarrier
{
    public class ShipAircraftCarrier : ShipBase
    {
        protected override float MinAngleRotation => 5f;
        public override ShipType Type => ShipType.AircraftCarrier;

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