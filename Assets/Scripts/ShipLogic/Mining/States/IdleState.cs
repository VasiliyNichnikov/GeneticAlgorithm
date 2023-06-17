using StateMachineLogic;

namespace ShipLogic.Mining.States
{
    public class IdleState : StateBase
    {
        public override string NameState => "Idle";
        private readonly ICommanderCommander _commanderCommander;

        public IdleState(StateMachine machine, ICommanderCommander commanderCommander) : base(machine)
        {
            _commanderCommander = commanderCommander;
        }

        public override void Enter()
        {
            base.Enter();
            _commanderCommander.TurnOffEngine();
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();

            if (_commanderCommander.HasPointForMovement)
            {
                Machine.ChangeState(_commanderCommander.Movement);
            }
        }
    }
}