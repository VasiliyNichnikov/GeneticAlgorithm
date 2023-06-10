using System.Collections.Generic;
using SpaceObjects;

namespace ShipLogic.Strategy.Attack
{
    public class MiningAttackLogic : IShipAttackLogic
    {
        public IEnumerable<ShipBase> Enemies => _logicBase.FoundEnemies;
        public IEnumerable<ShipBase> Allies => _logicBase.FoundAllies;
        public int NumberEnemies => _logicBase.FoundEnemies.Count;
        public int NumberAllies => _logicBase.FoundAllies.Count;

        public ShipBase SelectedEnemy => null;
        
        private readonly ShipAttackLogicBase _logicBase;

        public MiningAttackLogic(ShipBase ship)
        {
            _logicBase = new ShipAttackLogicBase(ship);
        }
        
        public void AddFoundShip(IDetectedObject detectedObject)
        {
            _logicBase.TryAddFoundShip(detectedObject);
        }

        public void RemoveFoundShip(IDetectedObject detectedObject)
        {
            _logicBase.TryRemoveFoundShip(detectedObject);
        }

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