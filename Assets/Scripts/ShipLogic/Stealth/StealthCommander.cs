using Map;
using Planets;
using ShipLogic.MainStates;
using ShipLogic.Strategy.Attack;
using StateMachineLogic;
using MovementState = ShipLogic.MainStates.MovementState;

namespace ShipLogic.Stealth
{
    public class StealthCommander : CommanderBase
    {
        public override StateBase Idle { get; protected set; }
        public override StateBase Attack { get; protected set; }
        public override StateBase Movement { get; protected set; }
        public override StateBase PrepareAttack { get; protected set; }

        private ITarget _target;

        public StealthCommander(ShipBase ship) : base(ship, new StealthAttackLogic(ship))
        {
            Route.OnEndPointsForMovement += UpdatePoints;
        }

        protected override void InitStateMachineAndStates()
        {
            Idle = new IdleState(Machine, this);
            Movement = new MovementState(Machine, this);
            // todo переделать состояния ниже
            PrepareAttack = new PreparingForAttackState(Machine, this);
            Attack = new AttackState(Machine, this);
            EscapeFromBattle = new EscapeFromBattleStateState(Machine, this);

            Machine.Init(Idle);
        }

        public override void SetPointForMovement(ITarget target)
        {
            _target = target;
            UpdatePoints();
        }
        
        private void UpdatePoints()
        {
            var target = SpaceMap.Map.GetSectorForSelectedShip(Ship.PlayerType, this);
            _target = target.ThreatLevel > _target.ThreatLevel ? target : SpaceMap.Map.GetSectorWithHighWeightForShip(Ship.PlayerType);
            
            var points = _target.GetPointsInSector();
            Route.AddPointForMovementRange(points);
        }

        public override void Dispose()
        {
            base.Dispose();
            Route.OnEndPointsForMovement -= UpdatePoints;
        }
    }
}