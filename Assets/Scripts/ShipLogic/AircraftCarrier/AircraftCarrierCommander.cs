using System;
using Planets;
using ShipLogic.AircraftCarrier.States;
using ShipLogic.MainStates;
using ShipLogic.Strategy.Attack;
using StateMachineLogic;
using UnityEngine;
using Utils;
using MovementStateAircraft = ShipLogic.AircraftCarrier.States.MovementState;
using PreparingForAttackState = ShipLogic.AircraftCarrier.States.PreparingForAttackState;

namespace ShipLogic.AircraftCarrier
{
    public class AircraftCarrierCommander : CommanderBase
    {
        public override StateBase Idle { get; protected set; }
        public override StateBase Attack { get; protected set; }
        public override StateBase Movement { get; protected set; }
        public override StateBase PrepareAttack { get; protected set; }
        
        /// <summary>
        /// Нужен ли скачек к точке к которой мы двигаемся
        /// </summary>
        public bool NeedJumpInPointForMovement { get; private set; }
        
        public StateBase JumpOnBattlefield { get; private set; }
        public StateBase PreparingForJump { get; private set; }

        /// <summary>
        /// Таймер перед прыжком
        /// </summary>
        private Timer _jumpTimer;
        
        /// <summary>
        /// Таймер для перезарядки прыжка
        /// </summary>
        private Timer _jumpRechargeTimer;

        public AircraftCarrierCommander(ShipBase ship) : base(ship, new AircraftCarrierAttackLogic(ship))
        {
        }

        protected override void InitStateMachineAndStates()
        {
            Idle = new IdleState(Machine, this);
            Movement = new MovementStateAircraft(Machine, this);
            PrepareAttack = new PreparingForAttackState(Machine, this);
            Attack = new AttackState(Machine, this);
            JumpOnBattlefield = new JumpOnBattlefieldState(Machine, this);
            PreparingForJump = new PreparingForJumpState(Machine, this);
            EscapeFromBattle = new EscapeFromBattleStateState(Machine, this);

            Machine.Init(Idle);
        }

        public override void SetPointForMovement(ITarget target)
        {
            Route.AddPointForMovement(target.GetPointToApproximate());
            // todo нужно будет учитывать перезарядился или нет скачок
            // todo вынести константу в отдельную переменную
            // todo это нужно делать долько для оказания помощи
            NeedJumpInPointForMovement = target.ThreatLevel >= 0.7f;
        }

        public void TurnToPointOfMovement()
        {
            Ship.Engine.TurnToTarget(Route.GetPointForMovement());
        }

        public bool CompletedTurnToPointOfMovement()
        {
            return Ship.SeeSelectedPointAngle(Route.GetPointForMovement());
        }

        public bool CanJumpToPointOfMovement()
        {
            return _jumpRechargeTimer == null && _jumpTimer == null && Ship.Engine.HasPath;
        }

        public void StartJumpTimer(Action onTimerEnd)
        {
            if (_jumpTimer != null || _jumpRechargeTimer != null)
            {
                Debug.LogError("The jump timer is already running or jump recharge timer is already running");
                return;
            }

            // todo время нужно будет загружать из лоудера
            _jumpTimer = Timer.StartTimer(5, onTimerEnd);
        }

        public void JumpToPointOfMovement()
        {
            if (_jumpTimer == null || _jumpTimer.IsRunning)
            {
                Debug.LogError("The ship is not ready to jump");
                return;
            }

            _jumpTimer = null;
            // todo время нужно будет загружать из лоудера
            _jumpRechargeTimer = Timer.StartTimer(10, () => _jumpRechargeTimer = null);
            Route.ClearAllPoints();
            Ship.Engine.TryJumpToLastPoint();
        }
    }
}