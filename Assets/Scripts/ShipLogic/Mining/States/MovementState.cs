using StateMachineLogic;

namespace ShipLogic.Mining.States
{
    public class MovementState : StateBase
    {
        private readonly IShipCommander _commander;
        public override string NameState => "Movement";

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

            if (!_commander.HasPointForMovement || _commander.IsNeedStop)
            {
                Machine.ChangeState(_commander.Idle);
                return;
            }
            
            _commander.MoveToSelectedPoint();
        }
    }
}