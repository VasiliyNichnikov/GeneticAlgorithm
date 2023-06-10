using StateMachineLogic;

namespace ShipLogic.MainStates
{
    public class EscapeFromBattleStateState : StateBase
    {
        public override string NameState => "Escape from battle";

        private readonly IShipCommander _commander;
        
        public EscapeFromBattleStateState(StateMachine machine, IShipCommander commander) : base(machine)
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
            
            if (!_commander.HasPointForMovement || _commander.IsNeedStop)
            {
                Machine.ChangeState(_commander.Idle);
                return;
            }
            
            _commander.MoveToSelectedPoint();
        }
    }
}