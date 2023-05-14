using StateMachineLogic;
using UnityEngine;

namespace ShipLogic.Stealth.States
{
    public class StealPreparingForAttackState : StateBase
    {
        private readonly IShipCommander _commander;
        private readonly ShipBase _ship;
        
        public StealPreparingForAttackState(StateMachine machine, IShipCommander commander, ShipBase ship) : base(machine)
        {
            _commander = commander;
            _ship = ship;
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();

            if (_commander.Enemy == null)
            {
                Machine.ChangeState(_commander.Movement);
                return;   
            }
            
            if (!_ship.SeeOtherShip(_commander.Enemy))
            {
                _ship.PreparingForBattle(_commander.Enemy, out var stack);
                if (!stack)
                {
                    return;
                }
            }

            Machine.ChangeState(_commander.Attack);
        }
    }
}