using UnityEngine;

namespace ShipLogic.Individual
{
    public class ShipIndividual : ShipBase
    {
        public override IShipGun Gun { get; protected set; }

        public override bool SeeOtherShip(ITargetToAttack ship)
        {
            return Vector3.Distance(Position, ship.Position) <= Detector.Radius;
        }

        public override bool CanAttackOtherShip(ITargetToAttack ship)
        {
            var direction = ship.Position - Position;
            direction.y = 0.0f;
            var angleRotation = Vector3.Angle(direction, transform.forward);
            return angleRotation <= 5f;
        }
    }
}