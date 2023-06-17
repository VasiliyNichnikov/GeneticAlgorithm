using System;
using System.Collections.Generic;
using Map;
using UnityEngine;

namespace ShipLogic.Mining
{
    public class ShipMining : ShipBase
    {
        public override ShipType Type => ShipType.Mining;
        protected override float MinAngleRotation => 5f;
        
        public override bool CanAttackOtherShip(IObjectOnMap ship)
        {
            return false;
        }

        public override IReadOnlyCollection<Vector3> GetPointsInSector()
        {
            throw new Exception("");
        }
    }
}