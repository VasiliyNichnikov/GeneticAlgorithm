namespace ShipLogic.Fighter
{
    public class ShipFighter : ShipBase
    {
        protected override float OnBoostSpeed(float currentSpeed)
        {
            throw new System.NotImplementedException();
        }

        protected override float OnSlowingDownSpeed(float currentSpeed)
        {
            throw new System.NotImplementedException();
        }

        public override bool SeeOtherShip(ITargetToAttack ship)
        {
            throw new System.NotImplementedException();
        }

        public override void PreparingForBattle(ITargetToAttack shipEnemy)
        {
            throw new System.NotImplementedException();
        }
    }
}