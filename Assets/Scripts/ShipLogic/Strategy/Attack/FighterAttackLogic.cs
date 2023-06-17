using System.Collections.Generic;
using System.Linq;
using SpaceObjects;

namespace ShipLogic.Strategy.Attack
{
    public class FighterAttackLogic : IShipAttackLogic
    {
        public IEnumerable<ShipBase> Enemies => _logicBase.FoundEnemies;
        public IEnumerable<ShipBase> Allies => _logicBase.FoundAllies;
        public int NumberEnemies => _logicBase.FoundEnemies.Count;
        public int NumberAllies => _logicBase.FoundAllies.Count;
        public ShipBase SelectedEnemy => _logicBase.SelectedEnemy;

        private readonly ShipAttackLogicBase _logicBase;

        public FighterAttackLogic(ShipBase ship)
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
            if (_logicBase.SelectedEnemy != null && _logicBase.SelectedEnemy.IsDead)
            {
                _logicBase.TryRemoveFoundShip(_logicBase.SelectedEnemy);
            }

            if (_logicBase.FoundEnemies.Count == 0)
            {
                return;
            }

            if (_logicBase.SelectedEnemy != null)
            {
                return;
            }

            var firstEnemy = _logicBase.FoundEnemies.FirstOrDefault();
            if (_logicBase.SelectedEnemy != null && firstEnemy != null && firstEnemy != _logicBase.SelectedEnemy)
            {
                _logicBase.LosingSelectedEnemy();
            }

            if (firstEnemy == null)
            {
                return;
            }

            _logicBase.SelectedEnemy = firstEnemy;
            _logicBase.CommanderCommander.SetPointForMovementToEnemy(_logicBase.SelectedEnemy);
        }

        public void Dispose()
        {
            _logicBase.Dispose();
        }
    }
}