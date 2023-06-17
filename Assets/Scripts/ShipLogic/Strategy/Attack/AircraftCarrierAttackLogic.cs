using System.Linq;
using Map;
using UnityEngine;

namespace ShipLogic.Strategy.Attack
{
    public class AircraftCarrierAttackLogic : IShipAttackLogic
    {
        public ShipBase SelectedEnemy => _logicBase.SelectedEnemy;

        private readonly ShipBase _ship;
        private readonly IShipDetector _detector;
        private readonly ShipAttackLogicBase _logicBase;

        public AircraftCarrierAttackLogic(ShipBase ship)
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

            if (_logicBase.SelectedEnemy != null)
            {
                return;
            }

            if (_detector.Enemies.Count == 0)
            {
                return;
            }

            var enemiesAsList = _detector.Enemies.ToList();
            enemiesAsList.Sort(SortFoundEnemies);
            var firstEnemy = enemiesAsList[0];

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

        private int SortFoundEnemies(IObjectOnMap a, IObjectOnMap b)
        {
            var weight1 = Vector3.Distance(_ship.ObjectPosition, a.ObjectPosition);
            var weight2 = Vector3.Distance(_ship.ObjectPosition, b.ObjectPosition);

            weight1 += Vector3.Angle(_ship.transform.forward, a.ObjectPosition);
            weight2 += Vector3.Angle(_ship.transform.forward, b.ObjectPosition);

            return (int)(weight1 - weight2);
        }

        public void Dispose()
        {
            _logicBase.Dispose();
        }
    }
}