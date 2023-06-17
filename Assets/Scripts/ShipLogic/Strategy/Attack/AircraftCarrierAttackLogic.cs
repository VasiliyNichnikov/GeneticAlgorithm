using System.Collections.Generic;
using System.Linq;
using SpaceObjects;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

namespace ShipLogic.Strategy.Attack
{
    public class AircraftCarrierAttackLogic : IShipAttackLogic
    {
        public IEnumerable<ShipBase> Enemies => _logicBase.FoundEnemies;
        public IEnumerable<ShipBase> Allies => _logicBase.FoundAllies;
        public int NumberEnemies => _logicBase.FoundEnemies.Count;
        public int NumberAllies => _logicBase.FoundAllies.Count;
        public ShipBase SelectedEnemy => _logicBase.SelectedEnemy;

        private readonly ShipBase _ship;
        private readonly ShipAttackLogicBase _logicBase;

        public AircraftCarrierAttackLogic(ShipBase ship)
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
            _logicBase.TryRemoveFoundShip(detectedObject);
        }

        public void CheckEnemiesForOpportunityToAttack()
        {
            if (_logicBase.SelectedEnemy != null && _logicBase.SelectedEnemy.IsDead)
            {
                _logicBase.TryRemoveFoundShip(_logicBase.SelectedEnemy);
            }

            if (_logicBase.SelectedEnemy != null)
            {
                return;
            }

            if (_logicBase.FoundEnemies.Count == 0)
            {
                return;
            }

            var firstEnemy = _logicBase.SortAndGetFirstEnemyShip(SortFoundEnemies);

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

        private int SortFoundEnemies(ShipBase a, ShipBase b)
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