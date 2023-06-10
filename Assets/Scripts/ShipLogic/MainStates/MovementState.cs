using StateMachineLogic;

namespace ShipLogic.MainStates
{
    public class MovementState : StateBase
    {
        public override string NameState => "MovementState";

        private readonly IShipCommander _commander;
        
        public MovementState(StateMachine machine, IShipCommander commander) : base(machine)
        {
            _commander = commander;
        }

        public override void Enter()
        {
            base.Enter();
            _commander.TurnOnEngine();
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();

            if (_commander.NeedEscapeFromBattle())
            {
                Machine.ChangeState(_commander.EscapeFromBattle);
                return;
            }

            if (_commander.HasEnemy && _commander.SeeOtherEnemyShip())
            {
                Machine.ChangeState(_commander.PrepareAttack);
                return;
            }

            if (!_commander.HasPointForMovement || _commander.IsNeedStop)
            {
                Machine.ChangeState(_commander.Idle);
                return;
            }
            
            _commander.MoveToSelectedPoint();
        }
    }
}