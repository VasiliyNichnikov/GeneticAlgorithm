using Group;
using Planets;
using ShipLogic.MainStates;
using ShipLogic.Strategy.Attack;
using StateMachineLogic;
using UnityEngine;
using MovementStateDefault = ShipLogic.MainStates.MovementState;
using PreparingForAttackFighterState = ShipLogic.MainStates.PreparingForAttackState;

namespace ShipLogic.Fighter
{
    // ISupportedGroup - должен быть на корабле конкретного типа, а не у коммандер
    // Очень не нравится что именно командер в группе, хотя может это более логично
    // так как корабль это просто машина, а вот уже благодаря командирам эта машина получает команды и логику поведения
    public class FighterCommander : CommanderBase, ISupportedGroup
    {
        public Vector3 ObjectPosition => PositionShip;
        
        public override StateBase Idle { get; protected set; }
        public override StateBase Attack { get; protected set; }
        public override StateBase Movement { get; protected set; }
        public override StateBase PrepareAttack { get; protected set; }

        private IShipGroup _group;


        public FighterCommander(ShipBase ship) : base(ship, new FighterAttackLogic(ship))
        {
        }

        public void InitGroup(IShipGroup group)
        {
            if (_group != null)
            {
                Debug.LogError("Group is already initialized");
                return;
            }
            
            _group = group;
            _group.OnUpdatePositionsInGroup += UpdatePointForMovementGroup;
        }
        
        protected override void InitStateMachineAndStates()
        {
            Idle = new IdleState(Machine, this);
            Movement = new MovementStateDefault(Machine, this);
            PrepareAttack = new PreparingForAttackFighterState(Machine, this);
            Attack = new AttackState(Machine, this);
            EscapeFromBattle = new EscapeFromBattleStateState(Machine, this);

            Machine.Init(Idle);
        }
        
        public override void SetPointForMovement(ITarget target)
        {
            if (_group != null)
            {
                _group.UpdatePointForMovement(target);
                return;
            }
            
            Route.AddPointForMovement(target.GetPointToApproximate());
        }

        public override void SetPointForMovementToEnemy(ITarget target)
        {
            if (_group != null)
            {
                _group.UpdatePointForMovement(target);
            }
            
            base.SetPointForMovementToEnemy(target);
        }

        private void UpdatePointForMovementGroup(Vector3[] positions)
        {
            if (Ship.IsDead)
            {
                return;
            }
            
            var index = _group.GetIndexShip(this);
            if (index < 0 || index >= positions.Length)
            {
                Debug.LogError($"Invalid ship index ({index})");
                return;
            }
            
            Route.AddPointForMovement(positions[index]);
        }

        public override void Dispose()
        {
            base.Dispose();
            _group.OnUpdatePositionsInGroup -= UpdatePointForMovementGroup;
        }
    }
}