using System.Collections.Generic;
using System.Linq;
using SpaceObjects;

namespace ShipLogic.Strategy.Attack
{
    public class StealthAttackLogic : IShipAttackLogic
    {
        public IEnumerable<ShipBase> Enemies => _logicBase.FoundEnemies;
        public IEnumerable<ShipBase> Allies => _logicBase.FoundAllies;
        public int NumberEnemies => _logicBase.FoundEnemies.Count;
        public int NumberAllies => _logicBase.FoundAllies.Count;
        public ShipBase SelectedEnemy => _logicBase.SelectedEnemy;

        private readonly ShipAttackLogicBase _logicBase;
        private readonly ShipBase _ship;

        public StealthAttackLogic(ShipBase ship)
        {
            _ship = ship;
            _logicBase = new ShipAttackLogicBase(ship);
        }

        public void AddFoundShip(IDetectedObject detectedObject)
        {
            _logicBase.TryAddFoundShip(detectedObject);
        }

        public void RemoveFoundShip(IDetectedObject detectedObject)
        {
            // todo после уничтожения корабля только тот кто атаковал очищает вражеский корабль
            _logicBase.TryRemoveFoundShip(detectedObject);
        }

        public void CheckEnemiesForOpportunityToAttack()
        {
            if (_logicBase.SelectedEnemy != null && _logicBase.SelectedEnemy.IsDead)
            {
                _logicBase.TryRemoveFoundShip(_logicBase.SelectedEnemy);
            }

            if (_logicBase.SelectedEnemy != null && _logicBase.SelectedEnemy.Gun.AttackingSelectedTarget(_ship))
            {
                return;
            }

            if (_logicBase.FoundEnemies.Count == 0)
            {
                return;
            }

            var foundEnemy = _logicBase.FoundEnemies.FirstOrDefault(enemy => enemy.Gun.AttackingSelectedTarget(_ship));
            if (foundEnemy == null)
            {
                return;
            }

            _logicBase.SelectedEnemy = foundEnemy;
            _logicBase.CommanderCommander.SetPointForMovementToEnemy(_logicBase.SelectedEnemy);
        }

        public void Dispose()
        {
            _logicBase.Dispose();
        }
    }
}