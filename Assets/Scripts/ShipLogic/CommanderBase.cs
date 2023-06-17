using CommandsShip;
using Map;
using Planets;
using ShipLogic.Strategy.Attack;
using StateMachineLogic;
using UnityEngine;

namespace ShipLogic
{
    public abstract class CommanderBase : ICommander
    {
        public float HealthPercentages => Ship.Health.CurrentHealthPercentages;
        public bool IsInBattle => HasEnemy;
        public ITarget ShipTarget => Ship;
        public Vector3 PositionShip => Ship.ObjectPosition;

        public abstract StateBase Idle { get; protected set; }
        public abstract StateBase Attack { get; protected set; }
        public abstract StateBase Movement { get; protected set; }
        public abstract StateBase PrepareAttack { get; protected set; }
        public StateBase EscapeFromBattle { get; protected set; }
        public bool HasEnemy => _shipAttackLogic.SelectedEnemy != null;
        public bool HasPointForMovement => Route.IsMovement;
        public bool IsNeedStop { get; private set; }
        public string NameCurrentState => Machine.CurrentState != null ? Machine.CurrentState.NameState : string.Empty;
        public ShipType ShipType => Ship.Type;

        protected readonly ShipRoute Route;
        protected readonly ShipBase Ship;
        protected readonly StateMachine Machine;

        private readonly TeamManager _teamManager;
        private readonly WeightCalculator _weightCalculator;
        private IShipAttackLogic _shipAttackLogic;

        protected CommanderBase(ShipBase ship, IShipAttackLogic attackLogic)
        {
            Ship = ship;
            Route = new ShipRoute(ship);
            Machine = new StateMachine();
            _shipAttackLogic = attackLogic;
            
            _teamManager = new TeamManager(this);
            _weightCalculator = new WeightCalculator(ship.GetDetector());

            InitStateMachineAndStates();
            SpaceMap.Map.OnMoveNonStaticObjects += CustomUpdate;
        }

        protected abstract void InitStateMachineAndStates();

        public abstract void SetPointForMovement(ITarget target);

        public bool NeedEscapeFromBattle()
        {
            return _teamManager.IsEscapeFromBattle;
        }


        private void CustomUpdate()
        {
            if (Ship.IsDead)
            {
                Ship.Hide();
                SpaceMap.Map.OnMoveNonStaticObjects -= CustomUpdate;
                return;
            }
            
            CheckPointForMovement();
            _shipAttackLogic.CheckEnemiesForOpportunityToAttack();

            Machine.CurrentState.UpdateLogic();
        }

        private void CheckPointForMovement()
        {
            if (!HasPointForMovement)
            {
                IsNeedStop = false;
                return;
            }

            IsNeedStop = Ship.Engine.IsStopped;
            if (IsNeedStop)
            {
                Route.ChangePointForMovement();
            }
        }


        public bool SeeOtherEnemyShip()
        {
            if (!HasEnemy)
            {
                Debug.LogError("Enemy is null");
                return false;
            }

            return Ship.SeeOtherShipDistance(_shipAttackLogic.SelectedEnemy);
        }

        public bool CanAttackOtherEnemyShip()
        {
            if (!HasEnemy)
            {
                Debug.LogError("Enemy is null");
                return false;
            }

            return Ship.CanAttackOtherShip(_shipAttackLogic.SelectedEnemy);
        }

        
        public bool IsDistanceToAttack()
        {
            if (!HasEnemy)
            {
                Debug.LogError("Enemy is null");
                return false;
            }

            return Ship.IsDistanceToAttack(_shipAttackLogic.SelectedEnemy);
        }
        
        
        public void TurnOnEngine()
        {
            Ship.TurnOnEngine();
        }

        public void TurnOffEngine()
        {
            Ship.TurnOffEngine();
        }

        public float CalculateWeight()
        {
            return _weightCalculator.GetWeight();
        }

        public void ExecuteCommand(Command command)
        {
            _teamManager.GiveCommand(command);
        }

        public void MoveToSelectedPoint()
        {
            Ship.Engine.SetTarget(Route.GetPointForMovement());
            Ship.Engine.Move();
        }

        public void TurnToEnemyShip()
        {
            if (!HasEnemy)
            {
                Debug.LogError("Enemy is null");
                return;
            }

            Ship.Engine.TurnToTarget(_shipAttackLogic.SelectedEnemy.ObjectPosition);
        }

        public void StartShoot()
        {
            Ship.Gun.StartShoot();
        }

        public void ShootInEnemy()
        {
            if (!HasEnemy)
            {
                Debug.LogError("Enemy is null");
                return;
            }

            Ship.Gun.SetTarget(_shipAttackLogic.SelectedEnemy);
            Ship.Gun.Shoot();
        }

        public void FinishShoot()
        {
            Ship.Gun.FinishShoot();
        }

        public void ChangeStateToIdle()
        {
            Machine.ChangeState(Idle);
        }

        public virtual void SetPointForMovementToEnemy(ITarget target)
        {
            Route.AddPointForMovementToEnemy(target.GetPointToApproximate());
        }

        public void SetPointForEscapeFromBattle(ITarget target)
        {
            Route.AddPointForEscapeFromBattle(target.GetPointToApproximate());
        }

#if UNITY_EDITOR

        public Vector3 GetPointForMovement()
        {
            return Route.GetPointForMovement();
        }
#endif

        public virtual void Dispose()
        {
            if (!Ship.IsDead)
            {
                SpaceMap.Map.OnMoveNonStaticObjects -= CustomUpdate;
            }
            
            // Main.Instance.ShipFactory.OnDestroyShip -= RemoveFoundEnemy;
            _shipAttackLogic.Dispose();
            _shipAttackLogic = null;
        }
    }
}