using System.Collections.Generic;
using DebugShip;
using SpaceObjects;
using UnityEngine;

namespace ShipLogic.Fighter
{
    public class ShipFighter : ShipBase
    {
        protected override float MinAngleRotation => 5f;
        public override float ThreatLevel => 0.5f;
        public override ShipType Type => ShipType.Fighter;
        
        protected override IShipCommander GetNewCommander()
        {
            if (Main.Instance.IsDebugMode)
            {
                return new DebugFighterCommander(this);
            }
            return new FighterCommander(this);
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