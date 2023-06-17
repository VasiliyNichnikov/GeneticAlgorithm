using Planets;
using ShipLogic.Mining.States;
using ShipLogic.Strategy.Attack;
using StateMachineLogic;

namespace ShipLogic.Mining
{
    public class MiningCommander : CommanderBase
    {
        public override StateBase Idle { get; protected set; }
        public override StateBase Attack { get; protected set; }
        public override StateBase Movement { get; protected set; }
        public override StateBase PrepareAttack { get; protected set; }

        public MiningCommander(ShipBase ship) : base(ship, new MiningAttackLogic())
        {
        }

        protected override void InitStateMachineAndStates()
        {
            Idle = new IdleState(Machine, this);
            Movement = new MovementState(Machine, this);
            PrepareAttack = null;
            Attack = null;

            Machine.Init(Idle);
        }

        public override void SetPointForMovement(ITarget target)
        {
            Route.AddPointForMovement(target.GetPointToApproximate());
        }
    }
}