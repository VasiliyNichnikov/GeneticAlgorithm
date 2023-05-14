using StateMachineLogic;

namespace ShipLogic.Stealth.States
{
    public class StealIdleState : StateBase
    {
        private readonly ShipStealth _ship;
        private readonly StealthCommander _commander;
        
        public StealIdleState(StateMachine machine, StealthCommander commander, ShipStealth ship) : base(machine)
        {
            _ship = ship;
            _commander = commander;
        }

        public override void Enter()
        {
            base.Enter();
            _ship.TurnOffEngine();
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            // Если у нас обнаружен враг, идем в бой, при условие что у нас есть шансы выжить
            // Если дана точка для движения, двигаемся к ней
            ;
            // todo в будущем переделаю разведчика
            if (_commander.Enemy != null)
            {
                Machine.ChangeState(_commander.PrepareAttack);
                return;
            }
            
            // Для разведчика в приоритете разведка)
            if (_commander.PointForMovement != null)
            {
                Machine.ChangeState(_commander.Movement);
            }
        }
    }
}