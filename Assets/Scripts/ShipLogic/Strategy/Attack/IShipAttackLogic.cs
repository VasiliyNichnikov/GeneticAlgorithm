using System;

namespace ShipLogic.Strategy.Attack
{
    public interface IShipAttackLogic : IDisposable
    {
        ShipBase SelectedEnemy { get; }
        void CheckEnemiesForOpportunityToAttack();
    }
}