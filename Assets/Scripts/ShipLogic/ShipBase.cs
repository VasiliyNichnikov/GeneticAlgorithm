using System;
using System.Linq;
using FindingPath;
using HandlerClicks;
using Players;
using SpaceObjects;
using UnityEngine;

namespace ShipLogic
{
    public abstract class ShipBase : MonoBehaviour, IObjectToClick, ITargetToAttack, IDisposable, IObjectOnMap
    {
        public MapObjectType TypeObject => MapObjectType.Ship;
        public bool IsStatic { get; private set; }

        public Vector3 WorldPosition => transform.position;
        public Vector3 LeftBottomPosition => transform.position;
        public Vector3 RightTopPosition => transform.position;
        
        public DetectedObjectType ObjectType => DetectedObjectType.Ship;
        public Vector3 Position => transform.position;
        public float ShipRadius => _shipData.RadiusShip;
        protected float SpeedMovement { get; private set; }
        public PlayerType PlayerType { get; private set; }
        public bool IsDead => Health.IsDead;

        public IShipHealth Health { get; private set; }
        public IShipEngine Engine { get; private set; }
        public abstract IShipGun Gun { get; protected set; }

        [SerializeField] protected ShipDetector Detector;

        [SerializeField] private ShipSkinData[] _skins;

        public ShipData CalculatedShipData => _calculatedShipData;

        public string NameCurrentState => _commander == null ? "Not commander" : _commander.NameCurrentState;

        private ShipSkinData _shipData;
        private IShipCommander _commander;
        private bool _isInitialized;
        private ShipData _calculatedShipData;
        private ShipClickHandler _clickHandler;
        private Action _addShipInCache;

        /// <summary>
        /// Процент поглащения урона броней
        /// </summary>
        private const float PercentageOfArmorAbsorption = 45f;


        private void Awake()
        {
            foreach (var skin in _skins)
            {
                if (skin == null)
                {
                    continue;
                }

                skin.gameObject.SetActive(false);
            }
        }

        public void Init(IBuilderShip builder)
        {
            if (_isInitialized)
            {
                Debug.LogError("Ship is already initialized");
                return;
            }
            
            InitSkin(builder.Data);
            _commander = builder.Commander;

            _isInitialized = true;
            PlayerType = builder.PlayerType;
            Detector.Init(builder.PlayerType, _commander);
            _shipData.EffectsManager.Init();
            
            // Инициализация цвета
            _shipData.SetMaterial(Main.Instance.MaterialStorage.GetMaterialForShip(PlayerType));
        }

        public void InitCache(Action addShipInCache)
        {
            _addShipInCache = addShipInCache;
        }

        public abstract bool SeeOtherShip(ITargetToAttack ship);

        public abstract bool CanAttackOtherShip(ITargetToAttack ship);

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
            gameObject.SetActive(false);
            _isInitialized = false;
            _addShipInCache?.Invoke();
            _shipData.gameObject.SetActive(false);
            TurnOffEngine();
            Gun.FinishShoot();
            // При уничтожение корабля теряем командира
            _commander.Dispose();
            _commander = null;
        }

        public void TurnOffEngine()
        {
            if (!_isInitialized)
            {
                return;
            }

            IsStatic = true;
            _shipData.Agent.enabled = false;
            _shipData.EffectsManager.TurnOffEngine();
        }

        public void TurnOnEngine()
        {
            if (!_isInitialized)
            {
                return;
            }
            
            IsStatic = false;
            _shipData.Agent.enabled = true;
            _shipData.EffectsManager.TurnOnEngine();
        }

        public void DestroyShip()
        {
            if (!_isInitialized)
            {
                return;
            }

            _shipData.EffectsManager.DestroyShip();
        }

        public void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!_isInitialized)
            {
                return;
            }
        }

        private void InitSkin(ShipData data)
        {
            var shipData = _skins.FirstOrDefault(d => d.Skin == data.SkinType);
            if (shipData == null)
            {
                Debug.LogError($"Not found skin with type: {data.SkinType}");
                return;
            }

            _calculatedShipData = data;
            _shipData = shipData;
            name = $"{name}_{_shipData.Skin.ToString()}";

            shipData.gameObject.SetActive(true);
            Detector.SetRadius(data.VisibilityRadius);
            SpeedMovement = data.SpeedMovement;

            // Инициализация обработчика нажатий
            _clickHandler = new ShipClickHandler(this, _calculatedShipData);

            // Инициализация здоровья
            var minMaxHealth = Main.Instance.ShipParameters.GetParameterByType(MainShipParameters.ParameterType.Health);
            Health = new ShipHealth(minMaxHealth.minValue, minMaxHealth.maxValue, 0, data.Armor,
                PercentageOfArmorAbsorption);
            // Инициализация двигателя
            Engine = new ShipEngine(transform, _shipData.Agent, _calculatedShipData.SpeedMovement,
                _calculatedShipData.SpeedMovement);
            // Инициализация оружия
            Gun = new ShipGun(this, _shipData.GunPoints, _calculatedShipData.RateOfFire, _calculatedShipData.GunPower);
        }

        public void Clicked()
        {
            _clickHandler.Clicked();
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
            if (_shipData == null || !_isInitialized)
            {
                return;
            }

            Gizmos.color = new Color(1, 0, 0, 0.25f);
            Gizmos.DrawSphere(transform.position, _shipData.RadiusShip);

            if (_commander != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(_commander.GetPointForMovement(), 1.5f);
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