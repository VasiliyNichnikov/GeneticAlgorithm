using StateMachineLogic;

namespace ShipLogic.MainStates
{
    public class MovementState : StateBase
    {
        public override string NameState => "MovementState";

        private readonly ICommanderCommander _commanderCommander;
        
        public MovementState(StateMachine machine, ICommanderCommander commanderCommander) : base(machine)
        {
            _commanderCommander = commanderCommander;
        }

        public override void Enter()
        {
            base.Enter();
            _commanderCommander.TurnOnEngine();
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();

            if (_commanderCommander.NeedEscapeFromBattle())
            {
                Machine.ChangeState(_commanderCommander.EscapeFromBattle);
                return;
            }

            if (_commanderCommander.HasEnemy && _commanderCommander.SeeOtherEnemyShip())
            {
                Machine.ChangeState(_commanderCommander.PrepareAttack);
                return;
            }

            if (!_commanderCommander.HasPointForMovement || _commanderCommander.IsNeedStop)
            {
                Machine.ChangeState(_commanderCommander.Idle);
                return;
            }
            
            _commanderCommander.MoveToSelectedPoint();
        }
    }
}