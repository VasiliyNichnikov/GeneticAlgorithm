using Planets;
using ShipLogic;
using ShipLogic.MainStates;
using ShipLogic.Strategy.Attack;
using StateMachineLogic;

namespace DebugShip
{
    public class DebugFighterCommander : CommanderBase
    {
        public override StateBase Idle { get; protected set; }

        public override StateBase Attack { get; protected set; }

        public override StateBase Movement { get; protected set; }

        public override StateBase PrepareAttack { get; protected set; }


        public DebugFighterCommander(ShipBase ship) : base(ship, new FighterAttackLogic(ship))
        {
        }

        protected override void InitStateMachineAndStates()
        {
            Idle = new IdleState(Machine, this);
            Movement = new MovementState(Machine, this);
            PrepareAttack = new PreparingForAttackState(Machine, this);
            Attack = new AttackState(Machine, this);
            EscapeFromBattle = new EscapeFromBattleStateState(Machine, this);

            Machine.Init(Idle);
        }

        public override void SetPointForMovement(ITarget target)
        {
            Route.AddPointForMovement(target.GetPointToApproximate());
        }
    }
}