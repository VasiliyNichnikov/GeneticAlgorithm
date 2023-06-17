using System;
using System.Collections.Generic;
using ShipLogic.Strategy.Attack;
using SpaceObjects;
using UnityEngine;

namespace ShipLogic.Mining
{
    public class ShipMining : ShipBase
    {
        public override float ThreatLevel => 1.0f;
        public override ShipType Type => ShipType.Mining;
        protected override float MinAngleRotation => 5f;

        protected override ICommanderCommander GetNewCommander()
        {
            return new MiningCommander(this, new MiningAttackLogic(this));
        }

        public override bool CanAttackOtherShip(IDetectedObject ship)
        {
            return false;
        }

        public override IReadOnlyCollection<Vector3> GetPointsInSector()
        {
            throw new Exception("");
        }
    }
}