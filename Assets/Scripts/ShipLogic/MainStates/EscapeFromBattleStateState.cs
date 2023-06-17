using StateMachineLogic;

namespace ShipLogic.MainStates
{
    public class EscapeFromBattleStateState : StateBase
    {
        public override string NameState => "Escape from battle";

        private readonly ICommanderCommander _commanderCommander;
        
        public EscapeFromBattleStateState(StateMachine machine, ICommanderCommander commanderCommander) : base(machine)
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