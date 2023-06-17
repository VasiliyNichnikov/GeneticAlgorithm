namespace ShipLogic.Strategy.Attack
{
    public class MiningAttackLogic : IShipAttackLogic
    {
        public ShipBase SelectedEnemy => null;

        public void CheckEnemiesForOpportunityToAttack()
        {
            // nothing
        }

        public void Dispose()
        {
            // nothing
        }
    }
}