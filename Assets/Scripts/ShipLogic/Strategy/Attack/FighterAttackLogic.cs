using System.Linq;

namespace ShipLogic.Strategy.Attack
{
    public class FighterAttackLogic : IShipAttackLogic
    {
        public ShipBase SelectedEnemy => _logicBase.SelectedEnemy;

        private readonly IShipDetector _detector;
        private readonly ShipAttackLogicBase _logicBase;

        public FighterAttackLogic(ShipBase ship)
        {
            _detector = ship.GetDetector();
            _logicBase = new ShipAttackLogicBase(ship);
        }

        public void CheckEnemiesForOpportunityToAttack()
        {
            if (_logicBase.SelectedEnemy != null && _logicBase.SelectedEnemy.IsDead)
            {
                _detector.TryRemoveFoundShip(_logicBase.SelectedEnemy);
            }

            if (_detector.Enemies.Count == 0)
            {
                return;
            }

            if (_logicBase.SelectedEnemy != null)
            {
                return;
            }

            var firstEnemy = _detector.Enemies.FirstOrDefault();
            if (_logicBase.SelectedEnemy != null && firstEnemy != null && firstEnemy != _logicBase.SelectedEnemy)
            {
                _logicBase.LosingSelectedEnemy();
            }

            if (firstEnemy == null)
            {
                return;
            }

            _logicBase.SelectedEnemy = firstEnemy;
            _logicBase.Commander.SetPointForMovementToEnemy(_logicBase.SelectedEnemy);
        }

        public void Dispose()
        {
            _logicBase.Dispose();
        }
    }
}