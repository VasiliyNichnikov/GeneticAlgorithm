using System;
using System.Collections.Generic;
using SpaceObjects;

namespace ShipLogic.Strategy.Attack
{
    public interface IShipAttackLogic : IDisposable
    {
        IEnumerable<ShipBase> Enemies { get; }
        IEnumerable<ShipBase> Allies { get; }

        int NumberEnemies { get; }
        int NumberAllies { get; }
        ShipBase SelectedEnemy { get; }
        void AddFoundShip(IDetectedObject detectedObject);
        void RemoveFoundShip(IDetectedObject detectedObject);

        void CheckEnemiesForOpportunityToAttack();
    }
}