using System;
using ShipLogic.Stealth.States;
using SpaceObjects;
using StateMachineLogic;
using UnityEngine;

namespace ShipLogic.Stealth
{
    public class StealthCommander : MonoBehaviour, IShipCommander, IDisposable
    {
        public StateBase Idle { get; private set; }
        public StateBase Movement { get; private set; }
        public StateBase Attack { get; private set; }
        public StateBase PrepareAttack { get; private set; }
        
        public Vector3? PointForMovement { get; private set; }
        public ITargetToAttack Enemy { get; private set; }

        [SerializeField]
        private ShipStealth _ship;

        [SerializeField, Header("Для теста")] private Transform _pointForMovement;
        
        
        private StateMachine _machine;


        public void SendDetectedObject(IDetectedObject detectedObject)
        {
            if (detectedObject.ObjectType == DetectedObjectType.Ship && detectedObject is ShipBase ship)
            {
                if (ship.PlayerType == _ship.PlayerType)
                {
                    return;
                }

                Enemy = ship;
            }
        }

        private void MoveToPoint(Vector3 point)
        {
            PointForMovement = point;
        }
        
        private void Start()
        {
            Init();
        }

        private void Init()
        {
            InitStateMachine();
            _ship.Init(this, Main.Instance.Map);

            Main.Instance.Map.AddObjectOnMap(_ship);
            Main.Instance.Map.OnMovementObjects += CustomUpdate;
        }

        private void InitStateMachine()
        {
            _machine = new StateMachine();
            Idle = new StealIdleState(_machine, this, _ship);
            Movement = new StealthMovementState(_machine,this, _ship);
            Attack = new StealthAttackState(_machine, this, _ship);
            PrepareAttack = new StealPreparingForAttackState(_machine, this, _ship);

            _machine.Init(Idle);
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            Main.Instance.Map.OnMovementObjects -= CustomUpdate;
        }

        private void CustomUpdate()
        {
            MoveToPoint(_pointForMovement.position);
            _machine.CurrentState.UpdateLogic();
        }
    }
}