using StateMachineLogic;

namespace ShipLogic.Mining.States
{
    public class MovementState : StateBase
    {
        private readonly ICommanderCommander _commanderCommander;
        public override string NameState => "Movement";

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

            if (!_commanderCommander.HasPointForMovement || _commanderCommander.IsNeedStop)
            {
                Machine.ChangeState(_commanderCommander.Idle);
                return;
            }
            
            _commanderCommander.MoveToSelectedPoint();
        }
    }
}