using System;
using System.Collections.Generic;
using ShipLogic.Editor;
using ShipLogic.Individual.States;
using SpaceObjects;
using StateMachineLogic;
using UnityEngine;
using Utils.Loader;

namespace ShipLogic.Individual
{
    public class IndividualCommander : MonoBehaviour, IShipCommander, IDisposable
    {
        public string NameCurrentState => _machine.CurrentState != null ? _machine.CurrentState.NameState : string.Empty;
        public StateBase Idle { get; private set; }
        public StateBase Attack { get; private set; }
        public StateBase Movement { get; private set; }
        public StateBase PrepareAttack { get; private set; }
        public StateBase Dead { get; private set; }
        public bool HasEnemy => _selectedEnemy != null;
        public bool HasPointForMovement => _route.IsMovement;
        public bool IsNeedStop { get; private set; }

        [SerializeField] private ShipIndividual _ship;

        [SerializeField] private Transform _pointForMovementTest;
        
        [SerializeField, Header("Характеристики корабля для тестов")]
        private ShipCharacteristics _characteristics;

        private readonly List<ShipBase> _foundEnemies = new List<ShipBase>();

        private StateMachine _machine;
        private ShipBase _selectedEnemy;
        private ShipRoute _route;
        
        private void Start()
        {
            Init();
        }
        
        // todo в будущем сюда будем передавать основные данные корабля
        private void Init()
        {
            _route = new ShipRoute();
            _ship.Init(this, _characteristics.ConvertToShipData());
            InitStateMachineAndStates();

            Main.Instance.OnUpdateGame += CustomUpdate;

            if (_pointForMovementTest != null)
            {
                _route.AddPointForMovement(_pointForMovementTest.position);
                _route.ChangeAndGetPointForMovement();
            }
        }

        public void AddFoundEnemy(IDetectedObject detectedObject)
        {
            var isEnemy = TryGetEnemy(detectedObject, out var enemy);
            if (!isEnemy)
            {
                return;
            }
            
            if (_foundEnemies.Contains(enemy))
            {
                Debug.LogWarning("Current detected object is contains in list");
                return;
            }

            _foundEnemies.Add(enemy);
        }

        public void RemoveFoundEnemy(IDetectedObject detectedObject)
        {
            var isEnemy = TryGetEnemy(detectedObject, out var enemy);
            if (!isEnemy)
            {
                return;
            }
            
            if (!_foundEnemies.Contains(enemy))
            {
                Debug.LogWarning("Current detected object is not contains in list");
                return;
            }

            if (_selectedEnemy == enemy)
            {
                LosingSelectedEnemy();
            }
            
            _foundEnemies.Remove(enemy);
        }


        private void CheckEnemiesForOpportunityToAttack()
        {
            if (_selectedEnemy != null && _selectedEnemy.IsDead)
            {
                _foundEnemies.Remove(_selectedEnemy);
                LosingSelectedEnemy();
            }

            if (_selectedEnemy != null)
            {
                return;
            }

            if (_foundEnemies.Count == 0)
            {
                return;
            }
            
            _foundEnemies.Sort(SortFoundEnemies);

            if (_selectedEnemy != null && _foundEnemies[0] != _selectedEnemy)
            {
                LosingSelectedEnemy();
            }
            
            _selectedEnemy = _foundEnemies[0];
            _route.SetHighPriorityPoint(_selectedEnemy.transform.position);
        }

        private int SortFoundEnemies(ShipBase a, ShipBase b)
        {
            var weight1 = Vector3.Distance(_ship.Position, a.Position);
            var weight2 = Vector3.Distance(_ship.Position, b.Position);

            weight1 += Vector3.Angle(_ship.transform.forward, a.Position);
            weight2 += Vector3.Angle(_ship.transform.forward, b.Position);

            return (int)(weight1 - weight2);
        }
        
        public bool SeeOtherEnemyShip()
        {
            if (!HasEnemy)
            {
                Debug.LogError("Enemy is null");
                return false;
            }
            
            return _ship.SeeOtherShip(_selectedEnemy);
        }

        public bool CanAttackOtherEnemyShip()
        {
            if (!HasEnemy)
            {
                Debug.LogError("Enemy is null");
                return false;
            }
            
            return _ship.CanAttackOtherShip(_selectedEnemy);
        }

        public void TurnOnEngine()
        {
            _ship.TurnOnEngine();
        }

        public void TurnOffEngine()
        {
            _ship.TurnOffEngine();
        }

        public void DestroyShip()
        {
            _ship.DestroyShip();
        }

        public void MoveToSelectedPoint()
        {
            _ship.Engine.SetTarget(_route.GetPointForMovement());
            _ship.Engine.Move();
        }

        public void TurnToEnemyShip()
        {
            if (!HasEnemy)
            {
                Debug.LogError("Enemy is null");
                return;
            }

            _ship.Engine.TurnToTarget(_selectedEnemy.Position);
        }

        public void StartShoot()
        {
            _ship.Gun.StartShoot();
        }

        public void ShootInEnemy()
        {
            if (!HasEnemy)
            {
                Debug.LogError("Enemy is null");
                return;
            }
            
            _ship.Gun.SetTarget(_selectedEnemy);
            _ship.Gun.Shoot();
        }

        public void FinishShoot()
        {
            _ship.Gun.FinishShoot();
        }

        private void InitStateMachineAndStates()
        {
            _machine = new StateMachine();
            Idle = new IdleState(_machine, this);
            Movement = new MovementState(_machine, this);
            PrepareAttack = new PreparingForAttackState(_machine, this);
            Attack = new AttackState(_machine, this);
            Dead = new DeadState(_machine, this);

            _machine.Init(Idle);
        }

        private void OnDestroy()
        {
            Dispose();
        }


        public void Dispose()
        {
            Main.Instance.OnUpdateGame -= CustomUpdate;
        }

        private void CustomUpdate()
        {
            CheckPointForMovement();
            CheckEnemiesForOpportunityToAttack();

            if (_ship.IsDead)
            {
                _machine.ChangeState(Dead);
            }
            
            _machine.CurrentState.UpdateLogic();
        }

        private bool TryGetEnemy(IDetectedObject detectedObject, out ShipBase ship)
        {
            ship = null;
            if (detectedObject.ObjectType != DetectedObjectType.Ship)
            {
                return false;
            }
            
            if (detectedObject is not ShipBase shipEnemy)
            {
                return false;
            }
            
            // Найден союзник
            if (IsAlly(shipEnemy))
            {
                return false;
            }
            
            // Корабль мертвый
            if (shipEnemy.IsDead)
            {
                return false;
            }

            ship = shipEnemy;
            return true;
        }
        
        private bool IsAlly(ShipBase ship)
        {
            return ship.PlayerType == _ship.PlayerType;
        }

        private void CheckPointForMovement()
        {
            if (!HasPointForMovement)
            {
                IsNeedStop = false;
                return;
            }
            IsNeedStop = Vector3.Distance(_route.GetPointForMovement(), _ship.transform.position) <= _ship.ShipRadius;
            if (IsNeedStop)
            {
                _route.ChangeAndGetPointForMovement();
            }
        }

        private void LosingSelectedEnemy()
        {
            if (_selectedEnemy == null)
            {
                Debug.LogError("Enemy is already null");
                return;
            }
            _machine.ChangeState(Idle);
            _selectedEnemy = null;
            _route.SetHighPriorityPoint(null);
        }
    }
}