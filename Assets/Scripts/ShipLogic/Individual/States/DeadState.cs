using StateMachineLogic;

namespace ShipLogic.Individual.States
{
    public class DeadState : StateBase
    {
        public override string NameState => "Dead";

        private readonly IShipCommander _commander;

        public DeadState(StateMachine machine, IShipCommander commander) : base(machine)
        {
            _commander = commander;
        }

        public override void Enter()
        {
            base.Enter();
            _commander.FinishShoot();
            _commander.TurnOffEngine();
            _commander.DestroyShip();
        }
    }
}