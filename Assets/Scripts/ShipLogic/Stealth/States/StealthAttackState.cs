using StateMachineLogic;

namespace ShipLogic.Stealth.States
{
    public class StealthAttackState : StateBase
    {
        private readonly IShipCommander _commander;
        private readonly ShipBase _ship;

        private ITargetToAttack _currentTarget;

        public StealthAttackState(StateMachine machine, IShipCommander commander, ShipBase ship) : base(machine)
        {
            _commander = commander;
            _ship = ship;
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();

            // todo нужно будет подумать над этим. 
            // думаю переходить обратно в режим Idle, если корабль противника вышел за радиус поиска
            // или был уничтожен
            // if (!_ship.SeeOtherShip(_commander.Enemy))
            // {
            //     Machine.ChangeState(_commander.Idle);
            //     return;
            // }

            if (_commander != null && _currentTarget != _commander.Enemy)
            {
                _currentTarget = _commander.Enemy;
                _ship.GetGun().SetTarget(_currentTarget);
            }

            _ship.GetGun().Shoot();
        }
    }
}