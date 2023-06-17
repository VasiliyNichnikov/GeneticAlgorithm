using System.Linq;

namespace ShipLogic.Strategy.Attack
{
    public class StealthAttackLogic : IShipAttackLogic
    {
        public ShipBase SelectedEnemy => _logicBase.SelectedEnemy;

        private readonly IShipDetector _detector;
        private readonly ShipAttackLogicBase _logicBase;
        private readonly ShipBase _ship;

        public StealthAttackLogic(ShipBase ship)
        {
            _ship = ship;
            _detector = ship.GetDetector();
            _logicBase = new ShipAttackLogicBase(ship);
        }

        public void CheckEnemiesForOpportunityToAttack()
        {
            if (_logicBase.SelectedEnemy != null && _logicBase.SelectedEnemy.IsDead)
            {
                _detector.TryRemoveFoundShip(_logicBase.SelectedEnemy);
            }

            if (_logicBase.SelectedEnemy != null && _logicBase.SelectedEnemy.Gun.AttackingSelectedTarget(_ship))
            {
                return;
            }

            if (_detector.Enemies.Count == 0)
            {
                return;
            }

            var foundEnemy = _detector.Enemies.FirstOrDefault(enemy => enemy.Gun.AttackingSelectedTarget(_ship));
            if (foundEnemy == null)
            {
                return;
            }

            _logicBase.SelectedEnemy = foundEnemy;
            _logicBase.Commander.SetPointForMovementToEnemy(_logicBase.SelectedEnemy);
        }

        public void Dispose()
        {
            _logicBase.Dispose();
        }
    }
}