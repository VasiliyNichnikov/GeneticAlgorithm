using StateMachineLogic;
using UnityEngine;

namespace ShipLogic.Stealth.States
{
    public class StealthMovementState : StateBase
    {
        private readonly ShipStealth _ship;
        private readonly StealthCommander _commander;
        private Vector3 _currentPointForMovement;
        
        public StealthMovementState(StateMachine machine, StealthCommander commander, ShipStealth ship) : base(machine)
        {
            _ship = ship;
            _commander = commander;
        }

        public override void Enter()
        {
            base.Enter();
            _ship.TurnOnEngine();
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();

            // todo в будущем переделаю разведчика
            if (_commander.Enemy != null)
            {
                Machine.ChangeState(_commander.PrepareAttack);
                return;
            }
            
            if (_commander.PointForMovement == null)
            {
                Machine.ChangeState(_commander.Idle);
                return;
            }

            if (_currentPointForMovement != _commander.PointForMovement.Value)
            {
                _currentPointForMovement = _commander.PointForMovement.Value;
                _ship.GetEngine().SetTarget(_currentPointForMovement);
            }
            
            _ship.GetEngine().Move();
        }
    }
}