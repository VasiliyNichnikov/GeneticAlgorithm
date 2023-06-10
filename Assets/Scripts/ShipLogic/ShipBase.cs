using System;
using System.Collections.Generic;
using HandlerClicks;
using Map;
using Planets;
using Players;
using SpaceObjects;
using UnityEngine;
using UnityEngine.AI;

namespace ShipLogic
{
    public abstract class ShipBase : MonoBehaviour, IObjectToClick, IDetectedObject, IDisposable, IObjectOnMap, ITarget
    {
        public MapObjectType TypeObject => MapObjectType.Ship;
        public bool IsStatic { get; private set; }
        public Vector3 LeftBottomPosition => transform.position;
        public Vector3 RightTopPosition => transform.position;

        public DetectedObjectType ObjectType => DetectedObjectType.Ship;
        public Vector3 ObjectPosition => transform.position;
        protected float SpeedMovement { get; private set; }
        public PlayerType PlayerType { get; private set; }
        public bool IsDead => Health.IsDead;

        public abstract float ThreatLevel { get; }
        public IShipHealth Health { get; private set; }
        public IShipEngine Engine { get; private set; }
        public IShipGun Gun { get; private set; }
        public float Radius => Agent.radius;
        public ShipData CalculatedShipData => _calculatedShipData;
        public abstract ShipType Type { get; }
        public string NameCurrentState => _commander == null ? "Not commander" : _commander.NameCurrentState;

        protected abstract float MinAngleRotation { get; }
        [SerializeField] private MeshRenderer Renderer;
        [SerializeField] protected ShipDetector Detector;
        [SerializeField] protected NavMeshAgent Agent;
        [SerializeField] protected GunPoint[] GunPoints;
        [SerializeField] protected ShipEffectsManager EffectsManager;

        private IShipCommander _commander;
        private bool _isInitialized;
        private ShipData _calculatedShipData;
        private ShipClickHandler _clickHandler;
        private Action _addShipInCache;

        /// <summary>
        /// Процент поглащения урона броней
        /// </summary>
        private const float PercentageOfArmorAbsorption = 45f;

        public void Init(IBuilderShip builder)
        {
            if (_isInitialized)
            {
                Debug.LogError("Ship is already initialized");
                return;
            }

            InitSkin(builder.Data);
            _commander = GetNewCommander();

            _isInitialized = true;
            PlayerType = builder.PlayerType;
            Detector.Init(builder.PlayerType, this);

            EffectsManager.Init();
            // Инициализация цвета
            Renderer.material = Main.Instance.MaterialStorage.GetMaterialForShip(PlayerType);
            
            // Добавление корабля на карту
            SpaceMap.Map.AddObjectOnMap(this);
        }

        public void InitCache(Action addShipInCache)
        {
            _addShipInCache = addShipInCache;
        }


        protected abstract IShipCommander GetNewCommander();

        // Эта штука в совокупностью CanAttackOtherShip создают баги
        public bool SeeOtherShipDistance(IDetectedObject ship)
        {
            return Vector3.Distance(ObjectPosition, ship.ObjectPosition) <= Detector.Radius;
        }

        public bool SeeSelectedPointAngle(Vector3 positionPoint)
        {
            var direction = positionPoint - ObjectPosition;
            direction.y = 0.0f;
            var angleRotation = Vector3.Angle(direction, transform.forward);
            return angleRotation <= MinAngleRotation;
        }
        
        public bool IsDistanceToAttack(IDetectedObject ship)
        {
            var distanceByShip = Vector3.Distance(ObjectPosition, ship.ObjectPosition);
            return distanceByShip > Radius && distanceByShip <= Detector.Radius;
        }

        public Vector3 GetPointToApproximate()
        {
            return ObjectPosition;
        }
        
        public abstract bool CanAttackOtherShip(IDetectedObject ship);


        public abstract IReadOnlyCollection<Vector3> GetPointsInSector();

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public IShipCommander GetCommander()
        {
            if (_commander == null)
            {
                Debug.LogError("Commander is null");
                return null;
            }

            return _commander;
        }

        public void Hide()
        {
            _isInitialized = false;
            _addShipInCache?.Invoke();
            TurnOffEngine();
            Gun.Dispose();
            // При уничтожение корабля теряем командира
            _commander.Dispose();
            _commander = null;

            gameObject.SetActive(false);
        }

        public void TurnOffEngine()
        {
            if (!_isInitialized)
            {
                return;
            }

            IsStatic = true;
            Agent.enabled = false;
            EffectsManager.TurnOffEngine();
        }

        public void TurnOnEngine()
        {
            if (!_isInitialized)
            {
                return;
            }

            IsStatic = false;
            Agent.enabled = true;
            EffectsManager.TurnOnEngine();
        }

        public void DestroyShip()
        {
            if (!_isInitialized)
            {
                return;
            }

            EffectsManager.DestroyShip();
        }

        public void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
        }

        private void InitSkin(ShipData data)
        {
            _calculatedShipData = data;
            Detector.SetRadius(data.VisibilityRadius);
            SpeedMovement = data.SpeedMovement;

            // Инициализация обработчика нажатий
            _clickHandler = new ShipClickHandler(this, _calculatedShipData);

            // Инициализация здоровья
            var minMaxHealth = Main.Instance.ShipParameters.GetParameterByType(MainShipParameters.ParameterType.Health);
            Health = new ShipHealth(minMaxHealth.minValue, minMaxHealth.maxValue, 0, data.Armor,
                PercentageOfArmorAbsorption);
            // Инициализация двигателя
            Engine = new ShipEngine(transform, Agent, _calculatedShipData.SpeedMovement,
                _calculatedShipData.SpeedMovement);
            // Инициализация оружия
            Gun = new ShipGun(this, GunPoints, _calculatedShipData.RateOfFire, _calculatedShipData.GunPower);
        }

        public void Clicked(Vector3 position)
        {
            _clickHandler.Clicked(position);
        }

        public void OnStartDrag()
        {
            // nothing
        }

        public void OnEndDrag()
        {
            // nothing
        }


#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            if (!_isInitialized)
            {
                return;
            }
            
            if (_commander == null)
            {
                return;
            }
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_commander.GetPointForMovement(), 1.5f);

            Gizmos.color = new Color(1, 0, 0, 0.6f);
            foreach (var enemy in _commander.GetFoundEnemies())
            {
                Gizmos.DrawSphere(enemy.ObjectPosition, enemy.Radius);
            }
            
            Gizmos.color = new Color(0, 1, 0, 0.6f);
            foreach (var ally in _commander.GetFoundAllies())
            {
                Gizmos.DrawSphere(ally.ObjectPosition, ally.Radius);
            }
            
            Gizmos.color = Color.blue;
            if (Engine?.PathDebug == null || Engine.PathDebug.Length == 0)
            {
                return;
            }

            var previewPoint = Engine.PathDebug[0];
            for (var i = 0; i < Engine.PathDebug.Length - 1; i++)
            {
                Gizmos.DrawLine(previewPoint, Engine.PathDebug[i + 1]);
                previewPoint = Engine.PathDebug[i + 1];
            }
        }
#endif
    }
}