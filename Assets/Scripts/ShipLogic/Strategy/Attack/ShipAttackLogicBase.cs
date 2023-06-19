using System;
using UnityEngine;

namespace ShipLogic.Strategy.Attack
{
    public class ShipAttackLogicBase : IDisposable
    {
        public ShipBase SelectedEnemy;
        public ICommander Commander => _commanderCache ??= _ship.GetCommander();
        private ICommander _commanderCache;
        
        private readonly ShipBase _ship;

        public ShipAttackLogicBase(ShipBase ship)
        {
            _ship = ship;
        }
        
        public void LosingSelectedEnemy()
        {
            if (SelectedEnemy == null)
            {
                Debug.LogError("Enemy is already null");
                return;
            }

            _ship.GetDetector().TryRemoveFoundShip(SelectedEnemy);
            Commander.ChangeStateToIdle();
            SelectedEnemy = null;
        }

        public void Dispose()
        {
            _commanderCache = null;
            SelectedEnemy = null;
        }
    }
}